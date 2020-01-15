using System;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V4.Media.Session;
using MediaManager.Platforms.Android.MediaSession;

namespace xamarin_audio_spike.Droid
{
    public class MusicControls
    {
        private MusicControlsBroadcastReceiver mMessageReceiver;
        private MusicControlsNotification notification;
        private MediaSessionCompat mediaSessionCompat;
        private const int notificationID = 7824;
        private AudioManager mAudioManager;
        private PendingIntent mediaButtonPendingIntent;
        private bool mediaButtonAccess = true;
        private Context context;
        private Activity activity;

        private const string musicControlsButton = "music-controls-media-button";
        private MusicControlsCallback mMediaSessionCallback = new MusicControlsCallback();

        public MusicControls()
        {

        }

        private void RegisterBroadcaster(Context context, MusicControlsBroadcastReceiver mMessageReceiver)
        {
            context.RegisterReceiver(mMessageReceiver, new IntentFilter("music-controls-previous"));
            context.RegisterReceiver(mMessageReceiver, new IntentFilter("music-controls-pause"));
            context.RegisterReceiver(mMessageReceiver, new IntentFilter("music-controls-play"));
            context.RegisterReceiver(mMessageReceiver, new IntentFilter("music-controls-next"));
            context.RegisterReceiver(mMessageReceiver, new IntentFilter(musicControlsButton));
            context.RegisterReceiver(mMessageReceiver, new IntentFilter("music-controls-destroy"));
            context.RegisterReceiver(mMessageReceiver, new IntentFilter(Intent.ActionHeadsetPlug));
            context.RegisterReceiver(mMessageReceiver, new IntentFilter(BluetoothHeadset.ActionAudioStateChanged));
            context.RegisterReceiver(mMessageReceiver, new IntentFilter(BluetoothHeadset.ActionConnectionStateChanged));
            context.RegisterReceiver(mMessageReceiver, new IntentFilter(BluetoothHeadset.ActionVendorSpecificHeadsetEvent));
            context.RegisterReceiver(mMessageReceiver, new IntentFilter(BluetoothHeadset.ExtraVendorSpecificHeadsetEventCmd));
            context.RegisterReceiver(mMessageReceiver, new IntentFilter(BluetoothHeadset.ExtraVendorSpecificHeadsetEventArgs));
            context.RegisterReceiver(mMessageReceiver, new IntentFilter(BluetoothHeadset.ExtraVendorSpecificHeadsetEventCmdType));
            context.RegisterReceiver(mMessageReceiver, new IntentFilter(BluetoothHeadset.VendorResultCodeCommandAndroid));
            context.RegisterReceiver(mMessageReceiver, new IntentFilter(BluetoothHeadset.VendorSpecificHeadsetEventCompanyIdCategory));
            context.RegisterReceiver(mMessageReceiver, new IntentFilter(Intent.ActionMediaButton));

            //var l = LocalBroadcastManager.GetInstance(context);
            //l.RegisterReceiver(mMessageReceiver, new IntentFilter("music-controls-previous"));
            //l.RegisterReceiver(mMessageReceiver, new IntentFilter("music-controls-pause"));
            //l.RegisterReceiver(mMessageReceiver, new IntentFilter("music-controls-play"));
            //l.RegisterReceiver(mMessageReceiver, new IntentFilter("music-controls-next"));
            //l.RegisterReceiver(mMessageReceiver, new IntentFilter("music-controls-media-button"));
            //l.RegisterReceiver(mMessageReceiver, new IntentFilter("music-controls-destroy"));
            //l.RegisterReceiver(mMessageReceiver, new IntentFilter(Intent.ActionHeadsetPlug));
            //l.RegisterReceiver(mMessageReceiver, new IntentFilter(BluetoothHeadset.ActionAudioStateChanged));
            //l.RegisterReceiver(mMessageReceiver, new IntentFilter(BluetoothHeadset.ActionConnectionStateChanged));
            //l.RegisterReceiver(mMessageReceiver, new IntentFilter(BluetoothHeadset.ActionVendorSpecificHeadsetEvent));
            //l.RegisterReceiver(mMessageReceiver, new IntentFilter(BluetoothHeadset.ExtraVendorSpecificHeadsetEventCmd));
            //l.RegisterReceiver(mMessageReceiver, new IntentFilter(BluetoothHeadset.ExtraVendorSpecificHeadsetEventArgs));
            //l.RegisterReceiver(mMessageReceiver, new IntentFilter(BluetoothHeadset.ExtraVendorSpecificHeadsetEventCmdType));
            //l.RegisterReceiver(mMessageReceiver, new IntentFilter(BluetoothHeadset.VendorResultCodeCommandAndroid));
            //l.RegisterReceiver(mMessageReceiver, new IntentFilter(BluetoothHeadset.VendorSpecificHeadsetEventCompanyIdCategory));
        }

        public void RegisterMediaButtonEvent()
        {
            mediaSessionCompat.SetMediaButtonReceiver(mediaButtonPendingIntent);

            //mAudioManager.RegisterMediaButtonEventReceiver(mediaButtonPendingIntent);
        }

