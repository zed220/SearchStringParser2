namespace SearchStringParser {
    public class SearchStringParseSettings {
        public SearchStringParseSettings(SearchMode searchMode, char phaseSeparator, char includeModificator, char excludeModificator, char groupModificator, char specificFieldModificator) {
            SearchMode = searchMode;
            PhaseSeparator = phaseSeparator;
            IncludeModificator = includeModificator;
            ExcludeModificator = excludeModificator;
            GroupModificator = groupModificator;
            SpecificFieldModificator = specificFieldModificator;
        }

        public SearchMode SearchMode { get; }
        public char PhaseSeparator { get; }
        public char IncludeModificator { get; }
        public char ExcludeModificator { get; }
        public char GroupModificator { get; }
        public char SpecificFieldModificator { get; }

        public static SearchStringParseSettings Default {
            get {
                return new SearchStringParseSettings(SearchMode.Like, ' ', '+', '-', '"', ':');
            }
        }
    }
}
