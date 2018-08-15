using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchStringParser {
    public class SearchStringParser {
        readonly SearchStringParseSettings settings;
        readonly SearchStringParseResult result = new SearchStringParseResult();

        SearchStringParser(SearchStringParseSettings settings) {
            this.settings = settings;
        }

        public static SearchStringParseResult Parse(string text, SearchStringParseSettings settings) {
            var parser = new SearchStringParser(settings);
            parser.ParseString(text);
            return parser.result;
        }

        enum PhaseMode { Default, Exclude, Include }

        void ParseString(string searchText) {
            if(searchText == null)
                return;
            string phase = String.Empty;
            PhaseMode mode = PhaseMode.Default;
            bool group = false;
            foreach(char c in searchText) {
                if(phase == string.Empty) {
                    if(c == settings.GroupModificator) {
                        group = true;
                        continue;
                    }
                    if(!group) {
                        if(c == settings.ExcludeModificator) {
                            mode = PhaseMode.Exclude;
                            continue;
                        }
                        if(c == settings.IncludeModificator) {
                            mode = PhaseMode.Include;
                            continue;
                        }
                    }
                }
                if(!group) {
                    if(c == settings.PhaseSeparator) {
                        BuildPhase(phase, mode);
                        phase = string.Empty;
                        mode = PhaseMode.Default;
                        continue;
                    }
                }
                if(c == settings.GroupModificator) {
                    BuildPhase(phase, mode);
                    phase = string.Empty;
                    mode = PhaseMode.Default;
                    continue;
                }
                phase += c;
            }
            BuildPhase(phase, mode);
        }

        void BuildPhase(string phase, PhaseMode mode) {
            if(phase == string.Empty)
                return;
            switch(mode) {
                case PhaseMode.Default:
                    AddInfo(result.ForAll, phase);
                    break;
                case PhaseMode.Exclude:
                    AddInfo(result.Exclude, phase);
                    break;
                case PhaseMode.Include:
                    AddInfo(result.Include, phase);
                    break;
            }
        }

        void AddInfo(List<SearchStringParseInfo> target, string phase) {
            target.Add(new SearchStringParseInfo(settings.SearchMode, phase));
        }
    }
}
