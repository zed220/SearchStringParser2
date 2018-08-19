using System.Collections.Generic;

namespace SearchStringParser {
    public class SearchStringParseInfo {
        public SearchStringParseInfo(string searchString, string field = null) {
            SearchString = searchString;
            Field = field;
        }

        public string SearchString { get; }
        public string Field { get; }
    }
}
