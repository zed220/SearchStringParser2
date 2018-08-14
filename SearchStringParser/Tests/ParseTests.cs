using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchStringParser.Tests {
    public class ParseTests {
        static SearchStringParseResult Parse(string text, SearchStringParseSettings settings = null) {
            return SearchStringParser.Parse(text, settings ?? SearchStringParseSettings.Default);
        }
        static void AssertSingleForAll(SearchStringParseResult result, SearchMode searchMode, params string[] searchTexts) {
            Assert.AreEqual(1, result.ForAll.Count);
            Assert.AreEqual(searchTexts.Length, result.ForAll.Single().SearchStrings.Count);
            for(int i = 0; i < searchTexts.Length; i++) {
                Assert.AreEqual(searchTexts[i], result.ForAll.Single().SearchStrings[i]);
                Assert.AreEqual(searchMode, result.ForAll.Single().SearchMode);
            }
        }

        [Test]
        public void SingleChar() {
            AssertSingleForAll(Parse("a"), SearchStringParseSettings.Default.SearchMode, "a");
            AssertSingleForAll(Parse("ab"), SearchStringParseSettings.Default.SearchMode, "ab");
        }
    }
}
