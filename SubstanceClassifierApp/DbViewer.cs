using System;
using System.Windows.Forms;

namespace SubstanceClassifierApp
{
    public partial class DbViewer : Form
    {
        public DbViewer()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Prideda lokalios duomenų bazės sąsają.
        /// </summary>
        private void DbViewer_Load(object sender, EventArgs e)
        {
            if (MainForm.Db != null)
                dataGridView1.DataSource = MainForm.Db.Substances.Local.ToBindingList();
        }
    }
}
