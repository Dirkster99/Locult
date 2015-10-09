namespace LocultApp.Models
{
  using System;

  /// <summary>
  /// Class implements the model that compares target and source
  /// language strings with each other and determines their difference:
  /// 
  /// 1> Only in source
  /// 2> Only in target
  /// 3> In source and target but not approved by translator.
  /// </summary>
  internal class EntryDiffModel
  {
    #region constructor
    /// <summary>
    /// (Hidden) Class constructor
    /// </summary>
    protected EntryDiffModel()
    {
            SourceEntry = null;
            TargetEntry = null;
            Diff = TypeOfDiff.Unknown;
    }

    /// <summary>
    /// Parameterized class constructor.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="comment"></param>
    /// <param name="difference"></param>
    public EntryDiffModel(string key,
                          string value,
                          string comment,
                          TypeOfDiff difference = TypeOfDiff.SourceOnly)
      : this()
    {
      switch (difference)
      {
        case TypeOfDiff.SourceOnly:
                    SourceEntry = new Entry(key, value, comment);
          break;

        case TypeOfDiff.TargetOnly:
                    TargetEntry = new Entry(key, value, comment);
          break;

        default:
          throw new System.NotImplementedException(difference.ToString());
      }
    }

    /// <summary>
    /// Copy class constructor
    /// </summary>
    public EntryDiffModel(EntryDiffModel copy)
    : this()
    {
      if (copy == null)
        return;

      if (copy.SourceEntry != null)
                SourceEntry = new Entry(copy.SourceEntry);

      if (copy.TargetEntry != null)
                TargetEntry = new Entry(copy.TargetEntry);

            Diff = copy.Diff;
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Gets source of translation.
    /// </summary>
    public Entry SourceEntry { get; private set; }

    /// <summary>
    /// Gets target of translation.
    /// </summary>
    public Entry TargetEntry { get; private set; }

    /// <summary>
    /// Gets current type of difference in translation.
    /// 
    /// 1> Only in source
    /// 2> Only in target
    /// 3> In source and target but not approved by translator.
    /// </summary>
    public TypeOfDiff Diff { get; private set; }

    /// <summary>
    /// Gets whether source data is available.
    /// </summary>
    public bool IsSourceSet
    {
      get
      {
        if (SourceEntry == null)
          return false;

        if (string.IsNullOrEmpty(SourceEntry.ValueString) ||
            string.IsNullOrWhiteSpace(SourceEntry.ValueString))
          return false;

        return true;
      }
    }

    /// <summary>
    /// Gets whether target data is available.
    /// </summary>
    public bool IsTargetSet
    {
      get
      {
        if (TargetEntry == null)
          return false;

        if (string.IsNullOrEmpty(TargetEntry.ValueString) ||
            string.IsNullOrWhiteSpace(TargetEntry.ValueString))
          return false;

        return true;
      }
    }

    /// <summary>
    /// Get key from Target or source language (depending on which one is present)
    /// or both - if both are set with the same string key.
    /// </summary>
    public string Key
    {
      get
      {
        if (IsSourceSet == true && IsTargetSet == true)
        {
          // Make sure key in source and target is always equal
          if (SourceEntry.KeyString != TargetEntry.KeyString)
            throw new NotSupportedException(string.Format("A diff entry model requires matching keys.\nEntry 1: {0}\nEntry 2: {1}",
                                                           SourceEntry, TargetEntry));

          return SourceEntry.KeyString;
        }
        else
        {
          if (IsSourceSet == true)
            return SourceEntry.KeyString;
          else
          {
            if (IsTargetSet == true)
              return TargetEntry.KeyString;
          }
        }

        return null;
      }
    }
    #endregion properties

    #region methods
    /// <summary>
    /// Reset target value and comment of translation (re-using existing key
    /// if available in source or target or both).
    /// </summary>
    /// <param name="value"></param>
    /// <param name="comment"></param>
    internal void SetTargetValueComment(string value, string comment = null)
    {
      if (IsTargetSet == false && IsSourceSet == false)
        throw new NotSupportedException("Setting Source Value Comment requires Target or Source to be present.");

      if (IsSourceSet == true)
                TargetEntry = new Entry(SourceEntry.KeyString, value, comment);
      else
                TargetEntry = new Entry(TargetEntry.KeyString, value, comment);
    }

    /// <summary>
    /// Reset source value and comment of translation (re-using existing key
    /// if available in source or target or both).
    /// </summary>
    /// <param name="value"></param>
    /// <param name="comment"></param>
    internal void SetSourceValueComment(string value, string comment = null)
    {
      if (IsTargetSet == false && IsSourceSet == false)
        throw new NotSupportedException("Setting Source Value Comment requires Target or Source to be present.");

      if (IsTargetSet == true)
                SourceEntry = new Entry(TargetEntry.KeyString, value, comment);
      else
                SourceEntry = new Entry(SourceEntry.KeyString, value, comment);
    }

    /// <summary>
    /// Assigns a new key to this entry -
    /// creates a new source entry if the assignment is completely new.
    /// </summary>
    /// <param name="key"></param>
    internal void SetKey(string key)
    {
      if (IsTargetSet == false && IsSourceSet == false)
      {
                SourceEntry = new Entry(TargetEntry.KeyString, string.Empty, string.Empty);
                Diff = TypeOfDiff.SourceOnly;
      }
      else
      {
        if (IsTargetSet == true)
                    TargetEntry.SetKey(key);

        if (IsSourceSet == true)
                    SourceEntry.SetKey(key);
      }
    }
    #endregion methods
  }
}
