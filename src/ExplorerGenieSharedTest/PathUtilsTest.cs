using System;
using System.Collections.Generic;
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

        [TestMethod]
        public void ExcludeTrailingBackslash_WorksCorrectly()
        {
            string path;
            string res;
            path = @"\\my-server\Temp\";
            res = PathUtils.ExcludeTrailingBackslash(path);
            Assert.AreEqual(@"\\my-server\Temp", res);

            path = @"\\my-server\Temp";
            res = PathUtils.ExcludeTrailingBackslash(path);
            Assert.AreEqual(@"\\my-server\Temp", res);

            path = @"\\my-server\Temp\mit Leerschlag.txt";
            res = PathUtils.ExcludeTrailingBackslash(path);
            Assert.AreEqual(@"\\my-server\Temp\mit Leerschlag.txt", res);

            path = string.Empty;
            res = PathUtils.ExcludeTrailingBackslash(path);
            Assert.AreEqual(string.Empty, res);
        }

        [TestMethod]
        public void GetUniqueFilename_ReturnsOriginalIfNotExisting()
        {
            string candidate = @"C:\notexisting.txt";
            string res = PathUtils.GetUniqueFilename(candidate, FileExists);
            Assert.AreEqual(candidate, res);
        }

        [TestMethod]
        public void GetUniqueFilename_AddsNumberToExistingFile()
        {
            string candidate = @"D:\existing1.txt";
            string res = PathUtils.GetUniqueFilename(candidate, FileExists);
            Assert.AreEqual(@"D:\existing1(1).txt", res);
        }

        [TestMethod]
        public void GetUniqueFilename_AddsNumberToExistingDirectory()
        {
            string candidate = @"D:\existing2";
            string res = PathUtils.GetUniqueFilename(candidate, FileExists);
            Assert.AreEqual(@"D:\existing2(1)", res);
        }

        [TestMethod]
        public void GetUniqueFilename_AddsNumberToExistingNumberedFile()
        {
            string candidate = @"D:\existing3.txt";
            string res = PathUtils.GetUniqueFilename(candidate, FileExists);
            Assert.AreEqual(@"D:\existing3(2).txt", res);
        }

        private bool FileExists(string filePath)
        {
            var existingPaths = new List<string> 
            { 
                @"D:\existing1.txt",
                @"D:\existing2",
                @"D:\existing3.txt",
                @"D:\existing3(1).txt",
            };
            return existingPaths.Contains(filePath);
        }
    }
}
