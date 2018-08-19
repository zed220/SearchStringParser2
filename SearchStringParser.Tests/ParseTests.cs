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
        static PhaseInfo Phase(string searchString) {
            return new PhaseInfo(searchString);
        }
        static PhaseInfo IncludePh(string searchString) {
            return new PhaseInfo(Include(searchString), SearchModificator.Include);
        }
        static PhaseInfo ExcludePh(string searchString) {
            return new PhaseInfo(Exclude(searchString), SearchModificator.Exclude);
        }
        static PhaseInfo[] SpecificFieldPh(string field, string phase) {
            return new[] { new PhaseInfo(field, SearchModificator.Field), new PhaseInfo(cSpecificField.ToString(), SearchModificator.None), new PhaseInfo(phase) };
        }
        static PhaseInfo[] SpecificFieldIncPh(string field, string phase) {
            return new[] { new PhaseInfo(cInclude.ToString(), SearchModificator.Include), new PhaseInfo(field, SearchModificator.Field), new PhaseInfo(cSpecificField.ToString(), SearchModificator.None), new PhaseInfo(phase, SearchModificator.Include) };
        }
        static PhaseInfo[] SpecificFieldExclPh(string field, string phase) {
            return new[] { new PhaseInfo(cExclude.ToString(), SearchModificator.Exclude), new PhaseInfo(field, SearchModificator.Field), new PhaseInfo(cSpecificField.ToString(), SearchModificator.None), new PhaseInfo(phase, SearchModificator.Exclude) };
        }
        static string Group(string searchString) {
            return cGroup + searchString + cGroup;
        }
        static string SpecificField(string field, string searchString) {
            return field + cSpecificField + searchString;
        }
        static string MakeSearchString(params string[] texts) {
            return string.Join(cSpace.ToString(), texts);
        }

        [Test]
        public void DefaultSettings() {
            Assert.AreEqual(5, typeof(SearchStringParseSettings).GetProperties(BindingFlags.Public | BindingFlags.Instance).Length);
            Assert.AreEqual(' ', SearchStringParseSettings.Default.PhaseSeparator);
            Assert.AreEqual('+', SearchStringParseSettings.Default.IncludeModificator);
            Assert.AreEqual('-', SearchStringParseSettings.Default.ExcludeModificator);
            Assert.AreEqual('"', SearchStringParseSettings.Default.GroupModificator);
            Assert.AreEqual(':', SearchStringParseSettings.Default.SpecificFieldModificator);
        }
        [Test]
        public void BoundaryValues_Elemental() {
            Parse(null).AssertRegular().AssertInclude().AssertExclude().AssertPhases();
            Parse("").AssertRegular().AssertInclude().AssertExclude().AssertPhases();
            Parse(cSpace.ToString()).AssertRegular().AssertInclude().AssertExclude().AssertPhases(" ");
            Parse(cSpace.ToString() + cSpace.ToString()).AssertRegular().AssertInclude().AssertExclude().AssertPhases("  ");
        }
        [Test]
        public void SimpleParsing() {
            Parse("a").AssertRegular("a").AssertInclude().AssertExclude().AssertPhases("a");
            Parse("ab").AssertRegular("ab").AssertInclude().AssertExclude().AssertPhases("ab");
            Parse("a ").AssertRegular("a").AssertInclude().AssertExclude().AssertPhases("a" + cSpace);
            Parse(" a").AssertRegular("a").AssertInclude().AssertExclude().AssertPhases(cSpace.ToString(), "a");
            Parse(" a ").AssertRegular("a").AssertInclude().AssertExclude().AssertPhases(cSpace.ToString(), "a" + cSpace);
            Parse(MakeSearchString("a", "b")).AssertRegular("a", "b").AssertInclude().AssertExclude().AssertPhases("a" + cSpace, "b");
            Parse(MakeSearchString("a", "b", "c")).AssertRegular("a", "b", "c").AssertInclude().AssertExclude().AssertPhases("a" + cSpace, "b" + cSpace, "c");
        }
        [Test]
        public void ModificatorInclude() {
            Parse(Include("a")).
                AssertRegular().AssertInclude("a").AssertExclude().
                AssertPhases(IncludePh("a"));
            Parse(Include("ab")).
                AssertRegular().AssertInclude("ab").AssertExclude().
                AssertPhases(IncludePh("ab"));
            Parse(Include("ab c")).
                AssertRegular("c").AssertInclude("ab").AssertExclude().
                AssertPhases(IncludePh("ab" + cSpace), Phase("c"));
            Parse("ab " + Include("c")).
                AssertRegular("ab").AssertInclude("c").AssertExclude().
                AssertPhases(Phase("ab" + cSpace), IncludePh("c"));
            Parse(MakeSearchString(Include("a"), Include("b"))).
                AssertRegular().AssertInclude("a", "b").AssertExclude().
                AssertPhases(IncludePh("a" + cSpace), IncludePh("b"));
        }
        [Test]
        public void ModificatorExclude() {
            Parse(Exclude("a")).
                AssertRegular().AssertExclude("a").AssertInclude().
                AssertPhases(ExcludePh("a"));
            Parse(Exclude("ab")).
                AssertRegular().AssertExclude("ab").AssertInclude().
                AssertPhases(ExcludePh("ab"));
            Parse(Exclude("ab c")).
                AssertRegular("c").AssertExclude("ab").AssertInclude().
                AssertPhases(ExcludePh("ab" + cSpace), Phase("c"));
            Parse("ab " + Exclude("c")).
                AssertRegular("ab").AssertExclude("c").AssertInclude().
                AssertPhases(Phase("ab" + cSpace), ExcludePh("c"));
            Parse(Exclude("a") + " " + Exclude("b")).
                AssertRegular().AssertExclude("a", "b").AssertInclude().
                AssertPhases(ExcludePh("a" + cSpace), ExcludePh("b"));
        }
        [Test]
        public void ModificatorIncludeExclude() {
            Parse(Exclude("a") + cSpace + Include("b")).
                AssertRegular().AssertExclude("a").AssertInclude("b").
                AssertPhases(ExcludePh("a" + cSpace), IncludePh("b"));
            Parse(Exclude("b") + cSpace + Include("a")).
                AssertRegular().AssertExclude("b").AssertInclude("a").
                AssertPhases(ExcludePh("b" + cSpace), IncludePh("a"));
            Parse(Exclude("a") + cSpace + "c" + cSpace + Include("b")).
                AssertRegular("c").AssertExclude("a").AssertInclude("b").
                AssertPhases(ExcludePh("a" + cSpace), Phase("c" + cSpace), IncludePh("b"));
            Parse(Exclude("b") + cSpace + "c" + cSpace + Include("a")).
                AssertRegular("c").AssertExclude("b").AssertInclude("a").
                AssertPhases(ExcludePh("b" + cSpace), Phase("c" + cSpace), IncludePh("a"));
        }
        [Test]
        public void ModificatorGroup() {
            Parse(Group("a")).
                AssertRegular("a").AssertInclude().AssertExclude().
                AssertPhases(Group("a"));
            Parse(Group("a b")).
                AssertRegular("a b").AssertInclude().AssertExclude().
                AssertPhases(Group("a b"));
            Parse(Include(Group("a"))).
                AssertRegular().AssertInclude("a").AssertExclude().
                AssertPhases(IncludePh(Group("a")));
            Parse(Exclude(Group("a"))).
                AssertRegular().AssertInclude().AssertExclude("a").
                AssertPhases(ExcludePh(Group("a")));
            Parse(Group(Include("a"))).
                AssertRegular(Include("a")).AssertInclude().AssertExclude().
                AssertPhases(Group(Include("a")));
            Parse(Group(Exclude("a"))).
                AssertRegular(Exclude("a")).AssertInclude().AssertExclude().
                AssertPhases(Group(Exclude("a")));
        }
        [Test]
        public void BoundaryValues_Constants() {
            Action<char> assert = c => {
                Parse(c.ToString()).
                AssertRegular(c.ToString()).AssertInclude().AssertExclude().
                AssertPhases(c.ToString());
            };
            assert(cExclude);
            assert(cInclude);
            assert(cGroup);
            assert(cSpecificField);
        }
        [Test]
        public void BoundaryValues_DoubleConstants() {
            Func<char, string> makeDouble = c => c.ToString() + c;
            Parse(makeDouble(cInclude)).
                AssertRegular().AssertInclude(Include(String.Empty)).AssertExclude().
                AssertPhases(IncludePh(cInclude.ToString()));
            Parse(makeDouble(cExclude)).
                AssertRegular().AssertInclude().AssertExclude(Exclude(String.Empty)).
                AssertPhases(ExcludePh(cExclude.ToString()));
            Parse(makeDouble(cGroup)).
                AssertRegular(cGroup.ToString()).AssertInclude().AssertExclude().
                AssertPhases(cGroup.ToString() + cGroup);
            Parse(makeDouble(cGroup) + cGroup).
                AssertRegular(cGroup.ToString()).AssertInclude().AssertExclude().
                AssertPhases(makeDouble(cGroup) + cGroup);
            Parse(makeDouble(cSpecificField)).
                AssertRegular(makeDouble(cSpecificField)).AssertInclude().AssertExclude().
                AssertPhases(makeDouble(cSpecificField));
        }
        [Test]
        public void UnfinishedGroup() {
            Parse(cGroup + "a").
                AssertRegular(cGroup + "a").AssertInclude().AssertExclude().
                AssertPhases(cGroup + "a");
            Parse("a" + cGroup).
                AssertRegular("a" + cGroup).AssertInclude().AssertExclude().
                AssertPhases("a" + cGroup);
            Parse(cGroup + MakeSearchString("a", "b")).
                AssertRegular(cGroup + "a", "b").AssertInclude().AssertExclude().
                AssertPhases(cGroup + "a" + cSpace, "b");
            Parse(Include(cGroup + MakeSearchString("a", "b"))).
                AssertRegular("b").AssertInclude(cGroup + "a").AssertExclude().
                AssertPhases(IncludePh(cGroup.ToString() + "a" + cSpace), Phase("b"));
            Parse(Include(cGroup + MakeSearchString("a", Exclude("b")))).
                AssertRegular().AssertInclude(cGroup + "a").AssertExclude("b").
                AssertPhases(IncludePh(cGroup + "a" + cSpace), ExcludePh("b"));
            Parse(MakeSearchString("a", cGroup + "b")).
                AssertRegular("a", cGroup + "b").AssertInclude().AssertExclude().
                AssertPhases("a" + cSpace, cGroup + "b");
            Parse(SpecificField("f", cGroup.ToString() + "a")).
                AssertFieldRegular("f", cGroup.ToString() + "a").
                AssertPhases(SpecificFieldPh("f", cGroup.ToString() + "a"));
            Parse(Include(SpecificField("f", cGroup.ToString() + "a"))).
                AssertFieldInclude("f", cGroup.ToString() + "a").
                AssertPhases(SpecificFieldIncPh("f", cGroup.ToString() + "a"));
            Parse(Exclude(SpecificField("f", cGroup.ToString() + "a"))).
                AssertFieldExclude("f", cGroup.ToString() + "a").
                AssertPhases(SpecificFieldExclPh("f", cGroup.ToString() + "a"));
        }
        [Test]
        public void IgnoreGrouping() {
            Parse(cGroup.ToString() + cGroup + cGroup).
                AssertRegular(cGroup.ToString()).AssertInclude().AssertExclude().
                AssertPhases(cGroup.ToString() + cGroup + cGroup);
        }
        [Test]
        public void SpecField() {
            Parse(SpecificField("f", "a")).
                AssertFieldRegular("f", "a").AssertInclude().AssertExclude().
                AssertPhases(SpecificFieldPh("f", "a"));
            Parse(Group(SpecificField("a", "b"))).
                AssertRegular(SpecificField("a", "b")).AssertInclude().AssertExclude().
                AssertPhases(Group(SpecificField("a", "b")));
            Parse(Include(Group(SpecificField("a", "b")))).
                AssertRegular().AssertInclude(SpecificField("a", "b")).AssertExclude().
                AssertPhases(IncludePh(Group(SpecificField("a", "b"))));
            Parse(Group(Include(SpecificField("a", "b")))).
                AssertRegular(Include(SpecificField("a", "b"))).AssertInclude().AssertExclude().
                AssertPhases(Group(Include(SpecificField("a", "b"))));
            Parse(Include(SpecificField("f", "a"))).
                AssertRegular().AssertFieldInclude("f", "a").AssertExclude().
                AssertPhases(SpecificFieldIncPh("f", "a"));
            Parse(Exclude(SpecificField("f", "a"))).
                AssertRegular().AssertFieldExclude("f", "a").AssertInclude().
                AssertPhases(SpecificFieldExclPh("f", "a"));
            Parse(SpecificField("f", String.Empty)).
                AssertRegular(SpecificField("f", String.Empty)).AssertInclude().AssertExclude().
                AssertPhases(SpecificField("f", String.Empty));
            Parse(Include(SpecificField("f", String.Empty))).
                AssertRegular().AssertInclude(SpecificField("f", String.Empty)).AssertExclude().
                AssertPhases(IncludePh(SpecificField("f", String.Empty)));
            Parse(Exclude(SpecificField("f", String.Empty))).
                AssertRegular().AssertExclude(SpecificField("f", String.Empty)).AssertInclude().
                AssertPhases(ExcludePh(SpecificField("f", String.Empty)));
            Parse(SpecificField("f", Exclude("a"))).
                AssertFieldRegular("f", Exclude("a")).
                AssertPhases(SpecificFieldPh("f", Exclude("a")));
            Parse(SpecificField("f", Include("a"))).
                AssertFieldRegular("f", Include("a")).
                AssertPhases(SpecificFieldPh("f", Include("a")));
        }
        [Test]
        public void RealCase1() {
            string s = MakeSearchString("regular", Include("plus"), Exclude("minus"), SpecificField("field", "someInField"));
            var phases = new List<PhaseInfo>() { Phase("regular" + cSpace), ExcludePh("minus" + cSpace), IncludePh("plus" + cSpace) };
            phases.AddRange(SpecificFieldPh("field", "someInField"));
            Parse(s).
                AssertRegular("regular").AssertExclude("minus").AssertInclude("plus").AssertFieldRegular("field", "someInField").
                AssertPhases(phases.ToArray());
        }
    }
}
