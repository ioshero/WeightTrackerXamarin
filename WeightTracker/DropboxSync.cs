using System;
using System.Threading.Tasks;
using DropboxSync.Android;
using Android.Content;
using Android.App;

namespace WeightTracker
{
    public class DropboxSync
    {
        private const string DROPBOX_API_KEY = "z35j57g4o6bmt5w";
        private const string DROPBOX_API_SECRET = "jeb84jqy7bgesfw";

        private DBAccountManager _dbAccountManager = null;
        private DBDatastore _dbDataStore = null;
        private DBAccount _dbAccount = null;

        private Context _context;
        private static DropboxSync _instance = null;
        private static readonly object padLock = new object();

        private bool _updateAfterReAuth = false;

        private DropboxSync(Context context)
        {
        }

        public static DropboxSync GetInstance(Context context)
        {
            if (_instance == null)
            {
                lock(padLock)
                {
                    _instance = new DropboxSync(context);
                }
            }

            if (_instance._context != context)
                _instance.SetContext(context);

            return _instance;
        }

        public void SetContext(Context context)
        {
            _context = context;
        }

        public void LinkDropBoxAccount()
        {
            // Setup Dropbox.
            _dbAccountManager = DBAccountManager.GetInstance (_context.ApplicationContext, DROPBOX_API_KEY, DROPBOX_API_SECRET);
            _dbAccountManager.LinkedAccountChanged += (sender, e) => 
            {
                if (e.P1.IsLinked)
                    Log("Account.LinkedAccountChanged", "Now linked to {0}", e.P1 != null ? e.P1.AccountInfo != null ? e.P1.AccountInfo.DisplayName : "nobody" : "null");
                else 
                {
                    Log("Account.LinkedAccountChanged", "Now unlinked from {0}", e.P1 != null ? e.P1.AccountInfo != null ? e.P1.AccountInfo.DisplayName : "nobody" : "null");
                    return;
                }

                _dbAccountManager = e.P0;
                _dbAccount = e.P1;
                StartApp (_dbAccount);
            };

            if (!_dbAccountManager.HasLinkedAccount) {
                StartDropBoxLink();
            }
            else {
                StartApp ();
            }
        }

        private void InitializeDropbox (DBAccount account)
        {
            if (_dbDataStore == null || !_dbDataStore.IsOpen || _dbDataStore.Manager.IsShutDown) 
            {
                _dbDataStore = DBDatastore.OpenDefault (account ?? _dbAccountManager.LinkedAccount);
                _dbAccount = account ?? _dbAccountManager.LinkedAccount;
                _dbDataStore.DatastoreChanged += HandleStoreChange;
            }
        }

        private void HandleStoreChange (object sender, DBDatastore.SyncStatusEventArgs e)
        {
            if (e.P0.SyncStatus.HasIncoming)
            {
                if (!_dbAccountManager.HasLinkedAccount) 
                    _dbDataStore.DatastoreChanged -= HandleStoreChange;

                Console.WriteLine ("Datastore needs to be re-synced.");
                _dbDataStore.Sync ();
            }
        }

        private void StartDropBoxLink()
        {
            _dbAccountManager.StartLink ((Activity)_context, Utilities.LINK_TO_DROPBOX_REQUEST);
        }

        public void UnlinkAccountManager()
        {
            Task.Run(() => _dbAccountManager.Unlink());
        }

        public void StartApp (DBAccount account = null)
        {
            InitializeDropbox (account);
            Log("StartApp", "Syncing measurements...");

            if (_updateAfterReAuth)
            {
                UpdateDropbox();
                _updateAfterReAuth = false;
            }

            _dbDataStore.Sync();
        }

        public void UpdateDropbox ()
        {
            Log("Updating Dropbox");

            bool needToReauth = false;
            if (_dbAccount == null)
                LinkDropBoxAccount();

            needToReauth = !_dbAccountManager.HasLinkedAccount;

            if (!needToReauth)
            {
                var fileSystem = DBFileSystem.ForAccount(_dbAccount);
                var newPath = DBPath.Root.GetChild("measurements.xml");
                DBFile file = null;

                if (fileSystem.Exists(newPath))
                    file = fileSystem.Open(newPath);
                else
                    file = fileSystem.Create(newPath);

                var measurements = BodyMeasurements.GetAllMeasurements();
                Utilities.ExportMeasurements(measurements);

                file.WriteFromExistingFile(new Java.IO.File(Utilities.ExportedDatabasePath), false);
                file.Close();
                file.Dispose();

                if (!VerifyStore()) {
                    RestartAuthFlow (); 
                } else {
                    _dbDataStore.Sync();
                }
            }
            else
            {
                _updateAfterReAuth = true;
            }
        }

        private bool VerifyStore ()
        {
            if (!_dbDataStore.IsOpen) {
                Log ("VerifyStore", "Datastore is NOT open.");
                return false;
            }
            if (_dbDataStore.Manager.IsShutDown) {
                Log ("VerifyStore", "Manager is shutdown.");
                return false;
            }
            if (!_dbAccountManager.HasLinkedAccount) {
                Log ("VerifyStore", "Account was unlinked while we weren't watching.");
                return false;
            }
            return true;
        }

        private void RestartAuthFlow ()
        {
            if (_dbAccountManager.HasLinkedAccount)
                _dbAccountManager.Unlink();
            else
                StartDropBoxLink();
        }


        private void Log (string location) {
            Log(location, String.Empty);           
        }

        private void Log (string location, string format, params object[] objects)
        {
//            var tag = String.Format("{0} {1}.{2}", GetType ().Name, (_lastMeasurement != null) ? _lastMeasurement.Date.ToString("MM/dd/yyyy") : string.Empty, location);
//            global::Android.Util.Log.Debug (tag, format, objects);
        }

    }
}

