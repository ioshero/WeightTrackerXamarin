using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using System.IO;
using System.Collections.Generic;
using Android.Views.InputMethods;
using DropboxSync.Android;
using System.Threading.Tasks;
using Android.Preferences;

namespace WeightTracker
{
    [Activity (Label = "Weight Tracker", MainLauncher = true)]
	public class MainActivity : Activity
	{
        #region Private Variables

		private ListView _measurementListView = null;
		private MeasurementAdapter _adapter = null;

        private double _totalSizeChange = 0;
        private double _grandTotalSizeChange = 0;

		private double _currentMeasurementValue = 0;

        #region EditTexts

		private EditText _weightTextBox = null;
		private EditText _leftArmTextBox = null;
		private EditText _rightArmTextBox = null;
		private EditText _chestTextBox = null;
		private EditText _waistTextBox = null;
		private EditText _absTextBox = null;
		private EditText _hipsTextBox = null;
		private EditText _leftThighTextBox = null;
		private EditText _rightThighTextBox = null;
		private EditText _leftCalfTextBox = null;
		private EditText _rightCalfTextBox = null;

        #endregion

        #region TextViews

        private TextView _dateTextView = null;

		private TextView _weightChange = null;
		private TextView _leftArmChange = null;
		private TextView _rightArmChange = null;
		private TextView _chestChange = null;
		private TextView _waistChange = null;
		private TextView _absChange = null;
		private TextView _hipsChange = null;
		private TextView _leftThighChange = null;
		private TextView _rightThighChange = null;
		private TextView _leftCalfChange = null;
		private TextView _rightCalfChange = null;

        private TextView _weightChangeTotal = null;
        private TextView _leftArmChangeTotal = null;
        private TextView _rightArmChangeTotal = null;
        private TextView _chestChangeTotal = null;
        private TextView _waistChangeTotal = null;
        private TextView _absChangeTotal = null;
        private TextView _hipsChangeTotal = null;
        private TextView _leftThighChangeTotal = null;
        private TextView _rightThighChangeTotal = null;
        private TextView _leftCalfChangeTotal = null;
        private TextView _rightCalfChangeTotal = null;
        private TextView _prevMeasChangeTotal = null;
        private TextView _totalMeasChange = null;

        #endregion

		private BodyMeasurements _lastMeasurement = null;
        private BodyMeasurements _firstMesurement = null;
		private DateTime _currentDate = DateTime.Today;

		private List<BodyMeasurements> _bodyMeasurements = null;

        #endregion

        public bool UseDropbox
        {
            get
            {
                var prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                return prefs.GetBoolean("useDropbox", false);
            }
        }

//        public bool UseGoogleDrive
//        {
//            get
//            {
//                var prefs = PreferenceManager.GetDefaultSharedPreferences(this);
//                return prefs.GetBoolean("useGoogleDrive", false);
//            }
//        }

		private const int DATE_DIALOG_ID = 0;

        protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			Utilities.CreateApplicationDirectory ();
			Utilities.CreateTables ();

			var currentUser = Settings.LoadSettings ().GetCurrentUser ();
			this.ActionBar.Title = "Measurements Tracker";
			this.ActionBar.Subtitle = string.Format ("Current User: {0}", currentUser.UserName);
			
            this.RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;
            var saveButton = FindViewById<Button>(Resource.Id.SaveButton);
            saveButton.Click += SaveMeasurements;

			_dateTextView = FindViewById<TextView> (Resource.Id.Date);
			_weightTextBox = FindViewById<EditText> (Resource.Id.WeightTextBox);
			_leftArmTextBox = FindViewById<EditText> (Resource.Id.LeftArmTextBox);
			_rightArmTextBox = FindViewById<EditText> (Resource.Id.RightArmTextBox);
			_chestTextBox = FindViewById<EditText> (Resource.Id.ChestTextBox);
			_waistTextBox = FindViewById<EditText> (Resource.Id.WaistTextBox);
			_absTextBox = FindViewById<EditText> (Resource.Id.AbsTextBox);
			_hipsTextBox = FindViewById<EditText> (Resource.Id.HipsTextBox);
			_leftThighTextBox = FindViewById<EditText> (Resource.Id.LeftThighTextBox);
			_rightThighTextBox = FindViewById<EditText> (Resource.Id.RightThighTextBox);
			_leftCalfTextBox = FindViewById<EditText> (Resource.Id.LeftCalfTextBox);
			_rightCalfTextBox = FindViewById<EditText> (Resource.Id.RightCalfTextBox);

