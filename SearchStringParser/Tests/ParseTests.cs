using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SearchStringParser.Tests {
    public class ParseTests {
        static SearchStringParseResult Parse(string text, SearchStringParseSettings settings = null) {
            return SearchStringParser.Parse(text, settings ?? SearchStringParseSettings.Default);
        }

        [Test]
        public void DefaultSettings() {
            Assert.AreEqual(2, typeof(SearchStringParseSettings).GetProperties(BindingFlags.Public | BindingFlags.Instance).Length);
            Assert.AreEqual(SearchMode.Like, SearchStringParseSettings.Default.SearchMode);
            Assert.AreEqual(" ", SearchStringParseSettings.Default.PhaseSeparator);
        }

        [Test]
        public void SinglePhase() {
            Parse("a").AssertSingleForAll("a");
            Parse("ab").AssertSingleForAll("ab");
        }

        [Test]
        public void TwoPhases() {
            var result = Parse("a b");
            Assert.AreEqual(0, result.FieldSpecific.Count);
            Assert.AreEqual(2, result.ForAll.Count);
            result.ForAll[0].AssertParseInfo("a");
            result.ForAll[1].AssertParseInfo("b");
        }
    }
}
