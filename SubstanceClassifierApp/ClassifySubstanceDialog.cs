using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace SubstanceClassifierApp
{
    /// <summary>
    /// Klasifikacijos langas
    /// </summary>
    public partial class ClassifySubstanceDialog
    {
        /// <summary>
        /// Parodo klasifikacijos langą ir grąžina rezultatą.
        /// </summary>
        /// <returns></returns>
        public static string ShowDialog()
        {
            Form prompt = new Form();
            prompt.Width = 500;
            prompt.Height = 300;
            prompt.Text = "Pasirinkite klasifikaciją";

            Label textLabel = new Label() { Left = 50, Top = 20, Width = 400, Text = "Klasifikacijos:" };

            ListBox inputBox = new ListBox() { Left = 50, Top = 50, Width = 400, Height = 200 };
            inputBox.Items.AddRange(GetAvailableClassifications());
            inputBox.SelectionMode = SelectionMode.MultiExtended;
            inputBox.Sorted = true;

            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Height = 35, Top = 300 };
            confirmation.Click += (sender, e) => { prompt.Close(); };

            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(inputBox);
            prompt.ShowDialog();

            return string.Join(", ", inputBox.SelectedItems.OfType<string>());
        }

        /// <summary>
        /// Grąžina visas įmanomas klasifikacijas lokalioje duomenų bazėje.
        /// </summary>
        private static string[] GetAvailableClassifications()
        {
            var classificationLists = MainForm.Db.Substances.Select((substance) => substance.Classification.Split(", ", StringSplitOptions.None));
            var flatList = new List<string>();
            foreach (var list in classificationLists)
                flatList.AddRange(list);

            return flatList.Where((classification) => !string.IsNullOrEmpty(classification)).Distinct().ToArray();
        }
    }
}

