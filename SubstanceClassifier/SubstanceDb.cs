using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SubstanceClassifier
{
	/// <summary>
	/// Lokali medžiagų duomenų bazė.
	/// </summary>
	public class SubstanceDb : DbContext
	{
		/// <summary>
		/// Medžiagos duomenų bazėje.
		/// </summary>
		public DbSet<Substance> Substances { get; set; }

		public SubstanceDb() : base()
		{
			Database.EnsureCreated(); // Užtikrina, kad duomenų bazė bus sukurta, jei jos nėra.
			Substances.Load(); // Užkrauna medžiagų lentelę iš duomenų bazės.
		}

		/// <summary>
		/// Duomenų bazės konfigūracija. Čia perduodamas duomenų bazės vardas.
		/// </summary>
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=SubstanceDb");
		}

		/// <summary>
		/// Atnaujina duomenų bazę.
		/// </summary>
		/// <param name="onProgress">Funkcija, kviečiama norint pasidalinti progresu apie operaciją.</param>
		/// <param name="maxPages">Maksimalus kraunamų ECHA duomenų bazės puslapių skaičius. Jei 0, kraunami visi.</param>
		public async Task UpdateDb(OnProgress onProgress, int maxPages)
		{
			// Užkraunami duomenys iš ECHA duomenų bazės.
			var substances = await QuerySubstancesFromEchaDb(onProgress, maxPages);

			try
			{
				// Atnaujinami duomenys lokalioje duomenų bazėje.
				await UpdateDb(substances, onProgress);
			}
			catch
			{
				// Jei kas nors nepavyko, transakcija atmetama.
				AbandonChanges();
				throw;
			}
			finally
			{
				// Bet kuriuo atveju užkraunami duomenys iš duomenų bazės, kad nebūtų neatitikimų.
				Substances.Load();
			}
		}

		/// <summary>
		/// Atmeta duomenų bazės tranzakciją.
		/// </summary>
		public void AbandonChanges()
		{
			var context = this;
			var changedEntries = context.ChangeTracker.Entries()
				.Where(x => x.State != EntityState.Unchanged).ToList();

			foreach (var entry in changedEntries)
			{
				switch (entry.State)
				{
					case EntityState.Modified:
						entry.CurrentValues.SetValues(entry.OriginalValues);
						entry.State = EntityState.Unchanged;
						break;
					case EntityState.Added:
						entry.State = EntityState.Detached;
						break;
					case EntityState.Deleted:
						entry.State = EntityState.Unchanged;
						break;
				}
			}
		}

		/// <summary>
		/// Užkrauna duomenis iš ECHA duomenų bazės
		/// </summary>
		private async Task<IDictionary<string, Substance>> QuerySubstancesFromEchaDb(OnProgress onProgress, int maxPages)
		{
			// Randamas puslapių skaičius
			string firstPageUrl = EchaParser.GetDatabaseUrl(1);
			string firstPageHtml = await WebClient.GetHtml(firstPageUrl);
			int queriedPageCount = EchaParser.GetPageCount(firstPageHtml);
			int pageCount = (maxPages > 0) ? Math.Min(maxPages, queriedPageCount) : queriedPageCount;

			// Naudojamos paraleiai-saugios duomenų struktūros operacijoms tarp gijų.
			var substances = new ConcurrentBag<Substance>();
			var completedPages = new ConcurrentBag<int>();

			DateTime started = DateTime.Now;
			for (int i = 0; i < 3; ++i) // Puslapį bandoma krauti nedaugiau 3 kartų.
			{
				try
				{
					var pagesToQuery = Enumerable.Range(1, pageCount).Except(completedPages); // Surandami neužkrauti puslapiai
					await Parallel.ForEachAsync(pagesToQuery, async (pageIndex, cancellationToken) => // Vykdoma paraleliai (reikalauja .NET 6 versijos)
					{
						// Užkrauname ir perskaitome duomenų bazės puslapį.
						string url = EchaParser.GetDatabaseUrl(pageIndex);
						string html = "";
						try
						{
							html = await WebClient.GetHtml(url);
						}
						catch (HttpRequestException)
						{
							return;
						}
						var contents = await EchaParser.ReadSubstances(html, onProgress);

						// Užkrautus mišinių objektus įdedame į bendrą sąrašą.
						foreach (var content in contents)
							substances.Add(content);

						completedPages.Add(pageIndex);
						ReportProgress(started.Ticks, completedPages.Count, pageCount, onProgress);
					});
				}
				catch { }
			}

			// ECHA duomenų bazė gali grąžinti duplikatus. Juos išfiltruojame.
			return substances.Distinct(new SubstanceComparer()).ToDictionary(substance => substance.CASNumber);
		}

		/// <summary>
		/// Paskelbia operacijos progresą.
		/// </summary>
		private void ReportProgress(long startTimeTicks, int completedItems, int maxItems, OnProgress onProgress)
		{
			var currentTime = DateTime.Now;
			var totalWaitTime = currentTime.Ticks - startTimeTicks;
			long averageWaitTime = totalWaitTime / completedItems;

			int remainingItems = maxItems - completedItems;
			long estimatedTimeRemaining = remainingItems * averageWaitTime;
			DateTime remaining = new DateTime(estimatedTimeRemaining);
			string estimatedTimeRemainingFormatted = $"{remaining.Hour} val. {remaining.Minute} min.";
			onProgress(new()
			{
				Minimum = 0,
				Maximum = maxItems,
				Current = completedItems,
				Text = $"Skaitomi duomenys iš ECHA duomenų bazės: {completedItems} / {maxItems}. " +
				$"Apytikris laukimo laikas: {(totalWaitTime > 0 ? estimatedTimeRemainingFormatted : "Skaičiuojama")}"
			});
		}

		/// <summary>
		/// Atnaujina lokalią duomenų bazę duotomis reikšmėmis
		/// </summary>
		private async Task UpdateDb(IDictionary<string, Substance> substances, OnProgress onProgress)
		{
			await Task.Run(() =>
			{
				onProgress.Invoke(new ProgressResponse() { Minimum = 0, Maximum = substances.Count, Current = 0, Text = "Tikrinamas atnaujintinų medžiagų sąrašas" });
				var changeSet = CreateChangeSet(substances);

				onProgress.Invoke(new ProgressResponse() { Minimum = 0, Maximum = changeSet.Count, Current = 0, Text = $"Atnaujinama duomenų bazė: {0} / {changeSet.Count}" });

				int current = 0;
				foreach (var (SubstanceKey, Operation) in changeSet)
				{
					onProgress.Invoke(new ProgressResponse() { Minimum = 0, Maximum = changeSet.Count, Current = 0, Text = $"Atnaujinama duomenų bazė: {current} / {changeSet.Count}" });
					switch (Operation)
					{
						case DbOperation.Delete:
							Substances.Remove(Substances.Find(SubstanceKey));
							break;
						case DbOperation.Add:
							Substances.Add(substances[SubstanceKey]);
							break;
						case DbOperation.Update:
							Substances.Remove(Substances.Find(SubstanceKey));
							Substances.Add(substances[SubstanceKey]);
							break;
						case DbOperation.NoOperation:
						default:
							continue;
					}
				}

				SaveChanges();
			});
		}

		/// <summary>
		/// Atskiria medžiagų pakeitimus
		/// </summary>
		private IDictionary<string, DbOperation> CreateChangeSet(IDictionary<string, Substance> substances)
		{
			var changeSet = new Dictionary<string, DbOperation>();

			foreach (var existingSubstance in Substances)
			{
				// Jei lokalioje duomenų bazėje medžiaga yra, o ECHA - nėra: Ištriname
				if (!substances.ContainsKey(existingSubstance.CASNumber))
					changeSet.Add(existingSubstance.CASNumber, DbOperation.Delete);
			}

			foreach (var newSubstance in substances)
			{
				var dbSubstance = Substances.Find(newSubstance.Key);

				// Jei lokalioje duomenų bazėje medžiagos nėra, o ECHA - yra: Pridedame
				if (dbSubstance == null)
				{
					changeSet.Add(newSubstance.Key, DbOperation.Add);
					continue;
				}

				// Jei medžiaga nepasikeitė - nieko nedarome. Kitu atveju, medžiagą pakeičiame.
				if (newSubstance.Equals(dbSubstance))
					changeSet.Add(dbSubstance.CASNumber, DbOperation.NoOperation);
				else
					changeSet.Add(dbSubstance.CASNumber, DbOperation.Update);
			}

			return changeSet;
		}

		enum DbOperation
		{
			Delete,
			Add,
			Update,
			NoOperation,
		}
	}
}

