using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using Xamarin.Forms;
using xamarinaudiospike;
using xamarin_audio_spike.Droid;

[assembly: Dependency(typeof(AndroidDownloader))]
namespace xamarin_audio_spike.Droid
{
  public class AndroidDownloader : IDownloader
  {
    public event EventHandler<DownloadEventArgs> OnFileDownloaded;

    public void DownloadFile(string url, string folder)
    {
      Console.WriteLine($"StorageDir: {Android.OS.Environment.ExternalStorageDirectory.AbsolutePath}");
      string pathToNewFolder = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, folder);
      Directory.CreateDirectory(pathToNewFolder);

      try
      {
        WebClient webClient = new WebClient();
        webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
        string pathToNewFile = Path.Combine(pathToNewFolder, Path.GetFileName(url));
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


