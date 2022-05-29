using System.Collections.Generic;

namespace SubstanceClassifier
{
    /// <summary>
    /// Grąžina informaciją apie mišinio klasifikaciją.
    /// </summary>
    public class Codes
    {
        /// <summary>
        /// Grąžina informaciją apie mišinio klasifikaciją.
        /// </summary>
        /// <param name="classifications"></param>
        /// <returns></returns>
        public static CodesOutput GetCodes(IList<string> classifications)
        {
            var output = new CodesOutput();
            foreach (var classification in classifications)
            {
                switch (classification)
                {
                    case "Acute Tox. 1 (oral)":
                    case "Acute Tox. 2 (oral)":
                        output.GhsCodes.Add("GHS06");
                        output.SignalWords.Add("Pavojinga");
                        output.HCodes.Add("H300");
                        break;
                    case "Acute Tox. 3 (oral)":
                        output.GhsCodes.Add("GHS06");
                        output.SignalWords.Add("Pavojinga");
                        output.HCodes.Add("H301");
                        break;
                    case "Acute Tox. 4 (oral)":
                        output.GhsCodes.Add("GHS07");
                        output.SignalWords.Add("Atsargiai");
                        output.HCodes.Add("H302");
                        break;
                    case "Acute Tox. 1 (dermal)":
                    case "Acute Tox. 2 (dermal)":
                        output.GhsCodes.Add("GHS06");
                        output.SignalWords.Add("Pavojinga");
                        output.HCodes.Add("H310");
                        break;
                    case "Acute Tox. 3 (dermal)":
                        output.GhsCodes.Add("GHS06");
                        output.SignalWords.Add("Pavojinga");
                        output.HCodes.Add("H311");
                        break;
                    case "Acute Tox. 4 (dermal)":
                        output.GhsCodes.Add("GHS07");
                        output.SignalWords.Add("Atsargiai");
                        output.HCodes.Add("H312");
                        break;
                    case "Acute Tox. 1 (inhale)":
                    case "Acute Tox. 2 (inhale)":
                        output.GhsCodes.Add("GHS06");
                        output.SignalWords.Add("Pavojinga");
                        output.HCodes.Add("H330");
                        break;
                    case "Acute Tox. 3 (inhale)":
                        output.GhsCodes.Add("GHS06");
                        output.SignalWords.Add("Pavojinga");
                        output.HCodes.Add("H331");
                        break;
                    case "Acute Tox. 4 (inhale)":
                        output.GhsCodes.Add("GHS07");
                        output.SignalWords.Add("Atsargiai");
                        output.HCodes.Add("H332");
                        break;
                    case "Unst. Expl.":
                        output.GhsCodes.Add("GHS01");
                        output.SignalWords.Add("Pavojinga");
                        output.HCodes.Add("H200");
                        break;
                    case "Expl. 1.1":
                        output.GhsCodes.Add("GHS01");
                        output.SignalWords.Add("Pavojinga");
                        output.HCodes.Add("H201");
                        break;
                    case "Expl. 1.2":
                        output.GhsCodes.Add("GHS01");
                        output.SignalWords.Add("Pavojinga");
                        output.HCodes.Add("H202");
                        break;
                    case "Expl. 1.3":
                        output.GhsCodes.Add("GHS01");
                        output.SignalWords.Add("Pavojinga");
                        output.HCodes.Add("H203");
                        break;
                    case "Expl. 1.4":
                        output.GhsCodes.Add("GHS01");
                        output.SignalWords.Add("Atsargiai");
                        output.HCodes.Add("H204");
                        break;
                    case "Expl. 1.5":
                        output.SignalWords.Add("Pavojinga");
                        output.HCodes.Add("H205");
                        break;
                    case "Flam. Gas 1":
                        output.GhsCodes.Add("GHS02");
                        output.SignalWords.Add("Pavojinga");
                        output.HCodes.Add("H220");
                        break;
                    case "Flam. Gas 2":
                        output.SignalWords.Add("Atsargiai");
                        output.HCodes.Add("H221");
                        break;
                    case "Chem. Unst. Gas A":
                        output.HCodes.Add("H230");
                        break;
                    case "Chem. Unst. Gas B":
                        output.HCodes.Add("H231");
                        break;
                    case "Aerosol 1":
                        output.GhsCodes.Add("GHS02");
                        output.HCodes.Add("H222");
                        output.HCodes.Add("H229");
                        output.SignalWords.Add("Pavojinga");
                        break;
                    case "Aerosol 2":
                        output.GhsCodes.Add("GHS02");
                        output.HCodes.Add("H223");
                        output.HCodes.Add("H229");
                        output.SignalWords.Add("Atsargiai");
                        break;
                    case "Aerosol 3":
                        output.HCodes.Add("H229");
                        output.SignalWords.Add("Atsargiai");
                        break;
                    case "Ox. Gas 1":
                        output.GhsCodes.Add("GHS03");
                        output.HCodes.Add("H270");
                        output.SignalWords.Add("Pavojinga");
                        break;
                    case "Press. Gas":
                    case "Press. Gas (Comp.)":
                    case "Press. Gas (Liq.)":
                        output.GhsCodes.Add("GHS04");
                        output.HCodes.Add("H280");
                        output.SignalWords.Add("Atsargiai");
                        break;
                    case "Press. Gas (Ref. Liq.)":
                    case "Press. Gas (Diss.)":
                        output.GhsCodes.Add("GHS04");
                        output.HCodes.Add("H281");
                        output.SignalWords.Add("Atsargiai");
                        break;
                    case "Flam. Liq. 1":
                        output.GhsCodes.Add("GHS02");
                        output.SignalWords.Add("Pavojinga");
                        output.HCodes.Add("H224");
                        break;
                    case "Flam. Liq. 2":
                        output.GhsCodes.Add("GHS02");
                        output.SignalWords.Add("Pavojinga");
                        output.HCodes.Add("H225");
                        break;
                    case "Flam. Liq. 3":
                        output.GhsCodes.Add("GHS02");
                        output.SignalWords.Add("Atsargiai");
                        output.HCodes.Add("H226");
                        break;
                    case "Flam. Sol. 1":
                        output.GhsCodes.Add("GHS02");
                        output.SignalWords.Add("Pavojinga");
                        output.HCodes.Add("H228");
                        break;
                    case "Flam. Sol. 2":
                        output.GhsCodes.Add("GHS02");
                        output.SignalWords.Add("Atsargiai");
                        output.HCodes.Add("H228");
                        break;
                    case "Self-react. A":
                        output.GhsCodes.Add("GHS01");
                        output.SignalWords.Add("Pavojinga");
                        output.HCodes.Add("H240");
                        break;
                    case "Self-react. B":
                        output.GhsCodes.Add("GHS01");
                        output.GhsCodes.Add("GHS02");
                        output.SignalWords.Add("Pavojinga");
                        output.HCodes.Add("H241");
                        break;
                    case "Self-react. C":
                    case "Self-react. D":
                        output.GhsCodes.Add("GHS02");
                        output.SignalWords.Add("Pavojinga");
                        output.HCodes.Add("H242");
                        break;
                    case "Self-react. E":
                    case "Self-react. F":
                        output.GhsCodes.Add("GHS02");
                        output.SignalWords.Add("Atsargiai");
                        output.HCodes.Add("H242");
                        break;
                    case "Pyr. Liq. 1":
                    case "Pyr. Sol. 1":
                        output.GhsCodes.Add("GHS02");
                        output.SignalWords.Add("Pavojinga");
                        output.HCodes.Add("H250");
                        break;
                    case "Self-heat. 1":
                        output.GhsCodes.Add("GHS02");
                        output.SignalWords.Add("Pavojinga");
                        output.HCodes.Add("H251");
                        break;
                    case "Self-heat. 2":
                        output.GhsCodes.Add("GHS02");
                        output.SignalWords.Add("Pavojinga");
                        output.HCodes.Add("H252");
                        break;
                    case "Water-react. 1":
                        output.GhsCodes.Add("GHS02");
                        output.SignalWords.Add("Pavojinga");
                        output.HCodes.Add("H260");
                        break;
                    case "Water-react. 2":
                        output.GhsCodes.Add("GHS02");
                        output.SignalWords.Add("Pavojinga");
                        output.HCodes.Add("H261");
                        break;
                    case "Water-react. 3":
                        output.GhsCodes.Add("GHS02");
                        output.SignalWords.Add("Atsargiai");
                        output.HCodes.Add("H261");
                        break;
                    case "Ox. Liq. 1":
                    case "Ox. Sol. 1":
                        output.GhsCodes.Add("GHS03");
                        output.SignalWords.Add("Pavojinga");
                        output.HCodes.Add("H271");
                        break;
                    case "Ox. Liq. 2":
                    case "Ox. Sol. 2":
                        output.GhsCodes.Add("GHS03");
                        output.SignalWords.Add("Pavojinga");
                        output.HCodes.Add("H272");
                        break;
                    case "Ox. Liq. 3":
                    case "Ox. Sol. 3":
                        output.GhsCodes.Add("GHS03");
                        output.SignalWords.Add("Atsargiai");
                        output.HCodes.Add("H272");
                        break;
                    case "Met. Corr. 1":
                        output.GhsCodes.Add("GHS05");
                        output.SignalWords.Add("Atsargiai");
                        output.HCodes.Add("H290");
                        break;
                    case "Skin Corr. 1":
                    case "Skin Corr. 1A":
                    case "Skin Corr. 1B":
                    case "Skin Corr. 1C":
                        output.GhsCodes.Add("GHS05");
                        output.SignalWords.Add("Pavojinga");
                        output.HCodes.Add("H314");
                        break;
                    case "Skin Irrit. 2":
                        output.GhsCodes.Add("GHS07");
                        output.SignalWords.Add("Atsargiai");
                        output.HCodes.Add("H315");
                        break;
                    case "Eye Dam. 1":
                        output.GhsCodes.Add("GHS05");
                        output.SignalWords.Add("Pavojinga");
                        output.HCodes.Add("H318");
                        break;
                    case "Eye Irrit. 2":
                    case "Eye Irrit. 2A":
                    case "Eye Irrit. 2B":
                        output.GhsCodes.Add("GHS07");
                        output.SignalWords.Add("Atsargiai");
                        output.HCodes.Add("H319");
                        break;
                    case "Resp. Sens. 1":
                    case "Resp. Sens. 1A":
                    case "Resp. Sens. 1B":
                        output.GhsCodes.Add("GHS08");
                        output.SignalWords.Add("Pavojinga");
                        output.HCodes.Add("H334");
                        break;
                    case "Skin Sens. 1":
                    case "Skin Sens. 1A":
                    case "Skin Sens. 1B":
                        output.GhsCodes.Add("GHS07");
                        output.SignalWords.Add("Atsargiai");
                        output.HCodes.Add("H317");
                        break;
                    case "Muta. 1":
                    case "Muta. 1A":
                    case "Muta. 1B":
                        output.GhsCodes.Add("GHS08");
                        output.SignalWords.Add("Pavojinga");
                        output.HCodes.Add("H340");
                        break;
                    case "Muta. 2":
                        output.GhsCodes.Add("GHS08");
                        output.SignalWords.Add("Atsargiai");
                        output.HCodes.Add("H341");
                        break;
                    case "Carc. 1":
                    case "Carc. 1A":
                    case "Carc. 1B":
                        output.GhsCodes.Add("GHS08");
                        output.SignalWords.Add("Pavojinga");
                        output.HCodes.Add("H350");
                        break;
                    case "Carc. 2":
                        output.GhsCodes.Add("GHS08");
                        output.SignalWords.Add("Atsargiai");
                        output.HCodes.Add("H351");
                        break;
                    case "Repr. 1":
                    case "Repr. 1A":
                    case "Repr. 1B":
                        output.GhsCodes.Add("GHS08");
                        output.SignalWords.Add("Pavojinga");
                        output.HCodes.Add("H360");
                        break;
                    case "Repr. 2":
                        output.GhsCodes.Add("GHS08");
                        output.SignalWords.Add("Atsargiai");
                        output.HCodes.Add("H361");
                        break;
                    case "Lact.":
                        output.HCodes.Add("H362");
                        break;
                    case "STOT SE 1":
                        output.GhsCodes.Add("GHS08");
                        output.SignalWords.Add("Pavojinga");
                        output.HCodes.Add("H370");
                        break;
                    case "STOT SE 2":
                        output.GhsCodes.Add("GHS08");
                        output.SignalWords.Add("Atsargiai");
                        output.HCodes.Add("H371");
                        break;
                    case "STOT SE 3":
                        output.GhsCodes.Add("GHS07");
                        output.SignalWords.Add("Atsargiai");
                        output.HCodes.Add("H335");
                        output.HCodes.Add("H336");
                        break;
                    case "STOT RE 1":
                        output.GhsCodes.Add("GHS08");
                        output.SignalWords.Add("Pavojinga");
                        output.HCodes.Add("H372");
                        break;
                    case "STOT RE 2":
                        output.GhsCodes.Add("GHS08");
                        output.SignalWords.Add("Pavojinga");
                        output.HCodes.Add("H373");
                        break;
                    case "Asp. Tox. 1":
                    case "Asp. Tox. 2":
                        output.GhsCodes.Add("GHS08");
                        output.SignalWords.Add("Pavojinga");
                        output.HCodes.Add("H304");
                        break;
                    case "Aquatic Acute 1":
                        output.GhsCodes.Add("GHS09");
                        output.SignalWords.Add("Pavojinga");
                        output.HCodes.Add("H400");
                        break;
                    case "Aquatic Chronic 1":
                        output.GhsCodes.Add("GHS09");
                        output.SignalWords.Add("Atsargiai");
                        output.HCodes.Add("H410");
                        break;
                    case "Aquatic Chronic 2":
                        output.GhsCodes.Add("GHS09");
                        output.HCodes.Add("H411");
                        break;
                    case "Aquatic Chronic 3":
                        output.HCodes.Add("H412");
                        break;
                    case "Aquatic Chronic 4":
                        output.HCodes.Add("H413");
                        break;
                    case "Ozone 1":
                        output.GhsCodes.Add("GHS07");
                        output.SignalWords.Add("Atsargiai");
                        output.HCodes.Add("H420");
                        break;
                }
            }
            output.GhsCodes=output.GhsCodes.Distinct().ToList();
            output.HCodes=output.HCodes.Distinct().ToList();
            return output;
        }
    }

