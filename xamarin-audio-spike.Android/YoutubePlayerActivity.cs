using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using YouTube.Player;

namespace xamarin_audio_spike.Droid
{

  [Activity(
    Label = "@string/youtube_player_activity_name",
    ScreenOrientation = ScreenOrientation.Nosensor,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.KeyboardHidden)]
  [MetaData("@string/minVersion", Value = "8")]
  [MetaData("@string/isLaunchableActivity", Value = "true")]
  public class YoutubePlayerActivity : YouTubeBaseActivity, IYouTubePlayerOnInitializedListener
  {
    protected override void OnCreate(Bundle savedInstanceState)
    {
      base.OnCreate(savedInstanceState);

      SetContentView(Resource.Layout.player_view);

      var youtubeView = FindViewById<YouTubePlayerView>(Resource.Id.youtube_view);
      youtubeView.Initialize("AIzaSyD-hUABQjY-mKqocVvi6I6xoNvjRighGDI", this);
    }

    public void OnInitializationSuccess(IYouTubePlayerProvider provider, IYouTubePlayer player, bool wasRestored)
    {
      if (!wasRestored)
      {
        player.CueVideo("Uh_rSDdzubg");
        player.Play();
      }
    }

    public void OnInitializationFailure(IYouTubePlayerProvider provider, YouTubeInitializationResult errorReason)
    {
        Toast.MakeText(this, "An error occured starting Youtube", ToastLength.Long).Show();
    }
  }
}