			_weightChange = FindViewById<TextView> (Resource.Id.WeightChange);
			_leftArmChange = FindViewById<TextView> (Resource.Id.LeftArmChange);
			_rightArmChange = FindViewById<TextView> (Resource.Id.RightArmChange);
			_chestChange = FindViewById<TextView> (Resource.Id.ChestChange);
			_waistChange = FindViewById<TextView> (Resource.Id.WaistChange);
			_absChange = FindViewById<TextView> (Resource.Id.AbsChange);
			_hipsChange = FindViewById<TextView> (Resource.Id.HipsChange);
			_leftThighChange = FindViewById<TextView> (Resource.Id.LeftThighChange);
			_rightThighChange = FindViewById<TextView> (Resource.Id.RightThighChange);
			_leftCalfChange = FindViewById<TextView> (Resource.Id.LeftCalfChange);
			_rightCalfChange = FindViewById<TextView> (Resource.Id.RightCalfChange);

            _weightChangeTotal = FindViewById<TextView> (Resource.Id.WeightChangeTotal);
            _leftArmChangeTotal = FindViewById<TextView> (Resource.Id.LeftArmChangeTotal);
            _rightArmChangeTotal = FindViewById<TextView> (Resource.Id.RightArmChangeTotal);
            _chestChangeTotal = FindViewById<TextView> (Resource.Id.ChestChangeTotal);
            _waistChangeTotal = FindViewById<TextView> (Resource.Id.WaistChangeTotal);
            _absChangeTotal = FindViewById<TextView> (Resource.Id.AbsChangeTotal);
            _hipsChangeTotal = FindViewById<TextView> (Resource.Id.HipsChangeTotal);
            _leftThighChangeTotal = FindViewById<TextView> (Resource.Id.LeftThighChangeTotal);
            _rightThighChangeTotal = FindViewById<TextView> (Resource.Id.RightThighChangeTotal);
            _leftCalfChangeTotal = FindViewById<TextView> (Resource.Id.LeftCalfChangeTotal);
            _rightCalfChangeTotal = FindViewById<TextView> (Resource.Id.RightCalfChangeTotal);
            _prevMeasChangeTotal = FindViewById<TextView>(Resource.Id.PrevMeasurementChangeTotal);
            _totalMeasChange = FindViewById<TextView>(Resource.Id.TotalMeasurementChange);

			_weightTextBox.FocusChange += TextViewFocusChanged;
			_leftArmTextBox.FocusChange += TextViewFocusChanged;
			_rightArmTextBox.FocusChange += TextViewFocusChanged;
			_chestTextBox.FocusChange += TextViewFocusChanged;
			_waistTextBox.FocusChange += TextViewFocusChanged;
			_absTextBox.FocusChange += TextViewFocusChanged;
			_hipsTextBox.FocusChange += TextViewFocusChanged;
			_leftThighTextBox.FocusChange += TextViewFocusChanged;
			_rightThighTextBox.FocusChange += TextViewFocusChanged;
			_leftCalfTextBox.FocusChange += TextViewFocusChanged;
			_rightCalfTextBox.FocusChange += TextViewFocusChanged;
            _rightCalfTextBox.KeyPress += RightCalfKeyPress;

			_dateTextView.Click += ShowDatePicker;

			_measurementListView = FindViewById<ListView> (Resource.Id.MeasurementsListView);
            _measurementListView.ItemClick += ListViewClick;
            _measurementListView.ItemsCanFocus = true;
            _measurementListView.Focusable = false;
            _measurementListView.FocusableInTouchMode = false;
            _measurementListView.Clickable = true;
			
