using System.Collections.Generic;
using ExplorerGenieShared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExplorerGenieSharedTest
{
    [TestClass]
    public class FilenameSorterTest
    {
        [TestMethod]
        public void Sort_DirectoriesBeforeFiles()
        {
            List<string> filenames = new List<string>
            {
                @"D:\temp\dir\cde",
                @"D:\temp\dir\file.def",
                @"D:\temp\dir\file2.abc",
                @"D:\temp\dir\file.abc",
                @"D:\temp2\dir\file.abc",
                @"D:\temp\dir\abc",
                @"D:\temp2\dir\fgh",
                @"D:\temp\dir\another file.abc",
            };

            FilenameSorter sorter = new FilenameSorter(path => !path.Contains("file"));
            sorter.Sort(filenames);
            Assert.AreEqual(8, filenames.Count);
            Assert.AreEqual(@"D:\temp\dir\abc", filenames[0]);
            Assert.AreEqual(@"D:\temp\dir\cde", filenames[1]);
            Assert.AreEqual(@"D:\temp2\dir\fgh", filenames[2]);
            Assert.AreEqual(@"D:\temp\dir\another file.abc", filenames[3]);
            Assert.AreEqual(@"D:\temp\dir\file.abc", filenames[4]);
            Assert.AreEqual(@"D:\temp\dir\file.def", filenames[5]);
            Assert.AreEqual(@"D:\temp\dir\file2.abc", filenames[6]);
            Assert.AreEqual(@"D:\temp2\dir\file.abc", filenames[7]);
        }
    }
}
