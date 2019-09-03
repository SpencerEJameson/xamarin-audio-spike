using System;
using xamarinaudiospike;

[assembly: Xamarin.Forms.Dependency(typeof(IVideoPlayer))]
namespace xamarin_audio_spike.iOS
{
  public class iOSVideoPlayer : IVideoPlayer
  {
    public void StartPlayer()
    {
      throw new NotImplementedException();
    }
  }
}