            PopulateMeasurements();
            UpdateDateView ();

            if (this.UseDropbox)
            {
                var sync = DropboxSync.GetInstance(this);
                sync.LinkDropBoxAccount();
            }
		}

        public override bool OnTouchEvent(MotionEvent e)
        {
            HideKeyboard();
            return base.OnTouchEvent(e);
        }

        protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
        {
            var sync = DropboxSync.GetInstance(this.ApplicationContext);
            if (requestCode == Utilities.LINK_TO_DROPBOX_REQUEST && resultCode != Result.Canceled) 
            {
                sync.StartApp();
            } 
            else if (requestCode == Utilities.LINK_TO_DROPBOX_REQUEST && resultCode == Result.Canceled)
            {
                sync.UnlinkAccountManager();
                sync.LinkDropBoxAccount ();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            this.MenuInflater.Inflate(Resource.Menu.MainMenu, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch(item.ItemId)
            {
                case Resource.Id.ActionViewCharts:
                    Intent intent = new Intent (this, typeof(ChartViewerActivity));
                    StartActivity(intent);
                    break;
                case Resource.Id.ActionSettings:
                    Intent inte = new Intent(this, typeof(PrefsActivity));
                    StartActivity(inte);
                    break;
				case Resource.Id.ActionViewUsers:
					Intent userIntent = new Intent (this, typeof(UsersActivity));
					StartActivity (userIntent);
					break;
				case Resource.Id.ActionSetCurrentUser:
					ShowSetCurrentUser ();
					break;
            }

            return base.OnOptionsItemSelected(item);
        }

        protected override Dialog OnCreateDialog (int id)
        {
            switch (id) {
                case DATE_DIALOG_ID:
                    return new DatePickerDialog (this, OnDateSet, _currentDate.Year, _currentDate.Month - 1, _currentDate.Day); 
            }
            return null;
        }

		private void ShowSetCurrentUser()
		{
			var builder = new AlertDialog.Builder (this);

			LinearLayout layout = new LinearLayout (this);
			layout.Orientation = Orientation.Horizontal;
			TextView textView = new TextView (this);
			textView.Text = "Select User Name:";
			textView.SetWidth (200);

			LinearLayout.LayoutParams margin = new LinearLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent,
				RelativeLayout.LayoutParams.WrapContent);
			margin.LeftMargin = 60;
			textView.LayoutParameters = margin;

			Spinner spinner = new Spinner (this);
			var users = User.GetUsers ();
			var userNames = users.Select (t => t.UserName).ToList();
			var adapter = new ArrayAdapter<string> (this, Android.Resource.Layout.SimpleSpinnerDropDownItem, userNames);
			spinner.Adapter = adapter;

			layout.AddView (textView);
			layout.AddView (spinner);

			builder.SetView(layout)
				.SetTitle ("Choose User")
				.SetPositiveButton ("OK", (dialog, id) =>
				{
					int index = spinner.SelectedItemPosition;
					var user = users[index];
					var setting = Settings.LoadSettings();
					setting.CurrentUserID = user.ID;
					setting.Save();

					this.ActionBar.Subtitle = string.Format ("Current User: {0}", user.UserName);
					PopulateMeasurements();
				})
				.SetNegativeButton ("Cancel", (dialog, id) => { });

			var alert = builder.Create ();
			alert.Show ();
		}

        private void ShowDatePicker (object sender, EventArgs e)
        {
            ShowDialog (DATE_DIALOG_ID);
        }

        private void OnDateSet (object sender, DatePickerDialog.DateSetEventArgs e)
        {
            _currentDate = e.Date;
            UpdateDateView ();
            ResetPreviousMeasurement();
        }

        private void RightCalfKeyPress (object sender, View.KeyEventArgs e)
        {
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                HideKeyboard();
            }

            e.Handled = false;
        }

