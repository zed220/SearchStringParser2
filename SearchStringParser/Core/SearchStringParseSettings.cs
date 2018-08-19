namespace SearchStringParser {
    public class SearchStringParseSettings {
        public SearchStringParseSettings(char phaseSeparator, char includeModificator, char excludeModificator, char groupModificator, char specificFieldModificator) {
            PhaseSeparator = phaseSeparator;
            IncludeModificator = includeModificator;
            ExcludeModificator = excludeModificator;
            GroupModificator = groupModificator;
            SpecificFieldModificator = specificFieldModificator;
        }

        public char PhaseSeparator { get; }
        public char IncludeModificator { get; }
        public char ExcludeModificator { get; }
        public char GroupModificator { get; }
        public char SpecificFieldModificator { get; }

        public static SearchStringParseSettings Default {
            get { return new SearchStringParseSettings(' ', '+', '-', '"', ':'); }
        }
    }
}
