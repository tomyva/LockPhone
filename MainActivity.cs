using Android.App;
using Android.App.Admin;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Widget;

namespace LockPhone;

[Activity(Label = "LockPhone", MainLauncher = true, Exported = true)]
public class MainActivity : Activity
{
    private const int DeviceAdminRequest = 71;
    private Switch? _featureSwitch;
    private TextView? _statusText;
    private bool _changingSwitch;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.main);

        _featureSwitch = FindViewById<Switch>(Resource.Id.featureSwitch);
        _statusText = FindViewById<TextView>(Resource.Id.statusText);
        FindViewById<Button>(Resource.Id.setupButton)!.Click += (_, _) => StartSetup();
        _featureSwitch!.CheckedChange += (_, e) =>
        {
            if (!_changingSwitch)
                SetFeature(e.IsChecked);
        };
    }

    protected override void OnResume()
    {
        base.OnResume();
        RefreshUi();
    }

    private void SetFeature(bool enabled)
    {
        if (!enabled)
        {
            GetSharedPreferences("LockPhone", FileCreationMode.Private)!.Edit()!.PutBoolean("feature_enabled", false)!.Apply();
            RefreshUi();
            return;
        }

        if (!HasDeviceAdmin())
        {
            StartSetup();
            return;
        }

        GetSharedPreferences("LockPhone", FileCreationMode.Private)!.Edit()!.PutBoolean("feature_enabled", true)!.Apply();
        StartActivity(new Intent(Settings.ActionAccessibilitySettings));
    }

    private void StartSetup()
    {
        if (!HasDeviceAdmin())
        {
            var component = new ComponentName(this, Java.Lang.Class.FromType(typeof(LockDeviceAdminReceiver))!);
            var intent = new Intent(DevicePolicyManager.ActionAddDeviceAdmin);
            intent.PutExtra(DevicePolicyManager.ExtraDeviceAdmin, component);
            intent.PutExtra(DevicePolicyManager.ExtraAddExplanation, "LockPhone needs permission to lock the screen after a double tap.");
            StartActivityForResult(intent, DeviceAdminRequest);
        }
        else
        {
            StartActivity(new Intent(Settings.ActionAccessibilitySettings));
        }
    }

    private bool HasDeviceAdmin()
    {
        var policy = (DevicePolicyManager?)GetSystemService(DevicePolicyService);
        var component = new ComponentName(this, Java.Lang.Class.FromType(typeof(LockDeviceAdminReceiver))!);
        return policy?.IsAdminActive(component) == true;
    }

    private void RefreshUi()
    {
        var enabled = GetSharedPreferences("LockPhone", FileCreationMode.Private)!.GetBoolean("feature_enabled", false) && HasDeviceAdmin();
        _changingSwitch = true;
        _featureSwitch!.Checked = enabled;
        _changingSwitch = false;
        _statusText!.Text = enabled ? "On — finish accessibility setup" : "Off";
        _statusText.SetTextColor(Android.Graphics.Color.ParseColor(enabled ? "#1D7A46" : "#D14343"));
    }
}
