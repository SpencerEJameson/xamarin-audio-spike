using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using Xamarin.Forms;
using xamarin_audio_spike.iOS;
using xamarinaudiospike;

[assembly: Dependency(typeof(IosDownloader))]
namespace xamarin_audio_spike.iOS
{
  public class IosDownloader : IDownloader
  {
    public event EventHandler<DownloadEventArgs> OnFileDownloaded;

    public void DownloadFile(string url, string folder)
    {
      string pathToNewFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), folder);
      Directory.CreateDirectory(pathToNewFolder);

      try
      {
        WebClient webClient = new WebClient();
        webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
        string pathToNewFile = Path.Combine(pathToNewFolder, Path.GetFileName(url));
        Console.WriteLine(pathToNewFile);
        webClient.DownloadFileAsync(new Uri(url), pathToNewFile);
      }
      catch (Exception ex)
      {
        if (OnFileDownloaded != null)
          OnFileDownloaded.Invoke(this, new DownloadEventArgs(false));
      }
    }

    private void Completed(object sender, AsyncCompletedEventArgs e)
    {
      if (e.Error != null)
      {
        if (OnFileDownloaded != null)
          OnFileDownloaded.Invoke(this, new DownloadEventArgs(false));
      }
      else
      {
        if (OnFileDownloaded != null)
          OnFileDownloaded.Invoke(this, new DownloadEventArgs(true));
      }
    }
  }
}
