namespace Settings.UserProfile.Persistable
{
    using MRUModelLib.Models.Persistables;
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Implements save/load of user session data via XML Serialization.
    /// </summary>
    [XmlRoot(ElementName = "Profile")]
    public class ProfilePersistable
    {
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region constructors
        /// <summary>
        /// Class constructor.
        /// </summary>
        public ProfilePersistable()
        {
            // Default session Data
            MainWindowPosSz = new ViewPosSizeModel(100, 100, 1000, 700);

            LastActiveSolution = LastActiveSourceFile = LastActiveTargetFile = string.Empty;
        }
        #endregion constructors

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

        [XmlElement(ElementName = "MRU")]
        public MRUListPersistable MRU { get; set; }
        #endregion properties

        #region methods
        /// <summary>
        /// Gets the persistable object from XML persistence.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static ProfilePersistable GetObjectFromPersistence(string fileName)
        {
            ProfilePersistable profileDataModel = null;

            try
            {
                if (System.IO.File.Exists(fileName))
                {
                    FileStream readFileStream = null;
                    try
                    {
                        // Create a new file stream for reading the XML file
                        readFileStream = new System.IO.FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);

                        // Create a new XmlSerializer instance with the type of the test class
                        XmlSerializer serializerObj = new XmlSerializer(typeof(ProfilePersistable));

                        // Load the object saved above by using the Deserialize function
                        profileDataModel = (ProfilePersistable)serializerObj.Deserialize(readFileStream);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                    }
                    finally
                    {
                        // Cleanup
                        if (readFileStream != null)
                            readFileStream.Close();
                    }
                }

                return profileDataModel;
            }
            catch (Exception e)
            {
                logger.Error(e);
            }

            return null;
        }

        /// <summary>
        /// Saves the persistable object to XML persistence.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="persistable"></param>
        /// <returns></returns>
        public static bool SaveObjectToPersistence(string fileName,
                                                   ProfilePersistable persistable)
        {
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.NewLineOnAttributes = true;
            xws.Indent = true;
            xws.IndentChars = "  ";
            xws.Encoding = System.Text.Encoding.UTF8;

            // Create a new file stream to write the serialized object to a file
            XmlWriter xw = null;
            try
            {
                xw = XmlWriter.Create(fileName, xws);

                // Create a new XmlSerializer instance with the type of the test class
                XmlSerializer serializerObj = new XmlSerializer(persistable.GetType());
                serializerObj.Serialize(xw, persistable);

                return true;
            }
            catch (Exception e)
            {
                logger.Error(e);
            }
            finally
            {
                if (xw != null)
                    xw.Close(); // Cleanup

            }

            return false;
        }
        #endregion methods
    }
}