        private void ListViewClick (object sender, AdapterView.ItemClickEventArgs e)
        {
            var selectedMeasurement = _bodyMeasurements[e.Position];

            AlertDialog.Builder builder = new AlertDialog.Builder (this);
            View view = View.Inflate (this, Resource.Layout.DisplayMeasurement, null);

            var textView = view.FindViewById<TextView>(Resource.Id.DisplayWeight);
            textView.Text = selectedMeasurement.Weight.ToString() + " lbs";

            textView = view.FindViewById<TextView>(Resource.Id.DisplayLeftArm);
            textView.Text = selectedMeasurement.LeftArm.ToString();

            textView = view.FindViewById<TextView>(Resource.Id.DisplayRightArm);
            textView.Text = selectedMeasurement.RightArm.ToString();

            textView = view.FindViewById<TextView>(Resource.Id.DisplayChest);
            textView.Text = selectedMeasurement.Chest.ToString();

            textView = view.FindViewById<TextView>(Resource.Id.DisplayWaist);
            textView.Text = selectedMeasurement.Waist.ToString();

            textView = view.FindViewById<TextView>(Resource.Id.DisplayAbs);
            textView.Text = selectedMeasurement.Abdominal.ToString();

            textView = view.FindViewById<TextView>(Resource.Id.DisplayHips);
            textView.Text = selectedMeasurement.Hips.ToString();

            textView = view.FindViewById<TextView>(Resource.Id.DisplayLeftThigh);
            textView.Text = selectedMeasurement.LeftThigh.ToString();

            textView = view.FindViewById<TextView>(Resource.Id.DisplayRightThigh);
            textView.Text = selectedMeasurement.RightThigh.ToString();

            textView = view.FindViewById<TextView>(Resource.Id.DisplayLeftCalf);
            textView.Text = selectedMeasurement.LeftCalf.ToString();

            textView = view.FindViewById<TextView>(Resource.Id.DisplayRightCalf);
            textView.Text = selectedMeasurement.RightCalf.ToString();

            textView = view.FindViewById<TextView>(Resource.Id.DisplayTotalSizeChange);
            textView.Text = selectedMeasurement.TotalSizeChange.ToString() + " inches";

            textView = view.FindViewById<TextView>(Resource.Id.DisplayTotalWeightChange);
            textView.Text = selectedMeasurement.TotalWeightChange.ToString() + " lbs";

            builder.SetView(view)
                .SetTitle("Measurements for " + selectedMeasurement.Date.ToString("MM-dd-yyyy"))
                .SetPositiveButton("OK", (dialog, es) => { });

            var alert = builder.Create();
            alert.Show();
        }

        private void SaveMeasurements (object sender, EventArgs e)
        {
            var bodyMeas = new BodyMeasurements ();
            bodyMeas.Date = _currentDate;

			bodyMeas.UserID = Settings.LoadSettings ().CurrentUserID;

            if (!string.IsNullOrEmpty (_weightTextBox.Text))
                bodyMeas.Weight =  _weightTextBox.Text.AsDouble ();

            if (!string.IsNullOrEmpty (_leftArmTextBox.Text))
                bodyMeas.LeftArm = _leftArmTextBox.Text.AsDouble ();

            if (!string.IsNullOrEmpty (_rightArmTextBox.Text))
                bodyMeas.RightArm = _rightArmTextBox.Text.AsDouble ();

            if (!string.IsNullOrEmpty (_chestTextBox.Text))
                bodyMeas.Chest = _chestTextBox.Text.AsDouble ();

            if (!string.IsNullOrEmpty (_waistTextBox.Text))
                bodyMeas.Waist = _waistTextBox.Text.AsDouble ();

            if (!string.IsNullOrEmpty (_absTextBox.Text))
                bodyMeas.Abdominal = _absTextBox.Text.AsDouble ();

            if (!string.IsNullOrEmpty (_hipsTextBox.Text))
                bodyMeas.Hips = _hipsTextBox.Text.AsDouble ();

            if (!string.IsNullOrEmpty (_leftThighTextBox.Text))
                bodyMeas.LeftThigh = _leftThighTextBox.Text.AsDouble ();

            if (!string.IsNullOrEmpty (_rightThighTextBox.Text))
                bodyMeas.RightThigh = _rightThighTextBox.Text.AsDouble ();

            if (!string.IsNullOrEmpty (_leftCalfTextBox.Text))
                bodyMeas.LeftCalf = _leftCalfTextBox.Text.AsDouble ();

            if (!string.IsNullOrEmpty (_rightCalfTextBox.Text))
                bodyMeas.RightCalf = _rightCalfTextBox.Text.AsDouble ();

            if (_lastMeasurement != null)
            {
                if (bodyMeas.Weight > 0)
                    bodyMeas.TotalWeightChange = bodyMeas.Weight - _lastMeasurement.Weight;

                bodyMeas.TotalSizeChange = _totalSizeChange;
            }

            bodyMeas.Save ();
            _lastMeasurement = bodyMeas;
            _totalSizeChange = 0;

            ClearScreen ();
            PopulateMeasurements();

            if (this.UseDropbox)
            {
                var sync = DropboxSync.GetInstance(this);
                sync.UpdateDropbox();
            }
        }

