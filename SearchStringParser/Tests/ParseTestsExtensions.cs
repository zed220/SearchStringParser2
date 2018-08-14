using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace SearchStringParser.Tests {
    public static class ParseTestsExtensions {
        const string Exclude = nameof(SearchStringParseResult.Exclude);

        public static SearchStringParseResult AssertSingleForAll(this SearchStringParseResult result, params string[] searchTexts) {
            return AssertSingle(result, nameof(SearchStringParseResult.ForAll), null, searchTexts);
        }
        public static SearchStringParseResult AssertSingleInclude(this SearchStringParseResult result, params string[] searchTexts) {
            return AssertSingle(result, nameof(SearchStringParseResult.Include), null, searchTexts);
        }
        public static SearchStringParseResult AssertSingleExclude(this SearchStringParseResult result, params string[] searchTexts) {
            return AssertSingle(result, nameof(SearchStringParseResult.Exclude), null, searchTexts);
        }
        static SearchStringParseResult AssertSingle(this SearchStringParseResult result, string propName, SearchMode? searchMode, params string[] searchTexts) {
            var collection = result.GetType().GetProperty(propName).GetValue(result) as List<SearchStringParseInfo>;
            Assert.AreEqual(searchTexts.Length, collection.Count);
            //Assert.AreEqual(0, result.FieldSpecific.Count);
            for(int i = 0; i < searchTexts.Length; i++)
                AssertParseInfo(collection[i], searchMode, searchTexts[i]);
            return result;
        }

        //static SearchStringParseInfo AssertParseInfo(this SearchStringParseInfo info, params string[] searchTexts) {
        //    return AssertParseInfo(info, null, searchTexts);
        //}
        static SearchStringParseInfo AssertParseInfo(this SearchStringParseInfo info, SearchMode? searchMode, string searchText) {
            Assert.AreEqual(searchMode ?? SearchStringParseSettings.Default.SearchMode, info.SearchMode);
            Assert.AreEqual(searchText, info.SearchString);
            return info;
        }
    }
}
