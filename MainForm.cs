using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace PulseBrowser
{
    public partial class MainForm : Form
    {
        TabControl tabControl;
        TextBox addressBox;
        Button btnBack, btnForward, btnRefresh, btnHome;
        Button btnBookmark, btnHistory, btnSettings;
        MenuStrip menuStrip;
        ToolStripMenuItem fileMenu, viewMenu, toolsMenu, helpMenu;
        StatusStrip statusStrip;
        ToolStripStatusLabel statusLabel;
        ToolStripProgressBar progressBar;
        FlowLayoutPanel bookmarkBar;
        Panel addressPanel;
        Button btnGo;

        BookmarkManager bookmarkManager;
        HistoryManager historyManager;
        SettingsManager settings;

        string homePage = "https://www.google.com";
        string searchEngine = "https://www.google.com/search?q=";

        public MainForm()
        {
            Text = "Pulse Browser";
            Size = new Size(1280, 800);
            MinimumSize = new Size(800, 500);
            BackColor = Color.FromArgb(240, 240, 240);

            bookmarkManager = new BookmarkManager();
            historyManager = new HistoryManager();
            settings = new SettingsManager();

            LoadSettings();
            LoadAppIcon();
            InitializeComponents();
            CreateNewTab(homePage);
        }

        void LoadSettings()
        {
            homePage = settings.Get("homepage", "https://www.google.com");
            searchEngine = settings.Get("searchengine", "https://www.google.com/search?q=");
        }

        void LoadAppIcon()
        {
            try
            {
                var asm = Assembly.GetExecutingAssembly();
                using (var stream = asm.GetManifestResourceStream("PulseBrowser.Pulse.ico"))
                {
                    if (stream != null)
                        Icon = new Icon(stream);
                }
            }
            catch { }
            if (Icon == null)
            {
                var path = Path.Combine(Application.StartupPath, "Pulse.ico");
                if (File.Exists(path))
                    Icon = new Icon(path);
            }
        }

        void InitializeComponents()
        {
            CreateMenuStrip();
            CreateNavigationBar();
            CreateBookmarkBar();
            CreateTabControl();
            CreateStatusStrip();
        }

        void CreateMenuStrip()
        {
            menuStrip = new MenuStrip();
            menuStrip.BackColor = Color.FromArgb(245, 245, 245);

            fileMenu = new ToolStripMenuItem("&File");
            fileMenu.DropDownItems.Add("New Tab", null, (s, e) => CreateNewTab(homePage));
            fileMenu.DropDownItems.Add("Close Tab", null, (s, e) => CloseCurrentTab());
            fileMenu.DropDownItems.Add(new ToolStripSeparator());
            fileMenu.DropDownItems.Add("Exit", null, (s, e) => Close());

            viewMenu = new ToolStripMenuItem("&View");
            viewMenu.DropDownItems.Add("Bookmarks", null, (s, e) => ShowBookmarkManager());
            viewMenu.DropDownItems.Add("History", null, (s, e) => ShowHistory());
            viewMenu.DropDownItems.Add(new ToolStripSeparator());
            viewMenu.DropDownItems.Add("Full Screen", null, (s, e) => ToggleFullScreen());

            toolsMenu = new ToolStripMenuItem("&Tools");
            toolsMenu.DropDownItems.Add("Settings", null, (s, e) => ShowSettings());
            toolsMenu.DropDownItems.Add("Downloads", null, (s, e) => ShowDownloads());
            toolsMenu.DropDownItems.Add(new ToolStripSeparator());
            toolsMenu.DropDownItems.Add("Clear Browsing Data", null, (s, e) => ClearBrowsingData());

            helpMenu = new ToolStripMenuItem("&Help");
            helpMenu.DropDownItems.Add("About", null, (s, e) => ShowAbout());

            menuStrip.Items.Add(fileMenu);
            menuStrip.Items.Add(viewMenu);
            menuStrip.Items.Add(toolsMenu);
            menuStrip.Items.Add(helpMenu);

            MainMenuStrip = menuStrip;
            Controls.Add(menuStrip);
        }

        void CreateNavigationBar()
        {
            addressPanel = new Panel();
            addressPanel.Dock = DockStyle.Top;
            addressPanel.Height = 40;
            addressPanel.Padding = new Padding(0, 2, 0, 2);
            addressPanel.BackColor = Color.FromArgb(230, 230, 230);

            var navPanel = new FlowLayoutPanel();
            navPanel.Dock = DockStyle.Fill;
            navPanel.FlowDirection = FlowDirection.LeftToRight;
            navPanel.Padding = new Padding(5, 2, 5, 2);
            navPanel.Height = 36;

            btnBack = CreateNavButton("◀", (s, e) => NavigateBack());
            btnForward = CreateNavButton("▶", (s, e) => NavigateForward());
            btnRefresh = CreateNavButton("⟳", (s, e) => RefreshCurrentTab());
            btnHome = CreateNavButton("⌂", (s, e) => NavigateTo(homePage));

            btnBack.Size = new Size(28, 28);
            btnForward.Size = new Size(28, 28);
            btnRefresh.Size = new Size(28, 28);
            btnHome.Size = new Size(28, 28);

            addressBox = new TextBox();
            addressBox.Size = new Size(400, 26);
            addressBox.Font = new Font("Segoe UI", 10);
            addressBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            addressBox.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) NavigateToCurrentAddress(); };
            addressBox.Enter += (s, e) => addressBox.SelectAll();

            btnGo = new Button();
            btnGo.Text = "Go";
            btnGo.Size = new Size(35, 26);
            btnGo.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnGo.Click += (s, e) => NavigateToCurrentAddress();

            btnBookmark = CreateNavButton("☆", (s, e) => ToggleBookmarkForCurrentPage());
            btnHistory = CreateNavButton("⏱", (s, e) => ShowHistory());
            btnSettings = CreateNavButton("⚙", (s, e) => ShowSettings());

            btnBookmark.Size = new Size(28, 28);
            btnHistory.Size = new Size(28, 28);
            btnSettings.Size = new Size(28, 28);

            navPanel.Controls.Add(btnBack);
            navPanel.Controls.Add(btnForward);
            navPanel.Controls.Add(btnRefresh);
            navPanel.Controls.Add(btnHome);
            navPanel.Controls.Add(addressBox);
            navPanel.Controls.Add(btnGo);
            navPanel.Controls.Add(btnBookmark);
            navPanel.Controls.Add(btnHistory);
            navPanel.Controls.Add(btnSettings);

            addressPanel.Controls.Add(navPanel);
            Controls.Add(addressPanel);
        }

        Button CreateNavButton(string text, EventHandler handler)
        {
            var btn = new Button();
            btn.Text = text;
            btn.Font = new Font("Segoe UI", 12);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Color.Transparent;
            btn.Click += handler;
            return btn;
        }

        void CreateBookmarkBar()
        {
            bookmarkBar = new FlowLayoutPanel();
            bookmarkBar.Dock = DockStyle.Top;
            bookmarkBar.Height = 30;
            bookmarkBar.Padding = new Padding(5, 2, 5, 2);
            bookmarkBar.BackColor = Color.FromArgb(220, 220, 220);
            bookmarkBar.AutoSize = false;
            Controls.Add(bookmarkBar);
            RefreshBookmarkBar();
        }

        void RefreshBookmarkBar()
        {
            bookmarkBar.Controls.Clear();
            var bookmarks = bookmarkManager.GetAll();
            foreach (var bm in bookmarks)
            {
                var btn = new Button();
                btn.Text = bm.Title;
                btn.Tag = bm.Url;
                btn.Font = new Font("Segoe UI", 9);
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.BackColor = Color.Transparent;
                btn.AutoSize = true;
                btn.MaximumSize = new Size(150, 26);
                btn.TextAlign = ContentAlignment.MiddleLeft;
                btn.Click += (s, e) =>
                {
                    if (btn.Tag is string url)
                        NavigateTo(url);
                };
                btn.ContextMenuStrip = CreateBookmarkContextMenu(bm);
                bookmarkBar.Controls.Add(btn);
            }
        }

        ContextMenuStrip CreateBookmarkContextMenu(Bookmark bm)
        {
            var menu = new ContextMenuStrip();
            menu.Items.Add("Open", null, (s, e) => NavigateTo(bm.Url));
            menu.Items.Add("Open in New Tab", null, (s, e) => CreateNewTab(bm.Url));
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add("Edit", null, (s, e) => EditBookmark(bm));
            menu.Items.Add("Delete", null, (s, e) =>
            {
                bookmarkManager.Remove(bm);
                RefreshBookmarkBar();
            });
            return menu;
        }

        void CreateTabControl()
        {
            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            tabControl.Padding = new Point(12, 4);
            tabControl.Font = new Font("Segoe UI", 9);
            tabControl.SelectedIndexChanged += TabChanged;
            Controls.Add(tabControl);

            tabControl.BringToFront();
        }

        void CreateStatusStrip()
        {
            statusStrip = new StatusStrip();
            statusStrip.BackColor = Color.FromArgb(230, 230, 230);

            statusLabel = new ToolStripStatusLabel("Ready");
            statusLabel.Spring = true;
            statusLabel.TextAlign = ContentAlignment.MiddleLeft;

            progressBar = new ToolStripProgressBar();
            progressBar.Width = 150;
            progressBar.Visible = false;

            statusStrip.Items.Add(statusLabel);
            statusStrip.Items.Add(progressBar);

            Controls.Add(statusStrip);
        }

        void SetStatus(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(SetStatus), text);
                return;
            }
            statusLabel.Text = text;
        }

        void SetProgress(int value, bool visible)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<int, bool>(SetProgress), value, visible);
                return;
            }
            progressBar.Value = value;
            progressBar.Visible = visible;
        }

        BrowserTab GetCurrentTab()
        {
            if (tabControl.SelectedTab == null) return null;
            return tabControl.SelectedTab.Tag as BrowserTab;
        }

        void TabChanged(object sender, EventArgs e)
        {
            var tab = GetCurrentTab();
            if (tab != null)
            {
                addressBox.Text = tab.CurrentUrl ?? "";
                Text = tab.Title + " - Opencode Browser";
                UpdateNavButtons();
                UpdateBookmarkButton();
            }
        }

        void CreateNewTab(string url)
        {
            var page = new TabPage("New Tab");
            var browserTab = new BrowserTab(page);
            page.Tag = browserTab;

            var browser = new WebBrowser();
            browser.Dock = DockStyle.Fill;
            browser.ScriptErrorsSuppressed = true;
            browser.IsWebBrowserContextMenuEnabled = true;
            browser.WebBrowserShortcutsEnabled = true;

            browser.Navigating += (s, e) =>
            {
                browserTab.CurrentUrl = e.Url.ToString();
                addressBox.Text = e.Url.ToString();
                SetStatus("Loading " + e.Url.Host + "...");
                SetProgress(30, true);
                UpdateNavButtons();
            };

            browser.DocumentTitleChanged += (s, e) =>
            {
                var title = browser.DocumentTitle;
                if (string.IsNullOrEmpty(title)) title = "New Tab";
                page.Text = TruncateTitle(title);
                browserTab.Title = title;
                if (tabControl.SelectedTab == page)
                    Text = title + " - Pulse Browser";
            };

            browser.ProgressChanged += (s, e) =>
            {
                if (e.CurrentProgress > 0 && e.MaximumProgress > 0)
                {
                    int pct = (int)(e.CurrentProgress * 100 / e.MaximumProgress);
                    SetProgress(pct, true);
                }
            };

            browser.DocumentCompleted += (s, e) =>
            {
                if (e.Url == browser.Url)
                {
                    browserTab.CurrentUrl = browser.Url.ToString();
                    browserTab.Title = browser.DocumentTitle ?? "Untitled";
                    SetProgress(100, false);
                    SetStatus("Done");
                    historyManager.Add(browserTab.Title, browser.Url.ToString());
                    UpdateNavButtons();
                    UpdateBookmarkButton();
                }
            };

            browser.StatusTextChanged += (s, e) =>
            {
                SetStatus(browser.StatusText);
            };

            browser.NewWindow += (s, e) =>
            {
                e.Cancel = true;
                var url = (s as WebBrowser)?.Url?.ToString();
                if (!string.IsNullOrEmpty(url))
                    CreateNewTab(url);
            };

            browserTab.Browser = browser;
            page.Controls.Add(browser);

            tabControl.TabPages.Add(page);
            tabControl.SelectedTab = page;

            if (!string.IsNullOrEmpty(url))
            {
                browser.Navigate(url);
            }
        }

        string TruncateTitle(string title, int maxLen = 25)
        {
            if (title.Length <= maxLen) return title;
            return title.Substring(0, maxLen - 2) + "...";
        }

        void CloseCurrentTab()
        {
            if (tabControl.TabPages.Count <= 1)
            {
                Close();
                return;
            }
            var tab = GetCurrentTab();
            if (tab?.Browser != null)
            {
                tab.Browser.Dispose();
            }
            tabControl.TabPages.Remove(tabControl.SelectedTab);
            TabChanged(null, null);
        }

        public void NavigateTo(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return;
            var tab = GetCurrentTab();
            if (tab?.Browser == null) return;

            url = url.Trim();

            if (!url.Contains("://") && !url.Contains("."))
            {
                url = searchEngine + Uri.EscapeDataString(url);
            }
            else if (!url.Contains("://"))
            {
                url = "https://" + url;
            }

            addressBox.Text = url;
            tab.Browser.Navigate(url);
        }

        public void NavigateToCurrentAddress()
        {
            NavigateTo(addressBox.Text);
        }

        void NavigateBack()
        {
            var tab = GetCurrentTab();
            if (tab?.Browser != null && tab.Browser.CanGoBack)
                tab.Browser.GoBack();
        }

        void NavigateForward()
        {
            var tab = GetCurrentTab();
            if (tab?.Browser != null && tab.Browser.CanGoForward)
                tab.Browser.GoForward();
        }

        void RefreshCurrentTab()
        {
            var tab = GetCurrentTab();
            if (tab?.Browser != null)
            {
                if (tab.Browser.IsBusy)
                    tab.Browser.Stop();
                tab.Browser.Refresh();
            }
        }

        void UpdateNavButtons()
        {
            var tab = GetCurrentTab();
            if (tab?.Browser != null)
            {
                btnBack.Enabled = tab.Browser.CanGoBack;
                btnForward.Enabled = tab.Browser.CanGoForward;
            }
        }

        void UpdateBookmarkButton()
        {
            var tab = GetCurrentTab();
            if (tab != null)
            {
                bool isBookmarked = bookmarkManager.Contains(tab.CurrentUrl);
                btnBookmark.Text = isBookmarked ? "★" : "☆";
                btnBookmark.ForeColor = isBookmarked ? Color.Goldenrod : Color.Black;
            }
        }

        void ToggleBookmarkForCurrentPage()
        {
            var tab = GetCurrentTab();
            if (tab == null || string.IsNullOrEmpty(tab.CurrentUrl)) return;

            if (bookmarkManager.Contains(tab.CurrentUrl))
            {
                bookmarkManager.Remove(tab.CurrentUrl);
            }
            else
            {
                bookmarkManager.Add(tab.Title, tab.CurrentUrl);
            }
            bookmarkManager.Save();
            RefreshBookmarkBar();
            UpdateBookmarkButton();
        }

        void ShowBookmarkManager()
        {
            using (var form = new BookmarkForm(bookmarkManager))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    RefreshBookmarkBar();
                }
            }
        }

        void ShowHistory()
        {
            using (var form = new HistoryForm(historyManager))
            {
                form.ShowDialog(this);
            }
        }

        void ShowSettings()
        {
            using (var form = new SettingsForm(settings))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    LoadSettings();
                }
            }
        }

        void ShowDownloads() { }

        void ShowAbout()
        {
            MessageBox.Show(
                "Pulse Browser v1.0\nA complete web browser built with C# and WinForms.",
                "About Pulse Browser",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        void ToggleFullScreen()
        {
            FormBorderStyle = FormBorderStyle == FormBorderStyle.None
                ? FormBorderStyle.Sizable
                : FormBorderStyle.None;
            WindowState = WindowState == FormWindowState.Maximized
                ? FormWindowState.Normal
                : FormWindowState.Maximized;
        }

        void ClearBrowsingData()
        {
            if (MessageBox.Show("Clear all browsing history?",
                "Clear Browsing Data", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                historyManager.Clear();
            }
        }

        void EditBookmark(Bookmark bm)
        {
            using (var form = new BookmarkForm(bookmarkManager, bm))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    bookmarkManager.Save();
                    RefreshBookmarkBar();
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            foreach (TabPage page in tabControl.TabPages)
            {
                if (page.Tag is BrowserTab bt && bt.Browser != null)
                {
                    bt.Browser.Dispose();
                }
            }
            base.OnFormClosing(e);
        }
    }

    public class BrowserTab
    {
        public TabPage Page { get; set; }
        public WebBrowser Browser { get; set; }
        public string Title { get; set; } = "New Tab";
        public string CurrentUrl { get; set; } = "";

        public BrowserTab(TabPage page)
        {
            Page = page;
        }
    }
}