    /// <summary>
    /// Informacija apie mišinio klasifikavimą.
    /// </summary>
    public struct CodesOutput
    {
        /// <summary>
        /// GHS kodai.
        /// </summary>
        public List<string> GhsCodes = new List<string>();

        /// <summary>
        /// H kodai.
        /// </summary>
        public List<string> HCodes = new List<string>();

        /// <summary>
        /// P kodai.
        /// </summary>
        public List<string> PCodes { get => GetPCodes(); }

        /// <summary>
        /// H frazės.
        /// </summary>
        public List<string> HPhrases { get => GetHPhrases(); }

        /// <summary>
        /// P frazės.
        /// </summary>
        public List<string> PPhrases { get => GetPPhrases(); }

        /// <summary>
        /// Signalinis žodis.
        /// </summary>
        public string SignalWord { get => GetSignalWord(); }

        public CodesOutput()
        {
        }

        internal List<string> SignalWords = new List<string>();

        /// <summary>
        /// Grąžina P kodus pagal H kodus.
        /// </summary>
        private List<string> GetPCodes()
        {
            List<string> result = new();
            foreach (string code in HCodes)
            {
                switch (code)
                {
                    case "H200":
                        result.Add("P201");
                        result.Add("P250");
                        result.Add("P280");
                        result.Add("P370+P372+P380+P373");
                        result.Add("P401");
                        result.Add("P501");
                        break;
                    case "H201":
                    case "H202":
                    case "H203":
                    case "H205":
                        result.Add("P210");
                        result.Add("P230");
                        result.Add("P234");
                        result.Add("P240");
                        result.Add("P250");
                        result.Add("P280");
                        result.Add("P370+P372+P380+P373");
                        result.Add("P401");
                        result.Add("P501");
                        break;
                    case "H204":
                        result.Add("P210");
                        result.Add("P234");
                        result.Add("P240");
                        result.Add("P250");
                        result.Add("P280");
                        result.Add("P370+P372+P380+P373");
                        result.Add("P401");
                        result.Add("P501");
                        break;
                    case "H220":
                    case "H221":
                        result.Add("P210");
                        result.Add("P377");
                        result.Add("P381");
                        result.Add("P403");
                        break;
                    case "H230":
                    case "H231":
                        result.Add("P202");
                        break;
                    case "H222":
                    case "H223":
                        result.Add("P211");
                        result.Add("P210");
                        result.Add("P251");
                        result.Add("P410+P412");
                        break;
                    case "H229":
                        result.Add("P210");
                        result.Add("P410+P412");
                        break;
                    case "H270":
                        result.Add("P220");
                        result.Add("P244");
                        result.Add("P370+P376");
                        result.Add("P403");
                        break;
                    case "H280":
                        result.Add("P410+P403");
                        break;
                    case "H281":
                        result.Add("P282");
                        result.Add("P336+P315");
                        result.Add("P403");
                        break;
                    case "H224":
                    case "H225":
                    case "H226":
                        result.Add("P210");
                        result.Add("P233");
                        result.Add("P240");
                        result.Add("P241");
                        result.Add("P242");
                        result.Add("P243");
                        result.Add("P280");
                        result.Add("P303+P361+P353");
                        result.Add("P370+P378");
                        result.Add("P403+P235");
                        result.Add("P501");
                        break;
                    case "H228":
                        result.Add("P210");
                        result.Add("P240");
                        result.Add("P241");
                        result.Add("P280");
                        result.Add("P370+P378");
                        break;
                    case "H240":
                    case "H241":
                    case "H242":
                        result.Add("P210");
                        result.Add("P234");
                        result.Add("P235");
                        result.Add("P240");
                        result.Add("P242");
                        result.Add("P280");
                        result.Add("P370+P372+P380+P373");
                        result.Add("P403");
                        result.Add("P411");
                        result.Add("P420");
                        result.Add("P501");
                        break;
                    case "H250":
                        result.Add("P210");
                        result.Add("P222");
                        result.Add("P231+P232");
                        result.Add("P233");
                        result.Add("P280");
                        result.Add("P302+P334");
                        result.Add("P370+P378");
                        break;
                    case "H251":
                    case "H252":
                        result.Add("P235");
                        result.Add("P280");
                        result.Add("P407");
                        result.Add("P413");
                        result.Add("P420");
                        break;
                    case "H260":
                    case "H261":
                        result.Add("P223");
                        result.Add("P231+P232");
                        result.Add("P280");
                        result.Add("P302+P335+P334");
                        result.Add("P370+P378");
                        result.Add("P402+P404");
                        result.Add("P501");
                        break;
                    case "H271":
                        result.Add("P210");
                        result.Add("P220");
                        result.Add("P280");
                        result.Add("P283");
                        result.Add("P306+P360");
                        result.Add("P371+P380+P375");
                        result.Add("P370+P378");
                        result.Add("P420");
                        result.Add("P501");
                        break;
                    case "H272":
                        result.Add("P210");
                        result.Add("P220");
                        result.Add("P280");
                        result.Add("P370+P378");
                        result.Add("P501");
                        break;
                    case "H290":
                        result.Add("P234");
                        result.Add("P390");
                        result.Add("P406");
                        break;
                    case "H300":
                    case "H301":
                        result.Add("P264");
                        result.Add("P270");
                        result.Add("P301+P310");
                        result.Add("P321");
                        result.Add("P330");
                        result.Add("P405");
                        result.Add("P501");
                        break;
                    case "H302":
                        result.Add("P264");
                        result.Add("P270");
                        result.Add("P301+P312");
                        result.Add("P330");
                        result.Add("P501");
                        break;
                    case "H310":
                        result.Add("P262");
                        result.Add("P264");
                        result.Add("P270");
                        result.Add("P280");
                        result.Add("P302+P352");
                        result.Add("P310");
                        result.Add("P321");
                        result.Add("P361+P364");
                        result.Add("P405");
                        result.Add("P501");
                        break;
                    case "H311":
                        result.Add("P280");
                        result.Add("P302+P352");
                        result.Add("P312");
                        result.Add("P321");
                        result.Add("P361+P364");
                        result.Add("P405");
                        result.Add("P501");
                        break;
                    case "H312":
                        result.Add("P280");
                        result.Add("P302+P352");
                        result.Add("P312");
                        result.Add("P321");
                        result.Add("P362+P364");
                        result.Add("P501");
                        break;
                    case "H330":
                        result.Add("P260");
                        result.Add("P271");
                        result.Add("P284");
                        result.Add("P304+P340");
                        result.Add("P310");
                        result.Add("P320");
                        result.Add("P403+P233");
                        result.Add("P405");
                        result.Add("P501");
                        break;
                    case "H331":
                        result.Add("P261");
                        result.Add("P271");
                        result.Add("P304+P340");
                        result.Add("P311");
                        result.Add("P321");
                        result.Add("P403+P233");
                        result.Add("P405");
                        result.Add("P501");
                        break;
                    case "H332":
                        result.Add("P261");
                        result.Add("P271");
                        result.Add("P304+P340");
                        result.Add("P312");
                        break;
                    case "H314":
                        result.Add("P260");
                        result.Add("P264");
                        result.Add("P280");
                        result.Add("P301+P330+P331");
                        result.Add("P303+P361+P353");
                        result.Add("P363");
                        result.Add("P304+P340");
                        result.Add("P310");
                        result.Add("P321");
                        result.Add("P305+P351+P338");
                        result.Add("P405");
                        result.Add("P501");
                        break;
                    case "H315":
                        result.Add("P264");
                        result.Add("P280");
                        result.Add("P302+P352");
                        result.Add("P321");
                        result.Add("P332+P313");
                        result.Add("P362+P364");
                        break;
                    case "H318":
                        result.Add("P280");
                        result.Add("P305+P351+P338");
                        result.Add("P310");
                        break;
                    case "H319":
                        result.Add("P264");
                        result.Add("P280");
                        result.Add("P305+P351+P338");
                        result.Add("P337+P313");
                        break;
                    case "H334":
                        result.Add("P261");
                        result.Add("P284");
                        result.Add("P304+P340");
                        result.Add("P342+P311");
                        result.Add("P501");
                        break;
                    case "H317":
                        result.Add("P261");
                        result.Add("P272");
                        result.Add("P280");
                        result.Add("P302+P352");
                        result.Add("P333+P313");
                        result.Add("P321");
                        result.Add("P362+P364");
                        result.Add("P501");
                        break;
                    case "H340":
                    case "H341":
                    case "H350":
                    case "H351":
                    case "H360":
                    case "H361":
                        result.Add("P201");
                        result.Add("P202");
                        result.Add("P280");
                        result.Add("P308+P313");
                        result.Add("P405");
                        result.Add("P501");
                        break;
                    case "H362":
                        result.Add("P201");
                        result.Add("P260");
                        result.Add("P263");
                        result.Add("P264");
                        result.Add("P270");
                        result.Add("P308+P313");
                        break;
                    case "H370":
                        result.Add("P260");
                        result.Add("P264");
                        result.Add("P270");
                        result.Add("P308+P311");
                        result.Add("P321");
                        result.Add("P405");
                        result.Add("P501");
                        break;
                    case "H371":
                        result.Add("P260");
                        result.Add("P264");
                        result.Add("P270");
                        result.Add("P308+P311");
                        result.Add("P405");
                        result.Add("P501");
                        break;
                    case "H335":
                        result.Add("P261");
                        result.Add("P271");
                        result.Add("P304+P340");
                        result.Add("P312");
                        result.Add("P403+P233");
                        result.Add("P405");
                        result.Add("P501");
                        break;
                    case "H372":
                        result.Add("P260");
                        result.Add("P264");
                        result.Add("P270");
                        result.Add("P314");
                        result.Add("P501");
                        break;
                    case "H373":
                        result.Add("P260");
                        result.Add("P314");
                        result.Add("P501");
                        break;
                    case "H304":
                        result.Add("P301+P310");
                        result.Add("P331");
                        result.Add("P405");
                        result.Add("P501");
                        break;
                    case "H400":
                    case "H410":
                    case "H411":
                        result.Add("P273");
                        result.Add("P391");
                        result.Add("P501");
                        break;
                    case "H412":
                    case "H413":
                        result.Add("P273");
                        result.Add("P501");
                        break;
                    case "H420":
                        result.Add("P502");
                        break;
                }
            }
            return result.Distinct().ToList();
        }

