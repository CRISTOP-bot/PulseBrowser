using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PulseBrowser
{
    public class BookmarkForm : Form
    {
        BookmarkManager manager;
        ListView listView;
        Button btnOpen, btnEdit, btnDelete, btnClose;

        public BookmarkForm(BookmarkManager manager, Bookmark editTarget = null)
        {
            this.manager = manager;
            Text = "Bookmark Manager";
            Size = new Size(550, 400);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9);

            if (editTarget != null)
            {
                ShowEditDialog(editTarget);
                return;
            }

            InitializeComponents();
            LoadBookmarks();
        }

        void InitializeComponents()
        {
            listView = new ListView();
            listView.Dock = DockStyle.Fill;
            listView.View = View.Details;
            listView.FullRowSelect = true;
            listView.GridLines = true;
            listView.Columns.Add("Title", 200);
            listView.Columns.Add("URL", 280);
            listView.DoubleClick += (s, e) => OpenSelected();

            var panel = new Panel();
            panel.Dock = DockStyle.Bottom;
            panel.Height = 40;

            btnOpen = new Button { Text = "Open", Width = 80, Height = 28 };
            btnOpen.Location = new Point(10, 6);
            btnOpen.Click += (s, e) => OpenSelected();

            btnEdit = new Button { Text = "Edit", Width = 80, Height = 28 };
            btnEdit.Location = new Point(100, 6);
            btnEdit.Click += (s, e) => EditSelected();

            btnDelete = new Button { Text = "Delete", Width = 80, Height = 28 };
            btnDelete.Location = new Point(190, 6);
            btnDelete.Click += (s, e) => DeleteSelected();

            btnClose = new Button { Text = "Close", Width = 80, Height = 28 };
            btnClose.Location = new Point(450, 6);
            btnClose.Click += (s, e) => { DialogResult = DialogResult.OK; Close(); };

            panel.Controls.Add(btnOpen);
            panel.Controls.Add(btnEdit);
            panel.Controls.Add(btnDelete);
            panel.Controls.Add(btnClose);

            Controls.Add(listView);
            Controls.Add(panel);
        }

        void LoadBookmarks()
        {
            listView.Items.Clear();
            foreach (var bm in manager.GetAll())
            {
                var item = new ListViewItem(bm.Title);
                item.SubItems.Add(bm.Url);
                item.Tag = bm;
                listView.Items.Add(item);
            }
        }

        void OpenSelected()
        {
            if (listView.SelectedItems.Count > 0)
            {
                var bm = listView.SelectedItems[0].Tag as Bookmark;
                if (bm != null)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                    var main = Owner as MainForm;
                    main?.NavigateTo(bm.Url);
                }
            }
        }

        void EditSelected()
        {
            if (listView.SelectedItems.Count > 0)
            {
                var bm = listView.SelectedItems[0].Tag as Bookmark;
                if (bm != null)
                    ShowEditDialog(bm);
            }
        }

        void DeleteSelected()
        {
            if (listView.SelectedItems.Count > 0)
            {
                var bm = listView.SelectedItems[0].Tag as Bookmark;
                if (bm != null)
                {
                    manager.Remove(bm);
                    LoadBookmarks();
                }
            }
        }

        void ShowEditDialog(Bookmark bm)
        {
            var form = new Form();
            form.Text = "Edit Bookmark";
            form.Size = new Size(400, 180);
            form.StartPosition = FormStartPosition.CenterParent;
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.MaximizeBox = false;
            form.MinimizeBox = false;

            var lblName = new Label { Text = "Name:", Location = new Point(10, 15), Size = new Size(60, 22) };
            var txtName = new TextBox { Text = bm.Title, Location = new Point(80, 12), Size = new Size(290, 22) };

            var lblUrl = new Label { Text = "URL:", Location = new Point(10, 45), Size = new Size(60, 22) };
            var txtUrl = new TextBox { Text = bm.Url, Location = new Point(80, 42), Size = new Size(290, 22) };

            var btnSave = new Button { Text = "Save", Location = new Point(210, 80), Size = new Size(75, 25) };
            var btnCancel = new Button { Text = "Cancel", Location = new Point(295, 80), Size = new Size(75, 25) };

            btnSave.Click += (s, e) =>
            {
                manager.Update(bm, txtName.Text, txtUrl.Text);
                form.DialogResult = DialogResult.OK;
                form.Close();
            };
            btnCancel.Click += (s, e) => form.Close();

            form.Controls.Add(lblName);
            form.Controls.Add(txtName);
            form.Controls.Add(lblUrl);
            form.Controls.Add(txtUrl);
            form.Controls.Add(btnSave);
            form.Controls.Add(btnCancel);

            form.ShowDialog(this);
            LoadBookmarks();
        }
    }
}
