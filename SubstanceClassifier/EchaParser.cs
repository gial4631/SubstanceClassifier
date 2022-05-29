using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace SubstanceClassifier
{
	/// <summary>
	/// Naudojama operacijos progresui nurodyti.
	/// </summary>
	public struct ProgressResponse
	{
		public int Minimum { get; internal set; }
		public int Maximum { get; internal set; }
		public int Current { get; internal set; }

		public string Text { get; internal set; }
	}

	/// <summary>
	/// Naudojama operacijos progresui nurodyti.
	/// </summary>
	public delegate void OnProgress(ProgressResponse progress);

	/// <summary>
	/// Skaito ECHA duomenų bazės failus.
	/// </summary>
	public static class EchaParser
	{
		/// <summary>
		/// Duomenų bazės URL.
		/// </summary>
		public static string DatabaseUrlFormat = @"https://echa.europa.eu/lt/information-on-chemicals/cl-inventory-database?p_p_id=dissclinventory_WAR_dissclinventoryportlet&p_p_lifecycle=0&p_p_state=normal&p_p_mode=view&_dissclinventory_WAR_dissclinventoryportlet_jspPage=%2Fhtml%2Fsearch%2Fsearch.jsp&_dissclinventory_WAR_dissclinventoryportlet_searching=true&_dissclinventory_WAR_dissclinventoryportlet_iterating=true&_dissclinventory_WAR_dissclinventoryportlet_criteriaParam=_dissclinventory_WAR_dissclinventoryportlet_criteriaKey2Zgm&_dissclinventory_WAR_dissclinventoryportlet_delta=50&_dissclinventory_WAR_dissclinventoryportlet_orderByCol=CLD_NAME&_dissclinventory_WAR_dissclinventoryportlet_orderByType=asc&_dissclinventory_WAR_dissclinventoryportlet_resetCur=false&_dissclinventory_WAR_dissclinventoryportlet_cur={0}";

		/// <summary>
		/// Grąžina URL į duomenų bazės puslapį.
		/// </summary>
		/// <param name="page">Puslapio skaičius.</param>
		public static string GetDatabaseUrl(int page) => string.Format(DatabaseUrlFormat, page);

		/// <summary>
		/// Grąžina duomenų bazės puslapių skaičių.
		/// </summary>
		/// <param name="firstPageHtml">Pirmojo puslapio HTML.</param>
		public static int GetPageCount(string firstPageHtml)
		{
			var htmlDoc = new HtmlDocument();
			htmlDoc.LoadHtml(firstPageHtml);

			var dropDownNode = FindChildClass(htmlDoc.DocumentNode, "dropdown-toggle direction-down max-display-items-15 btn btn-default");
			string dropDownTitle = dropDownNode.Attributes["title"].Value;

			string pageCount = Regex.Replace(dropDownTitle.Remove(0, "1 puslapis iš".Length), @"\s+", "");
			if (int.TryParse(pageCount, out int result))
				return result;

			return 0;
		}

		/// <summary>
		/// Perskaito medžiagas iš duoto ECHA HTML.
		/// </summary>
		/// <param name="databaseHtml">Duomenų bazės HTML.</param>
		/// <param name="onProgress">Funkcija, kviečiama skelbiant progresą.</param>
		/// <returns>Perskaitytos medžiagos.</returns>
		public async static Task<IList<Substance>> ReadSubstances(string databaseHtml, OnProgress onProgress)
		{
			var htmlDoc = new HtmlDocument();
			htmlDoc.LoadHtml(databaseHtml);

			// Imant lentelės eilutes tiesiogiai gaunama ne visa informacija.
			// Surandamos visos lentelės celės.
			var tableCells = htmlDoc.DocumentNode.SelectNodes("//td");
			onProgress.Invoke(new ProgressResponse() { Minimum = 0, Maximum = tableCells.Count, Current = 0 });

			// Lentelės celės sudėliojamos į eilutes.
			var tableRows = ToRows(tableCells, 6);
			return await Task.Run(() =>
			{
				IList<Substance> contents = new List<Substance>();
				foreach (var row in tableRows)
				{
					// Perskaitoma informacija kiekvienoje eilutėje.
					var substance = new Substance()
					{
						SubstanceName = ReadSubstanceName(row),
						SubstanceDescription = ReadSubstanceDescription(row),
						SubstanceInfoUrl = ReadSubstanceInfoUrl(row),
						ECNumber = ReadECNo(row),
						CASNumber = ReadCASNo(row),
						Classification = string.Join(", ", ReadClassification(row)),
						Source = ReadSource(row),
						DetailsUrl = ReadDetailsUrl(row),
					};

					// Jei informacijos netrūksta, pridedame į sąrašą.
					if (substance.Validate())
						contents.Add(substance);
					onProgress.Invoke(new ProgressResponse() { Minimum = 0, Maximum = tableCells.Count, Current = contents.Count * 6 });
				}

				return contents;
			});
		}

		/// <summary>
		/// Perskaito klasifikacijos H kodus.
		/// </summary>
		public static IList<string> ReadHCodes(string detailsHtml, string classification)
		{
			var htmlDoc = new HtmlDocument();
			htmlDoc.LoadHtml(detailsHtml);

			var toxCodes = new List<string>();

			// Surandamos visos lentelės eilutės su duota klasifikacija
			var tableRows = htmlDoc.DocumentNode.SelectNodes("//tr");
			var classificationRows = FindClassificationRows(tableRows, classification);
			if (classificationRows.Count == 0)
				return toxCodes;

			foreach (var classificationRow in classificationRows)
			{
				// Perskaitomas H kodas
				string hCode = ReadClassNode(classificationRow.ChildNodes[3], "CLInventoryHelpCursor");
				if (!string.IsNullOrEmpty(hCode))
					toxCodes.Add(hCode);
			}
			return toxCodes;
		}

		/// <summary>
		/// Perskaito klasifikacijos M faktorius.
		/// </summary>
		public static (double MFactor, double MChronicFactor) ReadMFactor(string detailsHtml)
		{
			var htmlDoc = new HtmlDocument();
			htmlDoc.LoadHtml(detailsHtml);

			// Surandamos visos lentelės
			var tables = htmlDoc.DocumentNode.SelectNodes("//tbody");
			if (tables.Count == 0)
				return (double.NaN, double.NaN);

			double mFactor = double.NaN;
			double mChronicFactor = double.NaN;
			foreach (var table in tables)
			{
				// Surandama celė, kurios tekstas prasideda su "M=" arba "M(Chronic)="
				string mFactorTextResult = FindChildInnerText(table, "M=") ?? FindChildInnerText(table, "M(Chronic)=");
				if (string.IsNullOrEmpty(mFactorTextResult))
					continue;

				// Ištraukiama M faktoriaus reikšmė
				string mFactorText = Regex.Match(mFactorTextResult, @"(M\=)\d+").Value;
				if (!string.IsNullOrEmpty(mFactorText) && double.TryParse(mFactorText.Replace("M=", ""), out double resultMFactor))
					mFactor = resultMFactor;

				// Ištraukiama lėtinės medžiagos M faktoriaus reikšmė
				string mChronicFactorText = Regex.Match(mFactorTextResult, @"(M\(Chronic\)\=)\d+").Value;
				if (!string.IsNullOrEmpty(mChronicFactorText) && double.TryParse(mChronicFactorText.Replace("M(Chronic)=", ""), out double resultMChronicFactor))
					mChronicFactor = resultMChronicFactor;

				// Jei abi reikšmės surastos, grąžinama.
				if (!double.IsNaN(mFactor) && !double.IsNaN(mChronicFactor))
					break;
			}

			return (mFactor, mChronicFactor);
		}

		/// <summary>
		/// Sukonvertuoja lentelės celes į eilutes.
		/// </summary>
		/// <param name="cells">Lentelės celės.</param>
		/// <param name="columnCount">Stulpelių skaičius.</param>
		/// <returns></returns>
		private static IList<IList<HtmlNode>> ToRows(IList<HtmlNode> cells, int columnCount)
		{
			Debug.Assert(cells.Count % columnCount == 0);

			var rows = new List<IList<HtmlNode>>();
			for (int i = 0; i < cells.Count;)
			{
				var row = new List<HtmlNode>();
				do
				{
					row.Add(cells[i++]);
				}
				while (i % columnCount != 0);
				rows.Add(row);
			}

			return rows;
		}

		/// <summary>
		/// Perskaito medžiagos vardą iš duotos eilutės.
		/// </summary>
		static string ReadSubstanceName(IList<HtmlNode> row)
		{
			return ReadClassNode(row[0], "substanceNameLink");
		}

		/// <summary>
		/// Perskaito medžiagos aprašymą iš duotos eilutės.
		/// </summary>
		static string ReadSubstanceDescription(IList<HtmlNode> row)
		{
			return ReadClassNode(row[0], "substanceDescription");
		}

		/// <summary>
		/// Perskaito medžiagos informacijos URL iš duotos eilutės.
		/// </summary>
		static string ReadSubstanceInfoUrl(IList<HtmlNode> row)
		{
			return ReadLinkNode(row[0]);
		}

		/// <summary>
		/// Perskaito medžiagos EC numerį iš duotos eilutės.
		/// </summary>
		static string ReadECNo(IList<HtmlNode> row)
		{
			return ReadNode(row[1]);
		}

		/// <summary>
		/// Perskaito medžiagos CAS numerį iš duotos eilutės.
		/// </summary>
		static string ReadCASNo(IList<HtmlNode> row)
		{
			return ReadNode(row[2]);
		}

		/// <summary>
		/// Perskaito medžiagos klasifikaciją iš duotos eilutės.
		/// </summary>
		static IList<string> ReadClassification(IList<HtmlNode> row)
		{
			return ReadMultiClassNode(row[3], "class-consensus-text");
		}

		/// <summary>
		/// Perskaito medžiagos šaltinį iš duotos eilutės.
		/// </summary>
		static string ReadSource(IList<HtmlNode> row)
		{
			return ReadNode(row[4]);
		}

		/// <summary>
		/// Perskaito medžiagos papildomos informacijos URL iš duotos eilutės.
		/// </summary>
		static string ReadDetailsUrl(IList<HtmlNode> row)
		{
			return ReadLinkNode(row[5]);
		}

		/// <summary>
		/// Perskaito HTML celę su duotu klasės atributu.
		/// </summary>
		static string ReadClassNode(HtmlNode node, string className)
		{
			// Surandama celė su duotu klasės atributu.
			var classNode = FindChildClass(node, className);
			if (classNode == null)
				return null;

			// Grąžinamas celės tekstas.
			return classNode.InnerText.Trim();
		}

		/// <summary>
		/// Perskaito HTML celes su duotu klasės atributu.
		/// </summary>
		static IList<string> ReadMultiClassNode(HtmlNode node, string className)
		{
			var classNodes = FindChildClasses(node, className);
			return classNodes.Select((classNode) => classNode.InnerText).Where(value => !string.IsNullOrEmpty(value)).ToList();
		}

		/// <summary>
		/// Perskaito HTML celę.
		/// </summary>
		static string ReadNode(HtmlNode node)
		{
			return node.InnerText.Trim();
		}

		/// <summary>
		/// Perskaito URL HTML celėje.
		/// </summary>
		static string ReadLinkNode(HtmlNode node)
		{
			// Jei celė turi nuorodą, grąžiname ją.
			string link = node.Attributes["href"]?.Value;
			if (!string.IsNullOrEmpty(link))
				return link.Trim();

			// Kitu atveju rekursyviai patikriname vaikus.
			foreach (var child in node.ChildNodes)
			{
				var inChild = ReadLinkNode(child);
				if (inChild != null)
					return inChild;
			}

			return null;
		}

		/// <summary>
		/// Suranda HTML celę su duota klase.
		/// </summary>
		static HtmlNode FindChildClass(HtmlNode parent, string className)
		{
			// Jei celė su duota klase, grąžiname.
			if (parent.Attributes["class"]?.Value == className)
				return parent;

			// Kitu atveju rekursyviai patikriname vaikus.
			foreach (var child in parent.ChildNodes)
			{
				var inChild = FindChildClass(child, className);
				if (inChild != null)
					return inChild;
			}

			return null;
		}

		/// <summary>
		/// Suranda celę, kurios tekstas prasideda duotu tekstu.
		/// </summary>
		static string FindChildInnerText(HtmlNode parent, string textStartsWith)
		{
			// Jei dabartinės celės tekstas prasideda duotu tekstu, grąžiname.
			if (parent.InnerText.Trim().StartsWith(textStartsWith))
				return parent.InnerText;

			// Kitu atveju, rekursyviai patikriname vaikus.
			foreach (var child in parent.ChildNodes)
			{
				var inChild = FindChildInnerText(child, textStartsWith);
				if (!string.IsNullOrEmpty(inChild))
					return inChild;
			}

			return null;
		}

		/// <summary>
		/// Suranda HTML celes su duota klase.
		/// </summary>
		static IList<HtmlNode> FindChildClasses(HtmlNode parent, string className)
		{
			// Jei dabartinė celė tinka, pridedama į sąrašą.
			var nodes = new List<HtmlNode>();
			if (parent.Attributes["class"]?.Value == className)
				nodes.Add(parent);

			// Rekursyviai patikrinami vaikai
			foreach (var child in parent.ChildNodes)
			{
				var inChild = FindChildClasses(child, className);
				if (inChild.Count != 0)
					nodes.AddRange(inChild);
			}

			return nodes;
		}

		/// <summary>
		/// Suranda HTML celes su duota klasifikacija.
		/// </summary>
		private static IList<HtmlNode> FindClassificationRows(HtmlNodeCollection tableRows, string classification)
		{
			return tableRows.Where((row) => row.ChildNodes.FirstOrDefault(child => child.InnerText.Trim() == classification) != default).ToList();
		}
	}
}

