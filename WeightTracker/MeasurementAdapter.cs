using System;
using Android.Widget;
using System.Collections.Generic;
using Android.Views;
using Android.Content;
using Android.App;
using Android.Preferences;

namespace WeightTracker
{
	public class MeasurementAdapter : BaseAdapter<BodyMeasurements>
	{
		private ListView _parent = null;
		private LayoutInflater layoutInflater;
		private List<BodyMeasurements> _measurements = null;
        private Context _context = null;

        private bool UseDropBox
        {
            get
            {
                if (_context == null)
                    return false;

                var prefs = PreferenceManager.GetDefaultSharedPreferences(_context);
                return prefs.GetBoolean("useDropbox", false);
            }
        }

        public event EventHandler MeasurementDeleted;
        private void OnMeasurementDeleted()
        {
            if (MeasurementDeleted != null)
                MeasurementDeleted(this, EventArgs.Empty);
        }

		public MeasurementAdapter (Context context, List<BodyMeasurements> measurements)
		{
            _context = context;
			_measurements = measurements;
			layoutInflater = LayoutInflater.From (context);
		}

		#region implemented abstract members of BaseAdapter

		public override long GetItemId (int position)
		{
            return _measurements[position].ID;
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			ViewHolder holder = null;
			if (convertView == null)
			{
				convertView = layoutInflater.Inflate (Resource.Layout.MeasurementRow, null);
				holder = new ViewHolder ();
				holder.DateTextView = convertView.FindViewById<TextView> (Resource.Id.RowDate);
				holder.DeleteImageButton = convertView.FindViewById<ImageButton> (Resource.Id.RowDeleteButton);
                holder.DeleteImageButton.Focusable = false;
                holder.DeleteImageButton.FocusableInTouchMode = false;
                holder.DeleteImageButton.Clickable = true;

				holder.TotalChangeTextView = convertView.FindViewById<TextView> (Resource.Id.RowTotalChange);
				holder.WeightChangeTextView = convertView.FindViewById<TextView> (Resource.Id.RowWeightChange);

				holder.DeleteImageButton.Click += DeleteRow;

				convertView.Tag = holder;
			}
			else
			{
				holder = (ViewHolder)convertView.Tag;
			}

			var mea = _measurements [position];
			holder.DateTextView.Text = mea.Date.ToString ("MM/dd/yyyy");
            holder.TotalChangeTextView.Text = mea.TotalSizeChange.ToString();
            holder.WeightChangeTextView.Text = mea.TotalWeightChange.ToString();

			_parent = (ListView)parent;

			return convertView;
		}

		private void DeleteRow (object sender, EventArgs e)
		{
            var builder = new AlertDialog.Builder(_context);
            builder.SetTitle("Delete Measurement")
                .SetMessage("Are you sure you want to delete this measurement?")
                .SetPositiveButton("Yes", (dialog, es) =>
                {
                    int position = _parent.GetPositionForView ((View)sender);
                    var measurement = _measurements[position];
                    _measurements.RemoveAt (position);
                    measurement.Delete();

                    if (this.UseDropBox)
                    {
                        var sync = DropboxSync.GetInstance(_context);
                        sync.UpdateDropbox();
                    }

                    this.NotifyDataSetChanged ();
                })
                .SetNegativeButton("No", (dialog, es) => {});

            var alert = builder.Create();
            alert.Show();
		}

		public override int Count {
			get {
				return _measurements.Count;
			}
		}

		public override BodyMeasurements this [int index] {
			get {
				return _measurements [index];
			}
		}

		#endregion


		private class ViewHolder : Java.Lang.Object
		{
			public TextView DateTextView { get; set; }
			public TextView TotalChangeTextView { get; set; }
			public TextView WeightChangeTextView { get; set; }
			public ImageButton DeleteImageButton { get; set; }
		}
	}
}

