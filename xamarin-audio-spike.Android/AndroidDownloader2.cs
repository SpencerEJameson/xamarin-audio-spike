using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using Xamarin.Forms;
using xamarinaudiospike;
using xamarin_audio_spike.Droid;

[assembly: Dependency(typeof(AndroidDownloader2))]
namespace xamarin_audio_spike.Droid
{
  public class AndroidDownloader2 : IDownloader
  {
    public event EventHandler<DownloadEventArgs> OnFileDownloaded;

    public void DownloadFile(string url, string folder)
    {
      Console.WriteLine("WEE I'M DOWNLOADING");
    }

    private void Completed(object sender, AsyncCompletedEventArgs e)
    {
      
    }
  }
}


