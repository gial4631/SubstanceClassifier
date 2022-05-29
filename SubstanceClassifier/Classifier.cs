using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SubstanceClassifier
{
    /// <summary>
    /// Naudojamas prireikus papildomų duomenų iš vartotojo (pavyzdžiui jei ECHA duombazėje nenurodytas medžiagos M faktorius).
    /// </summary>
    /// <param name="prompt">Tekstas (klausimo forma), kuris parodomas vartotojui.</param>
    /// <param name="onValidate">Funkcija, kuri validuoja vartotojo įvestą tekstą.</param>
    /// <returns>Vartotojo įvestas tekstas.</returns>
    public delegate string OnInputNeeded(string prompt, ValidateInput onValidate, string description = null);

    /// <summary>
    /// Naudojamas vartotojo įvesto teksto validacijai.
    /// </summary>
    /// <param name="inputValue">Vartotojo įvestas tekstas.</param>
    /// <returns>
    /// Jei įvestas tekstas validus, grąžina (true, null). 
    /// Kitu atveju - (false, error), kur error - tekstas, apibūdinantis kodėl vartotojo įvestas tekstas yra netinkamas.
    /// </returns>
    public delegate (bool Result, string Error) ValidateInput(string inputValue);

    /// <summary>
    /// Klasifikacijos įvesties eilutė. Pagalbinė struktūra parametrų grupavimui.
    /// </summary>
    public struct InputLine
    {
        /// <summary>
        /// Medžiagos CAS Numeris. Gali būti null.
        /// </summary>
        public string CASNumber;

        /// <summary>
        /// Medžiagos procentinė dalis mišinyje.
        /// </summary>
        public double Percentile;

        /// <summary>
        /// Medžiagos klasifikacija.
        /// </summary>
        public IList<string> Classification;

        /// <summary>
        /// Pagalbinis metodas leidžiantis <see cref="InputLine"/> objektą paversti <see cref="Tuple"/> struktūra.
        /// </summary>
        /// <param name="value"><see cref="InputLine"/> objektas.</param>
        /// <returns><see cref="Tuple"/> struktūra.</returns>
        public static implicit operator (string CASNumber, double Percentile, IList<string> Classification)(InputLine value)
            => (value.CASNumber, value.Percentile, value.Classification);

        /// <summary>
        /// Pagalbinis metodas leidžiantis <see cref="Tuple"/> struktūrą paversti <see cref="InputLine"/> objektu.
        /// </summary>
        /// <param name="value"><see cref="Tuple"/> struktūra.</param>
        /// <returns><see cref="InputLine"/> objektas.</returns>
        public static implicit operator InputLine((string CASNumber, double Percentile, IList<string> Classification) value)
            => new() { Classification = value.Classification, CASNumber = value.CASNumber, Percentile = value.Percentile };

    }

    /// <summary>
    /// Klasifikacijos išvestis. Pagalbinė struktūra parametrų grupavimui.
    /// </summary>
    public struct ClassificationOutput
    {
        /// <summary>
        /// Gauta mišinio klasifikacija.
        /// </summary>
        public List<string> Classification;

        /// <summary>
        /// Klasifikacijos klaidų įspėjimo žinutės (pavyzdžiui - klasifikuojama tik pagal atliktus bandymus).
        /// </summary>
        public List<string> Errors;

        /// <summary>
        /// Pagalbinis metodas leidžiantis <see cref="ClassificationOutput"/> objektą paversti <see cref="Tuple"/> struktūra.
        /// </summary>
        /// <param name="value"><see cref="ClassificationOutput"/> objektas.</param>
        /// <returns><see cref="Tuple"/> struktūra.</returns>
        public static implicit operator (List<string> Classification, List<string> Errors)(ClassificationOutput value)
            => (value.Classification, value.Errors);

        /// <summary>
        /// Pagalbinis metodas leidžiantis <see cref="Tuple"/> struktūrą paversti <see cref="ClassificationOutput"/> objektu.
        /// </summary>
        /// <param name="value"><see cref="Tuple"/> struktūra.</param>
        /// <returns><see cref="ClassificationOutput"/> objektas.</returns>
        public static implicit operator ClassificationOutput((List<string> Classification, List<string> Errors) value)
            => new() { Classification = value.Classification, Errors = value.Errors };
    }

    /// <summary>
    /// Mišinių klasifikatorius.
    /// </summary>
    public class Classifier
    {
        /// <summary>
        /// Klasifikatoriaus įvestis. Turi būti apibrėžta prieš kviečiant <see cref="Classify"/>.
        /// </summary>
        public List<InputLine> Input { get; set; }

        /// <summary>
        /// Klasifikatoriaus duomenų bazė. Turi būti apibrėžta prieš kviečiant <see cref="Classify"/>.
        /// </summary>
        public SubstanceDb Db { get; set; }

        /// <summary>
        /// Funkcija papildomam vartotojo paklausimui. Turi būti apibrėžta prieš kviečiant <see cref="Classify"/>.
        /// </summary>
        public OnInputNeeded OnInputNeeded { get; set; }

        /// <summary>
        /// Acute Tox. medžiagų H kodai. Surandami ECHA duomenų bazėje arba (neradus) paprašoma vartotojo patikslinimo.
        /// </summary>
        private IDictionary<(int, string), IList<string>> HazardCodes { get; set; } = new Dictionary<(int, string), IList<string>>();

        /// <summary>
        /// Resp. Sens. medžiagų fizinės būsenos. Paprašoma vartotojo patikslinimo.
        /// </summary>
        private IDictionary<int, string> PhysicalStates { get; set; } = new Dictionary<int, string>();

        /// <summary>
        /// Aquatic Acute medžiagų M faktoriai. Surandami ECHA duomenų bazėje arba (neradus) paprašoma vartotojo patikslinimo.
        /// </summary>
        private IDictionary<int, double> MFactors { get; set; } = new Dictionary<int, double>();

        /// <summary>
        /// Aquatic Chronic medžiagų M faktoriai. Surandami ECHA duomenų bazėje arba (neradus) paprašoma vartotojo patikslinimo.
        /// </summary>
        private IDictionary<int, double> MChronicFactors { get; set; } = new Dictionary<int, double>();

        /// <summary>
        /// Paklaida <see cref="double"/> reikšmių palyginimui.
        /// </summary>
        private const double Epsilon = 0.001;

        /// <summary>
        /// Suklasifikuoja mišinį.
        /// </summary>
        /// <returns>Žiūrėti <see cref="ClassificationOutput"/>.</returns>
        public async Task<ClassificationOutput> Classify()
        {
            var classificationResult = new ClassificationOutput() { Classification = new List<string>(), Errors = new List<string>() };

            // Jei paduota tik viena medžiaga ir jos yra ne mažiau nei 80 % (arba 100% dujoms), grąžiname tos medžiagos klasifikaciją.
            if (Input.Count == 1 && (Input[0].Percentile == 100 || (
                IsAlmostGreaterOrEqual(80, Input[0].Percentile) &&
                Input[0].Classification.FirstOrDefault(classification => classification.StartsWith("Flam. Gas")) == default &&
                Input[0].Classification.FirstOrDefault(classification => classification.StartsWith("Chem. Unst. Gas")) == default &&
                Input[0].Classification.FirstOrDefault(classification => classification.StartsWith("Aerosol")) == default &&
                Input[0].Classification.FirstOrDefault(classification => classification.StartsWith("Ox. Gas")) == default &&
                Input[0].Classification.FirstOrDefault(classification => classification.StartsWith("Press. Gas")) == default)))
                return (Input[0].Classification.ToList(), new List<string>());

            // Surandama trūkstama medžiagų informacija ECHA duomenų bazėje arba paprašant vartotojo patikslinimo.
            await GetAdditionalInfo();

            // Tikrinamos klasifikacijos taisyklės medžiagų, kurios pasitaiko mišinyje, kategorijoms .
            ClassifySubstance(ref classificationResult, ClassifyAcuteTox, "Acute Tox.");
            ClassifySubstance(ref classificationResult, ClassifyExplosive, "Expl.");
            ClassifySubstance(ref classificationResult, ClassifyFlammableGas, "Flam. Gas");
            ClassifySubstance(ref classificationResult, ClassifyChemUnstGas, "Chem. Unst. Gas");
            ClassifySubstance(ref classificationResult, ClassifyAerosol, "Aerosol");
            ClassifySubstance(ref classificationResult, ClassifyOxidisingGas, "Ox. Gas");
            ClassifySubstance(ref classificationResult, ClassifyPressureGas, "Press. Gas");
            ClassifySubstance(ref classificationResult, ClassifyFlammableLiquid, "Flam. Liq");
            ClassifySubstance(ref classificationResult, ClassifyFlammableSolid, "Flam. Sol");
            ClassifySubstance(ref classificationResult, ClassifySkinCorrosion, "Skin Corr", "Skin Irrit");
            ClassifySubstance(ref classificationResult, ClassifyEyeDamage, "Eye Dam", "Eye Irrit", "Skin Corr");
            ClassifySubstance(ref classificationResult, ClassifyRespSens, "Resp. Sens.");
            ClassifySubstance(ref classificationResult, ClassifySkinSens, "Skin Sens.");
            ClassifySubstance(ref classificationResult, ClassifyAquatic, "Aquatic Acute", "Aquatic Chronic");
            ClassifySubstance(ref classificationResult, ClassifySelfReact, "Self-react.");
            ClassifySubstance(ref classificationResult, ClassifyPyrLiq, "Pyr. Liq");
            ClassifySubstance(ref classificationResult, ClassifyPyrSol, "Pyr. Sol");
            ClassifySubstance(ref classificationResult, ClassifySelfHeat, "Self-heat.");
            ClassifySubstance(ref classificationResult, ClassifyWaterReact, "Water-react.");
            ClassifySubstance(ref classificationResult, ClassifyOxLiq, "Ox. Liq.");
            ClassifySubstance(ref classificationResult, ClassifyOxSol, "Ox. Sol.");
            ClassifySubstance(ref classificationResult, ClassifyMetCorr, "Met. Corr.");
            ClassifySubstance(ref classificationResult, ClassifyMuta, "Muta.");
            ClassifySubstance(ref classificationResult, ClassifyCarc, "Carc.");
            ClassifySubstance(ref classificationResult, ClassifyRepr, "Repr.");
            ClassifySubstance(ref classificationResult, ClassifyLact, "Lact.");
            ClassifySubstance(ref classificationResult, ClassifyStotSe, "STOT SE");
            ClassifySubstance(ref classificationResult, ClassifyStotRe, "STOT RE");
            ClassifySubstance(ref classificationResult, ClassifyAspTox, "Asp. Tox.");
            ClassifySubstance(ref classificationResult, ClassifyOzone, "Ozone");

            return classificationResult;
        }

        #region Pasiruošimas (GetAdditionalInfo)

        /// <summary>
        /// Suranda ir įrašo trūkstamą papildomą informaciją mišinio klasifikavimui (Acute Tox. medžiagų H kodai, Aquatic medžiagų M faktoriai, etc.)
        /// </summary>
        private async Task GetAdditionalInfo()
        {
            await GetHazardCodes();
            await GetMFactors();
            GetPhysicalStates();
        }

        /// <summary>
        /// Suranda ir įrašo Acute Tox. medžiagų H kodus.
        /// </summary>
        private async Task GetHazardCodes()
        {
            for (int i = 0; i < Input.Count; i++)
            {
                for (int j = 0; j < Input[i].Classification.Count; ++j)
                {
                    string classification = Input[i].Classification[j];
                    if (classification.StartsWith("Acute Tox."))
                    {
                        if (HazardCodes.ContainsKey((i, classification)))
                            continue;

                        var codes = await GetHazardCodes(Input[i].CASNumber, classification, i + 1);
                        HazardCodes.Add((i, classification), codes);
                    }
                }
            }
        }

        /// <summary>
        /// Suranda ir įrašo Aquatic medžiagų M faktorius.
        /// </summary>
        private async Task GetMFactors()
        {
            for (int i = 0; i < Input.Count; i++)
            {
                if (Input[i].Classification.FirstOrDefault(classification => classification.StartsWith("Aquatic Acute") || classification.StartsWith("Aquatic Chronic")) == default)
                    continue;

                var (MFactor, MChronicFactor) = await GetMFactors(Input[i].CASNumber, Input[i].Classification, i + 1);
                if (!MFactors.ContainsKey(i) && !double.IsNaN(MFactor))
                    MFactors.Add(i, MFactor);

                if (!MChronicFactors.ContainsKey(i) && !double.IsNaN(MChronicFactor))
                    MChronicFactors.Add(i, MChronicFactor);
            }
        }

        /// <summary>
        /// Suranda ir įrašo Res. Sens medžiagų fizines būsenas.
        /// </summary>
        private void GetPhysicalStates()
        {
            for (int i = 0; i < Input.Count; i++)
            {
                if (Input[i].Classification.FirstOrDefault(classification => classification.StartsWith("Resp. Sens")) == default)
                    continue;

                if (PhysicalStates.ContainsKey(i))
                    continue;

                var phycisalState = OnInputNeeded($"Įrašykite medžiagos {(string.IsNullOrEmpty(Input[i].CASNumber) ? i : Input[i].CASNumber)} fizinę būseną (k/s/d)", ValidatePhysicalState);
                PhysicalStates.Add(i, phycisalState);
            }
        }

        /// <summary>
        /// Suranda acute tox. medžiagos H kodus.
        /// </summary>
        /// <param name="casNumber">CAS numeris (gali būti null).</param>
        /// <param name="classification">Medžiagos klasifikacija.</param>
        /// <param name="substanceIndex">Medžiagos indeksas įvesties sąraše.</param>
        /// <returns>Medžiagos H kodai.</returns>
        private async Task<IList<string>> GetHazardCodes(string casNumber, string classification, int substanceIndex)
        {
            // Bandoma gauti kodą iš ECHA duomenų bazės.
            var queriedHCodes = await QueryHazardCode(casNumber, classification);
            if (queriedHCodes.Count != 0)
                return queriedHCodes;

            // Jei gauti kodo nepavyko, paprašoma vartotojo patikslinimo.
            return OnInputNeeded($"Įrašykite {substanceIndex}-osios medžiagos {classification} pavojingumo frazių kodus", ValidateHazardCodes, GetHazardCodeDescription()).Split(", ");
        }

        private string GetHazardCodeDescription()
        {
            return "Įveskite tik vieną iš šių H kodų:\n 1 kategorija: H300, H310, H330 \n 2 kategorija: H300, H310, H330 \n 3 kategorija: H301, H311, H331 \n 4 kategorija: H302, H312, H332";
        }

        /// <summary>
        /// Suranda aquatic medžiagos M faktorius.
        /// </summary>
        /// <param name="casNumber">CAS numeris (gali būti null).</param>
        /// <param name="classification">Medžiagos klasifikacija.</param>
        /// <param name="substanceIndex">Medžiagos indeksas įvesties sąraše.</param>
        /// <returns>Medžiagos H kodai.</returns>
        private async Task<(double MFactor, double MChronicFactor)> GetMFactors(string casNumber, IList<string> classification, int substanceIndex)
        {
            double mFactor = double.NaN;
            double mChronicFactor = double.NaN;

            // Bandoma gauti M faktorių reikšmes iš ECHA duomenų bazės.
            var queriedMFactor = await QueryMFactor(casNumber);

            // Jei Aquatic Acute medžiagos M faktoriaus nėra, prašoma vartotojo patikslinimo.
            if (double.IsNaN(queriedMFactor.MFactor))
            {
                if (classification.FirstOrDefault(entry => entry.StartsWith("Aquatic Acute")) != default)
                    mFactor = double.Parse(OnInputNeeded($"Įrašykite {substanceIndex}-osios medžiagos M faktorių", ValidateMFactor));
            }
            else
                mFactor = queriedMFactor.MFactor;

            // Jei Aquatic Chronic medžiagos M faktoriaus nėra, prašoma vartotojo patikslinimo.
            if (double.IsNaN(queriedMFactor.MChronicFactor))
            {
                if (classification.FirstOrDefault(entry => entry.StartsWith("Aquatic Chronic")) != default)
                    mChronicFactor = double.Parse(OnInputNeeded($"Įrašykite {substanceIndex}-osios medžiagos lėtinio M faktorių", ValidateMFactor));
            }
            else
                mChronicFactor = queriedMFactor.MChronicFactor;

            return (mFactor, mChronicFactor);
        }

        /// <summary>
        /// Suranda H kodą ECHA duomenų bazėje.
        /// </summary>
        /// <param name="casNumber">Medžiagos CAS numeris.</param>
        /// <param name="classification">Medžiagos klasifikacija.</param>
        /// <returns>Medžiagos H kodai nurodytai klasifikacijai.</returns>
        private async Task<IList<string>> QueryHazardCode(string casNumber, string classification)
        {
            if (string.IsNullOrEmpty(casNumber))
                return new List<string>();

            var substance = Db.Substances.Find(casNumber);
            if (substance == null)
                return null;

            string html = await WebClient.GetHtml(substance.DetailsUrl);
            return EchaParser.ReadHCodes(html, classification);
        }

        /// <summary>
        /// Suranda medžiagos M faktorių ECHA duomenų bazėje.
        /// </summary>
        /// <param name="casNumber">Medžiagos CAS numeris.</param>
        /// <returns>Medžiagos M faktoriai.</returns>
        private async Task<(double MFactor, double MChronicFactor)> QueryMFactor(string casNumber)
        {
            if (string.IsNullOrEmpty(casNumber))
                return (double.NaN, double.NaN);

            var substance = Db.Substances.Find(casNumber);
            if (substance == null)
                return (double.NaN, double.NaN);

            string html = await WebClient.GetHtml(substance.DetailsUrl);
            return EchaParser.ReadMFactor(html);
        }

        /// <summary>
        /// Validuoja vartotojo įrašytą H kodų sąrašą.
        /// </summary>
        /// <param name="input">Vartotojo įrašytas tekstas.</param>
        private (bool Result, string Error) ValidateHazardCodes(string input)
        {
            var hazardCodeList = input.Split(", ");
            var knownHazardCodes = ToxCode.ToList();
            foreach (var hazardCode in hazardCodeList)
                if (!knownHazardCodes.Contains(hazardCode))
                    return (false, $"Pavojingumo frazės kodas {hazardCode} neatpažintas. Atpažystamų kodų sąrašas: {string.Join(", ", knownHazardCodes)}. Prireikus įrašyti keletą kodų, atskirkite kableliais.");

            return (true, null);
        }

        /// <summary>
        /// Validuoja vartotojo įrašytą M faktorių.
        /// </summary>
        /// <param name="input">Vartotojo įrašytas tekstas.</param>
        private (bool Result, string Error) ValidateMFactor(string input)
        {
            if (!double.TryParse(input, out var _))
                return (false, "M faktorius turi būti skaičius");

            return (true, null);
        }

        /// <summary>
        /// Validuoja vartotojo įrašytą medžiagos fizinę būseną.
        /// </summary>
        /// <param name="input">Vartotojo įrašytas tekstas.</param>
        private (bool Result, string Error) ValidatePhysicalState(string input)
        {
            switch (input)
            {
                case "k":
                case "s":
                case "d":
                    return (true, null);
                default:
                    return (false, "Medžiagos būsena gali būti \"k\" (kieta), \"s\" (skystis) arba \"d\" (dujos).");
            }
        }

        #endregion

        #region Klasifikavimas (ClassifySubstance)

        /// <summary>
        /// Pagalbinis metodas mišinio klasifikacijos funkcijai iškviesti ir rezultatui apdoroti.
        /// </summary>
        /// <param name="result">Bendras klasifikacijos rezultatas.</param>
        /// <param name="classificationFunction">Klasifikacijos funkcija.</param>
        /// <param name="classificationStartsWith">
        /// Tikrininamų medžiagų klasifikacija.Paduodama klasės pavadinimo pradžia, 
        /// nenurodant konkrečios kategorijos (pavyzdžiui "Acute Tox." o ne "Acute Tox. 1").
        /// </param>
        private void ClassifySubstance(ref ClassificationOutput result, Func<IList<InputLine>, ClassificationOutput> classificationFunction, params string[] classificationStartsWith)
        {
            var substances = Input.Where(entry => entry.Classification.FirstOrDefault(classification => StartsWithAny(classification, classificationStartsWith)) != default).ToList();
            if (substances.Count > 0)
            {
                var classificationResult = classificationFunction(substances);
                result.Classification.AddRange(classificationResult.Classification);
                result.Errors.AddRange(classificationResult.Errors);
            }
        }

        /// <summary>
        /// Pagalbinis metodas mišinio klasifikacijos funkcijai iškviesti ir rezultatui apdoroti.
        /// </summary>
        /// <param name="result">Bendras klasifikacijos rezultatas.</param>
        /// <param name="classificationFunction">Klasifikacijos funkcija.</param>
        /// <param name="classificationStartsWith">
        /// Tikrininamų medžiagų klasifikacija.Paduodama klasės pavadinimo pradžia, 
        /// nenurodant konkrečios kategorijos (pavyzdžiui "Acute Tox." o ne "Acute Tox. 1").
        /// </param>
        private void ClassifySubstance(ref ClassificationOutput result, Func<ClassificationOutput> classificationFunction, params string[] classificationStartsWith)
        {
            var substances = Input.Where(entry => entry.Classification.FirstOrDefault(classification => StartsWithAny(classification, classificationStartsWith)) != default).ToList();
            if (substances.Count > 0)
            {
                var classificationResult = classificationFunction();
                result.Classification.AddRange(classificationResult.Classification);
                result.Errors.AddRange(classificationResult.Errors);
            }
        }

        /// <summary>
        /// Patikrina, ar tekstas prasideda bent vienu iš nurodytų tekstų.
        /// </summary>
        /// <param name="text">Tekstas.</param>
        /// <param name="startsWith">Galimi prasidėjimo variantai.</param>
        /// <returns></returns>
        private bool StartsWithAny(string text, params string[] startsWith)
        {
            return startsWith.Any(entry => text.StartsWith(entry));
        }

        /// <summary>
        /// Patikrina medžiagos klasifikavimą kaip Acute Tox.
        /// </summary>
        private ClassificationOutput ClassifyAcuteTox()
        {
            var classifications = new List<string>();

            // Kiekvienai acuteTox pavojingumo frazei klasifikuojama atskirai.
            foreach (ToxStatement toxStatement in Enum.GetValues(typeof(ToxStatement)))
            {
                if (toxStatement == ToxStatement.Unrecognized)
                    continue;

                // Paskaičiuojamas mišinio ATE.
                double ate = CalculateATE(toxStatement);
                if (double.IsNaN(ate))
                    continue;

                // Klasifikuojama pagal ATE duotai pavojingumo frazei.
                string classification = ClassifyAcuteTox(ate, toxStatement);
                if (!string.IsNullOrEmpty(classification))
                    classifications.Add(classification);
            }

            return (classifications, new List<string>());
        }

        #region ClassifyAcuteTox

        public static class ToxCode
        {
            // Prarijus
            public const string H300 = "H300";
            public const string H301 = "H301";
            public const string H302 = "H302";

            // Prisilietus
            public const string H310 = "H310";
            public const string H311 = "H311";
            public const string H312 = "H312";

            // Įkvėpus
            public const string H330 = "H330";
            public const string H331 = "H331";
            public const string H332 = "H332";

            public static IList<string> ToList()
            {
                return new List<string>
                {
                H300, H301, H302,
                H310, H311, H312,
                H330, H331, H332
                };
            }

            /// <summary>
            /// Gražina toksiškumo kategoriją pagal H kodą.
            /// </summary>
            /// <param name="toxCode">H kodas.</param>
            public static ToxStatement GetHazardStatement(string toxCode)
            {
                switch (toxCode)
                {
                    case H300:
                    case H301:
                    case H302:
                        return ToxStatement.IfSwallowed;
                    case H310:
                    case H311:
                    case H312:
                        return ToxStatement.InContactWithSkin;
                    case H330:
                    case H331:
                    case H332:
                        return ToxStatement.IfInhaled;
                    default:
                        return ToxStatement.Unrecognized;
                }
            }
        }

        public enum ToxStatement
        {
            Unrecognized = default,
            IfSwallowed,
            InContactWithSkin,
            IfInhaled,
        }

        public static class AcuteToxTypes
        {
            public const string AcuteTox1 = "Acute Tox. 1";
            public const string AcuteTox2 = "Acute Tox. 2";
            public const string AcuteTox3 = "Acute Tox. 3";
            public const string AcuteTox4 = "Acute Tox. 4";
        }

        /// <summary>
        /// Patikrina medžiagos klasifikavimą kaip Acute Tox. pagal ATE ir pavojingumo frazę.
        /// </summary>
        /// <param name="ate">Mišinio ATE pavojingumo frazei.</param>
        /// <param name="statement">Pavojingumo frazė.</param>
        /// <returns>Gauta klasifikacija.</returns>
        private string ClassifyAcuteTox(double ate, ToxStatement statement)
        {
            switch (statement)
            {
                case ToxStatement.IfSwallowed: return ClassifyAcuteToxIfSwallowed(ate);
                case ToxStatement.InContactWithSkin: return ClassifyAcuteToxInContactWithSkin(ate);
                case ToxStatement.IfInhaled: return ClassifyAcuteToxIfInhaled(ate);
                case ToxStatement.Unrecognized:
                default: return null;
            }
        }

        /// <summary>
        /// Patikrina medžiagos klasifikavimą kaip Acute Tox. prarijus pagal ATE.
        /// </summary>
        /// <param name="ate">Mišinio ATE.</param>
        /// <returns>Gauta klasifikacija.</returns>
        private string ClassifyAcuteToxIfSwallowed(double ate)
        {
            if (IsAlmostLessOrEqual(0, ate) && ate < 5)
                return $"{AcuteToxTypes.AcuteTox1} (oral)";

            if (IsAlmostLessOrEqual(5, ate) && ate < 50)
                return $"{AcuteToxTypes.AcuteTox2} (oral)";

            if (IsAlmostLessOrEqual(50, ate) && ate < 300)
                return $"{AcuteToxTypes.AcuteTox3} (oral)";

            if (IsAlmostLessOrEqual(300, ate) && ate < 2000)
                return $"{AcuteToxTypes.AcuteTox4} (oral)";

            return null;
        }

        /// <summary>
        /// Patikrina medžiagos klasifikavimą kaip Acute Tox. prisilietus pagal ATE.
        /// </summary>
        /// <param name="ate">Mišinio ATE.</param>
        /// <returns>Gauta klasifikacija.</returns>
        private string ClassifyAcuteToxInContactWithSkin(double ate)
        {
            if (IsAlmostLessOrEqual(0, ate) && ate < 50)
                return $"{AcuteToxTypes.AcuteTox1} (dermal)";

            if (IsAlmostLessOrEqual(50, ate) && ate < 200)
                return $"{AcuteToxTypes.AcuteTox2} (dermal)";

            if (IsAlmostLessOrEqual(200, ate) && ate < 1000)
                return $"{AcuteToxTypes.AcuteTox3} (dermal)";

            if (IsAlmostLessOrEqual(1000, ate) && ate < 2000)
                return $"{AcuteToxTypes.AcuteTox4} (dermal)";

            return null;
        }

        /// <summary>
        /// Patikrina medžiagos klasifikavimą kaip Acute Tox. įkvėpus pagal ATE.
        /// </summary>
        /// <param name="ate">Mišinio ATE.</param>
        /// <returns>Gauta klasifikacija.</returns>
        private string ClassifyAcuteToxIfInhaled(double ate)
        {
            if (IsAlmostLessOrEqual(0, ate) && ate < 0.5)
                return $"{AcuteToxTypes.AcuteTox1} (inhale)";

            if (IsAlmostLessOrEqual(0.5, ate) && ate < 2)
                return $"{AcuteToxTypes.AcuteTox2} (inhale)";

            if (IsAlmostLessOrEqual(2, ate) && ate < 10)
                return $"{AcuteToxTypes.AcuteTox3} (inhale)";

            if (IsAlmostLessOrEqual(10, ate) && ate < 20)
                return $"{AcuteToxTypes.AcuteTox4} (inhale)";

            return null;
        }

        /// <summary>
        /// Paskaičiuoja mišinio ATE duotai pavojingumo frazei.
        /// </summary>
        /// <param name="toxStatement">Pavojingumo frazė.</param>
        /// <returns>Paskaičiuotas ATE.</returns>
        private double CalculateATE(ToxStatement toxStatement)
        {
            double ateSum = 0.0;

            for (int i = 0; i < Input.Count; i++)
            {
                var substance = Input[i];
                var acuteToxTypes = substance.Classification.Where(classification => classification.StartsWith("Acute Tox. ")).Distinct();
                foreach (var acuteToxType in acuteToxTypes)
                {
                    var substanceToxCodes = HazardCodes[(i, acuteToxType)];
                    if (substanceToxCodes.FirstOrDefault(substanceToxCode => ToxCode.GetHazardStatement(substanceToxCode) == toxStatement) == default)
                        continue;

                    double ateFactor = GetAteFactor(toxStatement, acuteToxType);
                    if (ateFactor == 0.0)
                        continue;

                    ateSum += substance.Percentile / ateFactor;
                }
            }

            return ateSum == 0.0 ? double.NaN : 100 / ateSum;
        }

        /// <summary>
        /// Grąžina medžiagos ATE daugiklį pagal Acute Tox. klasifikacijos lygį ir pavojingumo frazę.
        /// </summary>
        /// <param name="statement">Pavojingumo frazė.</param>
        /// <param name="acuteToxType">Acute Tox. klasifikacijos lygis.</param>
        /// <returns>Medžiagos ATE daugiklis pagal Acute Tox. klasifikacijos lygį ir pavojingumo frazę.</returns>
        private double GetAteFactor(ToxStatement statement, string acuteToxType)
        {
            switch (statement)
            {
                case ToxStatement.IfSwallowed:
                    return GetAteSwallowedFactor(acuteToxType);
                case ToxStatement.InContactWithSkin:
                    return GetAteSkinFactor(acuteToxType);
                case ToxStatement.IfInhaled:
                    return GetAteInhaledFactor(acuteToxType);
                case ToxStatement.Unrecognized:
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Grąžina medžiagos ATE daugiklį pagal Acute Tox. klasifikacijos lygį įkvėpus.
        /// </summary>
        /// <param name="acuteToxType">Acute Tox. klasifikacijos lygis.</param>
        /// <returns>Medžiagos ATE daugiklis pagal Acute Tox. klasifikacijos lygį ir pavojingumo frazę.</returns>
        private double GetAteInhaledFactor(string acuteToxType)
        {
            switch (acuteToxType)
            {
                case AcuteToxTypes.AcuteTox1:
                    return 0.05;
                case AcuteToxTypes.AcuteTox2:
                    return 0.5;
                case AcuteToxTypes.AcuteTox3:
                    return 3;
                case AcuteToxTypes.AcuteTox4:
                    return 11;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Grąžina medžiagos ATE daugiklį pagal Acute Tox. klasifikacijos lygį prisilietus.
        /// </summary>
        /// <param name="acuteToxType">Acute Tox. klasifikacijos lygis.</param>
        /// <returns>Medžiagos ATE daugiklis pagal Acute Tox. klasifikacijos lygį ir pavojingumo frazę.</returns>
        private double GetAteSkinFactor(string acuteToxType)
        {
            switch (acuteToxType)
            {
                case AcuteToxTypes.AcuteTox1:
                    return 5;
                case AcuteToxTypes.AcuteTox2:
                    return 50;
                case AcuteToxTypes.AcuteTox3:
                    return 300;
                case AcuteToxTypes.AcuteTox4:
                    return 1100;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Grąžina medžiagos ATE daugiklį pagal Acute Tox. klasifikacijos lygį prarijus.
        /// </summary>
        /// <param name="acuteToxType">Acute Tox. klasifikacijos lygis.</param>
        /// <returns>Medžiagos ATE daugiklis pagal Acute Tox. klasifikacijos lygį ir pavojingumo frazę.</returns>
        private double GetAteSwallowedFactor(string acuteToxType)
        {
            switch (acuteToxType)
            {
                case AcuteToxTypes.AcuteTox1:
                    return 0.5;
                case AcuteToxTypes.AcuteTox2:
                    return 5;
                case AcuteToxTypes.AcuteTox3:
                    return 100;
                case AcuteToxTypes.AcuteTox4:
                    return 500;
                default:
                    return 0;
            }
        }

        #endregion

        /// <summary>
        /// Patikrina medžiagos klasifikavimą kaip Expl.
        /// </summary>
        private ClassificationOutput ClassifyExplosive()
        {
            var classification = new List<string>();
            var errors = new List<string>() { "Nestabiliųjų sprogiųjų medžiagų klasifikavimui žiūrėti CLP 2.1 skyrių" };
            return (classification, errors);
        }

        /// <summary>
        /// Patikrina medžiagos klasifikavimą kaip Flam. Gas.
        /// </summary>
        private ClassificationOutput ClassifyFlammableGas()
        {
            var classification = new List<string>();
            var errors = new List<string>() { "Degiųjų dujų klasifikavimui žiūrėti CLP 2.2 skyrių" };
            return (classification, errors);
        }

        /// <summary>
        /// Patikrina medžiagos klasifikavimą kaip Chem. Unst. Gas.
        /// </summary>
        private ClassificationOutput ClassifyChemUnstGas()
        {
            var classification = new List<string>();
            var errors = new List<string>() { "Chemiškai nestabilių dujų klasifikavimui žiūrėti CLP 2.2 skyrių." };
            return (classification, errors);
        }

        /// <summary>
        /// Patikrina medžiagos klasifikavimą kaip Aerosol.
        /// </summary>
        private ClassificationOutput ClassifyAerosol()
        {
            var classification = new List<string>();
            var errors = new List<string>() { "Aerozolių klasifikavimui žiūrėti CLP 2.3 skyrių" };
            return (classification, errors);
        }

        /// <summary>
        /// Patikrina medžiagos klasifikavimą kaip Oxidising Gas.
        /// </summary>
        private ClassificationOutput ClassifyOxidisingGas()
        {
            var classification = new List<string>();
            var errors = new List<string>() { "Oksiduojančių dujų klasifikavimui žiūrėti CLP 2.4 skyrių" };
            return (classification, errors);
        }

        /// <summary>
        /// Patikrina medžiagos klasifikavimą kaip Pressure Gas.
        /// </summary>
        private ClassificationOutput ClassifyPressureGas()
        {
            var classification = new List<string>();
            var errors = new List<string>() { "Slėgio veikiamų dujų klasifikacija žiūrėti CLP 2.5 skyrių" };
            return (classification, errors);
        }

        /// <summary>
        /// Patikrina medžiagos klasifikavimą kaip Flam. Liq.
        /// </summary>
        private ClassificationOutput ClassifyFlammableLiquid(IList<InputLine> flammableLiquidSubstances)
        {
            // Paskaičiuojamos medžiagų procentinės dalys mišinyje pagal klasifikaciją.
            double flamLiq1Percentile = GetClassificationPercentile(flammableLiquidSubstances, "Flam. Liq. 1");
            double flamLiq2Percentile = GetClassificationPercentile(flammableLiquidSubstances, "Flam. Liq. 2");
            double flamLiq3Percentile = GetClassificationPercentile(flammableLiquidSubstances, "Flam. Liq. 3");
            double flamLiq4Percentile = GetClassificationPercentile(flammableLiquidSubstances, "Flam. Liq. 4");

            // Jei klasifikuotos medžiagos daugiau, nei 80%, o kitų kategorijų nėra, klasifikuojame iškart.
            if (IsAlmostGreaterOrEqual(80, flamLiq1Percentile) && IsAlmostEqual(flamLiq2Percentile, 0) &&
                IsAlmostEqual(flamLiq3Percentile, 0) && IsAlmostEqual(flamLiq4Percentile, 0))
                return (new List<string> { "Flam. Liq. 1" }, new List<string>());

            if (IsAlmostEqual(flamLiq1Percentile, 0) && IsAlmostGreaterOrEqual(80, flamLiq1Percentile) &&
                IsAlmostEqual(flamLiq3Percentile, 0) && IsAlmostEqual(flamLiq4Percentile, 0))
                return (new List<string> { "Flam. Liq. 2" }, new List<string>());

            if (IsAlmostEqual(flamLiq1Percentile, 0) && IsAlmostEqual(flamLiq2Percentile, 0) &&
                IsAlmostGreaterOrEqual(80, flamLiq3Percentile) && IsAlmostEqual(flamLiq4Percentile, 0))
                return (new List<string> { "Flam. Liq. 3" }, new List<string>());

            if (IsAlmostEqual(flamLiq1Percentile, 0) && IsAlmostEqual(flamLiq2Percentile, 0) &&
                IsAlmostEqual(flamLiq3Percentile, 0) && IsAlmostGreaterOrEqual(80, flamLiq4Percentile))
                return (new List<string> { "Flam. Liq. 4" }, new List<string>());

            // Kitu atveju, reikia patikrinti mišinio pliūpsnio ir virimo temperatūras.
            double flameTemp = double.Parse(OnInputNeeded("Įrašykite mišinio pliūpsnio temperatūrą (°C):", input => ValidateDouble(input)));
            if (flameTemp > 60)
                return (new List<string> { "Flam. Liq. 4" }, new List<string>());

            if (IsAlmostGreaterOrEqual(23, flameTemp) && IsAlmostLessOrEqual(flameTemp, 60))
                return (new List<string> { "Flam. Liq. 3" }, new List<string>());

            double boilTemp = double.Parse(OnInputNeeded("Įrašykite mišinio pradinę virimo temperatūrą (°C):", input => ValidateDouble(input)));
            if (flameTemp < 23 && boilTemp > 35)
                return (new List<string> { "Flam. Liq. 2" }, new List<string>());

            if (flameTemp < 23 && IsAlmostLessOrEqual(boilTemp, 35))
                return (new List<string> { "Flam. Liq. 1" }, new List<string>());

            return (new List<string>(), new List<string>());
        }

        /// <summary>
        /// Patikrina medžiagos klasifikavimą kaip Flam. Sol.
        /// </summary>
        private ClassificationOutput ClassifyFlammableSolid(IList<InputLine> flammableSolidSubstances)
        {
            // Paskaičiuojamos medžiagų procentinės dalys mišinyje pagal klasifikaciją.
            double flamSol1Percentile = GetClassificationPercentile(flammableSolidSubstances, "Flam. Sol. 1");
            double flamSol2Percentile = GetClassificationPercentile(flammableSolidSubstances, "Flam. Sol. 2");

            // Jei klasifikuotos medžiagos daugiau, nei 80%, o kitų kategorijų nėra, klasifikuojame iškart.
            if (IsAlmostGreaterOrEqual(80, flamSol1Percentile) && IsAlmostEqual(flamSol2Percentile, 0))
                return (new List<string> { "Flam. Sol. 1" }, new List<string>());

            if (IsAlmostEqual(flamSol1Percentile, 0) && IsAlmostGreaterOrEqual(80, flamSol2Percentile))
                return (new List<string> { "Flam. Sol. 2" }, new List<string>());

            // Kitu atveju - žiūrėti CLP.
            var errors = new List<string>() { "Degiųjų kietųjų medžiagų klasifikavimui žiūrėti CLP 2.7 skyrių" };
            return (new List<string>(), errors);
        }

        /// <summary>
        /// Patikrina medžiagos klasifikavimą kaip Skin Corr. arba Skin Irrit.
        /// </summary>
        private ClassificationOutput ClassifySkinCorrosion(IList<InputLine> skinCorrosionSubstances)
        {
            // Paskaičiuojamos medžiagų procentinės dalys mišinyje pagal klasifikaciją.
            double skinCorr1Percentile = GetClassificationPercentile(skinCorrosionSubstances, "Skin Corr. 1");
            double skinCorr1APercentile = GetClassificationPercentile(skinCorrosionSubstances, "Skin Corr. 1A");
            double skinCorr1BPercentile = GetClassificationPercentile(skinCorrosionSubstances, "Skin Corr. 1B");
            double skinCorr1CPercentile = GetClassificationPercentile(skinCorrosionSubstances, "Skin Corr. 1C");
            double skinIrrit2Percentile = GetClassificationPercentile(skinCorrosionSubstances, "Skin Irrit. 2");

            // Klasifikuojama pagal procentines dalis.
            double skinCorrPercentile = skinCorr1Percentile + skinCorr1APercentile + skinCorr1BPercentile + skinCorr1CPercentile;
            if (IsAlmostGreaterOrEqual(5, skinCorrPercentile))
                return (new List<string> { "Skin Corr. 1" }, new List<string>());

            if (IsAlmostGreaterOrEqual(1, skinCorrPercentile) && skinCorrPercentile < 5)
                return (new List<string> { "Skin Irrit. 2" }, new List<string>());

            if (IsAlmostGreaterOrEqual(10, skinIrrit2Percentile))
                return (new List<string> { "Skin Irrit. 2" }, new List<string>());

            if (IsAlmostGreaterOrEqual(10, 10 * skinCorrPercentile + skinIrrit2Percentile))
                return (new List<string> { "Skin Irrit. 2" }, new List<string>());

            return (new List<string>(), new List<string>());
        }

        /// <summary>
        /// Patikrina medžiagos klasifikavimą kaip Eye Dam. arba Eye Irrit.
        /// </summary>
        private ClassificationOutput ClassifyEyeDamage(IList<InputLine> eyeDamageSubstances)
        {
            // Paskaičiuojamos medžiagų procentinės dalys mišinyje pagal klasifikaciją.
            double skinCorr1Percentile = GetClassificationPercentile(eyeDamageSubstances, "Skin Corr. 1");
            double skinCorr1APercentile = GetClassificationPercentile(eyeDamageSubstances, "Skin Corr. 1A");
            double skinCorr1BPercentile = GetClassificationPercentile(eyeDamageSubstances, "Skin Corr. 1B");
            double skinCorr1CPercentile = GetClassificationPercentile(eyeDamageSubstances, "Skin Corr. 1C");
            double eyeDamPercentile = GetClassificationPercentile(eyeDamageSubstances, "Eye Dam. 1");
            double eyeIrrit2Percentile = GetClassificationPercentile(eyeDamageSubstances, "Eye Irrit. 2");
            double eyeIrrit2APercentile = GetClassificationPercentile(eyeDamageSubstances, "Eye Irrit. 2A");
            double eyeIrrit2BPercentile = GetClassificationPercentile(eyeDamageSubstances, "Eye Irrit. 2B");

            double skinCorrPercentile = skinCorr1Percentile + skinCorr1APercentile + skinCorr1BPercentile + skinCorr1CPercentile;
            double eyeIrritPercentile = eyeIrrit2Percentile + eyeIrrit2APercentile + eyeIrrit2BPercentile;

            // Klasifikuojama pagal procentines dalis.
            if (IsAlmostGreaterOrEqual(3, skinCorrPercentile + eyeDamPercentile))
                return (new List<string> { "Eye Dam. 1" }, new List<string>());

            if (IsAlmostGreaterOrEqual(1, skinCorrPercentile + eyeDamPercentile) && skinCorrPercentile + eyeDamPercentile < 3)
                return (new List<string> { "Eye Irrit. 2" }, new List<string>());

            if (IsAlmostGreaterOrEqual(10, eyeIrritPercentile))
                return (new List<string> { "Eye Irrit. 2" }, new List<string>());

            if (IsAlmostGreaterOrEqual(10, 10 * (skinCorrPercentile + eyeDamPercentile) + eyeIrritPercentile))
                return (new List<string> { "Eye Irrit. 2" }, new List<string>());

            return (new List<string>(), new List<string>());
        }

        /// <summary>
        /// Patikrina medžiagos klasifikavimą kaip Resp. Sens.
        /// </summary>
        private ClassificationOutput ClassifyRespSens()
        {
            double respSens1Percentile = 0;
            double respSens1GasPercentile = 0;
            double respSens1APercentile = 0;
            double respSens1AGasPercentile = 0;
            double respSens1BPercentile = 0;
            double respSens1BGasPercentile = 0;

            // Paskaičiuojamos medžiagų procentinės dalys mišinyje pagal klasifikaciją.
            for (int i = 0; i < Input.Count; ++i)
            {
                string classification = Input[i].Classification.FirstOrDefault(classification => classification.StartsWith("Resp. Sens."));
                if (classification == default)
                    continue;

                if (!PhysicalStates.ContainsKey(i))
                    continue;

                // Medžiagų koncentracija sumuojama skirtingai pagal medžiagos fizinę būseną.
                string physicalState = PhysicalStates[i];
                switch (physicalState)
                {
                    case "k":
                    case "s":
                        AddRespSensPercentile(ref respSens1Percentile, ref respSens1APercentile, ref respSens1BPercentile, i, classification);
                        break;
                    case "d":
                        AddRespSensPercentile(ref respSens1GasPercentile, ref respSens1AGasPercentile, ref respSens1BGasPercentile, i, classification);
                        break;
                    default:
                        continue;
                }
            }

            // Klasifikuojama pagal procentines dalis.
            if (IsAlmostGreaterOrEqual(1, respSens1Percentile) || IsAlmostGreaterOrEqual(0.2, respSens1GasPercentile) ||
                IsAlmostGreaterOrEqual(0.1, respSens1APercentile) || IsAlmostGreaterOrEqual(0.1, respSens1AGasPercentile) ||
                IsAlmostGreaterOrEqual(1, respSens1BPercentile) || IsAlmostGreaterOrEqual(0.2, respSens1BGasPercentile))
                return (new List<string> { "Resp. Sens. 1" }, new List<string>());

            return (new List<string>(), new List<string>());
        }

        /// <summary>
        /// Prideda medžiagos kiekį į tinkamą sumą pagal medžiagos klasifikaciją.
        /// </summary>
        private void AddRespSensPercentile(ref double respSens1Percentile, ref double respSens1APercentile, ref double respSens1BPercentile, int substanceIndex, string classification)
        {
            if (classification == "Resp. Sens. 1")
                respSens1Percentile += Input[substanceIndex].Percentile;
            else if (classification == "Resp. Sens. 1A")
                respSens1APercentile += Input[substanceIndex].Percentile;
            else if (classification == "Resp. Sens. 1B")
                respSens1BPercentile += Input[substanceIndex].Percentile;
        }

        /// <summary>
        /// Patikrina medžiagos klasifikavimą kaip Skin Sens.
        /// </summary>
        private ClassificationOutput ClassifySkinSens(IList<InputLine> skinSensitiveSubstances)
        {
            // Paskaičiuojamos medžiagų procentinės dalys mišinyje pagal klasifikaciją.
            double skinSens1Percentile = GetClassificationPercentile(skinSensitiveSubstances, "Skin Sens. 1");
            double skinSens1APercentile = GetClassificationPercentile(skinSensitiveSubstances, "Skin Sens. 1A");
            double skinSens1BPercentile = GetClassificationPercentile(skinSensitiveSubstances, "Skin Sens. 1B");

            // Klasifikuojama pagal procentines dalis.
            if (IsAlmostGreaterOrEqual(1, skinSens1Percentile) ||
                IsAlmostGreaterOrEqual(0.1, skinSens1APercentile) ||
                IsAlmostGreaterOrEqual(1, skinSens1BPercentile))
                return (new List<string> { "Skin Sens. 1" }, new List<string>());

            return (new List<string>(), new List<string>());
        }

        /// <summary>
        /// Patikrina medžiagos klasifikavimą kaip Aquatic.
        /// </summary>
        private ClassificationOutput ClassifyAquatic()
        {
            double aquaticAcute1Percentile = 0;
            double aquaticChronic1Percentile = 0;
            double aquaticChronic2Percentile = 0;
            double aquaticChronic3Percentile = 0;
            double aquaticChronic4Percentile = 0;

            // Paskaičiuojamos medžiagų procentinės dalys mišinyje pagal klasifikaciją.
            for (int i = 0; i < Input.Count; ++i)
            {
                string aquaticAcuteClassification = Input[i].Classification.FirstOrDefault(classification => classification.StartsWith("Aquatic Acute"));
                string aquaticChronicClassification = Input[i].Classification.FirstOrDefault(classification => classification.StartsWith("Aquatic Chronic"));
                double percentile = Input[i].Percentile;
                if (aquaticAcuteClassification != default && MFactors.ContainsKey(i))
                {
                    aquaticAcute1Percentile += MFactors[i] * percentile;
                }

                // Medžiagos sumuojamos skirtingai pagal M daugiklius ir klasifikaciją.
                if (aquaticAcuteClassification != default && MChronicFactors.ContainsKey(i))
                {
                    double mChronicFactor = MChronicFactors[i];
                    switch (aquaticChronicClassification)
                    {
                        case "Aquatic Chronic 1":
                            aquaticChronic1Percentile += mChronicFactor * percentile;
                            aquaticChronic2Percentile += 10 * mChronicFactor * percentile;
                            aquaticChronic3Percentile += 100 * mChronicFactor * percentile;
                            aquaticChronic4Percentile += percentile;
                            break;
                        case "Aquatic Chronic 2":
                            aquaticChronic2Percentile += percentile;
                            aquaticChronic3Percentile += 10 * percentile;
                            aquaticChronic4Percentile += percentile;
                            break;
                        case "Aquatic Chronic 3":
                            aquaticChronic3Percentile += percentile;
                            aquaticChronic4Percentile += percentile;
                            break;
                        case "Aquatic Chronic 4":
                            aquaticChronic4Percentile += percentile;
                            break;
                    }
                }
            }

            // Klasifikuojama pagal procentines dalis.
            var classification = new List<string>();
            if (IsAlmostGreaterOrEqual(25, aquaticAcute1Percentile))
                classification.Add("Aquatic Acute 1");

            if (IsAlmostGreaterOrEqual(25, aquaticChronic1Percentile))
                classification.Add("Aquatic Chronic 1");
            else if (IsAlmostGreaterOrEqual(25, aquaticChronic2Percentile))
                classification.Add("Aquatic Chronic 2");
            else if (IsAlmostGreaterOrEqual(25, aquaticChronic3Percentile))
                classification.Add("Aquatic Chronic 3");
            else if (IsAlmostGreaterOrEqual(25, aquaticChronic4Percentile))
                classification.Add("Aquatic Chronic 4");

            return (classification, new List<string>());
        }

        /// <summary>
        /// Susumuoja medžiagų su duota klasifikaciją procentines dalis.
        /// </summary>
        /// <param name="substances">Visos galimos medžiagos.</param>
        /// <param name="classification">Reikialinga medžiagos klasifikacija.</param>
        /// <returns></returns>
        private double GetClassificationPercentile(IList<InputLine> substances, string classification)
        {
            return substances.
                Where(substance => substance.Classification.
                    FirstOrDefault(substanceClassification => substanceClassification.StartsWith(classification)) != default)
                .Aggregate(0.0, (result, substance) => result + substance.Percentile);
        }

        /// <summary>
        /// Validuoja vartotojo įvestą skaičių.
        /// </summary>
        /// <param name="input">Vartotojo įvestas tekstas.</param>
        private (bool Result, string Error) ValidateDouble(string input)
        {
            if (!double.TryParse(input, out var _))
                return (false, "Įveskite skaičių.");

            return (true, "");
        }

        #endregion

        /// <summary>
        /// Patikrina, ar duota reikšmė yra mažesnė arba lygi (su paklaida).
        /// </summary>
        /// <param name="value">Duota reikšmė.</param>
        /// <param name="higherBound">Lyginama reikšmė.</param>
        private bool IsAlmostLessOrEqual(double value, double higherBound)
        {
            return value < higherBound + Epsilon;
        }

        /// <summary>
        /// Patikrina, ar duota reikšmė yra didesnė arba lygi (su paklaida).
        /// </summary>
        /// <param name="lowerBound">Lyginama reikšmė.</param>
        /// <param name="value">Duota reikšmė.</param>
        private bool IsAlmostGreaterOrEqual(double lowerBound, double value)
        {
            return value + Epsilon > lowerBound;
        }

        /// <summary>
        /// Patikrina ar duotos reikšmės yra lygios (su paklaida).
        /// </summary>
        private bool IsAlmostEqual(double lhs, double rhs)
        {
            return Math.Abs(lhs - rhs) < Epsilon;
        }

        /// <summary>
        /// Patikrina medžiagos klasifikavimą kaip Self-React.
        /// </summary>
        private ClassificationOutput ClassifySelfReact()
        {
            var classification = new List<string>();
            var errors = new List<string>() { "Savaime reaguojančių medžiagų ir mišinių klasifikavimui žiūrėti CLP 2.8 skyrių" };
            return (classification, errors);
        }

        /// <summary>
        /// Patikrina medžiagos klasifikavimą kaip Pyr. Liq.
        /// </summary>
        private ClassificationOutput ClassifyPyrLiq()
        {
            var classification = new List<string>();
            var errors = new List<string>() { "Piroforonių skysčių klasifikavimui žiūrėti CLP 2.9 skyrių" };
            return (classification, errors);
        }

        /// <summary>
        /// Patikrina medžiagos klasifikavimą kaip Pyr. Sol.
        /// </summary>
        private ClassificationOutput ClassifyPyrSol()
        {
            var classification = new List<string>();
            var errors = new List<string>() { "Piroforonių kietųjų medžiagų klasifikavimui žiūrėti CLP 2.10 skyrių" };
            return (classification, errors);
        }

        /// <summary>
        /// Patikrina medžiagos klasifikavimą kaip Self-heat.
        /// </summary>
        private ClassificationOutput ClassifySelfHeat()
        {
            var classification = new List<string>();
            var errors = new List<string>() { "Savaime kaistančių medžiagų klasifikavimui žiūrėti CLP 2.11 skyrių" };
            return (classification, errors);
        }

        /// <summary>
        /// Patikrina medžiagos klasifikavimą kaip Water-react.
        /// </summary>
        private ClassificationOutput ClassifyWaterReact()
        {
            var classification = new List<string>();
            var errors = new List<string>() { "Su vandeniu reguojančių medžiagų klasifikavimui žiūrėti CLP 2.12 skyrių" };
            return (classification, errors);
        }

        /// <summary>
        /// Patikrina medžiagos klasifikavimą kaip Ox. Liq.
        /// </summary>
        private ClassificationOutput ClassifyOxLiq()
        {
            var classification = new List<string>();
            var errors = new List<string>() { "Oksiduojančių skysčių klasifikavimui žiūrėti CLP 2.13 skyrių" };
            return (classification, errors);
        }

        /// <summary>
        /// Patikrina medžiagos klasifikavimą kaip Ox. Sol.
        /// </summary>
        private ClassificationOutput ClassifyOxSol()
        {
            var classification = new List<string>();
            var errors = new List<string>() { "Oksiduojančių kietųjų medžiagų klasifikavimui žiūrėti CLP 2.14 skyrių" };
            return (classification, errors);
        }

        /// <summary>
        /// Patikrina medžiagos klasifikavimą kaip Met. Corr.
        /// </summary>
        private ClassificationOutput ClassifyMetCorr()
        {
            var classification = new List<string>();
            var errors = new List<string>() { "Ėsdinančių medžiagų ir mišinių klasifikavimui žiūrėti CLP 2.16 skyrių" };
            return (classification, errors);
        }

        /// <summary>
        /// Patikrina medžiagos klasifikavimą kaip Muta
        /// </summary>
        private ClassificationOutput ClassifyMuta(IList<InputLine> mutaSubstances)
        {
            // Paskaičiuojamos medžiagų procentinės dalys mišinyje pagal klasifikaciją.
            double muta1Percentile = GetClassificationPercentile(mutaSubstances, "Muta. 1");
            double muta1APercentile = GetClassificationPercentile(mutaSubstances, "Muta. 1A");
            double muta1BPercentile = GetClassificationPercentile(mutaSubstances, "Muta. 1B");
            double muta2Percentile = GetClassificationPercentile(mutaSubstances, "Muta. 2");


            // Klasifikuojama pagal procentines dalis.
            if (IsAlmostGreaterOrEqual(0.1, muta1Percentile))
                return (new List<string> { "Muta. 1" }, new List<string>());

            if (IsAlmostGreaterOrEqual(0.1, muta1APercentile))
                return (new List<string> { "Muta. 1A" }, new List<string>());

            if (IsAlmostGreaterOrEqual(0.1, muta1BPercentile))
                return (new List<string> { "Muta. 1B" }, new List<string>());

            if (IsAlmostGreaterOrEqual(1, muta2Percentile))
                return (new List<string> { "Muta. 2" }, new List<string>());

            return (new List<string>(), new List<string>());
        }

        /// <summary>
        /// Patikrina medžiagos klasifikavimą kaip Carc
        /// </summary>
        private ClassificationOutput ClassifyCarc(IList<InputLine> mutaSubstances)
        {
            // Paskaičiuojamos medžiagų procentinės dalys mišinyje pagal klasifikaciją.
            double carc1Percentile = GetClassificationPercentile(mutaSubstances, "Carc. 1");
            double carc1APercentile = GetClassificationPercentile(mutaSubstances, "Carc. 1A");
            double carc1BPercentile = GetClassificationPercentile(mutaSubstances, "Carc. 1B");
            double carc2Percentile = GetClassificationPercentile(mutaSubstances, "Carc. 2");


            // Klasifikuojama pagal procentines dalis.
            if (IsAlmostGreaterOrEqual(0.1, carc1Percentile))
                return (new List<string> { "Carc. 1" }, new List<string>());

            if (IsAlmostGreaterOrEqual(0.1, carc1APercentile))
                return (new List<string> { "Carc. 1A" }, new List<string>());

            if (IsAlmostGreaterOrEqual(0.1, carc1BPercentile))
                return (new List<string> { "Carc. 1B" }, new List<string>());

            if (IsAlmostGreaterOrEqual(1, carc2Percentile))
                return (new List<string> { "Carc. 2" }, new List<string>());

            return (new List<string>(), new List<string>());
        }

        /// <summary>
        /// Patikrina medžiagos klasifikavimą kaip Rper.
        /// </summary>
        private ClassificationOutput ClassifyRepr(IList<InputLine> reprSubstances)
        {
            // Paskaičiuojamos medžiagų procentinės dalys mišinyje pagal klasifikaciją.
            double repr1Percentile = GetClassificationPercentile(reprSubstances, "Repr. 1");
            double repr1APercentile = GetClassificationPercentile(reprSubstances, "Repr. 1A");
            double repr1BPercentile = GetClassificationPercentile(reprSubstances, "Repr. 1B");
            double repr2Percentile = GetClassificationPercentile(reprSubstances, "Repr. 2");


            // Klasifikuojama pagal procentines dalis.
            if (IsAlmostGreaterOrEqual(0.3, repr1Percentile))
                return (new List<string> { "Repr. 1" }, new List<string>());

            if (IsAlmostGreaterOrEqual(0.3, repr1APercentile))
                return (new List<string> { "Repr. 1A" }, new List<string>());

            if (IsAlmostGreaterOrEqual(0.3, repr1BPercentile))
                return (new List<string> { "Repr. 1B" }, new List<string>());

            if (IsAlmostGreaterOrEqual(3, repr2Percentile))
                return (new List<string> { "Repr. 2" }, new List<string>());

            return (new List<string>(), new List<string>());
        }

        /// <summary>
        /// Patikrina medžiagos klasifikavimą kaip Lact.
        /// </summary>
        private ClassificationOutput ClassifyLact(IList<InputLine> lactSubstances)
        {
            // Paskaičiuojamos medžiagų procentinės dalys mišinyje pagal klasifikaciją.
            double lact1Percentile = GetClassificationPercentile(lactSubstances, "Lact.");


            // Klasifikuojama pagal procentines dalis.
            if (IsAlmostGreaterOrEqual(0.3, lact1Percentile))
                return (new List<string> { "Lact. 1" }, new List<string>());

            return (new List<string>(), new List<string>());
        }

        /// <summary>
        /// Patikrina medžiagos klasifikavimą kaip STOT SE
        /// </summary>
        private ClassificationOutput ClassifyStotSe(IList<InputLine> stotSeSubstances)
        {
            // Paskaičiuojamos medžiagų procentinės dalys mišinyje pagal klasifikaciją.
            double stotSe1Percentile = GetClassificationPercentile(stotSeSubstances, "STOT SE 1");
            double stotSe2Percentile = GetClassificationPercentile(stotSeSubstances, "STOT SE 2");
            double stotSe3Percentile = GetClassificationPercentile(stotSeSubstances, "STOT SE 3");

            // Klasifikuojama pagal procentines dalis.
            if (IsAlmostGreaterOrEqual(10, stotSe1Percentile))
                return (new List<string> { "STOT SE 1" }, new List<string>());

            if (IsAlmostGreaterOrEqual(1, stotSe1Percentile) && stotSe1Percentile<10)
                return (new List<string> { "STOT SE 2" }, new List<string>());

            if (IsAlmostGreaterOrEqual(10, stotSe2Percentile))
                return (new List<string> { "STOT SE 2" }, new List<string>());

            if (IsAlmostGreaterOrEqual(20, stotSe3Percentile))
                return (new List<string> { "STOT SE 3" }, new List<string>());

            return (new List<string>(), new List<string>());
        }

        /// <summary>
        /// Patikrina medžiagos klasifikavimą kaip STOT RE
        /// </summary>
        private ClassificationOutput ClassifyStotRe(IList<InputLine> stotReSubstances)
        {
            // Paskaičiuojamos medžiagų procentinės dalys mišinyje pagal klasifikaciją.
            double stotRe1Percentile = GetClassificationPercentile(stotReSubstances, "STOT RE 1");
            double stotRe2Percentile = GetClassificationPercentile(stotReSubstances, "STOT RE 2");

            // Klasifikuojama pagal procentines dalis.
            if (IsAlmostGreaterOrEqual(10, stotRe1Percentile))
                return (new List<string> { "STOT RE 1" }, new List<string>());

            if (IsAlmostGreaterOrEqual(1, stotRe1Percentile) && stotRe1Percentile < 10)
                return (new List<string> { "STOT SE 2" }, new List<string>());

            if (IsAlmostGreaterOrEqual(10, stotRe2Percentile))
                return (new List<string> { "STOT SE 2" }, new List<string>());

            return (new List<string>(), new List<string>());
        }

        /// <summary>
        /// Patikrina medžiagos klasifikavimą kaip Asp. Tox.
        /// </summary>
        private ClassificationOutput ClassifyAspTox(IList<InputLine> aspToxSubstances)
        {
            // Paskaičiuojamos medžiagų procentinės dalys mišinyje pagal klasifikaciją.
            double aspTox1Percentile = GetClassificationPercentile(aspToxSubstances, "Asp. Tox. 1");

            // Klasifikuojama pagal procentines dalis.
            if (IsAlmostGreaterOrEqual(10, aspTox1Percentile))
                return (new List<string> { "Asp. Tox. 1" }, new List<string>());

            return (new List<string>(), new List<string>());
        }

        /// <summary>
        /// Patikrina medžiagos klasifikavimą kaip Ozone
        /// </summary>
        private ClassificationOutput ClassifyOzone(IList<InputLine> ozoneSubstances)
        {
            // Paskaičiuojamos medžiagų procentinės dalys mišinyje pagal klasifikaciją.
            double ozonePercentile = GetClassificationPercentile(ozoneSubstances, "Ozone 1");

            // Klasifikuojama pagal procentines dalis.
            if (IsAlmostGreaterOrEqual(0.1, ozonePercentile))
                return (new List<string> { "Asp. Tox. 1" }, new List<string>());

            return (new List<string>(), new List<string>());
        }
    }
}
