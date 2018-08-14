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
            var info = new SearchStringParseInfo(settings.SearchMode);
            result.ForAll.Add(info);
            ParsePhase(text, info);
        }

        void ParsePhase(string phase, SearchStringParseInfo info) {
            info.SearchStrings.Add(phase);
        }
    }

    public class SearchStringParseResult {
        public List<SearchStringParseInfo> ForAll { get; } = new List<SearchStringParseInfo>();
        public Dictionary<string, List<SearchStringParseInfo>> FieldSpecific { get; } = new Dictionary<string, List<SearchStringParseInfo>>();
    }

    public class SearchStringParseInfo {
        public SearchStringParseInfo(SearchMode searchMode) {
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
        public SearchStringParseSettings(SearchMode searchMode, string phaseSeparator) {
            SearchMode = searchMode;
            PhaseSeparator = phaseSeparator;
        }

        public SearchMode SearchMode { get; }
        public string PhaseSeparator { get; }

        public static SearchStringParseSettings Default {
            get {
                return new SearchStringParseSettings(SearchMode.Like, " ");
            }
        }
    }
}
