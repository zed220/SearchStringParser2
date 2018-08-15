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

        class ParsingState {
            public ParsingState() {
                Flush();
            }

            public string Phase;
            public PhaseMode PhaseMode;
            public bool Group;

            public void Flush() {
                Phase = String.Empty;
                PhaseMode = PhaseMode.Default;
                Group = false;
            }
        }

        void ParseString(string searchText) {
            if(searchText == null)
                return;
            ParsingState state = new ParsingState();
            foreach(char c in searchText) {
                if(state.Phase == string.Empty) {
                    if(c == settings.GroupModificator) {
                        state.Group = true;
                        continue;
                    }
                    if(!state.Group) {
                        if(c == settings.ExcludeModificator) {
                            state.PhaseMode = PhaseMode.Exclude;
                            continue;
                        }
                        if(c == settings.IncludeModificator) {
                            state.PhaseMode = PhaseMode.Include;
                            continue;
                        }
                    }
                }
                if(!state.Group) {
                    if(c == settings.PhaseSeparator) {
                        BuildPhase(state);
                        continue;
                    }
                }
                if(c == settings.GroupModificator) {
                    BuildPhase(state);
                    continue;
                }
                state.Phase += c;
            }
            BuildPhase(state);
        }

        void BuildPhase(ParsingState state) {
            BuildPhaseCore(state);
            state.Flush();
        }
        void BuildPhaseCore(ParsingState state) {
            if(state.Phase == string.Empty) {
                switch(state.PhaseMode) {
                    case PhaseMode.Default:
                        if(state.Group)
                            AddInfo(result.ForAll, settings.GroupModificator.ToString());
                        return;
                    case PhaseMode.Exclude:
                        AddInfo(result.ForAll, settings.ExcludeModificator.ToString());
                        break;
                    case PhaseMode.Include:
                        AddInfo(result.ForAll, settings.IncludeModificator.ToString());
                        break;
                }
                return;
            }
            switch(state.PhaseMode) {
                case PhaseMode.Default:
                    AddInfo(result.ForAll, state.Phase);
                    break;
                case PhaseMode.Exclude:
                    AddInfo(result.Exclude, state.Phase);
                    break;
                case PhaseMode.Include:
                    AddInfo(result.Include, state.Phase);
                    break;
            }
        }

        void AddInfo(List<SearchStringParseInfo> target, string phase) {
            target.Add(new SearchStringParseInfo(settings.SearchMode, phase));
        }
    }
}
