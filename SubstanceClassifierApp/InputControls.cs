using System;
using System.Drawing;
using System.Windows.Forms;

namespace SubstanceClassifierApp
{
    /// <summary>
    /// Įvesties teksto laukas
    /// </summary>
    abstract class InputTextBox : TextBox
    {
        protected abstract string GetDefaultString();
        protected abstract int GetDefaultWidth();

        public InputTextBox() : base()
        {
            Size = new Size(GetDefaultWidth(), 25);
            Text = GetDefaultString();
            Enter += new EventHandler(RemoveText);
            Leave += new EventHandler(AddText);
        }

        /// <summary>
        /// Išvalo tekstą paspaudus ant įvesties lauko.
        /// </summary>
        public void RemoveText(object sender, EventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (textBox.Text == GetDefaultString())
                {
                    textBox.Text = "";
                }
            }
        }

        /// <summary>
        /// Prideda numatytą tekstą išėjus iš įvesties lauko, bet nieko neįrašius
        /// </summary>
        public void AddText(object sender, EventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                    textBox.Text = GetDefaultString();
            }
        }
    }

    /// <summary>
    /// CAS numerio įvesties laukas
    /// </summary>
    class CASTextBox : InputTextBox
    {
        public CASTextBox() : base()
        {
            Leave += OnEdited;
        }

        /// <summary>
        /// Užpildo klasifikacijos lauką jei CAS numeris atpažintas duomenų bazėje.
        /// </summary>
        private void OnEdited(object sender, EventArgs e)
        {
            if (Text != "" && Text != GetDefaultString() && MainForm.Db != null && ClassificationTextBox != null && ClassifySubstanceButton != null)
            {
                // Bandome surasti medžiagą su duotu CAS numeriu.
                var substance = MainForm.Db.Substances.Find(Text);
                if (substance != null)
                {
                    // Radus užpildome Klasifikacijos lauką ir išjungiame klasifikacijos mygtuką.
                    ClassificationTextBox.Text = substance.Classification;
                    ClassifySubstanceButton.Enabled = false;
                }
                else
                {
                    // Kitu atveju išvalome klasifikacijos lauką ir įjungiame klasifikacijos mygtuką.
                    ClassificationTextBox.Text = "";
                    ClassifySubstanceButton.Enabled = true;
                }
            }
            else if (ClassificationTextBox != null && ClassifySubstanceButton != null)
            {
                ClassificationTextBox.Text = "";
                ClassifySubstanceButton.Enabled = true;
            }
        }

        protected override string GetDefaultString() => DefaultText;

        public static string DefaultText = "Įrašykite CAS numerį...";

        protected override int GetDefaultWidth() => 140;

        public ClassificationTextBox ClassificationTextBox { get; set; }
        public ClassifySubstanceButton ClassifySubstanceButton { get; set; }
    }

    /// <summary>
    /// Medžiagos procentinės dalies įvesties laukas.
    /// </summary>
    class PercentileTextBox : InputTextBox
    {
        public PercentileTextBox() : base()
        {
            KeyPress += OnKeyPress;
            KeyUp += OnKeyUp;
        }

        protected override string GetDefaultString() => "Procentinė sudėties dalis %";
        protected override int GetDefaultWidth() => 250;

        /// <summary>
        /// Patikrina, ar vedamas tekstas yra skaitinis.
        /// </summary>
        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Patikrina, ar bendra procentinių dalių suma neviršija 100
        /// </summary>
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (!MainForm.ValidatePercentiles())
            {
                BackColor = Color.Red;
            }
            else
            {
                BackColor = Color.White;
            }

        }
    }

    /// <summary>
    /// Medžiagos klasifikacijos laukas
    /// </summary>
    class ClassificationTextBox : InputTextBox
    {
        public ClassificationTextBox() : base()
        {
            ReadOnly = true; // Neleidžiame įrašyti reikšmių.
        }

        protected override string GetDefaultString() => "";
        protected override int GetDefaultWidth() => 225;
    }

    /// <summary>
    /// Kitos medžiagos pridėjimo mygtukas.
    /// </summary>
    class AddButton : Button
    {
        public AddButton() : base()
        {
            Size = new Size(30, 30);
            Text = "+";
            Click += AddClicked;
        }

        /// <summary>
        /// Prideda naują medžiagos įvesties eilutę.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AddClicked(object sender, EventArgs e)
        {
            MainForm.AddInputRow(Parent, Location.Y + 40);
            Visible = false;
        }
    }

    /// <summary>
    /// Klasifikacijos mygtukas
    /// </summary>
    class ClassifySubstanceButton : Button
    {
        public ClassifySubstanceButton() : base()
        {
            Size = new Size(200, 32);
            Text = "Pridėti klasifikaciją";
            Click += ClassifySubstance_Clicked;
        }

        public ClassificationTextBox ClassificationTextBox { get; set; }
        public CASTextBox CASTextBox { get; set; }

        /// <summary>
        /// Atidaro klasifikacijos langą ir užpildo klasifikacijos lauką.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ClassifySubstance_Clicked(object sender, EventArgs e)
        {
            if (ClassificationTextBox == null || CASTextBox == null)
                return;

            ClassificationTextBox.Text = ClassifySubstanceDialog.ShowDialog();
            if (!string.IsNullOrEmpty(ClassificationTextBox.Text))
                CASTextBox.Enabled = false;
            else
                CASTextBox.Enabled = true;
        }
    }
}



