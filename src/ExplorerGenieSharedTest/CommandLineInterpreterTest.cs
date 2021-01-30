using System.Collections.Generic;
using ExplorerGenieShared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExplorerGenieSharedTest
{
    [TestClass]
    public class CommandLineInterpreterTest
    {
        [TestMethod]
        public void SplitCommandLine_WorksWithOptionAndFilenames()
        {
            List<string> res = CommandLineInterpreter.SplitCommandLine(@"-option1 ""D:\"" ""file1.txt"" ""file with space1.txt""");
            Assert.AreEqual(4, res.Count);
            Assert.AreEqual("-option1", res[0]);
            Assert.AreEqual(@"D:\", res[1]);
            Assert.AreEqual(@"file1.txt", res[2]);
            Assert.AreEqual(@"file with space1.txt", res[3]);
        }

        [TestMethod]
        public void SplitCommandLine_WorksWithEmptyCommandLine()
        {
            List<string> res = CommandLineInterpreter.SplitCommandLine("");
            Assert.AreEqual(0, res.Count);
        }

        [TestMethod]
        public void SplitCommandLine_WorksWithOptionOnly()
        {
            List<string> res = CommandLineInterpreter.SplitCommandLine(@"-option1");
            Assert.AreEqual(1, res.Count);
            Assert.AreEqual("-option1", res[0]);
        }

        [TestMethod]
        public void SplitCommandLine_IgnoresEmptyParameters()
        {
            List<string> res = CommandLineInterpreter.SplitCommandLine(@" -option1  ""abc"" """"   ""def"" ");
            Assert.AreEqual(3, res.Count);
        }

        [TestMethod]
        public void ParseCommandLine_RecreatesAbsoluteFilepaths()
        {
            CommandLineArgs res = CommandLineInterpreter.ParseCommandLine(@"-option1 ""D:\"" ""file1.txt"" ""file with space1.txt""");
            Assert.AreEqual("-option1", res.Action);
            Assert.AreEqual(2, res.Filenames.Count);
            Assert.AreEqual(@"D:\file1.txt", res.Filenames[0]);
            Assert.AreEqual(@"D:\file with space1.txt", res.Filenames[1]);
        }

        [TestMethod]
        public void ParseCommandLine_WorksWithOptionOnly()
        {
            CommandLineArgs res = CommandLineInterpreter.ParseCommandLine(@"-option1");
            Assert.AreEqual("-option1", res.Action);
            Assert.AreEqual(0, res.Filenames.Count);
        }
    }
}
