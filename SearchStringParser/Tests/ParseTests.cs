using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchStringParser.Tests {
    public class ParseTests {
        static SearchStringParseResult Parse(string text, SearchStringParseSettings settings) {
            return SearchStringParser.Parse(text, settings);
        }

        [Test]
        public void SingleChar() {
            var result = Parse("a", SearchStringParseSettings.Default);
            Assert.AreEqual(1, result.ForAll.Count);
            Assert.AreEqual(1, result.ForAll.Single().SearchStrings.Count);
            Assert.AreEqual("a", result.ForAll.Single().SearchStrings.Single());
            Assert.AreEqual(SearchMode.Like, result.ForAll.Single().SearchMode);
        }
    }
}
