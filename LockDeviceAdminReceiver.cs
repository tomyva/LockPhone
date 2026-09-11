using Android.App.Admin;
using Android.Content;

namespace LockPhone;

[BroadcastReceiver(Label = "LockPhone device admin", Permission = "android.permission.BIND_DEVICE_ADMIN", Exported = true)]
[MetaData("android.app.device_admin", Resource = "@xml/device_admin_policies")]
[IntentFilter(new[] { DeviceAdminReceiver.ActionDeviceAdminEnabled })]
public class LockDeviceAdminReceiver : DeviceAdminReceiver
{
    public override void OnEnabled(Context context, Intent intent) { }
}
