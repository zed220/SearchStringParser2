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
        static string MakeExclude(string searchString) {
            return SearchStringParseSettings.Default.ExcludeModificator + searchString;
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
            Parse(null).AssertSingleForAll().AssertSingleInclude().AssertSingleExclude();
            Parse("").AssertSingleForAll().AssertSingleInclude().AssertSingleExclude();
            Parse(" ").AssertSingleForAll().AssertSingleInclude().AssertSingleExclude();
            Parse("  ").AssertSingleForAll().AssertSingleInclude().AssertSingleExclude();
        }

        [Test]
        public void SimpleParsing() {
            Parse("a").AssertSingleForAll("a").AssertSingleInclude().AssertSingleExclude();
            Parse("ab").AssertSingleForAll("ab").AssertSingleInclude().AssertSingleExclude();
            Parse("a ").AssertSingleForAll("a").AssertSingleInclude().AssertSingleExclude();
            Parse(" a").AssertSingleForAll("a").AssertSingleInclude().AssertSingleExclude();
            Parse(" a ").AssertSingleForAll("a").AssertSingleInclude().AssertSingleExclude();
            Parse("a b").AssertSingleForAll("a", "b").AssertSingleInclude().AssertSingleExclude();
            Parse("a b c").AssertSingleForAll("a", "b", "c").AssertSingleInclude().AssertSingleExclude();
        }

        [Test]
        public void ModificatorInclude() {
            Parse(MakeInclude("a")).AssertSingleForAll().AssertSingleInclude("a").AssertSingleExclude();
            Parse(MakeInclude("ab")).AssertSingleForAll().AssertSingleInclude("ab").AssertSingleExclude();
            Parse(MakeInclude("ab c")).AssertSingleForAll("c").AssertSingleInclude("ab").AssertSingleExclude();
            Parse("ab " + MakeInclude("c")).AssertSingleForAll("ab").AssertSingleInclude("c").AssertSingleExclude();
            Parse(MakeInclude("a") + " " + MakeInclude("b")).AssertSingleForAll().AssertSingleInclude("a", "b").AssertSingleExclude();
        }

        [Test]
        public void ModificatorExclude() {
            Parse(MakeExclude("a")).AssertSingleForAll().AssertSingleExclude("a").AssertSingleInclude();
            Parse(MakeExclude("ab")).AssertSingleForAll().AssertSingleExclude("ab").AssertSingleInclude();
            Parse(MakeExclude("ab c")).AssertSingleForAll("c").AssertSingleExclude("ab").AssertSingleInclude();
            Parse("ab " + MakeExclude("c")).AssertSingleForAll("ab").AssertSingleExclude("c").AssertSingleInclude();
            Parse(MakeExclude("a") + " " + MakeExclude("b")).AssertSingleForAll().AssertSingleExclude("a", "b").AssertSingleInclude();
        }

        [Test]
        public void ModificatorBoth() {
            Parse(MakeExclude("a") + " " + MakeInclude("b")).AssertSingleForAll().AssertSingleExclude("a").AssertSingleInclude("b");
            Parse(MakeExclude("b") + " " + MakeInclude("a")).AssertSingleForAll().AssertSingleExclude("b").AssertSingleInclude("a");
            Parse(MakeExclude("a") + " c " + MakeInclude("b")).AssertSingleForAll("c").AssertSingleExclude("a").AssertSingleInclude("b");
            Parse(MakeExclude("b") + " c " + MakeInclude("a")).AssertSingleForAll("c").AssertSingleExclude("b").AssertSingleInclude("a");
        }
    }
}
