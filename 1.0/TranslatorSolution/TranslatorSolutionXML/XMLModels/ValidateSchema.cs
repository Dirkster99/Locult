namespace TranslatorSolutionLib.XMLModels
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Schema;

    /// <summary>
    /// Helper class to validate a XML schema via XSD file
    /// Supports different ways of checking XML via external XSD
    /// or internal XSD (loaded from resource of the project)
    /// </summary>
    internal class ValidateSchema
    {
        #region Fields
        /// <summary>
        /// List of error messages encountered durring XSD based parser execution
        /// </summary>
        private System.Collections.Generic.List<string> mErrorMsg = new List<string>();

        /// <summary>
        /// List of warning messages encountered durring XSD based parser execution
        /// </summary>
        private System.Collections.Generic.List<string> mWarningMsg = new List<string>();
        #endregion Fields

        #region Properties
        /// <summary>
        /// Return errors encountered durring XML/XSD validation
        /// </summary>
        public List<string> Errors
        {
            get { return (mErrorMsg == null ? new List<string>() : mErrorMsg); }
            private set { }
        }

        /// <summary>
        /// Return warnings encountered durring XML/XSD validation
        /// </summary>
        public List<string> Warnings
        {
            get { return (mWarningMsg == null ? new List<string>() : mWarningMsg); }
            private set { }
        }
        #endregion Properties

        #region Public XSD CHECK Methods
        /// <summary>
        /// Check the contents of an XML file against a XSD
        /// stored anywhere in the file system.
        /// </summary>
        /// <param name="sXMLContent">PathFilename of XML file</param>
        /// <param name="settings">Optional settings to determine level of detail to check</param>
        public void CheckXML_XSD(string sXMLInputUri,
                                 XmlReaderSettings settings = null)
        {
            if (settings == null)
            {
                // Set the validation settings.
                settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.Schema;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            }

            settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);

            using (XmlReader reader = XmlReader.Create(sXMLInputUri, settings))
            {
                // Parse the file. 
                while (reader.Read())
                {
                }
            }
        }

        /// <summary>
        /// Check the contents of an XML file against a XSD stored in a Stream
        /// (the Stream may be stored as resource or may be stored somewhere else...)
        /// </summary>
        /// <param name="sXMLPath">PathFilename to XML file</param>
        /// <param name="streamXSD">Stream constructed from XSD (resource) file.</param>
        /// <param name="settings">Optional settings to determine level of detail to check</param>
        public void CheckXML_XSD(string sXMLPath,
                                       Stream streamXSD,
                                       XmlReaderSettings settings = null)
                {
                    StreamReader strmrStreamReader = new StreamReader(streamXSD);
                    System.Xml.Schema.XmlSchema xSchema = new System.Xml.Schema.XmlSchema();
                    xSchema = XmlSchema.Read(strmrStreamReader, null);

                    // Set the validation settings.
                    if (settings == null)
                    {
                        settings = new XmlReaderSettings();
                        settings.Schemas.Add(xSchema);
                        settings.ValidationType = ValidationType.Schema;
                        settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
                        settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
                        settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
                    }

                    settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);

                    using (XmlReader reader = XmlReader.Create(sXMLPath, settings))
                    {
                        // XmlReader reader = XmlReader.Create("SampleXML/DBRestore.xml", settings);
                        // Parse the file. 
                        while (reader.Read())
                        {
                        }
                    }
                }

        public void CheckXMLString_XSD(string sXMLContent,
                                       Stream streamXSD,
                                       XmlReaderSettings settings = null)
        {
            StreamReader strmrStreamReader = new StreamReader(streamXSD);
            System.Xml.Schema.XmlSchema xSchema = new System.Xml.Schema.XmlSchema();
            xSchema = XmlSchema.Read(strmrStreamReader, null);

            // Set the validation settings.
            if (settings == null)
            {
                settings = new XmlReaderSettings();
                settings.Schemas.Add(xSchema);
                settings.ValidationType = ValidationType.Schema;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            }

            settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);

            using (XmlReader reader = XmlReader.Create(new StringReader(sXMLContent), settings))
            {
                // XmlReader reader = XmlReader.Create("SampleXML/DBRestore.xml", settings);
                // Parse the file. 
                while (reader.Read())
                {
                }
            }
        }


        /// <summary>
        /// Check if the XML contains the required Namespace and is XSD conform.
        /// </summary>
        /// <param name="sXMLInputUri"></param>
        /// <param name="sXSD"></param>
        /// <param name="sNamespace"></param>
        /// <param name="settings"></param>
        public void CheckXMLNamespace(string sXMLInputUri,
                                      string sXSD,
                                      string sNamespace,
                                      XmlReaderSettings settings = null)
        {
            XmlSchemaSet sc = new XmlSchemaSet();
            sc.Add(sNamespace, sXSD);

            // Set the validation settings.
            if (settings == null)
            {
                // Set the validation settings.
                settings = new XmlReaderSettings();
                settings.Schemas = sc;
                settings.ValidationType = ValidationType.Schema;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            }

            settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);

            XmlReader reader = XmlReader.Create(sXMLInputUri, settings);

            while (reader.Read())
            {
                if ((reader.NodeType == XmlNodeType.Element ||
                      reader.NodeType == XmlNodeType.Attribute)
                    && reader.NamespaceURI != sNamespace)
                {
                    mErrorMsg.Add("XML namespace: '" + reader.NamespaceURI + "' is not a valid namespace.");
                }
            }
        }
        #endregion Public XSD CHECK Methods

        #region Public Methods
        /// <summary>
        /// Get all XML validation warnings formated as a string with new kine seperators.
        /// </summary>
        /// <returns></returns>
        public string GetWarnings()
        {
            string s = string.Empty;
            foreach (string i in Warnings)
            {
                s += "Warning: " + i;
            }

            return s;
        }

        /// <summary>
        /// Get all XML validation errors formated as a string with new kine seperators.
        /// </summary>
        /// <returns></returns>
        public string GetErrors()
        {
            string s = string.Empty;
            foreach (string i in Errors)
            {
                s += "Error: " + i;
            }

            return s;
        }

        /// <summary>
        /// Determine whether there are any warnings or errors
        /// encountered durring XML validation.
        /// </summary>
        /// <returns></returns>
        public bool SchemaIsValid()
        {
            return ((Errors.Count + Warnings.Count) > 0 ? false : true);
        }
        #endregion Public Methods

        #region Protected Methods
        /// <summary>
        /// Internal messaging function to receive Warnings/Errors
        /// encountered durring XML/XSD validation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void ValidationCallBack(object sender, ValidationEventArgs args)
        {
            switch (args.Severity)
            {
                case XmlSeverityType.Warning:
                    mWarningMsg.Add(string.Format("Line: {0}, Position: {1} {2}",
                                                     args.Exception.LineNumber, args.Exception.LinePosition, args.Exception.Message));
                    break;

                case XmlSeverityType.Error:
                    mErrorMsg.Add(string.Format("Line: {0}, Position: {1} {2}",
                                                     args.Exception.LineNumber, args.Exception.LinePosition, args.Exception.Message));
                    break;

                default:
                    mWarningMsg.Add("Unhandled Severity of type: "
                                        + args.Severity.ToString() + args.Message);
                    break;
            }
        }
        #endregion Protected Methods
    }
}
