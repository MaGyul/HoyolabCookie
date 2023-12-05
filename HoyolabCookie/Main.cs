using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Core.DevToolsProtocolExtension;

namespace HoyolabCookie
{
    public partial class Main : Form
    {
        //private static readonly string copypage = "<style>label{opacity:0;transition:.3s}</style><script>function a() {if (window.lastId) {clearTimeout(lastId);window.lastId = undefined}chrome.webview.hostObjects.clipboard.SetText(copydata);document.querySelector('label').style.opacity = '1';window.lastId = setTimeout(() => {document.querySelector('label').style.opacity = '0';}, 1500);}</script><button onclick=\"a()\">클릭하여 쿠키 정보 복사하기</button></br><label>복사되었습니다.</label>";
        private DevToolsProtocolHelper helper;
        private I18n i18n = new I18n();

        public Main()
        {
            InitializeComponent();
            i18n.init();
        }

        private async void Main_Load(object sender, EventArgs e)
        {
            try
            {
                await wv2.EnsureCoreWebView2Async();
            }
            catch
            {
                var l = i18n.i18n["webview2_missing"];
                if (MessageBox.Show(l["message"].InnerText, l["title"].InnerText, MessageBoxButtons.OK, MessageBoxIcon.Error) 
                    == DialogResult.OK) Application.Exit();
            }
        }

        private async void wv2_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (wv2.CoreWebView2 == null) return;
            wv2.CoreWebView2.NavigationCompleted += async (object _, CoreWebView2NavigationCompletedEventArgs __) =>
            {
                wv2.CoreWebView2.AddHostObjectToScript("i18n", i18n);
                await wv2.CoreWebView2.ExecuteScriptAsync(Properties.Resources.i18n_initialize);
                await wv2.CoreWebView2.ExecuteScriptAsync(Properties.Resources.changePage.Replace("{al_html}", Properties.Resources.article_list));
                await wv2.CoreWebView2.ExecuteScriptAsync(Properties.Resources.loaded_login);
            };
            wv2.CoreWebView2.FrameCreated += (object _, CoreWebView2FrameCreatedEventArgs ev) =>
                ev.Frame.NavigationCompleted += (object __, CoreWebView2NavigationCompletedEventArgs ___) =>
                    ev.Frame.ExecuteScriptAsync(Properties.Resources.login_remove_close);
            wv2.CoreWebView2.NewWindowRequested += (object _, CoreWebView2NewWindowRequestedEventArgs ev) =>
            {
                if (ev.Uri.Contains("cross-login.html"))
                    ev.Handled = true;
            };
            wv2.CoreWebView2.NavigationStarting += (object _, CoreWebView2NavigationStartingEventArgs __) => wv2.CoreWebView2.CookieManager.DeleteAllCookies();
            helper = wv2.CoreWebView2.GetDevToolsProtocolHelper();
            helper.Network.ResponseReceived += Network_ResponseReceived;
            await helper.Network.EnableAsync();

            wv2.CoreWebView2.Navigate("https://hoyolab.com/home");
        }

        private async void Network_ResponseReceived(object sender, Network.ResponseReceivedEventArgs e)
        {
            if (e != null && e.Response != null && e.Response.MimeType == "")
            {
                var cookies = await GetAccountCookies();
                if (cookies.Count > 1)
                {
                    var sb = new StringBuilder();
                    foreach (var cookie in cookies)
                    {
                        if (sb.Length > 0) sb.Append("; ");
                        sb.Append($"{cookie.Key}={cookie.Value}");
                    }
                    await wv2.CoreWebView2.ExecuteScriptAsync($"copydata = '{sb}'");
                }
            }
        }

        private async Task<Dictionary<string, string>> GetAccountCookies()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            var cookies = await helper.Network.GetCookiesAsync(new string[] { "https://hoyolab.com" });
            foreach (var cookie in cookies)
            {
                switch (cookie.Name)
                {
                    case "ltuid":
                    case "ltuid_v2":
                    case "ltoken":
                    case "ltoken_v2":
                    case "cookie_token":
                    case "cookie_token_v2":
                    case "account_mid":
                    case "account_mid_v2":
                    case "ltmid":
                    case "ltmid_v2":
                    case "account_id":
                    case "account_id_v2":
                    case "mi18nLang":
                        result.Add(cookie.Name, cookie.Value);
                        break;
                    default: break;
                }
            }
            return result;
        }

        [ClassInterface(ClassInterfaceType.AutoDual)]
        [ComVisible(true)]
        public class I18nAnotherClass
        {
            public string Prop { get; set; } = "i18n";
        }

        [ClassInterface(ClassInterfaceType.AutoDual)]
        [ComVisible(true)]
        public class I18n
        {
            internal XmlNode i18n = null;

            public void init()
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(Properties.Resources.i18n);
                var lang = xml["lang"];
                i18n = lang[CultureInfo.CurrentUICulture.TwoLetterISOLanguageName] ?? lang["en"];
            }

            public string get(string key)
            {
                if (i18n == null) return "i18n is not defined.";
                return i18n[key]?.InnerText ?? $"{key} not found in i18n.";
            }

            public I18nAnotherClass AnotherObject { get; set; } = new I18nAnotherClass();

            // Sample indexed property.
            [System.Runtime.CompilerServices.IndexerName("Items")]
            public string this[int index]
            {
                get => _mDictionary[index];
                set => _mDictionary[index] = value;
            }
            private Dictionary<int, string> _mDictionary = new Dictionary<int, string>();
        }
    }
}
