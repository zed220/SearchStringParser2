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
        static string MakeInclude(string searchString) {
            return SearchStringParseSettings.Default.IncludeModificator + searchString;
        }

        [Test]
        public void DefaultSettings() {
            Assert.AreEqual(4, typeof(SearchStringParseSettings).GetProperties(BindingFlags.Public | BindingFlags.Instance).Length);
            Assert.AreEqual(SearchMode.Like, SearchStringParseSettings.Default.SearchMode);
            Assert.AreEqual(" ", SearchStringParseSettings.Default.PhaseSeparator);
            Assert.AreEqual("+", SearchStringParseSettings.Default.IncludeModificator);
            Assert.AreEqual("-", SearchStringParseSettings.Default.ExcludeModificator);
        }

        [Test]
        public void BoundaryValues() {
            Parse(null).AssertSingleForAll().AssertSingleInclude();
            Parse("").AssertSingleForAll().AssertSingleInclude();
            Parse(" ").AssertSingleForAll().AssertSingleInclude();
            Parse("  ").AssertSingleForAll().AssertSingleInclude();
        }

        [Test]
        public void SimpleParsing() {
            Parse("a").AssertSingleForAll("a").AssertSingleInclude();
            Parse("ab").AssertSingleForAll("ab").AssertSingleInclude();
            Parse("a ").AssertSingleForAll("a").AssertSingleInclude();
            Parse(" a").AssertSingleForAll("a").AssertSingleInclude();
            Parse(" a ").AssertSingleForAll("a").AssertSingleInclude();
            Parse("a b").AssertSingleForAll("a", "b").AssertSingleInclude();
            Parse("a b c").AssertSingleForAll("a", "b", "c").AssertSingleInclude();
        }

        [Test]
        public void ModificatorInclude() {
            Parse(MakeInclude("a")).AssertSingleForAll().AssertSingleInclude("a");
        }
    }
}
