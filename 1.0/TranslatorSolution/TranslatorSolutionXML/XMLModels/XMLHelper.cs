namespace TranslatorSolutionLib.XMLModels
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Xml.Serialization;

    /// <summary>
    /// THis class cann be used to read and write XML from and to Poco objects generated with XSD.exe:
    /// https://msdn.microsoft.com/en-us/library/x6c1kb0s(v=vs.110).aspx
    /// 
    /// Source: http://blog.icanmakethiswork.io/2012/11/xsdxml-schema-generator-xsdexe-taking.html
    /// 
    /// Generate class from XSD file with:
    /// xsd.exe "C:\\Contact.xsd" /classes /out:"C:\\" /namespace:"MyNameSpace"
    /// 
    /// Sample usage for methods in this class:
    /// 
    /// using MyNameSpace;
    /// 
    /// //Make a new contact
    /// contact myContact = new contact();
    /// 
    /// //Serialize the contact to XML
    /// string myContactXML = XmlConverter<contact>.ToXML(myContact);
    /// 
    /// //Deserialize the XML back into an object
    /// contact myContactAgain = XmlConverter<contact>.ToObject(myContactXML);
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class XmlConverter<T>
    {
        private static XmlSerializer _serializer = null;

        #region Static Constructor
        /// <summary>
        /// Static constructor that initialises the serializer for this type
        /// </summary>
        static XmlConverter()
        {
            _serializer = new XmlSerializer(typeof(T));
        }
        #endregion

        #region Public
        /// <summary>
        /// Deserialize the supplied XML into an object
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T ToObject(string xml)
        {
            return (T)_serializer.Deserialize(new StringReader(xml));
        }

        /// <summary>
        /// Serialize the supplied object into XML
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToXML(T obj)
        {
            using (var memoryStream = new MemoryStream())
            {
                _serializer.Serialize(memoryStream, obj);

                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }

        /// <summary>
        /// Pulls an XSD file from an application resource and uses it to
        /// verify whether submitted XML file in filesystem is valid or not
        /// </summary>
        /// <param name="sXML">XML file to validate</param>
        /// <returns>Error message (if any). Returns string with zero length
        /// if everythings fine.</returns>
        private static string TestXML(string sXML, string XSD_Location)
        {
            string sMsg = string.Empty;
            ////string sAssembly = App.ResourceAssembly.Location;                           // .Net 4.0
            string sAssembly = System.Reflection.Assembly.GetExecutingAssembly().CodeBase; // .Net 2.0

            // Replace the information in '<>' brackets with a valid path
            // to a XSD file (that you added into your Visual Studio project)
            // Be careful: Names are case sensitiv and '.' are delimters.
            // Make sure your XSD file is an 'embedded resource'
            // "<Namespace>.<FolderName>.<Filename>.xsd"
            ////const string XSD_Location = "DB.Model.ServerLogins.XML.ServerLogins.xsd";
            ValidateSchema vs = new ValidateSchema();

            Assembly a = Assembly.LoadFrom(sAssembly);

            using (Stream strm = a.GetManifestResourceStream(XSD_Location))
            {
                if (strm == null)
                    return (string.Format("Unable to load XSD: '{0}' file from internal resource.", XSD_Location));

                vs.CheckXMLString_XSD(sXML, strm);

                if (vs.SchemaIsValid() == false)  // Return strings describing problems (if any)
                    sMsg = ////"The following problems are detected in " + sXML + ": " +
                           (vs.GetErrors().Length > 0 ? Environment.NewLine + vs.GetErrors() : string.Empty)
                           + (vs.GetWarnings().Length > 0 ? Environment.NewLine + vs.GetWarnings() : string.Empty);
            }

            return sMsg;
        }

        /// <summary>
        /// Deserialize the supplied XML into an object
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T ToObject(string xml, string xsdURi)
        {
            string Msg = TestXML(xml, xsdURi);

            if (Msg != string.Empty)
            {
                Console.WriteLine(Msg);
                throw new Exception(Msg);
                ////return default(T);
            }

            return (T)_serializer.Deserialize(new StringReader(xml));
        }
        #endregion
    }
}
