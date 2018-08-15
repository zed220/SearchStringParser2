using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchStringParser {
    public class SearchStringParser {
        #region Inners

        enum PhaseMode { Default, Exclude, Include }
        class ParsingState {
            public ParsingState() {
                Flush();
            }

            public string Phase;
            public PhaseMode PhaseMode;
            public bool GroupStarted;
            public bool GroupFinished;

            public void Flush() {
                Phase = String.Empty;
                PhaseMode = PhaseMode.Default;
                GroupStarted = false;
                GroupFinished = false;
            }
        }

        #endregion

        readonly SearchStringParseSettings settings;
        readonly SearchStringParseResult result = new SearchStringParseResult();

        ParsingState state = new ParsingState();

        SearchStringParser(SearchStringParseSettings settings) {
            this.settings = settings;
        }

        public static SearchStringParseResult Parse(string text, SearchStringParseSettings settings) {
            if(String.IsNullOrWhiteSpace(text))
                return new SearchStringParseResult();
            var parser = new SearchStringParser(settings);
            parser.ParseString(text);
            return parser.result;
        }

        void ParseString(string searchText) {
            for(int i = 0; i < searchText.Length; i++) {
                char c = searchText[i];
                char? next_c = i < searchText.Length - 1 ? (char?)searchText[i + 1] : null;
                if(state.Phase == string.Empty) {
                    if(!state.GroupStarted) {
                        if(c == settings.GroupModificator) {
                            state.GroupStarted = true;
                            continue;
                        }
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
                if(!state.GroupStarted) {
                    if(c == settings.PhaseSeparator) {
                        BuildPhase();
                        continue;
                    }
                }
                if(state.GroupStarted) {
                    bool validGroupEnd = next_c == null || next_c == settings.PhaseSeparator;
                    if(validGroupEnd && c == settings.GroupModificator) {
                        state.GroupFinished = true;
                        BuildPhase();
                        continue;
                    }
                }
                state.Phase += c;
            }
            BuildPhase();
        }

        void BuildPhase() {
            BuildPhaseCore();
            state.Flush();
        }
        void BuildPhaseCore() {
            if(state.Phase == string.Empty) {
                switch(state.PhaseMode) {
                    case PhaseMode.Default:
                        if(state.GroupStarted)
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
            if(state.GroupStarted && !state.GroupFinished)
                state.Phase = settings.GroupModificator + state.Phase;
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
