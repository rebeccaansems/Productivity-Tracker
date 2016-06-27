using System;
using System.IO;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Diagnostics;
using Android.OS;

namespace Productivity_Tracker
{

    [BroadcastReceiver]
    public class AlarmReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            var resultIntent = new Intent(context, typeof(MainActivity));
            resultIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);

            var pending = PendingIntent.GetActivity(context, 0,
                resultIntent,
                PendingIntentFlags.CancelCurrent);

            var builder =
                new Notification.Builder(context)
                    .SetDefaults(NotificationDefaults.All)
                    .SetContentTitle("Productivity Tracker")
                    .SetContentText("How productive are you feeling?")
                    .SetSmallIcon(Resource.Drawable.icon_notification)
                    //.SetWhen(DateTime.Now.Ticks);//one hour
                    .SetLights(100, 300, 1000);

            builder.SetContentIntent(pending);

            var notification = builder.Build();

            var manager = NotificationManager.FromContext(context);
            manager.Notify(1337, notification);
        }
    }
}