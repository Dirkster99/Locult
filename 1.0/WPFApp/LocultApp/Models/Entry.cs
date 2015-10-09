namespace LocultApp.Models
{
  using System;

  /// <summary>
  /// Model an resource string entry of a file
  /// such that it can be consumed by a collection
  /// (to model all entries in a string resource file).
  /// </summary>
  [Serializable]
  public class Entry
  {
    #region constructor
    /// <summary>
    /// Empty constructor
    /// </summary>
    public Entry()
    {
            KeyString = string.Empty;
            ValueString = string.Empty;
            CommentString = string.Empty;
    }

    /// <summary>
    /// Parameterized class constructor
    /// </summary>
    /// <param name="keyString"></param>
    /// <param name="valueString"></param>
    /// <param name="commentString"></param>
    public Entry(string keyString,
                 string valueString,
                 string commentString = null)
      : this()
    {
            KeyString = keyString;
            ValueString = valueString;
            CommentString = commentString;
    }

    /// <summary>
    /// Copy Constructor of an entry
    /// </summary>
    /// <param name="item">Entry to copy data from</param>
    public Entry(Entry item)
    : this()
    {
      if (item == null)
        return;

            KeyString = item.KeyString;
            ValueString = item.ValueString;
            CommentString = item.CommentString;
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Get/set Path of the file containing this log item
    /// </summary>
    public string KeyString { get; private set; }

    /// <summary>
    /// Get/set Time delta for this entry in comparison to the previous entry
    /// </summary>
    public string ValueString { get; private set; }

    /// <summary>
    /// Get/set logger name of log file
    /// </summary>
    public string CommentString { get; private set; }
    #endregion properties

    #region methods
    /// <summary>
    /// Determine if this object equals another object of the same type
    /// (another object instanciated from ame class).
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
      var e = obj as Entry;

      if (e == null)
        return false;

      return e.KeyString == KeyString
          && e.ValueString == ValueString
          && e.CommentString == CommentString;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    internal void SetKey(string key)
    {
            KeyString = key;
    }
    #endregion methods
  }
}
