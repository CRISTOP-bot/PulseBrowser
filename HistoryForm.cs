using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PulseBrowser
{
    public class HistoryForm : Form
    {
        HistoryManager manager;
        ListView listView;
        TextBox searchBox;
        Button btnClear, btnClose;

        public HistoryForm(HistoryManager manager)
        {
            this.manager = manager;
            Text = "History";
            Size = new Size(600, 450);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9);

            InitializeComponents();
            LoadHistory();
        }

        void InitializeComponents()
        {
            var topPanel = new Panel();
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 35;
            topPanel.Padding = new Padding(5, 4, 5, 2);

            searchBox = new TextBox();
            searchBox.Size = new Size(200, 24);
            searchBox.Location = new Point(5, 5);
            searchBox.TextChanged += (s, e) => LoadHistory(searchBox.Text);

            var lblSearch = new Label();
            lblSearch.Text = "Search:";
            lblSearch.Location = new Point(210, 8);
            lblSearch.Size = new Size(50, 20);

            topPanel.Controls.Add(searchBox);
            topPanel.Controls.Add(lblSearch);
            Controls.Add(topPanel);

            listView = new ListView();
            listView.Dock = DockStyle.Fill;
            listView.View = View.Details;
            listView.FullRowSelect = true;
            listView.GridLines = true;
            listView.Columns.Add("Title", 250);
            listView.Columns.Add("URL", 280);
            listView.DoubleClick += (s, e) => OpenSelected();

            var bottomPanel = new Panel();
            bottomPanel.Dock = DockStyle.Bottom;
            bottomPanel.Height = 40;

            btnClear = new Button { Text = "Clear All", Width = 100, Height = 28 };
            btnClear.Location = new Point(10, 6);
            btnClear.Click += (s, e) => ClearHistory();

            btnClose = new Button { Text = "Close", Width = 80, Height = 28 };
            btnClose.Location = new Point(490, 6);
            btnClose.Click += (s, e) => Close();

            bottomPanel.Controls.Add(btnClear);
            bottomPanel.Controls.Add(btnClose);

            Controls.Add(listView);
            Controls.Add(bottomPanel);
        }

        void LoadHistory(string search = "")
        {
            listView.Items.Clear();
            var entries = string.IsNullOrEmpty(search)
                ? manager.GetAll()
                : manager.Search(search);

            foreach (var entry in entries)
            {
                var item = new ListViewItem(entry.Title);
                item.SubItems.Add(entry.Url);
                item.Tag = entry;
                listView.Items.Add(item);
            }
        }

        void OpenSelected()
        {
            if (listView.SelectedItems.Count > 0)
            {
                var entry = listView.SelectedItems[0].Tag as HistoryEntry;
                if (entry != null)
                {
                    Close();
                    var main = Owner as MainForm;
                    main?.NavigateTo(entry.Url);
                }
            }
        }

        void ClearHistory()
        {
            if (MessageBox.Show("Clear all history?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                manager.Clear();
                LoadHistory();
            }
        }
    }
}
