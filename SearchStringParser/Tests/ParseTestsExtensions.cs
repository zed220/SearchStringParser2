using NUnit.Framework;
using System.Linq;

namespace SearchStringParser.Tests {
    public static class ParseTestsExtensions {
        public static void AssertSingleForAll(this SearchStringParseResult result, params string[] searchTexts) {
            AssertSingleForAll(result, null, searchTexts);
        }
        public static void AssertSingleForAll(this SearchStringParseResult result, SearchMode? searchMode, params string[] searchTexts) {
            Assert.AreEqual(1, result.ForAll.Count);
            Assert.AreEqual(0, result.FieldSpecific.Count);
            Assert.AreEqual(searchTexts.Length, result.ForAll.Single().SearchStrings.Count);
            for(int i = 0; i < searchTexts.Length; i++)
                AssertParseInfo(result.ForAll.Single(), searchMode, searchTexts);
        }
        public static void AssertParseInfo(this SearchStringParseInfo info, params string[] searchTexts) {
            AssertParseInfo(info, null, searchTexts);
        }
        public static void AssertParseInfo(this SearchStringParseInfo info, SearchMode? searchMode, params string[] searchTexts) {
            Assert.AreEqual(searchMode ?? SearchStringParseSettings.Default.SearchMode, info.SearchMode);
            for(int i = 0; i < searchTexts.Length; i++)
                Assert.AreEqual(searchTexts[i], info.SearchStrings[i]);
        }
    }
}
