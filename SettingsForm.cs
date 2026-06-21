using System;
using System.Drawing;
using System.Windows.Forms;

namespace PulseBrowser
{
    public class SettingsForm : Form
    {
        SettingsManager settings;
        TextBox txtHomePage;
        ComboBox cmbSearchEngine;
        Button btnSave, btnCancel;

        public SettingsForm(SettingsManager settings)
        {
            this.settings = settings;
            Text = "Settings";
            Size = new Size(450, 250);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Font = new Font("Segoe UI", 9);

            InitializeComponents();
            LoadSettings();
        }

        void InitializeComponents()
        {
            var lblHome = new Label();
            lblHome.Text = "Home Page:";
            lblHome.Location = new Point(15, 20);
            lblHome.Size = new Size(80, 22);

            txtHomePage = new TextBox();
            txtHomePage.Location = new Point(100, 18);
            txtHomePage.Size = new Size(320, 22);

            var lblSearch = new Label();
            lblSearch.Text = "Search Engine:";
            lblSearch.Location = new Point(15, 55);
            lblSearch.Size = new Size(80, 22);

            cmbSearchEngine = new ComboBox();
            cmbSearchEngine.Location = new Point(100, 53);
            cmbSearchEngine.Size = new Size(320, 22);
            cmbSearchEngine.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSearchEngine.Items.Add("https://www.google.com/search?q=");
            cmbSearchEngine.Items.Add("https://www.bing.com/search?q=");
            cmbSearchEngine.Items.Add("https://search.yahoo.com/search?p=");
            cmbSearchEngine.Items.Add("https://duckduckgo.com/?q=");
            cmbSearchEngine.Items.Add("https://www.baidu.com/s?wd=");

            btnSave = new Button();
            btnSave.Text = "Save";
            btnSave.Location = new Point(260, 170);
            btnSave.Size = new Size(80, 30);
            btnSave.Click += (s, e) => SaveSettings();

            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Location = new Point(350, 170);
            btnCancel.Size = new Size(80, 30);
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            Controls.Add(lblHome);
            Controls.Add(txtHomePage);
            Controls.Add(lblSearch);
            Controls.Add(cmbSearchEngine);
            Controls.Add(btnSave);
            Controls.Add(btnCancel);
        }

        void LoadSettings()
        {
            txtHomePage.Text = settings.Get("homepage", "https://www.google.com");
            var currentEngine = settings.Get("searchengine", "https://www.google.com/search?q=");
            cmbSearchEngine.SelectedItem = currentEngine;
            if (cmbSearchEngine.SelectedIndex < 0)
                cmbSearchEngine.SelectedIndex = 0;
        }

        void SaveSettings()
        {
            settings.Set("homepage", txtHomePage.Text);
            settings.Set("searchengine", cmbSearchEngine.SelectedItem?.ToString() ?? "https://www.google.com/search?q=");
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
