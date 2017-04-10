using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Preferences;

namespace WeightTracker
{
    [Activity(Label = "PrefsActivity", Theme = "@android:style/Theme.Black.NoTitleBar")]			
    public class PrefsActivity : PreferenceActivity, ISharedPreferencesOnSharedPreferenceChangeListener
    {
        private bool _suppressChange = false;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            this.AddPreferencesFromResource(Resource.Xml.prefs);
        }

        protected override void OnResume()
        {
            base.OnResume();

            this.PreferenceScreen.SharedPreferences.RegisterOnSharedPreferenceChangeListener(this);
        }

        protected override void OnPause()
        {
            base.OnPause();
            this.PreferenceScreen.SharedPreferences.UnregisterOnSharedPreferenceChangeListener(this);
        }

        protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
        {
            var sync = DropboxSync.GetInstance(this.ApplicationContext);
            if (requestCode == Utilities.LINK_TO_DROPBOX_REQUEST && resultCode != Result.Canceled) 
            {
                sync.StartApp();
            } 
            else 
            {
                sync.UnlinkAccountManager();
                sync.LinkDropBoxAccount ();
            }
        }

        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
            if (_suppressChange)
            {
                _suppressChange = false;
                return;
            }

            var pref = this.FindPreference(key);
            if (pref.Key == "useDropbox")
            {
                var sync = DropboxSync.GetInstance(this);
                var value = ((CheckBoxPreference)pref).Checked;
                AlertDialog.Builder builder = null;

                if (value)
                {
                    builder = CreateBulder("Are you sure you want to link to Dropbox? You may be prompted to allow this application to have access to your Dropbox account.");
                    builder
                        .SetPositiveButton("Yes", (dialog, e) =>
                        {
                            sync.LinkDropBoxAccount();
                        })
                        .SetNegativeButton("No", (dialog, e) => 
                        {
                            _suppressChange = true;
                            ((CheckBoxPreference)pref).Checked = false; 
                        });
                }
                else
                {
                    builder = CreateBulder("Are you sure you want to unlink to Dropbox? Files will no longer sync to your Dropbox account.");
                    builder
                        .SetPositiveButton("Yes", (dialog, e) =>
                        {
                            sync.UnlinkAccountManager();
                        })
                        .SetNegativeButton("No", (dialog, e) => 
                        {
                            _suppressChange = true;
                            ((CheckBoxPreference)pref).Checked = true;
                        });
                }

                var alert = builder.Create();
                alert.Show();
            }
            else if (pref.Key == "useGoogleDrive")
            {
                // show alert for drive
            }
        }

        private AlertDialog.Builder CreateBulder(string message)
        {
            var builder = new AlertDialog.Builder(this);
            return builder.SetMessage(message);
        }
    }
}

