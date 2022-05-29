using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Windows.Forms;
using SubstanceClassifier;

namespace SubstanceClassifierApp
{
    /// <summary>
    /// Pagrindinis langas
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// Lokali duomenų bazė.
        /// </summary>
        internal static SubstanceDb Db = new();

        /// <summary>
        /// Įvesties eilutės.
        /// </summary>
        internal static List<(CASTextBox CASNumber, PercentileTextBox Percentile, ClassificationTextBox Classification)> Input { get; } = new();

        /// <summary>
        /// Ar duomenų bazė yra atnaujinama
        /// </summary>
        private bool isUpdatingDb = false;

        /// <summary>
        /// Ar duomenų bazės atnaujinimas buvo atšauktas
        /// </summary>
        private bool isCancelled = false;

        public MainForm()
        {
            InitializeComponent();
            AddInputRow(inputGroup, 30); // Pridedame vieną įvesties eilutę.
        }

        /// <summary>
        /// Atidaro duomenų bazės peržiūros langą.
        /// </summary>
        private void viewDbButton_Click(object sender, EventArgs e)
        {
            DbViewer form = new();
            form.Show();
        }

        /// <summary>
        /// Atnaujina duomenų bazę.
        /// </summary>
        private async void updateDbButton_Click(object sender, EventArgs e)
        {
            // Jei duomenų bazė yra atnaujinama, operacija buvo atšaukta.
            if (isUpdatingDb)
            {
                isCancelled = true;
                return;
            }

            // Įsitikiname, kad vartotojas tikrai nori atnaujinti duomenų bazę.
            DialogResult dialogResult = MessageBox.Show("Duomenų bazės atnaujinimas gali užtrukti. Ar tikrai norite pradėti operaciją", "Įspėjimas", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.No)
            {
                CleanUpAfterUpdate();
                return;
            }

            // Pakeičiame vartotojo sąsają į duomenų bazės atnaujinimo būseną.
            SetUpForUpdate();

            try
            {
                // Demonstraciniams tikslams galime apriboti kraunamų duomenų kiekį.
                int maxPages = 0;
                if (int.TryParse(ConfigurationSettings.AppSettings["MaxEchaDbPages"], out int result))
                    maxPages = result;

                // Atnaujiname duomenų bazę.
                await Db.UpdateDb(OnProgress, maxPages);
            }
            catch (HttpRequestException error)
            {
                MessageBox.Show($"Klaida laukiant atsakymo iš duomenų bazės: {error.Message}");
                CleanUpAfterUpdate();
                return;
            }

            // Grąžiname vartotojo sąsają iš duomenų bazės atnaujinimo būseną.
            CleanUpAfterUpdate();
        }

        /// <summary>
        /// Atnaujina progreso informaciją
        /// </summary>
        void OnProgress(ProgressResponse progress)
        {
            // Vartotojo sąsajos operacijas galime atlikti tik vartotojo sąsajos gijoje.
            RunOnUiThread(() =>
            {
                // Jei operacija buvo atšaukta, grąžiname vartotojo sąsają iš duomenų bazės atnaujinimo būsenos.
                if (isCancelled)
                {
                    CleanUpAfterUpdate();
                    return;
                }

                // Atnaujiname progreso juostą.
                if (progress.Minimum != progressBar1.Minimum)
                    progressBar1.Minimum = progress.Minimum;

                if (progress.Maximum != progressBar1.Maximum)
                    progressBar1.Maximum = progress.Maximum;

                if (progress.Current != progressBar1.Value + 1)
                    progressBar1.Value = progress.Current;
                else
                    progressBar1.PerformStep();

                // Atnaujiname progreso tekstą.
                progressText.Text = progress.Text;
            });


            if (isCancelled)
            {
                // Įvykdome atšaukimą darbinėje gijoje.
                isCancelled = false;
                throw new OperationCanceledException();
            }
        }

        /// <summary>
        /// Pakeičia vartotojo sąsają į duomenų bazės atnaujinimo būseną.
        /// </summary>
        void SetUpForUpdate()
        {
            isUpdatingDb = true;
            progressText.Text = "Pradedamas duomenų bazės atnaujinimas";
            updateDbButton.Text = "Atšaukti";
        }


        /// <summary>
        /// Grąžina vartotojo sąsają iš duomenų bazės atnaujinimo būsenos.
        /// </summary>
        void CleanUpAfterUpdate()
        {
            updateDbButton.Text = "Atnaujinti duomenų bazę";
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 3;
            progressBar1.Value = 0;
            progressBar1.Step = 1;
            progressText.Text = "";
            isUpdatingDb = false;
        }

        /// <summary>
        /// Prideda įvesties eilutę.
        /// </summary>
        internal static void AddInputRow(Control inputGroup, int yCoordinate)
        {
            var casTextBox = new CASTextBox() { Location = new Point(10, yCoordinate) };
            var percentileTextBox = new PercentileTextBox() { Location = new Point(casTextBox.Location.X + casTextBox.Width + 10, yCoordinate) };
            var classificationTextBox = new ClassificationTextBox() { Location = new Point(percentileTextBox.Location.X + percentileTextBox.Width + 10, yCoordinate) };
            var classifySubstanceButton = new ClassifySubstanceButton() { Location = new Point(classificationTextBox.Location.X + classificationTextBox.Width + 10, yCoordinate) };
            var addButton = new AddButton() { Location = new Point(classifySubstanceButton.Location.X + classifySubstanceButton.Width + 10, yCoordinate) };

            casTextBox.ClassificationTextBox = classificationTextBox;
            casTextBox.ClassifySubstanceButton = classifySubstanceButton;
            classifySubstanceButton.ClassificationTextBox = classificationTextBox;
            classifySubstanceButton.CASTextBox = casTextBox;

            inputGroup.Controls.Add(casTextBox);
            inputGroup.Controls.Add(percentileTextBox);
            inputGroup.Controls.Add(classificationTextBox);
            inputGroup.Controls.Add(classifySubstanceButton);
            inputGroup.Controls.Add(addButton);

            Input.Add(new() { CASNumber = casTextBox, Percentile = percentileTextBox, Classification = classificationTextBox });
        }

