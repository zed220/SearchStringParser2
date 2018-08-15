using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace SearchStringParser.Tests {
    public static class ParseTestsExtensions {
        public static SearchStringParseResult AssertRegular(this SearchStringParseResult result, params string[] searchTexts) {
            AssertSingle(result.Regular, null, searchTexts);
            return result;
        }
        public static SearchStringParseResult AssertInclude(this SearchStringParseResult result, params string[] searchTexts) {
            AssertSingle(result.Include, null, searchTexts);
            return result;
        }
        public static SearchStringParseResult AssertExclude(this SearchStringParseResult result, params string[] searchTexts) {
            AssertSingle(result.Exclude, null, searchTexts);
            return result;
        }
        public static SearchStringParseResult AssertFieldRegular(this SearchStringParseResult result, string field, params string[] searchTexts) {
            AssertSingle(result.Regular, field, searchTexts);
            return result;
        }
        public static SearchStringParseResult AssertFieldInclude(this SearchStringParseResult result, string field, params string[] searchTexts) {
            AssertSingle(result.Include, field, searchTexts);
            return result;
        }
        public static SearchStringParseResult AssertFieldExclude(this SearchStringParseResult result, string field, params string[] searchTexts) {
            AssertSingle(result.Exclude, field, searchTexts);
            return result;
        }

        static void AssertSingle(List<SearchStringParseInfo> collection, string field, params string[] searchTexts) {
            Assert.AreEqual(searchTexts.Length, collection.Count);
            for(int i = 0; i < searchTexts.Length; i++)
                AssertParseInfo(collection[i], field, searchTexts[i]);
        }
        static SearchStringParseInfo AssertParseInfo(this SearchStringParseInfo info, string field, string searchText) {
            Assert.AreEqual(SearchStringParseSettings.Default.SearchMode, info.SearchMode);
            Assert.AreEqual(field, info.Field);
            Assert.AreEqual(searchText, info.SearchString);
            return info;
        }
    }
}