        /// <summary>
        /// Grąžina H frazes pagal H kodus.
        /// </summary>
        private List<string> GetHPhrases()
        {
            List<string> result = new();
            foreach (string code in HCodes)
            {
                switch (code)
                {
                    case "H200":
                        result.Add("Nestabilios sprogios medžiagos");
                        break;
                    case "H201":
                        result.Add("Sprogios medžiagos, kelia masinio sprogimo pavojų");
                        break;
                    case "H202":
                        result.Add("Sprogios medžiagos, kelia didelį išsvaidymo pavojų");
                        break;
                    case "H203":
                        result.Add("Sprogios medžiagos, kelia gaisro, sprogimo ar išsvaidymo pavojų");
                        break;
                    case "H204":
                        result.Add("Gaisro arba išsvaidymo pavojus");
                        break;
                    case "H205":
                        result.Add("Per gaisrą gali sukelti masinį sprogimą");
                        break;
                    case "H220":
                        result.Add("Ypač degios dujos");
                        break;
                    case "H221":
                        result.Add("Degios dujos");
                        break;
                    case "H222":
                        result.Add("Ypač degus aerozolis");
                        break;
                    case "H223":
                        result.Add("Degus aerozolis");
                        break;
                    case "H224":
                        result.Add("Ypač degūs skystis ir garai");
                        break;
                    case "H225":
                        result.Add("Labai degūs skystis ir garai");
                        break;
                    case "H226":
                        result.Add("Degūs skystis ir garai");
                        break;
                    case "H228":
                        result.Add("Degi kietoji medžiaga");
                        break;
                    case "H229":
                        result.Add("Slėginė talpykla. Kaitinama gali sprogti");
                        break;
                    case "H230":
                        result.Add("Gali tapti sprogiomis net ir nesant oro");
                        break;
                    case "H231":
                        result.Add("Gali tapti sprogiomis net ir nesant oro, esant didesniam slėgiui ir (arba) temperatūrai");
                        break;
                    case "H240":
                        result.Add("Kaitinant gali sprogti");
                        break;
                    case "H241":
                        result.Add("Kaitinant gali sukelti gaisrą arba sprogti");
                        break;
                    case "H242":
                        result.Add("Kaitinant gali sukelti gaisrą");
                        break;
                    case "H250":
                        result.Add("Veikiami oro savaime užsidega");
                        break;
                    case "H251":
                        result.Add("Savaime kaistančios, gali užsidegti");
                        break;
                    case "H252":
                        result.Add("Laikant dideliais kiekiais savaime kaista, gali užsidegti");
                        break;
                    case "H260":
                        result.Add("Kontaktuodami su vandeniu išskiria degias dujas, kurios gali savaime užsidegti");
                        break;
                    case "H261":
                        result.Add("Kontaktuodami su vandeniu išskiria degias dujas");
                        break;
                    case "H270":
                        result.Add("Gali sukelti arba padidinti gaisrą, oksidatorius");
                        break;
                    case "H271":
                        result.Add("Gali sukelti gaisrą arba sprogimą, stiprus oksidatorius");
                        break;
                    case "H272":
                        result.Add("Gali padidinti gaisrą, oksidatorius");
                        break;
                    case "H280":
                        result.Add("Turi slėgio veikiamų dujų, kaitinant gali sprogti");
                        break;
                    case "H281":
                        result.Add("Turi atšaldytų dujų, gali sukelti kriogeninius nušalimus arba pažeidimus");
                        break;
                    case "H290":
                        result.Add("Gali ėsdinti metalus");
                        break;
                    case "H300":
                        result.Add("Mirtina prarijus");
                        break;
                    case "H301":
                        result.Add("Toksiška prarijus");
                        break;
                    case "H302":
                        result.Add("Kenksminga prarijus");
                        break;
                    case "H304":
                        result.Add("Prarijus ir patekus į kvėpavimo takus, gali sukelti mirtį");
                        break;
                    case "H310":
                        result.Add("Mirtina susilietus su oda");
                        break;
                    case "H311":
                        result.Add("Toksiška susilietus su oda");
                        break;
                    case "H312":
                        result.Add("Kenksminga susilietus su oda");
                        break;
                    case "H314":
                        result.Add("Smarkiai nudegina odą ir pažeidžia akis");
                        break;
                    case "H315":
                        result.Add("Dirgina odą");
                        break;
                    case "H317":
                        result.Add("Gali sukelti alerginę odos reakciją");
                        break;
                    case "H318":
                        result.Add("Smarkiai pažeidžia akis");
                        break;
                    case "H319":
                        result.Add("Sukelia smarkų akių dirginimą");
                        break;
                    case "H330":
                        result.Add("Mirtina įkvėpus");
                        break;
                    case "H331":
                        result.Add("Toksiška įkvėpus");
                        break;
                    case "H332":
                        result.Add("Kenksminga įkvėpus");
                        break;
                    case "H334":
                        result.Add("Įkvėpus gali sukelti alerginę reakciją, astmos simptomus arba apsunkinti kvėpavimą");
                        break;
                    case "H335":
                        result.Add("Gali dirginti kvėpavimo takus");
                        break;
                    case "H336":
                        result.Add("Gali sukelti mieguistumą arba galvos svaigimą");
                        break;
                    case "H340":
                        result.Add("Gali sukelti genetinius defektus");
                        break;
                    case "H341":
                        result.Add("Įtariama, kad gali sukelti genetinius defektus");
                        break;
                    case "H350":
                        result.Add("Gali sukelti vėžį");
                        break;
                    case "H351":
                        result.Add("Įtariama, kad sukelia vėžį");
                        break;
                    case "H360":
                        result.Add("Gali pakenkti vaisingumui arba negimusiam vaikui");
                        break;
                    case "H361":
                        result.Add("Įtariama, kad kenkia vaisingumui arba negimusiam vaikui");
                        break;
                    case "H362":
                        result.Add("Gali pakenkti žindomam vaikui");
                        break;
                    case "H370":
                        result.Add("Kenkia organams");
                        break;
                    case "H371":
                        result.Add("Gali pakenkti organams");
                        break;
                    case "H372":
                        result.Add("Kenkia organams, jeigu medžiaga veikia ilgai arba kartotinai");
                        break;
                    case "H373":
                        result.Add("Gali pakenkti organams, jeigu medžiaga veikia ilgai arba kartotinai");
                        break;
                    case "H400":
                        result.Add("Labai toksiška vandens organizmams");
                        break;
                    case "H410":
                        result.Add("Labai toksiška vandens organizmams, sukelia ilgalaikius pakitimus");
                        break;
                    case "H411":
                        result.Add("Toksiška vandens organizmams, sukelia ilgalaikius pakitimus");
                        break;
                    case "H412":
                        result.Add("Kenksminga vandens organizmams, sukelia ilgalaikius pakitimus");
                        break;
                    case "H413":
                        result.Add("Gali sukelti ilgalaikį kenksmingą poveikį vandens organizmams");
                        break;
                }
            }
            return result.Distinct().ToList();
        }

