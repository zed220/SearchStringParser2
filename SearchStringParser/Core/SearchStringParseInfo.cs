using System.Collections.Generic;

namespace SearchStringParser {
    public class SearchStringParseInfo {
        public SearchStringParseInfo(SearchMode searchMode) {
            SearchMode = searchMode;
        }

        public List<string> SearchStrings { get; } = new List<string>();
        public SearchMode SearchMode { get; }
    }
}
