using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchStringParser {
    public class SearchStringParser {
        readonly SearchStringParseSettings settings;

        SearchStringParseResult result = new SearchStringParseResult();

        SearchStringParser(SearchStringParseSettings settings) {
            this.settings = settings;
        }

        public static SearchStringParseResult Parse(string text, SearchStringParseSettings settings) {
            var parser = new SearchStringParser(settings);
            parser.ParseCore(text);
            return parser.result;
        }

        void ParseCore(string searchText) {
            //var info = new SearchStringParseInfo(settings.SearchMode);
            foreach(var phase in (searchText ?? String.Empty).Split(new[] { settings.PhaseSeparator }, StringSplitOptions.RemoveEmptyEntries)) {
                ParsePhase(phase);
            }
            //if(info.SearchStrings.Count > 0)
            //    result.ForAll.Add(info);
        }

        void ParsePhase(string phase) {
            if(phase.StartsWith(settings.IncludeModificator)) {
                AddInfo(result.Include, phase.Remove(0, settings.IncludeModificator.Length));
                return;
            }
            AddInfo(result.ForAll, phase);
        }

        void AddInfo(List<SearchStringParseInfo> target, string phase) {
            var info = new SearchStringParseInfo(settings.SearchMode, phase);
            target.Add(info);
        }
    }
}
