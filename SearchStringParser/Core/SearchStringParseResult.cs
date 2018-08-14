using System.Collections.Generic;

namespace SearchStringParser {
    public class SearchStringParseResult {
        public List<SearchStringParseInfo> ForAll { get; } = new List<SearchStringParseInfo>();
        public List<SearchStringParseInfo> Include { get; } = new List<SearchStringParseInfo>();
        //public Dictionary<string, List<SearchStringParseInfo>> FieldSpecific { get; } = new Dictionary<string, List<SearchStringParseInfo>>();
    }
}