		private void TextViewFocusChanged (object sender, View.FocusChangeEventArgs e)
        {
            if (e.HasFocus)
			{
				_currentMeasurementValue = ((TextView)sender).Text.AsDouble ();
				return;
			}

            if (_lastMeasurement == null)
                ResetPreviousMeasurement();

            if (_firstMesurement == null)
                _firstMesurement = BodyMeasurements.GetFirstMeasurement();

            TextView changeTextView = null;
            double previousValue = 0;
            double newValue = 0;

            TextView changeTotalTextView = null;
            double previousValueTotal = 0;

            if (sender == _weightTextBox)
            {
                changeTextView = FindViewById<TextView> (Resource.Id.WeightChange);
                previousValue = (_lastMeasurement != null) ? _lastMeasurement.Weight : 0;
                newValue = _weightTextBox.Text.AsDouble ();

                changeTotalTextView = FindViewById<TextView>(Resource.Id.WeightChangeTotal);
                previousValueTotal = (_firstMesurement != null) ? _firstMesurement.Weight : 0;
            }
            else if (sender == _leftArmTextBox)
            {
                changeTextView = FindViewById<TextView> (Resource.Id.LeftArmChange);
                previousValue = (_lastMeasurement != null) ? _lastMeasurement.LeftArm : 0;
                newValue = _leftArmTextBox.Text.AsDouble ();

                changeTotalTextView = FindViewById<TextView>(Resource.Id.LeftArmChangeTotal);
                previousValueTotal = (_firstMesurement != null) ? _firstMesurement.LeftArm : 0;
            }
            else if (sender == _rightArmTextBox)
            {
                changeTextView = FindViewById<TextView> (Resource.Id.RightArmChange);
                previousValue = (_lastMeasurement != null) ? _lastMeasurement.RightArm : 0;
                newValue = _rightArmTextBox.Text.AsDouble ();

                changeTotalTextView = FindViewById<TextView>(Resource.Id.RightArmChangeTotal);
                previousValueTotal = (_firstMesurement != null) ? _firstMesurement.RightArm : 0;
            }
            else if (sender == _chestTextBox)
            {
                changeTextView = FindViewById<TextView> (Resource.Id.ChestChange);
                previousValue = (_lastMeasurement != null) ? _lastMeasurement.Chest : 0;
                newValue = _chestTextBox.Text.AsDouble ();

                changeTotalTextView = FindViewById<TextView>(Resource.Id.ChestChangeTotal);
                previousValueTotal = (_firstMesurement != null) ? _firstMesurement.Chest : 0;
            }
            else if (sender == _waistTextBox)
            {
                changeTextView = FindViewById<TextView> (Resource.Id.WaistChange);
                previousValue = (_lastMeasurement != null) ? _lastMeasurement.Waist : 0;
                newValue = _waistTextBox.Text.AsDouble ();

                changeTotalTextView = FindViewById<TextView>(Resource.Id.WaistChangeTotal);
                previousValueTotal = (_firstMesurement != null) ? _firstMesurement.Waist : 0;
            }
            else if (sender == _absTextBox)
            {
                changeTextView = FindViewById<TextView> (Resource.Id.AbsChange);
                previousValue = (_lastMeasurement != null) ? _lastMeasurement.Abdominal : 0;
                newValue = _absTextBox.Text.AsDouble ();

                changeTotalTextView = FindViewById<TextView>(Resource.Id.AbsChangeTotal);
                previousValueTotal = (_firstMesurement != null) ? _firstMesurement.Abdominal : 0;
            }
            else if (sender == _hipsTextBox)
            {
                changeTextView = FindViewById<TextView> (Resource.Id.HipsChange);
                previousValue = (_lastMeasurement != null) ? _lastMeasurement.Hips : 0;
                newValue = _hipsTextBox.Text.AsDouble ();

                changeTotalTextView = FindViewById<TextView>(Resource.Id.HipsChangeTotal);
                previousValueTotal = (_firstMesurement != null) ? _firstMesurement.Hips : 0;
            }
            else if (sender == _leftThighTextBox)
            {
                changeTextView = FindViewById<TextView> (Resource.Id.LeftThighChange);
                previousValue = (_lastMeasurement != null) ? _lastMeasurement.LeftThigh : 0;
                newValue = _leftThighTextBox.Text.AsDouble ();

                changeTotalTextView = FindViewById<TextView>(Resource.Id.LeftThighChangeTotal);
                previousValueTotal = (_firstMesurement != null) ? _firstMesurement.LeftThigh : 0;
            }
            else if (sender == _rightThighTextBox)
            {
                changeTextView = FindViewById<TextView> (Resource.Id.RightThighChange);
                previousValue = (_lastMeasurement != null) ? _lastMeasurement.RightThigh : 0;
                newValue = _rightThighTextBox.Text.AsDouble ();

                changeTotalTextView = FindViewById<TextView>(Resource.Id.RightThighChangeTotal);
                previousValueTotal = (_firstMesurement != null) ? _firstMesurement.RightThigh : 0;
            }
            else if (sender == _leftCalfTextBox)
            {
                changeTextView = FindViewById<TextView> (Resource.Id.LeftCalfChange);
                previousValue = (_lastMeasurement != null) ? _lastMeasurement.LeftCalf : 0;
                newValue = _leftCalfTextBox.Text.AsDouble ();

                changeTotalTextView = FindViewById<TextView>(Resource.Id.LeftCalfChangeTotal);
                previousValueTotal = (_firstMesurement != null) ? _firstMesurement.LeftCalf : 0;
            }
            else if (sender == _rightCalfTextBox)
            {
                changeTextView = FindViewById<TextView> (Resource.Id.RightCalfChange);
                previousValue = (_lastMeasurement != null) ? _lastMeasurement.RightCalf : 0;
                newValue = _rightCalfTextBox.Text.AsDouble ();

                changeTotalTextView = FindViewById<TextView>(Resource.Id.RightCalfChangeTotal);
                previousValueTotal = (_firstMesurement != null) ? _firstMesurement.RightCalf : 0;
            }

			if (_currentMeasurementValue == newValue)
				return;

            PopulateChange(changeTextView, previousValue, newValue, false);
            PopulateChange(changeTotalTextView, previousValueTotal, newValue, true);
        }

