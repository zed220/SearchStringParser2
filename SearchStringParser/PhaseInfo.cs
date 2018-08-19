namespace SearchStringParser {
    public class PhaseInfo {
        public PhaseInfo(string text, SearchModificator modificator = SearchModificator.None) {
            Text = text;
            Modificator = modificator;
        }

        public string Text { get; }
        public SearchModificator Modificator { get; }
    }
}
