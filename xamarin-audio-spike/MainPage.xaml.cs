using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using MediaManager;
using MediaManager.Media;
using MediaManager.Playback;
using Xamarin.Essentials;
using xamarinaudiospike;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Plugin.FirebasePushNotification;
using System.Reflection;
using System.IO;
using MediaManager.Library;

namespace xamarin_audio_spike
{
  // Learn more about making custom code visible in the Xamarin.Forms previewer
  // by visiting https://aka.ms/xamarinforms-previewer
  [DesignTimeVisible(false)]
  public partial class MainPage : ContentPage
  {
    IDownloader downloader = DependencyService.Get<IDownloader>();
    //IVideoPlayer videoPlayer = DependencyService.Get<IVideoPlayer>();

    public MainPage()
    {
      InitializeComponent();
      CrossFirebasePushNotification.Current.Subscribe("notification_new_episodes");
      CrossMediaManager.Current.PositionChanged += OnPositionChanged;
            //CrossMediaManager.Current.Notification.Enabled = true;
            CrossMediaManager.Current.MediaControls.NextImpl = new Action(() =>
            {
                //CrossMediaManager.Current.Pause();
                CrossMediaManager.Current.StepForward();
                //CrossMediaManager.Current.Play();
            });

            CrossMediaManager.Current.MediaControls.SeekForwardImpl = new Action(() =>
            {
                CrossMediaManager.Current.StepForward();
            });

            CrossMediaManager.Current.MediaControls.SkipBackwardImpl = new Action(() =>
            {
                CrossMediaManager.Current.StepBackward();
            });

            CrossMediaManager.Current.MediaControls.SkipForwardImpl = new Action(() =>
            {
                CrossMediaManager.Current.StepForward();
            });

            CrossMediaManager.Current.MediaControls.MediaButtonEventImpl = new Func<object, bool>((intent) =>
            {
                CrossMediaManager.Current.StepForward();
                return true;
            });

            //CrossMediaManager.Current.Bluetooth
            //CrossMediaManager.Current.
            downloader.OnFileDownloaded += OnFileDownloaded;

      SetupClipCreator();
    }

    private void OnFileDownloaded(object sender, DownloadEventArgs e)
    {
      if (e.FileSaved)
      {
        DisplayAlert("Downloader", "File saved successfully", "Close");
      }
      else
      {
        DisplayAlert("Downloader", "Error while saving the file", "Close");
      }
    }

    async void OnPlayButtonClicked(object sender, EventArgs e)
    {
      MediaItem mediaItem = new MediaItem("https://traffic.libsyn.com/secure/draudioarchives/07182019_the_dave_ramsey_show_archive_1.mp3?dest-id=412720");
     
      //mediaItem.Title = "some goober title";
      //mediaItem.Artist = "The Dave Ramsey Show";
      //mediaItem.Album = "Album name";

      //IList<MediaItem> mediaItems = new[] {
      //  new MediaItem("https://cdn.ramseysolutions.net/audio/mp3/askdave/1500_small_business/llc_partnerships_sole_proprietorships/1503_042914_accepting_cards.mp3"),
      //  new MediaItem("https://cdn.ramseysolutions.net/audio/mp3/askdave/600_debt/credit_card/0601_070611_credit_card_disagreement.mp3"),
      //  new MediaItem("https://cdn.ramseysolutions.net/audio/mp3/askdave/1600_stewardship/tithing/1602_020316_dont_tithe_with_credit_cards.mp3"),
      //  new MediaItem("https://cdn.ramseysolutions.net/audio/mp3/askdave/600_debt/credit_card/0601_080812_dont_need_cards.mp3"),
      //  new MediaItem("https://cdn.ramseysolutions.net/audio/mp3/askdave/600_debt/general/0600_112211_living_without_credit.mp3")
      //};

      await CrossMediaManager.Current.Play(mediaItem);
    }

    async void OnStopButtonClicked(object sender, EventArgs e)
    {
      await CrossMediaManager.Current.Stop();
    }

