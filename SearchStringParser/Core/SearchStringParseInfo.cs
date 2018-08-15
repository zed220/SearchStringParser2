using System.Collections.Generic;

namespace SearchStringParser {
    public class SearchStringParseInfo {
        public SearchStringParseInfo(string searchString, SearchMode searchMode, string field = null) {
            SearchString = searchString;
            SearchMode = searchMode;
            Field = field;
        }

        public string SearchString { get; }
        public SearchMode SearchMode { get; }
        public string Field { get; }
    }
}
