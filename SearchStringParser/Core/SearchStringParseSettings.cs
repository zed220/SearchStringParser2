namespace SearchStringParser {
    public class SearchStringParseSettings {
        public SearchStringParseSettings(SearchMode searchMode, string phaseSeparator, string includeModificator, string excludeModificator) {
            SearchMode = searchMode;
            PhaseSeparator = phaseSeparator;
            IncludeModificator = includeModificator;
            ExcludeModificator = excludeModificator;
        }

        public SearchMode SearchMode { get; }
        public string PhaseSeparator { get; }
        public string IncludeModificator { get; }
        public string ExcludeModificator { get; }

        public static SearchStringParseSettings Default {
            get {
                return new SearchStringParseSettings(SearchMode.Like, " ", "+", "-");
            }
        }
    }
}
