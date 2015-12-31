using App1.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Storage.Streams;
using Windows.UI.Popups;



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
            StatusText.Text = String.Empty;

            await GetMETARUsingHTTP();

            StorageFolder Folder = await ApplicationData.Current.TemporaryFolder.GetFolderAsync("Files");
            StorageFile File = await Folder.GetFileAsync("METAR.txt");

            var Text = await FileIO.ReadLinesAsync(File);

            foreach (var line in Text.AsEnumerable().Skip(1))
            {
                StatusText.Text += line + "\r\n";
            }

            //Alternate method, but unsure how to handle max i and when lines < i
            //IList<string> lines = await FileIO.ReadLinesAsync(file);

            //for (int i = 1; i <= 5; i++ )
            //{
            //    StatusText.Text += lines[i];
            //    if (lines == null) break; ??
            //}
        }

        public static async Task GetMETARUsingHTTP()
        {
            Uri uri = new Uri("http://dd.weather.gc.ca/bulletins/alphanumeric/20151229/SA/CYVR/05/SACN62_CYVR_290500___36812");
            HttpClient Client = new HttpClient();

            var Message = await Client.GetAsync(uri);
            var Content = await Message.Content.ReadAsStringAsync();

            StorageFolder Folder = await ApplicationData.Current.TemporaryFolder.CreateFolderAsync("Files", CreationCollisionOption.OpenIfExists);
            StorageFile File = await Folder.CreateFileAsync("METAR.txt", CreationCollisionOption.ReplaceExisting);

            await FileIO.WriteTextAsync(File, Content);

            Client.Dispose();
        }
    }
}
