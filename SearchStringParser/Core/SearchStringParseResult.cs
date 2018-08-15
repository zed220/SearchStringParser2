using System.Collections.Generic;

namespace SearchStringParser {
    public class SearchStringParseResult {
        public List<SearchStringParseInfo> Regular { get; } = new List<SearchStringParseInfo>();
        public List<SearchStringParseInfo> Include { get; } = new List<SearchStringParseInfo>();
        public List<SearchStringParseInfo> Exclude { get; } = new List<SearchStringParseInfo>();
    }
}
