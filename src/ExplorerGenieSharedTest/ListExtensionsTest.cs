using System.Collections.Generic;
using ExplorerGenieShared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExplorerGenieSharedTest
{
    [TestClass]
    public class ListExtensionsTest
    {
        [TestMethod]
        public void ModifyEach_AltersEachElement()
        {
            List<string> list = new List<string>()
            {
                "Sugus",
                "Caramel",
            };
            list.ModifyEach(element => element + " is sweet");

            Assert.AreEqual("Sugus is sweet", list[0]);
            Assert.AreEqual("Caramel is sweet", list[1]);
        }
    }
}
