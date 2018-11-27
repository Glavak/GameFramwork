using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using GameFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameFrameworkTests
{
    [TestClass]
    class FileMergerTests
    {
        [TestMethod]
        public void TestMergeLeaderboards()
        {
            var a = new Dictionary<string, string>
            {
                {"player1", "42.5"},
                {"player5", "11.2"}
            };
            var b = new Dictionary<string, string>
            {
                {"player2", "18.3"},
                {"player5", "35"}
            };

            NetworkFile fileA = new NetworkFile(DhtUtils.GenerateFileId(), Guid.Empty, a.ToImmutableDictionary());
            NetworkFile fileB = new NetworkFile(DhtUtils.GenerateFileId(), Guid.Empty, b.ToImmutableDictionary());

            //FilesMerger.MergeFiles();
        }
    }
}
