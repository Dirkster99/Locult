namespace LocultApp.Models
{
  /// <summary>
  /// This enum is used to track the states of
  /// differences between a source and target language string.
  /// </summary>
  public enum TypeOfDiff
  {
    /// <summary>
    /// Is set by default in default class constructor and remains if corresponding class
    /// is not initialized or processed correctly.
    /// </summary>
    Unknown = -1,

    /// <summary>
    /// Is set when only source value is available but no target values.
    /// </summary>
    SourceOnly = 0,

    /// <summary>
    /// Is set when only target value is available but no source value.
    /// </summary>
    TargetOnly = 1,

    /// <summary>
    /// Is set when source and target have a value but its not checked for translation errors.
    /// </summary>
    SourceAndTarget = 2
    // , Update is not supported yet since it require a backup of the checked translation string
  }
}
