namespace Settings.Interfaces
{
    using Settings.UserProfile;
    using Settings.UserProfile.Persistable;

    /// <summary>
    /// Defines an interface for objects that can manage user session data.
    /// </summary>
    public interface IProfile
    {
        #region properties
        /// <summary>
        /// Gets position and size of MainWindow
        /// </summary>
        ViewPosSizeModel MainWindowPosSz { get; }

        /// <summary>
        /// Remember the last active solution file name and path of last session.
        /// 
        /// This can be useful when selecting active document in next session or
        /// determining a useful default path when there is no document currently open.
        /// </summary>
        string LastActiveSolution { get; }

        /// <summary>
        /// Remember the last active path and name of last active document.
        /// 
        /// This can be useful when selecting active document in next session or
        /// determining a useful default path when there is no document currently open.
        /// </summary>
        string LastActiveSourceFile { get; }

        /// <summary>
        /// Remember the last active path and name of last active document.
        /// 
        /// This can be useful when selecting active document in next session or
        /// determining a useful default path when there is no document currently open.
        /// </summary>
        string LastActiveTargetFile { get; }

        /// <summary>
        /// Gets the model of the list entries for the most recently used files list.
        /// </summary>
        MRUModelLib.Interfaces.IMRUList MRU { get; }
        #endregion properties

        #region methods
        string GetLastActivePath();

        void CheckSettingsOnLoad(double SystemParameters_VirtualScreenLeft, double SystemParameters_VirtualScreenTop);

        /// <summary>
        /// Get an object that represents the current states and
        /// can be persisted (to standard XML serialization).
        /// </summary>
        /// <returns></returns>
        ProfilePersistable GetObjectForPersistence();

        /// <summary>
        /// Set current object states from an object that represents
        /// the persisted values (from standard XML serialization).
        /// </summary>
        /// <returns></returns>
        void SetObjectFromPersistence(ProfilePersistable persist);

        /// <summary>
        /// Resets the filename and path of the Last Active Solution.
        /// </summary>
        /// <param name="fileLocation"></param>
        void SetLastActiveSolution(string fileLocation);

        /// <summary>
        /// Resets the main window position with the new values.
        /// </summary>
        /// <param name="viewPosSizeModel"></param>
        void SetMainWindowPosSz(ViewPosSizeModel viewPosSizeModel);

        /// <summary>
        /// Resets the current MRU model items.
        /// </summary>
        /// <param name="mru"></param>
        void ResetMRUModel(MRUModelLib.Interfaces.IMRUList mru);
        #endregion methods
    }
}
