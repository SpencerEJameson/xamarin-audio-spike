using System;
using Android.Content;
using Xamarin.Forms;
using xamarinaudiospike;
using xamarin_audio_spike.Droid;
using Android.OS;
using Android.App;

[assembly: Dependency(typeof(AndroidVideoPlayer))]
namespace xamarin_audio_spike.Droid
{
  public class AndroidVideoPlayer : IVideoPlayer
  {
    public void StartPlayer()
    {
      var intent = new Intent(Android.App.Application.Context, typeof(YoutubePlayerActivity));
      intent.AddFlags(ActivityFlags.NewTask);
      Android.App.Application.Context.StartActivity(intent);
    }
  }
}