        public void UnregisterMediaButtonEvent()
        {
            mediaSessionCompat.SetMediaButtonReceiver(null);
        }


        public void Initialize(Context context)
        {
            mMessageReceiver = new MusicControlsBroadcastReceiver(this);
            RegisterBroadcaster(context, mMessageReceiver);

            mediaSessionCompat = new MediaSessionCompat(context, "music-controls-media-session", null, mediaButtonPendingIntent);
            mediaSessionCompat.SetFlags(MediaSessionCompat.FlagHandlesMediaButtons | MediaSessionCompat.FlagHandlesTransportControls);

            SetMediaPlaybackState((int)PlaybackStateCompat.StatePaused);

            mediaSessionCompat.Active = true;
            mediaSessionCompat.SetCallback(mMediaSessionCallback);

            try
            {
                mAudioManager = (AudioManager)context.GetSystemService(Context.AudioService);
                var headsetIntent = new Intent(musicControlsButton);
                mediaButtonPendingIntent = PendingIntent.GetBroadcast(context, 0, headsetIntent, PendingIntentFlags.UpdateCurrent);
                RegisterMediaButtonEvent();

                //MediaButtonReceiver.HandleIntent(mediaSessionCompat, headsetIntent);
                //BroadcastReceiver.
            }
            catch (Exception exc)
            {

            }
        }

        private void SetMediaPlaybackState(int state)
        {
            PlaybackStateCompat.Builder playbackstateBuilder = new PlaybackStateCompat.Builder();
            if (state == PlaybackStateCompat.StatePlaying)
            {
                playbackstateBuilder.SetActions(PlaybackStateCompat.ActionPlayPause | PlaybackStateCompat.ActionPause | PlaybackStateCompat.ActionSkipToNext | PlaybackStateCompat.ActionSkipToPrevious |
                    PlaybackStateCompat.ActionPlayFromMediaId |
                    PlaybackStateCompat.ActionPlayFromSearch | PlaybackStateCompat.ActionPlayFromUri| PlaybackStateCompat.ActionPlay);
                playbackstateBuilder.SetState(state, PlaybackStateCompat.PlaybackPositionUnknown, 1.0f);
            }
            else
            {
                playbackstateBuilder.SetActions(PlaybackStateCompat.ActionPlayPause | PlaybackStateCompat.ActionPlay | PlaybackStateCompat.ActionSkipToNext | PlaybackStateCompat.ActionSkipToPrevious |
                    PlaybackStateCompat.ActionPlayFromMediaId |
                    PlaybackStateCompat.ActionPlayFromSearch | PlaybackStateCompat.ActionPlayFromUri | PlaybackStateCompat.ActionPause);
                playbackstateBuilder.SetState(state, PlaybackStateCompat.PlaybackPositionUnknown, 0);
            }
            mediaSessionCompat.SetPlaybackState(playbackstateBuilder.Build());
        }
    }

    [BroadcastReceiver(Enabled = true, Exported = true)]
    public class MusicControlsBroadcastReceiver : BroadcastReceiver
    {
        private MusicControls mc;

        public MusicControlsBroadcastReceiver()
        {

        }

        public MusicControlsBroadcastReceiver(MusicControls musicControls)
        {
            mc = musicControls;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            Console.WriteLine("MUSIC CONTROLS");
        }
    }

    public class MusicControlsCallback : MediaSessionCompat.Callback
    {
        public override void OnPlay()
        {
            Console.WriteLine("OnPlay");
            base.OnPlay();
        }

        public override bool OnMediaButtonEvent(Intent mediaButtonEvent)
        {
            Console.WriteLine("OnMediaButtonEvent");
            return base.OnMediaButtonEvent(mediaButtonEvent);
        }

        public override void OnSkipToNext()
        {
            Console.WriteLine("OnSkipToNext");
            base.OnSkipToNext();
        }

        public override void OnSkipToPrevious()
        {
            Console.WriteLine("OnSkipToPrevious");
            base.OnSkipToPrevious();
        }

        public override void OnPause()
        {
            Console.WriteLine("OnPause");
            base.OnPause();
        }

        public override void OnPlayFromUri(Android.Net.Uri uri, Bundle extras)
        {
            base.OnPlayFromUri(uri, extras);
        }

        public override void OnPlayFromMediaId(string mediaId, Bundle extras)
        {
            base.OnPlayFromMediaId(mediaId, extras);
        }
    }


    public class MusicControlsNotification
    {
        Context context;
        public Notification.Builder builder;

        private void CreateBuilder()
        {
            Notification.Builder builder = new Notification.Builder(context);
            Intent previousIntent = new Intent("music-controls-previous");
            PendingIntent previousPendingIntent = PendingIntent.GetBroadcast(context, 1, previousIntent, 0);
            builder.AddAction(0, "", previousPendingIntent);


            Intent nextIntent = new Intent("music-controls-next");
            PendingIntent nextPendingIntent = PendingIntent.GetBroadcast(context, 1, nextIntent, 0);
            builder.AddAction(1, "", nextPendingIntent);


            this.builder = builder;
        }
    }
}
