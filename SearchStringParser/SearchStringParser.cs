using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchStringParser {
    public class SearchStringParser {
        SearchStringParseResult result = new SearchStringParseResult();

        SearchStringParser() { }

        public static SearchStringParseResult Parse(string text, SearchStringParseSettings settings) {
            var parser = new SearchStringParser();
            parser.ParseCore(text, settings);
            return parser.result;
        }

        void ParseCore(string text, SearchStringParseSettings settings) {
            var info = new SearchStringFieldParseInfo(settings.SearchMode);
            result.ForAll.Add(info);
            ParsePhase(text, info);
        }

        void ParsePhase(string phase, SearchStringFieldParseInfo info) {
            info.SearchStrings.Add(phase);
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
        public SearchStringParseSettings(SearchMode searchMode) {
            SearchMode = searchMode;
        }

        public SearchMode SearchMode { get; }

        public static SearchStringParseSettings Default {
            get {
                return new SearchStringParseSettings(SearchMode.Like);
            }
        }
    }
}
