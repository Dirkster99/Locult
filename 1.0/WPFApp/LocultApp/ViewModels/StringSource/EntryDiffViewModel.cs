namespace LocultApp.ViewModels.StringSource
{
  using System;
  using System.ComponentModel;
  using LocultApp.Models;

  /// <summary>
  /// Class manages two entries and tracks their difference state
  /// using the <seealso cref="TypeOfDiff"/> enumeration.
  /// </summary>
  public sealed class EntryDiffViewModel : Base.ViewModelBase, IEditableObject
  {
    #region fields
    private EntryDiffModel mTranslationDiff;

    private int mSourceRowId;
    private int mTargetRowId;
    #endregion fields

    #region constructor
    /// <summary>
    /// Parameterized class constructor
    /// </summary>
    /// <param name="item"></param>
    /// <param name="difference"></param>
    public EntryDiffViewModel(string key, string value, string comment, int rowid,
                              TypeOfDiff difference = TypeOfDiff.SourceOnly)
      : this()
    {
            Parent = null;

            mTranslationDiff = new EntryDiffModel(key, value, comment, difference);
    }

    /// <summary>
    /// Copy class constructor
    /// </summary>
    /// <param name="copyThis"></param>
    public EntryDiffViewModel(EntryDiffViewModel copyThis)
    : this()
    {
      if (copyThis == null)
        return;

            Parent = null;
            mTranslationDiff = new EntryDiffModel(copyThis.mTranslationDiff);
            TargetRowId = copyThis.TargetRowId;
            SourceRowId = copyThis.SourceRowId;
    }

    /// <summary>
    /// Standard class constructor
    /// </summary>
    public EntryDiffViewModel()
    {
            Parent = null;
            mTranslationDiff = null;

            mSourceRowId = -1;
            mTargetRowId = -1;
    }
    #endregion constructor

    #region properties
    internal IEntryDiffViewModelParent Parent { get; set; }

    /// <summary>
    /// Get property to determine whether source data is available.
    /// </summary>
    public bool IsSourceSet
    {
      get
      {
        if (mTranslationDiff == null)
          return false;

        return mTranslationDiff.IsSourceSet;
      }
    }

    /// <summary>
    /// Get property to determine whether target data is available.
    /// </summary>
    public bool IsTargetSet
    {
      get
      {
        if (mTranslationDiff == null)
          return false;

        return mTranslationDiff.IsTargetSet;
      }
    }

    /// <summary>
    /// Get property to determine if there is a difference between source and target
    /// and what the state of that difference is.
    /// </summary>
    public TypeOfDiff Difference
    {
      get
      {
        if (IsSourceSet == true && IsTargetSet == false)
          return TypeOfDiff.SourceOnly;

        if (IsSourceSet == false && IsTargetSet == true)
          return TypeOfDiff.TargetOnly;

        return TypeOfDiff.SourceAndTarget;
      }
    }

    #region key properties
    /// <summary>
    /// Get key from Target or source language (depending on which one is present)
    /// or both - if both are set with the same string key.
    /// </summary>
    public string Key
    {
      get
      {
        if (mTranslationDiff == null)
          return null;

        return mTranslationDiff.Key;
      }

      set
      {
        if (mTranslationDiff == null)
          return;

        if (mTranslationDiff.Key != value)
        {
                    mTranslationDiff.SetKey(value);
                    RaisePropertyChanged(() => Key);
                    RaisePropertyChanged(() => SourceKey);
                    RaisePropertyChanged(() => TargetKey);          
        }
      }
    }

    /// <summary>
    /// Gets the key of the source language entry.
    /// </summary>
    public string SourceKey
    {
      get
      {
        if (mTranslationDiff == null)
          return null;

        if (mTranslationDiff.SourceEntry == null)
          return null;

        return mTranslationDiff.SourceEntry.KeyString;
      }
    }

    /// <summary>
    /// Gets the key of the target language entry.
    /// </summary>
    public string TargetKey
    {
      get
      {
        if (mTranslationDiff == null)
          return null;

        if (mTranslationDiff.TargetEntry == null)
          return null;

        return mTranslationDiff.TargetEntry.KeyString;
      }
    }
    #endregion key properties

    #region Source properties
    /// <summary>
    /// Get/set source string of translation in 2 languages.
    /// </summary>
    public string SourceValue
    {
      get
      {
        if (mTranslationDiff == null)
          return null;

        if (mTranslationDiff.SourceEntry != null)
          return mTranslationDiff.SourceEntry.ValueString;

        return null;
      }
      
      set
      {
        if (mTranslationDiff == null)
          return;

                mTranslationDiff.SetSourceValueComment(value);

                RaisePropertyChanged(() => SourceValue);
                RaisePropertyChanged(() => Difference);
      }
    }

    /// <summary>
    /// Get/set comment for source language in translation of 2 languages.
    /// </summary>
    public string SourceComment
    {
      get
      {
        if (mTranslationDiff == null)
          return null;

        if (mTranslationDiff.SourceEntry != null)
          return mTranslationDiff.SourceEntry.CommentString;

        return null;
      }

      ////set
      ////{
      ////  if (this.mSourceComment != value)
      ////  {
      ////    this.mSourceComment = value;
      ////    this.RaisePropertyChanged(() => this.SourceComment);
      ////  }
      ////}
    }

    /// <summary>
    /// Gets the RowId of the source language entry.
    /// </summary>
    public int SourceRowId
    {
      get
      {
        return mSourceRowId;
      }

      set
      {
        if (mSourceRowId != value)
        {
                    mSourceRowId = value;
                    RaisePropertyChanged(() => SourceRowId);
        }
      }
    }
    #endregion Source properties

    #region Target properties
    /// <summary>
    /// Get/set target string of translation in 2 languages.
    /// </summary>
    public string TargetValue
    {
      get
      {
        if (mTranslationDiff == null)
          return null;

        if (mTranslationDiff.TargetEntry != null)
          return mTranslationDiff.TargetEntry.ValueString;

        return null;
      }

      set
      {
        if (mTranslationDiff == null)
          return;

                mTranslationDiff.SetTargetValueComment(value);

                RaisePropertyChanged(() => TargetValue);
                RaisePropertyChanged(() => Difference);
      }
    }

    /// <summary>
    /// Get/set comment for target language in translation of 2 languages.
    /// </summary>
    public string TargetComment
    {
      get
      {
        if (mTranslationDiff == null)
          return null;

        if (mTranslationDiff.TargetEntry != null)
          return mTranslationDiff.TargetEntry.CommentString;

        return null;
      }

      ////set
      ////{
      ////  if (this.mTargetComment != value)
      ////  {
      ////    this.mTargetComment = value;
      ////    this.RaisePropertyChanged(() => this.TargetComment);
      ////  }
      ////}
    }

    /// <summary>
    /// Gets the RowId of the target language entry.
    /// </summary>
    public int TargetRowId
    {
      get
      {
        return mTargetRowId;
      }

      set
      {
        if (mTargetRowId != value)
        {
                    mTargetRowId = value;
                    RaisePropertyChanged(() => TargetRowId);
        }
      }
    }
    #endregion Target properties
    #endregion properties

    #region methods
    #region IEditableObject
    private EntryDiffViewModel backupCopy;
    private bool mEdit;

    /// <summary>
    /// Method is executed when user starts to edit a cell in the gridview.
    /// </summary>
    void IEditableObject.BeginEdit()
    {
      if (mEdit == true)
        return;

            mEdit = true;
      backupCopy = MemberwiseClone() as EntryDiffViewModel;
    }

    void IEditableObject.CancelEdit()
    {
      if (mEdit == false)
        return;

            mEdit = false;

      // Rollback edits
      // this.Name = backupCopy.Name;
      // this.Id = backupCopy.Id;
      // this.StartDate = backupCopy.StartDate;
      // this.EndDate = backupCopy.EndDate;
    }

    /// <summary>
    /// Method is executed when user ends editing a cell in the gridview.
    /// (User has pressed AltGR+Enter to confirm change)
    /// </summary>
    void IEditableObject.EndEdit()
    {
      if (mEdit == false)
        return;

            mEdit = false;
      backupCopy = null;

      if (Parent != null)
                Parent.UpdateDiffEntry(this);  // Tell parent that this entry has been updated
    }
    #endregion IEditableObject

    /// <summary>
    /// Assign a value and optional comment to the target language.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="comment"></param>
    internal void SetTargetValueComment(string value, string comment = null)
    {
      if (mTranslationDiff == null)
        throw new NotSupportedException(Local.Strings.STR_ERROR_SOURCE_COMMENT_WITHOUT_TRANSLATION);

            mTranslationDiff.SetTargetValueComment(value, comment);

            RaisePropertyChanged(() => TargetValue);
            RaisePropertyChanged(() => TargetComment);
            RaisePropertyChanged(() => Difference);
    }

    /// <summary>
    /// Assign a value and optional comment to the source language.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="comment"></param>
    internal void SetSourceValueComment(string value, string comment = null)
    {
      if (mTranslationDiff == null)
        throw new NotSupportedException(LocultApp.Local.Strings.STR_ERROR_TARGET_COMMENT_WITHOUT_TRANSLATION);

            mTranslationDiff.SetSourceValueComment(value, comment);

            RaisePropertyChanged(() => SourceValue);
            RaisePropertyChanged(() => SourceComment);
            RaisePropertyChanged(() => Difference);
    }
    #endregion methods
  }
}
