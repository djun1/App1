using App1.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.Web.Http;
using Windows.UI.ApplicationSettings;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace App1
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private static List<string> Files = new List<string>();
        private static string BaseURL;
        private static HttpClient Client;
        private StorageFolder DataFolder;
        private StorageFile DataFile;
        private static HttpResponseMessage Message;
        private static string HomeStation;
        private static string HomeStationURL;
        private static string DataType;


        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public MainPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            int i;

            StatusText.Text = String.Empty;
            DataFolder = await ApplicationData.Current.TemporaryFolder.CreateFolderAsync("Files", CreationCollisionOption.OpenIfExists);

            Files.Clear();

            await GetListOfLatestFiles(Files);

            for (i = 0; i < Files.Count(); i++)
            {
                await GetDataUsingHTTP(HomeStationURL.Substring(0, HomeStationURL.Length - 3) + Files[i], DataFolder, Files[i]);

                // Print text from file in textblock.

                var Text = await FileIO.ReadLinesAsync(DataFile);

                foreach (var line in Text.AsEnumerable().Skip(1))
                {
                    StatusText.Text += line + "\r\n";
                }
            }

            Files.Clear();

            await GetListOfAdditionalLatestFiles(Files);

            for (i = 0; i < Files.Count(); i++)
            {
                await GetDataUsingHTTP(HomeStationURL.Substring(0, HomeStationURL.Length - 3) + Files[i], DataFolder, Files[i]);

                // Print text from file in textblock.

                var Text = await FileIO.ReadLinesAsync(DataFile);

                foreach (var line in Text.AsEnumerable().Skip(1))
                {
                    StatusText.Text += line + "\r\n";
                }
            }

        }

        public static async Task GetListOfLatestFiles(List<string> FileNames)
        {
            int i;
            Uri URI;
            string StartDateTimeString;
            Regex RegEx;
            string RegExString;
            DateTime CurrDateTime = DateTime.Now.ToUniversalTime();
            DateTime StartDateTime;
            BaseURL = "http://dd.weather.gc.ca/bulletins/alphanumeric/";

            HttpClient Client = new HttpClient();

            HomeStation = "CYVR";

            for (i = 0; i < 4; i++)
            {
                DataType = "SA"; 
                StartDateTime = CurrDateTime.Subtract(new TimeSpan(i, 0, 0)); ;
                StartDateTimeString = StartDateTime.Year.ToString() + StartDateTime.Month.ToString("D2")
                    + StartDateTime.Day.ToString("D2") + "/" + DataType + "/" + HomeStation + "/"
                    + StartDateTime.Hour.ToString("D2") + "/";
                HomeStationURL = BaseURL + StartDateTimeString;
                URI = new Uri(HomeStationURL);

                RegExString = ">[A-Z]+[0-9]+_" + HomeStation + "_[0-9]+___[0-9]+<";

                var HttpClientTask = Client.GetAsync(URI);
                RegEx = new Regex(RegExString);
                Message = await HttpClientTask;

                MatchCollection Matches = RegEx.Matches(Message.Content.ToString());

                foreach (Match match in Matches)
                {
                    if (match.Success)
                    {
                        string tmp = match.ToString();	//The regular expression matches the "<" and ">" signs around the filename. These signs have to be removed before adding the filename to the list
                        FileNames.Add(StartDateTime.Hour.ToString("D2") + "/" + tmp.Substring(1, tmp.Length - 2));
                    }
                }
            }

            Client.Dispose();
        }

        public static async Task GetListOfAdditionalLatestFiles(List<string> FileNames)
        {
            int i;
            Uri URI;
            string StartDateTimeString;
            Regex RegEx;
            string RegExString;
            DateTime CurrDateTime = DateTime.Now.ToUniversalTime();
            DateTime StartDateTime;
            BaseURL = "http://dd.weather.gc.ca/bulletins/alphanumeric/";

            HttpClient Client = new HttpClient();

            HomeStation = "CYVR";

            for (i = 0; i < 6; i++)
            {
                DataType = "FT";
                StartDateTime = CurrDateTime.Subtract(new TimeSpan(i, 0, 0)); ;
                StartDateTimeString = StartDateTime.Year.ToString() + StartDateTime.Month.ToString("D2")
                    + StartDateTime.Day.ToString("D2") + "/" + DataType + "/" + "CWAO" + "/"
                    + StartDateTime.Hour.ToString("D2") + "/";
                HomeStationURL = BaseURL + StartDateTimeString;
                URI = new Uri(HomeStationURL);

                RegExString = ">[A-Z]+[0-9]+_+[A-Z]+_+[0-9]+__" + HomeStation + "_[0-9]+<";

                var HttpClientTask = Client.GetAsync(URI);
                RegEx = new Regex(RegExString);
                Message = await HttpClientTask;

                MatchCollection Matches = RegEx.Matches(Message.Content.ToString());

                foreach (Match match in Matches)
                {
                    if (match.Success)
                    {
                        string tmp = match.ToString();	//The regular expression matches the "<" and ">" signs around the filename. These signs have to be removed before adding the filename to the list
                        FileNames.Add(StartDateTime.Hour.ToString("D2") + "/" + tmp.Substring(1, tmp.Length - 2));
                    }
                }
            }

            Client.Dispose();
        }

        public async Task GetDataUsingHTTP(string URL, StorageFolder Folder, string File)
        {
            var URI = new Uri(URL);
            HttpClient Client = new HttpClient();

            var Message = await Client.GetAsync(URI);
            var Content = await Message.Content.ReadAsStringAsync();

            DataFile = await Folder.CreateFileAsync(File.Substring(3), CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(DataFile, Content);

            Client.Dispose();
        }
    }
}
