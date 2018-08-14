using System;
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

        void ParseCore(string searchText, SearchStringParseSettings settings) {
            ParseRegularText(searchText, settings);
        }

        void ParseRegularText(string searchText, SearchStringParseSettings settings) {
            var info = new SearchStringParseInfo(settings.SearchMode);
            result.ForAll.Add(info);
            foreach(var phase in (searchText ?? String.Empty).Split(new[] { settings.PhaseSeparator }, StringSplitOptions.RemoveEmptyEntries)) {
                ParsePhase(phase, info);
            }
        }

        void ParsePhase(string phase, SearchStringParseInfo info) {
            info.SearchStrings.Add(phase);
        }
    }
}
