namespace Settings.UserProfile
{
    using Settings.UserProfile.Persistable;

    /// <summary>
    /// This class implements the model of the user profile part
    /// of the application. Typically, users have implicit run-time
    /// settings that should be re-activated when the application
    /// is re-started at a later point in time (e.g. window size
    /// and position).
    /// 
    /// This class organizes these per user specific profile settings
    /// and is responsible for their storage (at program end) and
    /// retrieval at the start-up of the application.
    /// </summary>
    internal class Profile : Settings.Interfaces.IProfile
    {
        #region fields
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion fields

        #region constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        public Profile()
        {
            // Session Data
            MainWindowPosSz = new ViewPosSizeModel(100, 100, 1000, 700);

            LastActiveSolution = LastActiveSourceFile = LastActiveTargetFile = string.Empty;

            MRU = MRUModelLib.Factory.CreateMRUList();
        }
        #endregion constructor

        #region properties
        /// <summary>
        /// Gets position and size of MainWindow
        /// </summary>
        public ViewPosSizeModel MainWindowPosSz { get; protected set; }

        /// <summary>
        /// Remember the last active solution file name and path of last session.
        /// 
        /// This can be useful when selecting active document in next session or
        /// determining a useful default path when there is no document currently open.
        /// </summary>
        public string LastActiveSolution { get; protected set; }

        /// <summary>
        /// Remember the last active path and name of last active document.
        /// 
        /// This can be useful when selecting active document in next session or
        /// determining a useful default path when there is no document currently open.
        /// </summary>
        public string LastActiveSourceFile { get; protected set; }

        /// <summary>
        /// Remember the last active path and name of last active document.
        /// 
        /// This can be useful when selecting active document in next session or
        /// determining a useful default path when there is no document currently open.
        /// </summary>
        public string LastActiveTargetFile { get; protected set; }

        /// <summary>
        /// Gets the model of the list entries for the most recently used files list.
        /// </summary>
        public MRUModelLib.Interfaces.IMRUList MRU { get; protected set; }
        #endregion properties

        #region methods
        /// <summary>
        /// Method ensures that window is visible when
        /// resolution changes between user sessions.
        /// </summary>
        /// <param name="SystemParameters_VirtualScreenLeft"></param>
        /// <param name="SystemParameters_VirtualScreenTop"></param>
        public void CheckSettingsOnLoad(double SystemParameters_VirtualScreenLeft,
                                        double SystemParameters_VirtualScreenTop)
        {
            if (MainWindowPosSz == null)
                MainWindowPosSz = new ViewPosSizeModel(100, 100, 600, 500);

            if (MainWindowPosSz.DefaultConstruct == true)
                MainWindowPosSz = new ViewPosSizeModel(100, 100, 600, 500);

            MainWindowPosSz.SetValidPos(SystemParameters_VirtualScreenLeft,
                                       SystemParameters_VirtualScreenTop);
        }

        /// <summary>
        /// Get the path of the file or empty string if file does not exists on disk.
        /// </summary>
        /// <returns></returns>
        public string GetLastActivePath()
        {
            try
            {
                if (System.IO.File.Exists(LastActiveSolution))
                    return System.IO.Path.GetDirectoryName(LastActiveSolution);
            }
            catch
            {
            }

            return string.Empty;
        }

        /// <summary>
        /// Resets the filename and path of the Last Active Solution and adds it into the MRU.
        /// </summary>
        /// <param name="fileLocation"></param>
        public void SetLastActiveSolution(string fileLocation)
        {
            LastActiveSolution = fileLocation;
            MRU.Add(fileLocation);
        }

        /// <summary>
        /// Resets the main window position with the new values.
        /// </summary>
        /// <param name="viewPosSizeModel"></param>
        public void SetMainWindowPosSz(ViewPosSizeModel viewPosSizeModel)
        {
            if (viewPosSizeModel == null)
                viewPosSizeModel = new ViewPosSizeModel();

            this.MainWindowPosSz = viewPosSizeModel;
        }

        /// <summary>
        /// Get an object that represents the current states and
        /// can be persisted (to standard XML serialization).
        /// </summary>
        /// <returns></returns>
        public ProfilePersistable GetObjectForPersistence()
        {
            var ret = new ProfilePersistable();

            ret.MRU = this.MRU.GetObjectForPersistence();
            ret.MainWindowPosSz = this.MainWindowPosSz;
            ret.LastActiveSolution = this.LastActiveSolution;
            ret.LastActiveSourceFile = this.LastActiveSolution;
            ret.LastActiveSourceFile = this.LastActiveSolution;
            ret.LastActiveTargetFile = this.LastActiveTargetFile;

            return ret;
        }

        /// <summary>
        /// Set current object states from an object that represents
        /// the persisted values (from standard XML serialization).
        /// </summary>
        /// <returns></returns>
        public void SetObjectFromPersistence(ProfilePersistable persist)
        {
            if (persist == null)
                return;

            this.MRU.SetObjectFromPersistence(persist.MRU);
            this.MainWindowPosSz = persist.MainWindowPosSz;
            this.LastActiveSolution = persist.LastActiveSolution;
            this.LastActiveSolution = persist.LastActiveSolution;
            this.LastActiveSolution = persist.LastActiveSolution;
            this.LastActiveTargetFile = persist.LastActiveTargetFile;
        }

        /// <summary>
        /// Resets the current MRU model items.
        /// </summary>
        /// <param name="mru"></param>
        public void ResetMRUModel(MRUModelLib.Interfaces.IMRUList mru)
        {
            MRU.Clear();

            if (mru == null)
                return;

            foreach (var item in mru.Entries)
                MRU.Add(item);
        }
        #endregion methods
    }
}
