using System;
using System.Windows.Forms;

namespace PulseBrowser
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                SetBrowserEmulation();
            }
            catch { }

            Application.Run(new MainForm());
        }

        static void SetBrowserEmulation()
        {
            var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(
                @"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION");
            var appName = AppDomain.CurrentDomain.FriendlyName;
            key.SetValue(appName, 11001, Microsoft.Win32.RegistryValueKind.DWord);
            key.Close();
        }
    }
}
