namespace SearchStringParser {
    public class SearchStringParseSettings {
        public SearchStringParseSettings(SearchMode searchMode, char phaseSeparator, char includeModificator, char excludeModificator, char groupModificator) {
            SearchMode = searchMode;
            PhaseSeparator = phaseSeparator;
            IncludeModificator = includeModificator;
            ExcludeModificator = excludeModificator;
            GroupModificator = groupModificator;
        }

        public SearchMode SearchMode { get; }
        public char PhaseSeparator { get; }
        public char IncludeModificator { get; }
        public char ExcludeModificator { get; }
        public char GroupModificator { get; }

        public static SearchStringParseSettings Default {
            get {
                return new SearchStringParseSettings(SearchMode.Like, ' ', '+', '-', '"');
            }
        }
    }
}
