using System;
using System.IO;
using ExplorerGenieShared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExplorerGenieSharedTest
{
    [TestClass]
    public class PathUtilsTest
    {
        [TestMethod]
        public void ExpandUncFilename_ReturnsOriginalIfLocal()
        {
            string calcPath = Path.Combine(Environment.SystemDirectory, "calc.exe");
            string res = PathUtils.ExpandUncFilename(calcPath);
            Assert.AreEqual(calcPath, res);
        }

        [TestMethod]
        public void ConvertToUri_FormatsCorrectlyWithUnicode4()
        {
            string path = @"C:\Temp\mit Leerschlag🧞";
            string res = PathUtils.ConvertToUri(path);
            Assert.AreEqual(@"file:///C:/Temp/mit%20Leerschlag%F0%9F%A7%9E", res);
        }

        [TestMethod]
        public void ConvertToC_EscapesCorrectly()
        {
            string path = @"C:\Temp\mit Leerschlag";
            string res = PathUtils.ConvertToC(path);
            Assert.AreEqual(@"C:\\Temp\\mit Leerschlag", res);
        }

        [TestMethod]
        public void ConvertToOutlook_WorksWithLocaleDrive()
        {
            string path = @"C:\Temp\mit Leerschlag.txt";
            string res = PathUtils.ConvertToOutlook(path);
            Assert.AreEqual(@"<file://C:\Temp\mit Leerschlag.txt>", res);
        }

        [TestMethod]
        public void ConvertToOutlook_WorksWithNetworkDrive()
        {
            string path = @"\\my-server\Temp\mit Leerschlag.txt";
            string res = PathUtils.ConvertToOutlook(path);
            Assert.AreEqual(@"<\\my-server\Temp\mit Leerschlag.txt>", res);
        }
    }
}
