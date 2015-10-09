namespace TranslationSolutionViewModelLib.SolutionModelVisitors
{
    using System.Threading.Tasks;
    using TranslatorSolutionLib.Models;

    /// <summary>
    /// Class hides complexity of visitor pattern extensions on solution model
    /// and provides extended services, such as, saving and loading to/from XML.
    /// </summary>
    public class PersistSolutionModelInXML
    {
        /// <summary>
        /// Returns a started task that loads the solution
        /// from the given location and name.
        /// </summary>
        /// <param name="solutionPathName"></param>
        /// <returns></returns>
        public Task<TranslatorSolution> LoadSolutionAsync(string solutionPathName)
        {
            return Task.Factory.StartNew<TranslatorSolution>(() =>
            {
                return LoadSolution(solutionPathName);
            });
        }

        /// <summary>
        /// Convert XML string into solution model domain model object representation.
        /// </summary>
        /// <param name="xmlString"></param>
        /// <param name="modelSolution"></param>
        /// <returns></returns>
        public bool SetXMLString(string xmlString,
                                 TranslatorSolutionLib.Models.TranslatorSolution modelSolution,
                                 string solutionPathName = null)
        {
            var XMLSource = new XMLToModel();

            // Retrieve content back into object value
            return XMLSource.SetXMLString(modelSolution, xmlString, solutionPathName);
        }

        /// <summary>
        /// Returns a started task that saves the solution
        /// in the given location with the given name.
        /// </summary>
        /// <param name="solutionPathName"></param>
        /// <returns></returns>
        public Task<bool> SaveSolutionAsync(TranslatorSolution modelSolution,
                                            string solutionPathName)
        {
            return Task.Factory.StartNew<bool>(() =>
            {
                return SaveSolution(modelSolution, solutionPathName);
            });
        }

        /// <summary>
        /// Gets an XML formated string that persists the current state of this solution.
        /// </summary>
        /// <returns></returns>
        public string GetXMLString(TranslatorSolution modelSolution)
        {
            // Persist content into string
            var XMLStringSource = new ModelToXML();
            return XMLStringSource.ToString(modelSolution);
        }

        /// <summary>
        /// Load the xml from the given location.
        /// </summary>
        /// <param name="solutionPathName"></param>
        /// <returns></returns>
        public TranslatorSolution LoadSolution(string solutionPathName)
        {
            var xmlString = System.IO.File.ReadAllText(solutionPathName);

            TranslatorSolutionLib.Models.TranslatorSolution sol = new TranslatorSolutionLib.Models.TranslatorSolution();

            // Retrieve content back into object value
            if (SetXMLString(xmlString, sol, solutionPathName) == true)
                return sol;

            return null;
        }

        /// <summary>
        /// Saves the current content of the solution object to disk.
        /// </summary>
        /// <param name="solution"></param>
        /// <param name="solutionPathName"></param>
        public bool SaveSolution(TranslatorSolution modelSolution,
                                            string solutionPathName)
        {
            string xml = GetXMLString(modelSolution);

            System.IO.File.WriteAllText(solutionPathName, xml);

            return true;
        }
    }
}
