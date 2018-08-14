namespace SearchStringParser {
    public class SearchStringParseSettings {
        public SearchStringParseSettings(SearchMode searchMode, string phaseSeparator, string includeModificator) {
            SearchMode = searchMode;
            PhaseSeparator = phaseSeparator;
            IncludeModificator = includeModificator;
        }

        public SearchMode SearchMode { get; }
        public string PhaseSeparator { get; }
        public string IncludeModificator { get; }

        public static SearchStringParseSettings Default {
            get {
                return new SearchStringParseSettings(SearchMode.Like, " ", "+");
            }
        }
    }
}