        private void MeasurementDeleted (object sender, EventArgs e)
        {
            ResetPreviousMeasurement();
        }

        private void PopulateChange(TextView view, double previousValue, double newValue, bool isTotal)
        {
            if (newValue <= 0)
                return;

            double result = newValue - previousValue;
            view.Text = (previousValue == 0) ? "0" : string.Format ("{0}", result);

            Color color = Color.White;
            if (result > 0)
                color = Color.Red;
            else if (result < 0)
                color = Color.Green;

            if (previousValue == 0)
                color = Color.White;

            view.SetTextColor (color);

            if (view == _weightChange ||
                view == _weightChangeTotal)
                return;

            if (previousValue == 0)
                return;

            if (!isTotal)
            {
                _totalSizeChange += result;
                _prevMeasChangeTotal.Text = _totalSizeChange.ToString();
            }
            else
            {
                _grandTotalSizeChange += result;
                _totalMeasChange.Text = _grandTotalSizeChange.ToString();
            }
        }
            
        private void PopulateMeasurements()
        {
            _bodyMeasurements = BodyMeasurements.GetAllMeasurements ();

            if (_adapter != null)
            {
                _adapter.MeasurementDeleted -= MeasurementDeleted;
                _adapter = null;
            }

            _adapter = new MeasurementAdapter (this, _bodyMeasurements);
            _adapter.MeasurementDeleted += MeasurementDeleted;
            _measurementListView.Adapter = _adapter;
            _adapter.NotifyDataSetChanged ();
        }

