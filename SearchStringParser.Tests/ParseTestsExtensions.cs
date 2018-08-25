using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchStringParser.Tests {
    public static class ParseTestsExtensions {
        public static SearchStringParseResult AssertPhases(this SearchStringParseResult result, PhaseInfo[] expected1, PhaseInfo expected2, params PhaseInfo[] expectedAdd) {
            return AssertPhases(result, Enumerable.Concat(expected1, new[] { expected2 }).ToArray(), expectedAdd);
        }
        public static SearchStringParseResult AssertPhases(this SearchStringParseResult result, PhaseInfo expected1, PhaseInfo expected2, params PhaseInfo[] expectedAdd) {
            return AssertPhases(result, new[] { expected1, expected2 }, expectedAdd);
        }
        public static SearchStringParseResult AssertPhases(this SearchStringParseResult result, PhaseInfo expected, params PhaseInfo[] expectedAdd) {
            return AssertPhases(result, new[] { expected }, expectedAdd);
        }
        public static SearchStringParseResult AssertPhases(this SearchStringParseResult result, PhaseInfo[] expected, params PhaseInfo[] expectedAdd) {
            var expectedColelction = expectedAdd == null ? expected : Enumerable.Concat(expected, expectedAdd).ToArray();
            CollectionAssert.AreEquivalent(expectedColelction, result.PhaseInfos, BuildDiffMessage(expectedColelction, result.PhaseInfos));
            return result;
            //return AssertPhases(result, Enumerable.Concat(expected, expectedAdd).ToArray());
        }
        //public static SearchStringParseResult AssertPhases(this SearchStringParseResult result, params PhaseInfo[] expected) {
        //    CollectionAssert.AreEquivalent(expected, result.PhaseInfos, BuildDiffMessage(expected, result.PhaseInfos));
        //    return result;
        //}
        static string BuildDiffMessage(PhaseInfo[] expected, List<PhaseInfo> phaseInfos) {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Expected/Actual Counts:{expected.Length}/{phaseInfos.Count}");
            sb.AppendLine("Expected:");
            Action<PhaseInfo> fillPhaseInfo = info => {
                sb.AppendLine($"{nameof(PhaseInfo.Text)}:{info.Text}, {nameof(PhaseInfo.Modificator)}:{info.Modificator}, {nameof(PhaseInfo.Grouped)}:{info.Grouped}");
            };
            foreach(var el in expected)
                fillPhaseInfo(el);
            sb.AppendLine("Actual:");
            foreach(var el in phaseInfos)
                fillPhaseInfo(el);
            return sb.ToString();
        }
        public static SearchStringParseResult AssertPhases(this SearchStringParseResult result) {
            return AssertPhases(result, new PhaseInfo[0]);
        }
        public static SearchStringParseResult AssertPhases(this SearchStringParseResult result, params string[] expected) {
            return AssertPhases(result, expected == null ? null : expected.Select(phase => new PhaseInfo(phase)).ToArray());
        }
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
            collection = collection.Where(c => c.Field == field).ToList();
            Assert.AreEqual(searchTexts.Length, collection.Count);
            for(int i = 0; i < searchTexts.Length; i++)
                AssertParseInfo(collection[i], field, searchTexts[i]);
        }
        static SearchStringParseInfo AssertParseInfo(this SearchStringParseInfo info, string field, string searchText) {
            Assert.AreEqual(field, info.Field);
            Assert.AreEqual(searchText, info.SearchString);
            return info;
        }
    }
}
