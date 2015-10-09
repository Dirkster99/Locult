namespace Settings.UserProfile
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  using System.Xml.Serialization;

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
    public class Profile : Settings.Interfaces.IProfile
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
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Get/set position and size of MainWindow
    /// </summary>
    [XmlElement(ElementName = "MainWindowPos")]
    public ViewPosSizeModel MainWindowPosSz { get; set; }

    /// <summary>
    /// Remember the last active solution file name and path of last session.
    /// 
    /// This can be useful when selecting active document in next session or
    /// determining a useful default path when there is no document currently open.
    /// </summary>
    [XmlAttribute(AttributeName = "LastActiveSolution")]
    public string LastActiveSolution { get; set; }

    /// <summary>
    /// Remember the last active path and name of last active document.
    /// 
    /// This can be useful when selecting active document in next session or
    /// determining a useful default path when there is no document currently open.
    /// </summary>
    [XmlAttribute(AttributeName = "LastActiveSourceFile")]
    public string LastActiveSourceFile { get; set; }

    /// <summary>
    /// Remember the last active path and name of last active document.
    /// 
    /// This can be useful when selecting active document in next session or
    /// determining a useful default path when there is no document currently open.
    /// </summary>
    [XmlAttribute(AttributeName = "LastActiveTargetFile")]
    public string LastActiveTargetFile { get; set; }
    #endregion properties

    #region methods
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
    #endregion methods
  }
}
