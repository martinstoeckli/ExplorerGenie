using System.Diagnostics;
using ExplorerGenieShared;
using ExplorerGenieShared.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExplorerGenieSharedTest
{
    [TestClass]
    public class GotoToolStarterTest
    {
        [TestMethod]
        public void Prepare_CorrectlyHandlesAsAdmin()
        {
            CustomGotoToolModel model = new CustomGotoToolModel
            {
                CommandLine = "explorer.exe",
                AsAdmin = false,
            };
            ProcessStartInfo res = GotoToolStarter.Prepare(model, @"D:\Test.txt");
            Assert.AreEqual("open", res.Verb);

            model = new CustomGotoToolModel
            {
                CommandLine = "explorer.exe",
                AsAdmin = true,
            };
            res = GotoToolStarter.Prepare(model, @"D:\Test.txt");
            Assert.AreEqual("runas", res.Verb);
        }

        [TestMethod]
        public void Prepare_ReplacesVariablesFullPath()
        {
            CustomGotoToolModel model = new CustomGotoToolModel
            {
                CommandLine = @"tool.exe ""{P}"" ""{p}""",
            };
            ProcessStartInfo res;

            // Test with file
            res = GotoToolStarter.Prepare(model, @"C:\Windows\win.ini");
            Assert.AreEqual(@"tool.exe", res.FileName);
            Assert.AreEqual(@"""C:\Windows\win.ini"" ""C:\Windows\win.ini""", res.Arguments);

            // Test with directory
            res = GotoToolStarter.Prepare(model, @"C:\Windows");
            Assert.AreEqual(@"tool.exe", res.FileName);
            Assert.AreEqual(@"""C:\Windows\"" ""C:\Windows\""", res.Arguments);
        }

        [TestMethod]
        public void Prepare_ReplacesVariablesDirectoryOnly()
        {
            CustomGotoToolModel model = new CustomGotoToolModel
            {
                CommandLine = @"tool.exe ""{D}"" ""{d}""",
            };
            ProcessStartInfo res;

            // Test with file
            res = GotoToolStarter.Prepare(model, @"C:\Windows\win.ini");
            Assert.AreEqual(@"tool.exe", res.FileName);
            Assert.AreEqual(@"""C:\Windows\"" ""C:\Windows\""", res.Arguments);

            // Test with directory
            res = GotoToolStarter.Prepare(model, @"C:\Windows");
            Assert.AreEqual(@"tool.exe", res.FileName);
            Assert.AreEqual(@"""C:\Windows\"" ""C:\Windows\""", res.Arguments);
        }

        [TestMethod]
        public void Prepare_ReplacesVariablesFileOnly()
        {
            CustomGotoToolModel model = new CustomGotoToolModel
            {
                CommandLine = @"tool.exe ""{F}"" ""{f}""",
            };
            ProcessStartInfo res;

            // Test with file
            res = GotoToolStarter.Prepare(model, @"C:\Windows\win.ini");
            Assert.AreEqual(@"tool.exe", res.FileName);
            Assert.AreEqual(@"""win.ini"" ""win.ini""", res.Arguments);

            // Test with directory
            res = GotoToolStarter.Prepare(model, @"C:\Windows");
            Assert.AreEqual(@"tool.exe", res.FileName);
            Assert.AreEqual(@""""" """"", res.Arguments);

            res = GotoToolStarter.Prepare(model, @"C:\Windows\");
            Assert.AreEqual(@"tool.exe", res.FileName);
            Assert.AreEqual(@""""" """"", res.Arguments);
        }

        [TestMethod]
        public void Prepare_HandlesEnquotedExecutable()
        {
            CustomGotoToolModel model = new CustomGotoToolModel();
            ProcessStartInfo res;

            model.CommandLine = @"""tool.exe"" ""param 1""";
            res = GotoToolStarter.Prepare(model, @"C:\Windows\win.ini");
            Assert.AreEqual(@"tool.exe", res.FileName);
            Assert.AreEqual(@"""param 1""", res.Arguments);

            model.CommandLine = @"""tool.exe""";
            res = GotoToolStarter.Prepare(model, @"C:\Windows\win.ini");
            Assert.AreEqual(@"tool.exe", res.FileName);
            Assert.AreEqual(string.Empty, res.Arguments);

            model.CommandLine = @"  ""to ol.exe""  ";
            res = GotoToolStarter.Prepare(model, "dummy");
            Assert.AreEqual(@"to ol.exe", res.FileName);
            Assert.AreEqual(string.Empty, res.Arguments);
        }

        [TestMethod]
        public void Prepare_HandlesArgumentOnly()
        {
            CustomGotoToolModel model = new CustomGotoToolModel();
            ProcessStartInfo res;

            model.CommandLine = @"{P}";
            res = GotoToolStarter.Prepare(model, @"C:\Windows\writer.exe");
            Assert.AreEqual(@"C:\Windows\writer.exe", res.FileName);
            Assert.AreEqual(string.Empty, res.Arguments);

            model.CommandLine = string.Empty;
            res = GotoToolStarter.Prepare(model, @"C:\Windows\win.ini");
            Assert.AreEqual(string.Empty, res.FileName);
            Assert.AreEqual(string.Empty, res.Arguments);
        }

        [TestMethod]
        public void DetermineExecutableLength_WorksCorrectly()
        {
            int res;
            res = GotoToolStarter.DetermineExecutableLength(null);
            Assert.AreEqual(0, res);

            res = GotoToolStarter.DetermineExecutableLength(@"explorer.exe");
            Assert.AreEqual(12, res);

            res = GotoToolStarter.DetermineExecutableLength(@"'explorer.exe'");
            Assert.AreEqual(14, res);

            res = GotoToolStarter.DetermineExecutableLength(@"""explorer.exe""");
            Assert.AreEqual(14, res);

            res = GotoToolStarter.DetermineExecutableLength(@"""ex plorer.exe""");
            Assert.AreEqual(15, res);

            res = GotoToolStarter.DetermineExecutableLength(@"""ex'plorer.exe""");
            Assert.AreEqual(15, res);

            res = GotoToolStarter.DetermineExecutableLength(@"'ex""plorer.exe'");
            Assert.AreEqual(15, res);

            res = GotoToolStarter.DetermineExecutableLength(@"explorer.exe ");
            Assert.AreEqual(12, res);

            res = GotoToolStarter.DetermineExecutableLength(@"explorer.exe param1");
            Assert.AreEqual(12, res);
        }

        [TestMethod]
        public void StartProcess_ReallyStartNewProcess()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                Verb = "open", // "runas" for admin
                WindowStyle = ProcessWindowStyle.Normal,
            };

            startInfo.FileName = @"cmd.exe";
            startInfo.Arguments = @"/k ""cd /d D:\ExplorerGenieTest\with space\""";
            //GotoToolStarter.StartProcess(startInfo);

            startInfo.FileName = @"powershell.exe";
            startInfo.Arguments = @"-noexit -command Set-Location -LiteralPath 'D:\ExplorerGenieTest\with[brakes]\'";
            //GotoToolStarter.StartProcess(startInfo);

            startInfo.FileName = @"explorer.exe";
            startInfo.Arguments = @"/root,""D:\ExplorerGenieTest\with space\""";
            //GotoToolStarter.StartProcess(startInfo);

            startInfo.FileName = @"explorer.exe";
            startInfo.Arguments = @"/select,""D:\ExplorerGenieTest\with space\test.txt""";
            //GotoToolStarter.StartProcess(startInfo);

            startInfo.FileName = @"cmd.exe /k ""cd /d D:\ExplorerGenieTest\with space\""";
            startInfo.Arguments = null;
            //GotoToolStarter.StartProcess(startInfo);
        }
    }
}
