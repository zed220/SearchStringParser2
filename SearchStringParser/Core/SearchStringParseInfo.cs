using System.Collections.Generic;

namespace SearchStringParser {
    public class SearchStringParseInfo {
        public SearchStringParseInfo(SearchMode searchMode, string searchString) {
            SearchMode = searchMode;
            SearchString = searchString;
        }

        public string SearchString { get; }
        public SearchMode SearchMode { get; }
    }
}
