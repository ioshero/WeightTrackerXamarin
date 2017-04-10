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

namespace WeightTracker
{
	[Activity (Label = "UsersActivity")]			
	public class UsersActivity : Activity
	{
		private const int USER_NAME_EDIT_TEXT = 7000;

		private ListView _usersListView = null;
		private ImageButton _addNewButton = null;

		private ArrayAdapter<string> _adapter = null;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.ViewUsers);

			var currentUser = Settings.LoadSettings ().GetCurrentUser ();
			this.ActionBar.Title = "Measurements Tracker";
			this.ActionBar.Subtitle = string.Format ("Current User: {0}", currentUser.UserName);

			_addNewButton = FindViewById<ImageButton> (Resource.Id.AddNewButton);
			_addNewButton.Click += AddNewUser;

			var userNames = User.GetUsers ().Select (t => t.UserName).ToList();
			_usersListView = FindViewById<ListView> (Resource.Id.UsersListView);
			_usersListView.ItemClick += UsersItemClicked;
			_adapter = new ArrayAdapter<string> (this, Android.Resource.Layout.SimpleListItem1, userNames);
			_usersListView.Adapter = _adapter;
			_adapter.NotifyDataSetChanged ();
		}

		private void UsersItemClicked (object sender, AdapterView.ItemClickEventArgs e)
		{
			var users = User.GetUsers ();
			var user = users [e.Position];

			CreateUserNameAlert (user);
		}

		private void AddNewUser (object sender, EventArgs e)
		{
			CreateUserNameAlert (new User ());
		}

		private void CreateUserNameAlert(User user)
		{
			var layout = CreateUserNameAlertLayout (user.UserName);
			var editText = layout.FindViewById<EditText> (USER_NAME_EDIT_TEXT);

			var builder = new AlertDialog.Builder (this);

			builder.SetView (layout)
				.SetTitle ("Edit User Name")
				.SetPositiveButton ("OK", (dialog, id) =>
				{
					user.UserName = editText.Text;
					user.Save();
					var users = User.GetUsers().Select(t => t.UserName).ToList();
					_adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, users);
					_usersListView.Adapter = _adapter;
					_adapter.NotifyDataSetChanged();

					if (user.ID == Settings.LoadSettings().CurrentUserID)
					{
						this.ActionBar.Subtitle = string.Format ("Current User: {0}", user.UserName);
					}
				})
				.SetNegativeButton ("Cancel", (dialog, id) => { });

			var alert = builder.Create ();
			alert.Show ();
		}

		private View CreateUserNameAlertLayout(string userName)
		{
			LinearLayout layout = new LinearLayout (this);
			layout.Orientation = Orientation.Horizontal;
			TextView textView = new TextView (this);
			textView.Text = "Enter Name:";
			textView.SetWidth (150);

			LinearLayout.LayoutParams margin = new LinearLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent,
				RelativeLayout.LayoutParams.WrapContent);
			margin.LeftMargin = 60;
			textView.LayoutParameters = margin;

			EditText editText = new EditText (this);
			editText.Id = USER_NAME_EDIT_TEXT;
			editText.Text = userName;
			editText.SetWidth (300);
			layout.AddView (textView);
			layout.AddView (editText);

			return layout;
		}
	}
}

