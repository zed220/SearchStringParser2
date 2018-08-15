using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SearchStringParser.Tests {
    public class ParseTests {
        static char cInclude { get { return SearchStringParseSettings.Default.IncludeModificator; } }
        static char cExclude { get { return SearchStringParseSettings.Default.ExcludeModificator; } }
        static char cGroup { get { return SearchStringParseSettings.Default.GroupModificator; } }
        static char cSpace { get { return SearchStringParseSettings.Default.PhaseSeparator; } }

        static SearchStringParseResult Parse(string text, SearchStringParseSettings settings = null) {
            return SearchStringParser.Parse(text, settings ?? SearchStringParseSettings.Default);
        }
        static string Include(string searchString) {
            return cInclude + searchString;
        }
        static string Exclude(string searchString) {
            return cExclude + searchString;
        }
        static string Group(string searchString) {
            return cGroup + searchString + cGroup;
        }

        [Test]
        public void DefaultSettings() {
            Assert.AreEqual(5, typeof(SearchStringParseSettings).GetProperties(BindingFlags.Public | BindingFlags.Instance).Length);
            Assert.AreEqual(SearchMode.Like, SearchStringParseSettings.Default.SearchMode);
            Assert.AreEqual(' ', SearchStringParseSettings.Default.PhaseSeparator);
            Assert.AreEqual('+', SearchStringParseSettings.Default.IncludeModificator);
            Assert.AreEqual('-', SearchStringParseSettings.Default.ExcludeModificator);
            Assert.AreEqual('"', SearchStringParseSettings.Default.GroupModificator);
        }
        [Test]
        public void BoundaryValues_Elemental() {
            Parse(null).AssertSingleForAll().AssertSingleInclude().AssertSingleExclude();
            Parse("").AssertSingleForAll().AssertSingleInclude().AssertSingleExclude();
            Parse(cSpace.ToString()).AssertSingleForAll().AssertSingleInclude().AssertSingleExclude();
            Parse(cSpace.ToString() + cSpace.ToString()).AssertSingleForAll().AssertSingleInclude().AssertSingleExclude();
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
            Parse(Include("a")).AssertSingleForAll().AssertSingleInclude("a").AssertSingleExclude();
            Parse(Include("ab")).AssertSingleForAll().AssertSingleInclude("ab").AssertSingleExclude();
            Parse(Include("ab c")).AssertSingleForAll("c").AssertSingleInclude("ab").AssertSingleExclude();
            Parse("ab " + Include("c")).AssertSingleForAll("ab").AssertSingleInclude("c").AssertSingleExclude();
            Parse(Include("a") + " " + Include("b")).AssertSingleForAll().AssertSingleInclude("a", "b").AssertSingleExclude();
        }
        [Test]
        public void ModificatorExclude() {
            Parse(Exclude("a")).AssertSingleForAll().AssertSingleExclude("a").AssertSingleInclude();
            Parse(Exclude("ab")).AssertSingleForAll().AssertSingleExclude("ab").AssertSingleInclude();
            Parse(Exclude("ab c")).AssertSingleForAll("c").AssertSingleExclude("ab").AssertSingleInclude();
            Parse("ab " + Exclude("c")).AssertSingleForAll("ab").AssertSingleExclude("c").AssertSingleInclude();
            Parse(Exclude("a") + " " + Exclude("b")).AssertSingleForAll().AssertSingleExclude("a", "b").AssertSingleInclude();
        }
        [Test]
        public void ModificatorIncludeExclude() {
            Parse(Exclude("a") + " " + Include("b")).AssertSingleForAll().AssertSingleExclude("a").AssertSingleInclude("b");
            Parse(Exclude("b") + " " + Include("a")).AssertSingleForAll().AssertSingleExclude("b").AssertSingleInclude("a");
            Parse(Exclude("a") + " c " + Include("b")).AssertSingleForAll("c").AssertSingleExclude("a").AssertSingleInclude("b");
            Parse(Exclude("b") + " c " + Include("a")).AssertSingleForAll("c").AssertSingleExclude("b").AssertSingleInclude("a");
        }
        [Test]
        public void ModificatorGroup() {
            Parse(Group("a")).AssertSingleForAll("a").AssertSingleInclude().AssertSingleExclude();
            Parse(Include(Group("a"))).AssertSingleForAll().AssertSingleInclude("a").AssertSingleExclude();
            Parse(Exclude(Group("a"))).AssertSingleForAll().AssertSingleInclude().AssertSingleExclude("a");
            Parse(Group(Include("a"))).AssertSingleForAll(Include("a")).AssertSingleInclude().AssertSingleExclude();
            Parse(Group(Exclude("a"))).AssertSingleForAll(Exclude("a")).AssertSingleInclude().AssertSingleExclude();
        }
        [Test]
        public void BoundaryValues_Constants() {
            Action<char> assert = c => {
                Parse(c.ToString()).AssertSingleForAll(c.ToString()).AssertSingleInclude().AssertSingleExclude();
                Parse(c.ToString() + c.ToString()).AssertSingleForAll(c.ToString()).AssertSingleInclude().AssertSingleExclude();
            };
            assert(cExclude);
            assert(cInclude);
            assert(cGroup);
        }
        [Test]
        public void UnfinishedGroup() {
            Parse(cGroup + "a").AssertSingleForAll(cGroup + "a").AssertSingleInclude().AssertSingleExclude();
            Parse("a" + cGroup).AssertSingleForAll("a" + cGroup).AssertSingleInclude().AssertSingleExclude();
        }
    }
}