        private void HideKeyboard()
        {
            InputMethodManager manager = (InputMethodManager) GetSystemService(InputMethodService);
            manager.HideSoftInputFromWindow(_rightCalfTextBox.WindowToken, 0);
        }

        private void ResetPreviousMeasurement()
        {
            _lastMeasurement = BodyMeasurements.GetLastMeasurement(_currentDate);
        }

		private void UpdateDateView()
		{
			_dateTextView.Text = _currentDate.ToString ("MM/dd/yyyy");
		}

		private void ClearScreen()
		{
			_weightTextBox.Text = string.Empty;
			_leftArmTextBox.Text = string.Empty;
			_rightArmTextBox.Text = string.Empty;
			_chestTextBox.Text = string.Empty;
			_waistTextBox.Text = string.Empty;
			_absTextBox.Text = string.Empty;
			_hipsTextBox.Text = string.Empty;
			_leftThighTextBox.Text = string.Empty;
			_rightThighTextBox.Text = string.Empty;
			_leftCalfTextBox.Text = string.Empty;
			_rightCalfTextBox.Text = string.Empty;

			_weightChange.Text = "0";
			_weightChange.SetTextColor (Color.White);
			_leftArmChange.Text = "0";
			_leftArmChange.SetTextColor (Color.White);
			_rightArmChange.Text = "0";
			_rightArmChange.SetTextColor (Color.White);
			_chestChange.Text = "0";
			_chestChange.SetTextColor (Color.White);
			_waistChange.Text = "0";
			_waistChange.SetTextColor (Color.White);
			_absChange.Text = "0";
			_absChange.SetTextColor (Color.White);
			_hipsChange.Text = "0";
			_hipsChange.SetTextColor (Color.White);
			_leftThighChange.Text = "0";
			_leftThighChange.SetTextColor (Color.White);
			_rightThighChange.Text = "0";
			_rightThighChange.SetTextColor (Color.White);
			_leftCalfChange.Text = "0";
			_leftCalfChange.SetTextColor (Color.White);
			_rightCalfChange.Text = "0";
			_rightCalfChange.SetTextColor (Color.White);

            _weightChangeTotal.Text = "0";
            _weightChangeTotal.SetTextColor (Color.White);
            _leftArmChangeTotal.Text = "0";
            _leftArmChangeTotal.SetTextColor (Color.White);
            _rightArmChangeTotal.Text = "0";
            _rightArmChangeTotal.SetTextColor (Color.White);
            _chestChangeTotal.Text = "0";
            _chestChangeTotal.SetTextColor (Color.White);
            _waistChangeTotal.Text = "0";
            _waistChangeTotal.SetTextColor (Color.White);
            _absChangeTotal.Text = "0";
            _absChangeTotal.SetTextColor (Color.White);
            _hipsChangeTotal.Text = "0";
            _hipsChangeTotal.SetTextColor (Color.White);
            _leftThighChangeTotal.Text = "0";
            _leftThighChangeTotal.SetTextColor (Color.White);
            _rightThighChangeTotal.Text = "0";
            _rightThighChangeTotal.SetTextColor (Color.White);
            _leftCalfChangeTotal.Text = "0";
            _leftCalfChangeTotal.SetTextColor (Color.White);
            _rightCalfChangeTotal.Text = "0";
            _rightCalfChangeTotal.SetTextColor (Color.White);

            _prevMeasChangeTotal.Text = "0";
            _totalMeasChange.Text = "0";

			_dateTextView.RequestFocus ();
		}
	}
}