        /// <summary>
        /// Įvykdo funkciją vartotojo sąsajos gijoje.
        /// </summary>
        void RunOnUiThread(Action action)
        {
            // Vartotojo sąsają pasieksime kviesdami veiksmą ant "this" objekto.
            this.Invoke(action);
        }

        /// <summary>
        /// Išvalo įvesties eilutes.
        /// </summary>
        private void ClearData(object sender, EventArgs e)
        {
            inputGroup.Controls.Clear();
            Input.Clear();
            AddInputRow(inputGroup, 30);
        }

        /// <summary>
        /// Klasifikuoja mišinį.
        /// </summary>
        private async void Classify(object sender, EventArgs e)
        {
            if (!ValidatePercentiles())
            {
                MessageBox.Show("Mišinio procentinių dalių suma viršija 100%.");
                return;
            }

            progressText.Text = "Klasifikuojama";
            classificationTextBox.Text = "";

            var input = ConvertInput(Input).Select(inputLine => (InputLine)inputLine).ToList();
            var classifier = new Classifier() { Input = input, Db = Db, OnInputNeeded = OnInputNeeded };

            string output = "";
            string errors = "";
            string codes = "";
            try
            {
                var classificationResult = await classifier.Classify();
                output = $"Klasifikacija: {string.Join(", ", classificationResult.Classification)}\n";
                foreach (var error in classificationResult.Errors)
                {
                    errors += $"Įspėjimas: {error}\n";
                }

                var codesOutput = Codes.GetCodes(classificationResult.Classification);
                codes += $"Mišinio piktogramų nr.: {string.Join("; ", codesOutput.GhsCodes.Distinct())}\n";
                codes += $"Signalinis žodis: {codesOutput.SignalWord}\n";
                codes += $"Pavojingumo H frazių kodai: {string.Join("; ", codesOutput.HCodes.Distinct())}\n";
                codes += $"Pavojingumo H frazės: {string.Join(". ", codesOutput.HPhrases.Distinct())}\n";
                codes += $"Pavojingumo P frazių koda: {string.Join("; ", codesOutput.PCodes.Distinct())}\n";
                codes += $"Pavojingumo P frazės: {string.Join(". ", codesOutput.PPhrases.Distinct())}\n";

            }
            catch (HttpRequestException exception)
            {
                MessageBox.Show($"Nepavyko prisijungti prie ECHA duomenų bazės: {exception.Message}");
                return;
            }
            finally
            {
                classificationTextBox.Text = output;
                errorsTextBox.Text = errors;
                warningTextBox.Text = codes;
                progressText.Text = "";
            }
        }

        /// <summary>
        /// Parodo papildomos įvesties langą.
        /// </summary>
        private string OnInputNeeded(string prompt, ValidateInput onValidate, string description = null)
        {
            Form promptForm = new Form();
            promptForm.Width = 700;
            promptForm.Height = 400;
            promptForm.Text = prompt;

            Label textLabel = new Label() { Left = 50, Top = 20 };
            textLabel.MaximumSize = new Size(300, 100);
            textLabel.AutoSize = true;

            TextBox inputBox = new TextBox() { Left = 50, Top = 100, Width = 275 };

            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Height = 35, Top = 100 };
            confirmation.Click += (sender, e) =>
            {
                var validationResult = onValidate(inputBox.Text);
                if (validationResult.Result == false)
                {
                    inputBox.BackColor = Color.Red;
                    textLabel.Text = validationResult.Error;
                }
                else
                {
                    promptForm.Close();
                }
            };

            Label descriptionLabel = new Label() { Left = 50, Top = 150 };
            descriptionLabel.MaximumSize = new Size(300, 200);
            descriptionLabel.AutoSize = true;
            descriptionLabel.Text = description;

            promptForm.Controls.Add(confirmation);
            promptForm.Controls.Add(textLabel);
            promptForm.Controls.Add(inputBox);
            promptForm.Controls.Add(descriptionLabel);
            promptForm.ShowDialog();

            return inputBox.Text;
        }

        /// <summary>
        /// Konvertuoja įvestį į reikiamus duomenų tipus.
        /// </summary>
        private static List<(string CASNumber, double Percentile, string[] Classification)> ConvertInput(List<(CASTextBox CASNumber, PercentileTextBox Percentile, ClassificationTextBox Classification)> input)
        {
            List<(string CASNumber, double Percentile, string[] Classification)> result = new();
            foreach (var entry in input)
            {
                string casNumber = entry.CASNumber.Text == CASTextBox.DefaultText ? null : entry.CASNumber.Text;
                double percentile = double.TryParse(entry.Percentile.Text, out double perc) ? perc : 0.0;
                string[] classification = entry.Classification.Text.Split(", ");
                result.Add(new() { CASNumber = casNumber, Percentile = percentile, Classification = classification });
            }

            return result;
        }

        /// <summary>
        /// Patikrina, ar bendra mišinio medžiagų procentinė vertė neviršija 100.
        /// </summary>
        /// <returns></returns>
        internal static bool ValidatePercentiles()
        {
            double percentiles = Input.Select((substance) =>
            {
                if (double.TryParse(substance.Percentile.Text, out double percentile))
                    return percentile;

                return 0;
            }).Aggregate(0.0, (result, percentile) => result + percentile);

            return percentiles <= 100.0;
        }
    }
}

