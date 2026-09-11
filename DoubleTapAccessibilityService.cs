using Android.AccessibilityServices;
using Android.App.Admin;
using Android.Content;
using Android.Views.Accessibility;

namespace LockPhone;

[Service(Label = "LockPhone double tap", Permission = "android.permission.BIND_ACCESSIBILITY_SERVICE", Exported = true)]
[IntentFilter(new[] { "android.accessibilityservice.AccessibilityService" })]
[MetaData("android.accessibilityservice", Resource = "@xml/accessibility_service_config")]
public class DoubleTapAccessibilityService : AccessibilityService
{
    public override void OnAccessibilityEvent(AccessibilityEvent? e) { }
    public override void OnInterrupt() { }

    public override bool OnGesture(AccessibilityGestureEvent gestureEvent)
    {
        if (gestureEvent.GestureId == (int)AccessibilityGesture.DoubleTap && FeatureEnabled())
        {
            var policy = (DevicePolicyManager?)GetSystemService(DevicePolicyService);
            var component = new ComponentName(this, Java.Lang.Class.FromType(typeof(LockDeviceAdminReceiver))!);
            if (policy?.IsAdminActive(component) == true)
                policy.LockNow();
            return true;
        }
        return base.OnGesture(gestureEvent);
    }

    private bool FeatureEnabled() => GetSharedPreferences("LockPhone", FileCreationMode.Private)!
        .GetBoolean("feature_enabled", false);
}
