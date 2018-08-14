using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchStringParser {
    public class SearchStringParser {
        SearchStringParser() { }

        public static SearchStringParseResult Parse(string text, SearchStringParseSettings settings) {
            return new SearchStringParser().ParseCore(text, settings);
        }

        SearchStringParseResult ParseCore(string text, SearchStringParseSettings settings) {
            return null;
        }
    }

    public class SearchStringParseResult {
        public List<SearchStringFieldParseInfo> ForAll { get; } = new List<SearchStringFieldParseInfo>();
        public Dictionary<string, List<SearchStringFieldParseInfo>> FieldSpecific { get; } = new Dictionary<string, List<SearchStringFieldParseInfo>>();
    }

    public class SearchStringFieldParseInfo {
        public SearchStringFieldParseInfo(SearchMode searchMode) {
            SearchMode = searchMode;
        }

        public List<string> SearchStrings { get; } = new List<string>();
        public SearchMode SearchMode { get; }
    }

    public enum SearchMode {
        Like,
        Contains,
        Equals,
        StartsWith,
        EndsWith
    }

    public class SearchStringParseSettings {
        public static SearchStringParseSettings Default {
            get {
                return new SearchStringParseSettings();
            }
        }
    }
}
