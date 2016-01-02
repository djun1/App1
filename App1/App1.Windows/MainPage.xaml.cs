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
        private static List<string> DataFiles = new List<string>();
        private static List<string> HomeStationURLs = new List<string>();
        private static List<string> SortedURLs = new List<string>();
        private static string BaseURL;
        private StorageFolder DataFolder;
        private StorageFile DataFile;
        private static HttpResponseMessage Message;
        private static string HomeStation;
        private static string HomeStationURL;
        private static string[] DataTypeArray = { "SA", "SP", "FT", "FB", "FA" };

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
            HomeStation = "CYJT";
            BaseURL = "http://dd.weather.gc.ca/bulletins/alphanumeric/";
            DataFolder = await ApplicationData.Current.TemporaryFolder.CreateFolderAsync("Files", CreationCollisionOption.OpenIfExists);
            
            await GetListOfLatestFiles(HomeStationURLs, DataFiles, "SA");
            await GetListOfLatestFiles(HomeStationURLs, DataFiles, "SP");
            await GetListOfLatestFiles(HomeStationURLs, DataFiles, "FT");
            for (i = 0; i < DataFiles.Count(); i++)
            {
                // Sort URLs here.


                await GetDataUsingHTTP(HomeStationURLs[i], DataFolder, DataFiles[i]);

                

                var Text = await FileIO.ReadLinesAsync(DataFile);
                foreach (var line in Text.AsEnumerable().Skip(1))
                {
                    StatusText.Text += line + "\r\n";
                }
            }
        }
       
        public async Task GetListOfLatestFiles(List<string> URLs, List<string> FileNames, string DataType)
        {
            int i;
            Uri URI;
            DateTime CurrDateTime = DateTime.Now.ToUniversalTime();
            DateTime StartDateTime;
            string StartDateTimeString;
            string[] TAFAmendArray = { "", "AAA" };
            Regex RegEx;
            string RegExString;


            
            HttpClient Client = new HttpClient();

            for (i = 0; i < 3; i++) //Get list of METARs and SPECIs for the past 3 hours.
            {
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
                        string tmp = match.ToString();
                        FileNames.Add(tmp.Substring(1, tmp.Length - 2));
                        URLs.Add(HomeStationURL + tmp.Substring(1, tmp.Length - 2));
                    }
                }

            }

            if (DataType.Equals("FT")) //Get list of TAF and TAF Amends for the past 6 hours.
            {
                foreach (string TAFAmend in TAFAmendArray)
                {
                    for (i = 0; i < 6; i++)
                    {
                        StartDateTime = CurrDateTime.Subtract(new TimeSpan(i, 0, 0)); ;
                        StartDateTimeString = StartDateTime.Year.ToString() + StartDateTime.Month.ToString("D2")
                            + StartDateTime.Day.ToString("D2") + "/" + DataType + "/" + "CWAO" + "/"
                            + StartDateTime.Hour.ToString("D2") + "/";
                        HomeStationURL = BaseURL + StartDateTimeString;

                        URI = new Uri(HomeStationURL);

                        RegExString = ">[A-Z]+[0-9]+_+[A-Z]+_+[0-9]+_" + TAFAmend + "_" + HomeStation + "_[0-9]+<";

                        var HttpClientTask = Client.GetAsync(URI);
                        RegEx = new Regex(RegExString);
                        Message = await HttpClientTask;

                        MatchCollection Matches = RegEx.Matches(Message.Content.ToString());

                        foreach (Match match in Matches)
                        {
                            if (match.Success)
                            {
                                string tmp = match.ToString();
                                FileNames.Add(tmp.Substring(1, tmp.Length - 2));
                                URLs.Add(HomeStationURL + tmp.Substring(1, tmp.Length - 2));
                            }
                        }
                    }
                }

            }

            else if (DataType.Equals("FB")) //Get list of Upper Winds.
            {
                for (i = 0; i < 6; i++)
                {
                    StartDateTime = CurrDateTime.Subtract(new TimeSpan(i, 0, 0)); ;
                    StartDateTimeString = StartDateTime.Year.ToString() + StartDateTime.Month.ToString("D2")
                        + StartDateTime.Day.ToString("D2") + "/" + DataType + "/" + "CWAO" + "/"
                        + StartDateTime.Hour.ToString("D2") + "/";
                    HomeStationURL = BaseURL + StartDateTimeString;

                    URI = new Uri(HomeStationURL);

                    RegExString = ">[A-Z]+[0-9]+_+[A-Z]+_+[0-9]+___+[0-9]+<";

                    var HttpClientTask = Client.GetAsync(URI);
                    RegEx = new Regex(RegExString);
                    Message = await HttpClientTask;

                    MatchCollection Matches = RegEx.Matches(Message.Content.ToString());

                    foreach (Match match in Matches)
                    {
                        if (match.Success)
                        {
                            string tmp = match.ToString();
                            FileNames.Add(tmp.Substring(1, tmp.Length - 2));
                            URLs.Add(HomeStationURL + tmp.Substring(1, tmp.Length - 2));
                        }
                    }
                }
            }

            else if (DataType.Equals("FA")) //Get list of VFR Route Forecasts (available Mar-Oct only).
            {
                for (i = 0; i < 6; i++)
                {
                    StartDateTime = CurrDateTime.Subtract(new TimeSpan(i, 0, 0)); ;
                    StartDateTimeString = StartDateTime.Year.ToString() + StartDateTime.Month.ToString("D2")
                        + StartDateTime.Day.ToString("D2") + "/" + DataType + "/" + "CWEG" + "/"
                        + StartDateTime.Hour.ToString("D2") + "/";
                    HomeStationURL = BaseURL + HomeStationURL;

                    URI = new Uri(HomeStationURL);

                    RegExString = ">[A-Z]+[0-9]+_+[A-Z]+_+[0-9]+___+[0-9]+<";

                    var HttpClientTask = Client.GetAsync(URI);
                    RegEx = new Regex(RegExString);
                    Message = await HttpClientTask;

                    MatchCollection Matches = RegEx.Matches(Message.Content.ToString());

                    foreach (Match match in Matches)
                    {
                        if (match.Success)
                        {
                            string tmp = match.ToString();
                            FileNames.Add(tmp.Substring(1, tmp.Length - 2));
                            URLs.Add(HomeStationURL + tmp.Substring(1, tmp.Length - 2));
                        }
                    }
                }
            }
        }

        public async Task GetDataUsingHTTP(string URL, StorageFolder Folder, string File)
        {
            var URI = new Uri(URL);
            HttpClient Client = new HttpClient();

            var Message = await Client.GetAsync(URI);
            var Content = await Message.Content.ReadAsStringAsync();

            DataFile = await Folder.CreateFileAsync(File, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(DataFile, Content);
        }
    }
}