        /// <summary>
        /// Grąžina P frazes pagal P kodus.
        /// </summary>
        /// <returns></returns>
        private List<string> GetPPhrases()
        {
            List<string> result = new();
            foreach (string code in PCodes)
            {
                switch (code)
                {
                    case "P101":
                        result.Add("Jei reikalinga gydytojo konsultacija, su savimi turėkite produkto talpyklą arba jo etiketę");
                        break;
                    case "P102":
                        result.Add("Laikyti vaikams neprieinamoje vietoje");
                        break;
                    case "P103":
                        result.Add("Prieš naudojimą perskaityti etiketę");
                        break;
                    case "P201":
                        result.Add("Prieš naudojimą gauti specialias instrukcijas");
                        break;
                    case "P202":
                        result.Add("Nenaudoti, jeigu neperskaityti ar nesuprasti visi saugos įspėjimai");
                        break;
                    case "P210":
                        result.Add("Laikyti atokiau nuo šilumos šaltinių, karštų paviršių, žiežirbų, atviros liepsnos arba kitų degimo šaltinių. Nerūkyti");
                        break;
                    case "P211":
                        result.Add("Nepurkšti į atvirą liepsną arba kitus degimo šaltinius");
                        break;
                    case "P220":
                        result.Add("Laikyti atokiau nuo drabužių bei kitų degiųjų medžiagų");
                        break;
                    case "P221":
                        result.Add("Imtis visų atsargumo priemonių, kad nebūtų sumaišyta su degiomis medžiagomis");
                        break;
                    case "P222":
                        result.Add("Saugoti nuo kontakto su oru");
                        break;
                    case "P223":
                        result.Add("Saugoti nuo bet kokio galimo kontakto su vandeniu, nes smarkiai reaguoja ir gali susidaryti ugnies pliūpsnis");
                        break;
                    case "P230":
                        result.Add("Laikyti sudrėkintą vandeniu");
                        break;
                    case "P231":
                        result.Add("Tvarkyti inertinėse dujose");
                        break;
                    case "P232":
                        result.Add("Saugoti nuo drėgmės");
                        break;
                    case "P233":
                        result.Add("Talpyklą laikyti sandariai uždarytą");
                        break;
                    case "P234":
                        result.Add("Laikyti tik originalioje talpykloje");
                        break;
                    case "P235":
                        result.Add("Laikyti vėsioje vietoje");
                        break;
                    case "P240":
                        result.Add("Įžeminti/įtvirtinti talpyklą ir priėmimo įrangą");
                        break;
                    case "P241":
                        result.Add("Naudoti sprogimui atsparią elektros/ventiliacijos/apšvietimo/.../ įrangą");
                        break;
                    case "P242":
                        result.Add("Naudoti tik kibirkščių nekeliančius įrankius");
                        break;
                    case "P243":
                        result.Add("Imtis atsargumo priemonių statinei iškrovai išvengti");
                        break;
                    case "P244":
                        result.Add("Saugoti, kad ant redukcinių vožtuvų nepatektų riebalų ir tepalų");
                        break;
                    case "P250":
                        result.Add("Nešlifuoti/netrankyti/.../netrinti");
                        break;
                    case "P251":
                        result.Add("Nepradurti ir nedeginti net panaudoto");
                        break;
                    case "P260":
                        result.Add("Neįkvėpti dulkių/dūmų/dujų/rūko/garų/aerozolio");
                        break;
                    case "P261":
                        result.Add("Stengtis neįkvėpti dulkių/dūmų/dujų/rūko/garų/aerozolio");
                        break;
                    case "P262":
                        result.Add("Saugotis, kad nepatektų į akis, ant odos ar drabužių");
                        break;
                    case "P263":
                        result.Add("Vengti kontakto nėštumo metu/maitinant krūtimi");
                        break;
                    case "P264":
                        result.Add("Po naudojimo kruopščiai nuplauti...");
                        break;
                    case "P270":
                        result.Add("Naudojant šį produktą, nevalgyti, negerti ir nerūkyti");
                        break;
                    case "P271":
                        result.Add("Naudoti tik lauke arba gerai vėdinamoje patalpoje");
                        break;
                    case "P272":
                        result.Add("Užterštų darbo drabužių negalima išnešti iš darbo vietos");
                        break;
                    case "P273":
                        result.Add("Saugoti, kad nepatektų į aplinką");
                        break;
                    case "P280":
                        result.Add("Mūvėti apsaugines pirštines/dėvėti apsauginius drabužius/naudoti akių (veido) apsaugos priemones");
                        break;
                    case "P281":
                        result.Add("Naudoti reikalaujamas asmenines apsaugos priemones");
                        break;
                    case "P282":
                        result.Add("Mūvėti nuo šalčio izoliuojančias pirštines/naudoti veido skydelį/akių apsaugos priemones");
                        break;
                    case "P283":
                        result.Add("Dėvėti ugniai/liepsnai atsparius/antipireninius drabužius");
                        break;
                    case "P284":
                        result.Add("Naudoti kvėpavimo takų apsaugos priemones");
                        break;
                    case "P285":
                        result.Add("Esant nepakankamam vėdinimui, naudoti kvėpavimo takų apsaugos priemones");
                        break;
                    case "P231+P232":
                        result.Add("Tvarkyti inertinėse dujose. Saugoti nuo drėgmės");
                        break;
                    case "P235+P410":
                        result.Add("Laikyti vėsioje vietoje. Saugoti nuo saulės šviesos");
                        break;
                    case "P310":
                        result.Add("Nedelsiant skambinti į APSINUODIJIMŲ KONTROLĖS IR INFORMACIJOS BIURĄ arba kreiptis į gydytoją");
                        break;
                    case "P311":
                        result.Add("Skambinti į APSINUODIJIMŲ KONTROLĖS IR INFORMACIJOS BIURĄ arba kreiptis į gydytoją");
                        break;
                    case "P312":
                        result.Add("Pasijutus blogai, skambinti į APSINUODIJIMŲ KONTROLĖS IR INFORMACIJOS BIURĄ arba kreiptis į gydytoją");
                        break;
                    case "P313":
                        result.Add("Kreiptis į gydytoją");
                        break;
                    case "P314":
                        result.Add("Pasijutus blogai, kreiptis į gydytoją");
                        break;
                    case "P315":
                        result.Add("Nedelsiant kreiptis į gydytoją");
                        break;
                    case "P320":
                        result.Add("Būtinas skubus specialus gydymas (žr. ... šioje etiketėje)");
                        break;
                    case "P321":
                        result.Add("Specialus gydymas (žr. ... šioje etiketėje)");
                        break;
                    case "P322":
                        result.Add("Specialios priemonės (žr. ... šioje etiketėje)");
                        break;
                    case "P330":
                        result.Add("Išskalauti burną");
                        break;
                    case "P331":
                        result.Add("NESKATINTI vėmimo");
                        break;
                    case "P332":
                        result.Add("Jeigu sudirginama oda:");
                        break;
                    case "P333":
                        result.Add("Jeigu sudirginama oda arba ją išberia");
                        break;
                    case "P334":
                        result.Add("Įmerkti į vėsų vandenį/apvynioti šlapiais tvarsčiais");
                        break;
                    case "P335":
                        result.Add("Neprilipusias daleles nuvalyti nuo odos");
                        break;
                    case "P336":
                        result.Add("Prišalusias daleles atitirpinti drungnu vandeniu. Netrinti paveiktos zonos");
                        break;
                    case "P337":
                        result.Add("Jei akių dirginimas nepraeina:");
                        break;
                    case "P338":
                        result.Add("Išimti kontaktinius lęšius, jeigu jie yra ir jeigu lengvai galima tai padaryti. Toliau plauti akis");
                        break;
                    case "P340":
                        result.Add("Išnešti nukentėjusįjį į gryną orą: jam būtina ramybė ir padėtis, leidžianti laisvai kvėpuoti");
                        break;
                    case "P341":
                        result.Add("Jeigu nukentėjusiajam sunku kvėpuoti, išnešti jį į gryną orą; jam būtina ramybė ir padėtis, leidžianti laisvai kvėpuoti");
                        break;
                    case "P342":
                        result.Add("Jeigu pasireiškia respiraciniai simptomai:");
                        break;
                    case "P350":
                        result.Add("Atsargiai nuplauti dideliu kiekiu muilo ir vandens");
                        break;
                    case "P351":
                        result.Add("Atsargiai plauti vandeniu kelias minutes");
                        break;
                    case "P352":
                        result.Add("Plauti dideliu kiekiu muilo ir vandens");
                        break;
                    case "P353":
                        result.Add("Odą nuplauti vandeniu/čiurkšle");
                        break;
                    case "P360":
                        result.Add("Prieš nuvelkant užterštus drabužius, nedelsiant juos ir odą nuplauti dideliu kiekiu vandens");
                        break;
                    case "P361":
                        result.Add("Nedelsiant nuvilkti/pašalinti visus užterštus drabužius");
                        break;
                    case "P362":
                        result.Add("Nusivilkti užterštus drabužius ir išskalbti prieš vėl juos apsivelkant");
                        break;
                    case "P363":
                        result.Add("Užterštus drabužius išskalbti prieš vėl juos apsivelkant");
                        break;
                    case "P372":
                        result.Add("Sprogimo pavojus gaisro atveju");
                        break;
                    case "P373":
                        result.Add("NEGESINTI gaisro, jeigu ugnis pasiekia sprogmenis");
                        break;
                    case "P374":
                        result.Add("Gaisrą gesinti laikantis įprastinio atsargumo pakankamu atstumu");
                        break;
                    case "P375":
                        result.Add("Gaisrą gesinti iš toli dėl sprogimo pavojaus");
                        break;
                    case "P376":
                        result.Add("Sustabdyti nuotėkį, jeigu galima saugiai tai padaryti");
                        break;
                    case "P377":
                        result.Add("Dujų nuotėkio sukeltas gaisras: Negesinti, nebent nuotėkį būtų galima saugiai sustabdyti");
                        break;
                    case "P378":
                        result.Add("Gesinimui naudoti...");
                        break;
                    case "P380":
                        result.Add("Evakuoti zoną");
                        break;
                    case "P381":
                        result.Add("Pašalinti visus uždegimo šaltinius, jeigu galima saugiai tai padaryti");
                        break;
                    case "P390":
                        result.Add("Absorbuoti išsiliejusią medžiagą, siekiant išvengti materialinės žalos");
                        break;
                    case "P391":
                        result.Add("Surinkti ištekėjusią medžiagą");
                        break;
                    case "P301+P310":
                        result.Add("PRARIJUS: Nedelsiant skambinti į APSINUODIJIMŲ KONTROLĖS IR INFORMACIJOS BIURĄ arba kreiptis į gydytoją");
                        break;
                    case "P301+P312":
                        result.Add("PRARIJUS: Pasijutus blogai, skambinti į APSINUODIJIMŲ KONTROLĖS IR INFORMACIJOS BIURĄ arba kreiptis į gydytoją");
                        break;
                    case "P301+P330+P331":
                        result.Add("PRARIJUS: išskalauti burną. NESKATINTI vėmimo");
                        break;
                    case "P302+P334":
                        result.Add("PATEKUS ANT ODOS: Įmerkti į vėsų vandenį/apvynioti šlapiais tvarsčiais");
                        break;
                    case "P302+P350":
                        result.Add("PATEKUS ANT ODOS: Atsargiai nuplauti dideliu kiekiu muilo ir vandens");
                        break;
                    case "P302+P352":
                        result.Add("PATEKUS ANT ODOS: nuplauti dideliu kiekiu muilo ir vandens");
                        break;
                    case "P303+P361+P353":
                        result.Add("PATEKUS ANT ODOS (arba plaukų): Nedelsiant nuvilkti/pašalinti visus užterštus drabužius. Odą nuplauti vandeniu / čiurkšle");
                        break;
                    case "P304+P340":
                        result.Add("ĮKVĖPUS: Išnešti nukentėjusįjį į gryną orą; jam būtina ramybė ir padėtis, leidžianti laisvai kvėpuoti");
                        break;
                    case "P304+P341":
                        result.Add("ĮKVĖPUS: Jeigu nukentėjusiajam sunku kvėpuoti, išnešti jį į gryną orą; jam būtina ramybė ir padėtis, leidžianti laisvai kvėpuoti");
                        break;
                    case "P305+P351+P338":
                        result.Add("PATEKUS Į AKIS: Kelias minutes atsargiai plauti vandeniu. Išimti kontaktinius lęšius, jeigu jie yra ir jeigu lengvai galima tai padaryti.Toliau plauti akis");
                        break;
                    case "P306+P360":
                        result.Add("PATEKUS ANT DRABUŽIŲ: Prieš nuvelkant užterštus drabužius, nedelsiant juos ir odą nuplauti dideliu kiekiu vandens");
                        break;
                    case "P307+P311":
                        result.Add("Esant sąlyčiui: Skambinti į APSINUODIJIMŲ KONTROLĖS IR INFORMACIJOS BIURĄ arba kreiptis į gydytoją");
                        break;
                    case "P308+P313":
                        result.Add("Esant sąlyčiui arba jeigu numanomas sąlytis: kreiptis į gydytoją");
                        break;
                    case "P309+P311":
                        result.Add("Esant sąlyčiui arba pasijutus blogai: Skambinti į APSINUODIJIMŲ KONTROLĖS IR INFORMACIJOS BIURĄ arba kreiptis į gydytoją");
                        break;
                    case "P332+P313":
                        result.Add("Jeigu sudirginama oda: kreiptis į gydytoją");
                        break;
                    case "P333+P313":
                        result.Add("Jeigu sudirginama oda arba ją išberia: kreiptis į gydytoją");
                        break;
                    case "P335+P334":
                        result.Add("Neprilipusias daleles nuvalyti nuo odos. Įmerkti į vėsų vandenį/apvynioti šlapiais rankšluosčiais");
                        break;
                    case "P337+P313":
                        result.Add("Jeigu akių dirginimas nepraeina: kreiptis į gydytoją");
                        break;
                    case "P342+P311":
                        result.Add("Jeigu pasireiškia respiraciniai simptomai: skambinti į APSINUODIJIMŲ KONTROLĖS IR INFORMACIJOS BIURĄ arba kreiptis į gydytoją");
                        break;
                    case "P370+P376":
                        result.Add("Gaisro atveju: sustabdyti nuotėkį, jeigu galima saugiai tai padaryti");
                        break;
                    case "P370+P378":
                        result.Add("Gaisro atveju: gesinimui naudoti...");
                        break;
                    case "P370+P380":
                        result.Add("Gaisro atveju: evakuoti zoną");
                        break;
                    case "P370+P380+P375":
                        result.Add("Gaisro atveju: evakuoti zoną. Gaisrą gesinti iš toli dėl sprogimo pavojaus");
                        break;
                    case "P371+P380+P375":
                        result.Add("Didelio gaisro ir didelių kiekių atveju: evakuoti zoną. Gaisrą gesinti iš toli dėl sprogimo pavojaus");
                        break;
                    case "P401":
                        result.Add("Laikyti...");
                        break;
                    case "P402":
                        result.Add("Laikyti sausoje vietoje");
                        break;
                    case "P403":
                        result.Add("Laikyti gerai vėdinamoje vietoje");
                        break;
                    case "P404":
                        result.Add("Laikyti uždaroje talpykloje");
                        break;
                    case "P405":
                        result.Add("Laikyti užrakintą");
                        break;
                    case "P406":
                        result.Add("Laikyti korozijai atsparioje talpykloje/... turinčioje atsparią vidinę dangą");
                        break;
                    case "P407":
                        result.Add("Palikti oro tarpą tarp eilių/palečių");
                        break;
                    case "P410":
                        result.Add("Saugoti nuo saulės šviesos");
                        break;
                    case "P411":
                        result.Add("Laikyti ne aukštesnėje kaip...°C/...°F temperatūroje");
                        break;
                    case "P412":
                        result.Add("Nelaikyti aukštesnėje kaip 50 °C/122 °F temperatūroje");
                        break;
                    case "P413":
                        result.Add("Didesnius kaip ... kg/... lbs medžiagos kiekius laikyti ne aukštesnėje kaip...°C/...°F temperatūroje");
                        break;
                    case "P420":
                        result.Add("Laikyti atokiau nuo kitų medžiagų");
                        break;
                    case "P402+P404":
                        result.Add("Laikyti sausoje vietoje. Laikyti uždarytoje talpykloje");
                        break;
                    case "P403+P233":
                        result.Add("Laikyti gerai vėdinamoje vietoje. Talpyklą laikyti sandariai uždarytą");
                        break;
                    case "P403+P235":
                        result.Add("Laikyti gerai vėdinamoje vietoje. Laikyti vėsioje vietoje");
                        break;
                    case "P410+P403":
                        result.Add("Saugoti nuo saulės šviesos. Laikyti gerai vėdinamoje vietoje");
                        break;
                    case "P410+P412":
                        result.Add("Saugoti nuo saulės šviesos. Nelaikyti aukštesnėje kaip 50 °C/122 °F temperatūroje");
                        break;
                    case "P411+P235":
                        result.Add("Laikyti ne aukštesnėje kaip...°C/...°F temperatūroje. Laikyti vėsioje vietoje");
                        break;
                    case "P501":
                        result.Add("Turinį/talpyklą išmesti į ...");
                        break;
                }
            }
            return result.Distinct().ToList();
        }

        /// <summary>
        /// Grąžina signalinį žodį.
        /// </summary>
        /// <returns></returns>
        private string GetSignalWord()
        {
            if (SignalWords.Contains("Pavojinga"))
                return "Pavojinga";

            if (SignalWords.Contains("Atsargiai"))
                return "Atsargiai";

            return "Nėra";
        }
    }
}

