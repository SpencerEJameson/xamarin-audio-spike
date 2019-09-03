using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using MediaManager;
using Plugin.FirebasePushNotification;

namespace xamarin_audio_spike.Droid
{
  [Activity(Label = "xamarin_audio_spike", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
  public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
  {
    protected override void OnCreate(Bundle savedInstanceState)
    {
      TabLayoutResource = Resource.Layout.Tabbar;
      ToolbarResource = Resource.Layout.Toolbar;

      base.OnCreate(savedInstanceState);

      CrossMediaManager.Current.Init(this);
      Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, savedInstanceState);

      if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
      {
        // default notification channel id
        FirebasePushNotificationManager.DefaultNotificationChannelId = "FirebasePushNotificationChannel";
        // default notification channel name
        FirebasePushNotificationManager.DefaultNotificationChannelName = "General";
      }

#if DEBUG
      FirebasePushNotificationManager.Initialize(this, true);
#else
      FirebasePushNotificationManager.Initialize(this, false);
#endif

      // handle notification when app is closed
      CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
      {
        // handle
      };

      Xamarin.Essentials.Platform.Init(this, savedInstanceState);
      global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
      LoadApplication(new App());
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
    {
      Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
      //PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);

      base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }
  }
}