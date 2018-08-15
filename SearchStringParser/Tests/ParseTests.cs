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
        static char cSpecificField { get { return SearchStringParseSettings.Default.SpecificFieldModificator; } }

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
        static string SpecificField(string field, string searchString) {
            return field + cSpecificField + searchString;
        }

        [Test]
        public void DefaultSettings() {
            Assert.AreEqual(6, typeof(SearchStringParseSettings).GetProperties(BindingFlags.Public | BindingFlags.Instance).Length);
            Assert.AreEqual(SearchMode.Like, SearchStringParseSettings.Default.SearchMode);
            Assert.AreEqual(' ', SearchStringParseSettings.Default.PhaseSeparator);
            Assert.AreEqual('+', SearchStringParseSettings.Default.IncludeModificator);
            Assert.AreEqual('-', SearchStringParseSettings.Default.ExcludeModificator);
            Assert.AreEqual('"', SearchStringParseSettings.Default.GroupModificator);
            Assert.AreEqual(':', SearchStringParseSettings.Default.SpecificFieldModificator);
        }
        [Test]
        public void BoundaryValues_Elemental() {
            Parse(null).AssertRegular().AssertInclude().AssertExclude();
            Parse("").AssertRegular().AssertInclude().AssertExclude();
            Parse(cSpace.ToString()).AssertRegular().AssertInclude().AssertExclude();
            Parse(cSpace.ToString() + cSpace.ToString()).AssertRegular().AssertInclude().AssertExclude();
        }
        [Test]
        public void SimpleParsing() {
            Parse("a").AssertRegular("a").AssertInclude().AssertExclude();
            Parse("ab").AssertRegular("ab").AssertInclude().AssertExclude();
            Parse("a ").AssertRegular("a").AssertInclude().AssertExclude();
            Parse(" a").AssertRegular("a").AssertInclude().AssertExclude();
            Parse(" a ").AssertRegular("a").AssertInclude().AssertExclude();
            Parse("a b").AssertRegular("a", "b").AssertInclude().AssertExclude();
            Parse("a b c").AssertRegular("a", "b", "c").AssertInclude().AssertExclude();
        }
        [Test]
        public void ModificatorInclude() {
            Parse(Include("a")).AssertRegular().AssertInclude("a").AssertExclude();
            Parse(Include("ab")).AssertRegular().AssertInclude("ab").AssertExclude();
            Parse(Include("ab c")).AssertRegular("c").AssertInclude("ab").AssertExclude();
            Parse("ab " + Include("c")).AssertRegular("ab").AssertInclude("c").AssertExclude();
            Parse(Include("a") + " " + Include("b")).AssertRegular().AssertInclude("a", "b").AssertExclude();
        }
        [Test]
        public void ModificatorExclude() {
            Parse(Exclude("a")).AssertRegular().AssertExclude("a").AssertInclude();
            Parse(Exclude("ab")).AssertRegular().AssertExclude("ab").AssertInclude();
            Parse(Exclude("ab c")).AssertRegular("c").AssertExclude("ab").AssertInclude();
            Parse("ab " + Exclude("c")).AssertRegular("ab").AssertExclude("c").AssertInclude();
            Parse(Exclude("a") + " " + Exclude("b")).AssertRegular().AssertExclude("a", "b").AssertInclude();
        }
        [Test]
        public void ModificatorIncludeExclude() {
            Parse(Exclude("a") + " " + Include("b")).AssertRegular().AssertExclude("a").AssertInclude("b");
            Parse(Exclude("b") + " " + Include("a")).AssertRegular().AssertExclude("b").AssertInclude("a");
            Parse(Exclude("a") + " c " + Include("b")).AssertRegular("c").AssertExclude("a").AssertInclude("b");
            Parse(Exclude("b") + " c " + Include("a")).AssertRegular("c").AssertExclude("b").AssertInclude("a");
        }
        [Test]
        public void ModificatorGroup() {
            Parse(Group("a")).AssertRegular("a").AssertInclude().AssertExclude();
            Parse(Group("a b")).AssertRegular("a b").AssertInclude().AssertExclude();
            Parse(Include(Group("a"))).AssertRegular().AssertInclude("a").AssertExclude();
            Parse(Exclude(Group("a"))).AssertRegular().AssertInclude().AssertExclude("a");
            Parse(Group(Include("a"))).AssertRegular(Include("a")).AssertInclude().AssertExclude();
            Parse(Group(Exclude("a"))).AssertRegular(Exclude("a")).AssertInclude().AssertExclude();
        }
        [Test]
        public void BoundaryValues_Constants() {
            Action<char> assert = c => {
                Parse(c.ToString()).AssertRegular(c.ToString()).AssertInclude().AssertExclude();
            };
            assert(cExclude);
            assert(cInclude);
            assert(cGroup);
            assert(cSpecificField);
        }
        [Test]
        public void BoundaryValues_DoubleConstants() {
            Func<char, string> makeDouble = c => c.ToString() + c;
            Parse(makeDouble(cInclude)).AssertRegular().AssertInclude(cInclude.ToString()).AssertExclude();
            Parse(makeDouble(cExclude)).AssertRegular().AssertInclude().AssertExclude(cExclude.ToString());
            Parse(makeDouble(cGroup)).AssertRegular(cGroup.ToString()).AssertInclude().AssertExclude();
            Parse(makeDouble(cGroup) + cGroup).AssertRegular(cGroup.ToString()).AssertInclude().AssertExclude();
            Parse(makeDouble(cSpecificField)).AssertRegular(cSpecificField.ToString()).AssertInclude().AssertExclude();
        }
        [Test]
        public void UnfinishedGroup() {
            Parse(cGroup + "a").AssertRegular(cGroup + "a").AssertInclude().AssertExclude();
            Parse("a" + cGroup).AssertRegular("a" + cGroup).AssertInclude().AssertExclude();
        }
        [Test]
        public void IgnoreGrouping() {
            Parse(cGroup.ToString() + cGroup + cGroup).AssertRegular(cGroup.ToString()).AssertInclude().AssertExclude();
        }
        [Test]
        public void SpecField() {
            Parse(SpecificField("f", "a")).AssertFieldRegular("f", "a").AssertInclude().AssertExclude();
        }
    }
}
