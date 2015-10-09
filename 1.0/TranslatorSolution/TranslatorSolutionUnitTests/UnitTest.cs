namespace TranslatorSolutionUnitTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TranslationSolutionViewModelLib.SolutionModelVisitors;
    using TranslationSolutionViewModelLib.ViewModels;
    using TranslatorSolutionLib.Models;

    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestXMLReadWriteWithPoco()
        {
            var solution = new TranslatorSolution();

            solution.SetName("My TestSolution");
            solution.SetComment("This is test solution for testing purposes only");

            var project = new Project();

            project.SetSourceFile(new FileReference(@"C:\TEMP\Source.resx",
                                                     "RESX",
                                                     "This is a test source file comment"));

            project.AddTargetFile(new FileReference(@"C:\TEMP\Source_DE_DE.resx",
                                                     "RESX",
                                                     "This is a test target file comment"));

            solution.AddProject(project);

            // Persist content into string
            var modelExtension = new PersistSolutionModelInXML();
            var XML = modelExtension.GetXMLString(solution);

            var loadedSolution = new PersistSolutionModelInXML();
            TranslatorSolutionLib.Models.TranslatorSolution sol = new TranslatorSolutionLib.Models.TranslatorSolution();

            // Retrieve content back into object values and check if this worked as expected
            Assert.AreEqual(loadedSolution.SetXMLString(XML, sol), true);
        }

        [TestMethod]
        public void TestXMLInternediateReadWithPoco()
        {
            var solution = new TranslatorSolution();

            solution.SetName("My TestSolution");
            solution.SetComment("This is test solution for testing purposes only");

            var project = new Project();

            project.SetSourceFile(new FileReference(@"C:\TEMP\Source.resx",
                                                     "RESX",
                                                     "This is a test source file comment"));

            project.AddTargetFile(new FileReference(@"C:\TEMP\Source_DE_DE.resx",
                                                     "RESX",
                                                     "This is a test target file comment"));

            solution.AddProject(project);

            // Persist content into string
            var modelExtension = new PersistSolutionModelInXML();
            var XML = modelExtension.GetXMLString(solution);

            var XMLStringSource = new ModelToXML();
            var XML1 = XMLStringSource.ToString(solution);

            Assert.AreEqual(string.Compare(XML, XML1), 0);
        }

        [TestMethod]
        public void TestXMLReadWriteIntermediateWithPoco()
        {
            var solution = new TranslatorSolution();

            solution.SetName("My TestSolution");
            solution.SetComment("This is test solution for testing purposes only");

            var project = new Project();

            project.SetSourceFile(new FileReference(@"C:\TEMP\Source.resx",
                                                     "RESX",
                                                     "This is a test source file comment"));

            project.AddTargetFile(new FileReference(@"C:\TEMP\Source_DE_DE.resx",
                                                     "RESX",
                                                     "This is a test target file comment"));

            solution.AddProject(project);

            // Persist content into string
            var XMLStringSource = new ModelToXML();
            var XML = XMLStringSource.ToString(solution);

            var XMLSource = new XMLToModel();
            TranslatorSolutionLib.Models.TranslatorSolution sol = new TranslatorSolutionLib.Models.TranslatorSolution();

            // Retrieve content back into object values and check if this worked as expected
            Assert.AreEqual(XMLSource.SetXMLString(sol, XML), true);

            // Now back to XML and compare with first XML representation
            var XMLStringSource1 = new ModelToXML();
            var XML1 = XMLStringSource1.ToString(sol);
            Assert.AreEqual(string.Compare(XML, XML1, false), 0);
        }
    }
}
