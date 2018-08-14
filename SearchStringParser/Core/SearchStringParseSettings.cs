namespace SearchStringParser {
    public class SearchStringParseSettings {
        public SearchStringParseSettings(SearchMode searchMode, string phaseSeparator) {
            SearchMode = searchMode;
            PhaseSeparator = phaseSeparator;
        }

        public SearchMode SearchMode { get; }
        public string PhaseSeparator { get; }

        public static SearchStringParseSettings Default {
            get {
                return new SearchStringParseSettings(SearchMode.Like, " ");
            }
        }
    }
}