    void OnPositionChanged(object sender, PositionChangedEventArgs args)
    {

    }

    async void OnDownloadButtonClicked(object sender, EventArgs e)
    {

      string audioUrl = "https://traffic.libsyn.com/secure/draudioarchives/07312019_the_dave_ramsey_show_archive_1.mp3";
      //var mainDir = FileSystem.AppDataDirectory;

      //var destination = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "episode.mp3");

      //await new System.Net.WebClient().DownloadFileTaskAsync(new Uri(audioUrl), destination);

      PermissionStatus status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);

      await DisplayAlert("Pre - Results", status.ToString(), "OK");

      if (status != PermissionStatus.Granted)
      {
        if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Storage))
        {
          await DisplayAlert("Storing Files", "We need file storage permissions to be able to download episodes for offline playback", "OK");
        }

        var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
        status = results[Permission.Storage];
      }

      if (status == PermissionStatus.Granted)
      {
        downloader.DownloadFile(audioUrl, "episodes");
      }
      else if (status != PermissionStatus.Unknown)
      {
        await DisplayAlert("Storage Denied", "You'll need to go to settings and enable file storage", "OK");
      }
    }

    async void OnPlayDownloadedClicked(object sender, EventArgs e)
    {
      var destination = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "episodes/07312019_the_dave_ramsey_show_archive_1.mp3");

      destination = "file://" + destination;

      MediaItem mediaItem = new MediaItem(destination);

      mediaItem.Title = "some goober title";
      mediaItem.Artist = "The Dave Ramsey Show";
      mediaItem.Album = "Album name";

      await CrossMediaManager.Current.Play(mediaItem);
    }

    void OnYoutubeVideoClicked(object sender, EventArgs e)
    {
      //videoPlayer.StartPlayer();
    }

    void SetupClipCreator()
    {
      string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "clip_creator/");

      if (!Directory.Exists(path))
      {
        Directory.CreateDirectory(path);
      }

      var assembly = IntrospectionExtensions.GetTypeInfo(typeof(MainPage)).Assembly;

      Stream stream = assembly.GetManifestResourceStream("xamarin-audio-spike.testpage.html");
      FileStream html = File.Create(Path.Combine(path, "testpage.html"));
      stream.CopyTo(html);
      html.Close();

      Stream jsStream = assembly.GetManifestResourceStream("xamarin-audio-spike.somefuncs.js");
      FileStream js = File.Create(Path.Combine(path, "somefuncs.js"));
      jsStream.CopyTo(js);
      js.Close();

      Console.WriteLine($"EXISTS: {(File.Exists(Path.Combine(path, "testpage.html"))).ToString()}");
      Console.WriteLine($"EXISTS: {(File.Exists(Path.Combine(path, "somefuncs.js"))).ToString()}");
    }

    void OnWebviewButtonClicked(object sender, EventArgs e)
    {
      string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "clip_creator/");
      string htmlPath = Path.Combine(path, "testpage.html");
      string totalPath = $"file://{htmlPath}";

      UrlWebViewSource source = new UrlWebViewSource
      {
        Url = totalPath
      };

      myWebView.Source = source;
    }


    async void OnDoJSFunctionClicked(object sender, EventArgs e)
    {
            var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
            var status = results[Permission.Storage];
        

            //if (status == PermissionStatus.Granted)
            //{
            //    string deviceID = UIDevice.CurrentDevice.IdentifierForVendor.AsString();
            //    Console.WriteLine(deviceID);
            //    foreach (string file in Directory.EnumerateDirectories($"/var/mobile/Containers/Data/Application/E0A5E377-44F2-4A93-A78A-5B195BCA0E79"))
            //    {
            //        Console.WriteLine(file);
            //    }
            //}
            //Console.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            
      //string result = await myWebView.EvaluateJavaScriptAsync($"multiply({10})");
      //Console.WriteLine($"RESULT VALUE: {result}");
    }
  }
}
