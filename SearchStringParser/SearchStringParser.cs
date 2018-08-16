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
            public string Field;

            public void Flush() {
                Phase = String.Empty;
                Field = null;
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
            ParseString(searchText, true);
        }

        void ParseString(string searchText, bool ckeckGroup) {
            for(int i = 0; i < searchText.Length; i++) {
                char c = searchText[i];
                char? next_c = i < searchText.Length - 1 ? (char?)searchText[i + 1] : null;
                if(state.Phase == string.Empty) {
                    if(!state.GroupStarted) {
                        if(ckeckGroup && c == settings.GroupModificator) {
                            state.GroupStarted = true;
                            continue;
                        }
                        if(state.PhaseMode == PhaseMode.Default) {
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
                }
                if(!state.GroupStarted) {
                    if(c == settings.PhaseSeparator) {
                        BuildPhase();
                        continue;
                    }
                    if(c == settings.SpecificFieldModificator) {
                        state.Field = state.Phase;
                        state.Phase = string.Empty;
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
            if(state.GroupStarted) {
                state.Flush();
                ParseString(searchText, false);
                return;
            }
            BuildPhase();
        }

        void BuildPhase() {
            BuildPhaseCore();
            state.Flush();
        }
        void BuildPhaseCore() {
            if(state.Phase == string.Empty) {
                if(state.Field != null) {
                    state.Phase = state.Field + settings.SpecificFieldModificator;
                    state.Field = null;
                }
            }
            if(state.Phase == string.Empty) {
                switch(state.PhaseMode) {
                    case PhaseMode.Default:
                        if(state.GroupStarted)
                            AddInfo(result.Regular, settings.GroupModificator.ToString(), state.Field);
                        return;
                    case PhaseMode.Exclude:
                        AddInfo(result.Regular, settings.ExcludeModificator.ToString(), state.Field);
                        break;
                    case PhaseMode.Include:
                        AddInfo(result.Regular, settings.IncludeModificator.ToString(), state.Field);
                        break;
                }
                return;
            }
            if(state.GroupStarted && !state.GroupFinished)
                state.Phase = settings.GroupModificator + state.Phase;
            switch(state.PhaseMode) {
                case PhaseMode.Default:
                    AddInfo(result.Regular, state.Phase, state.Field);
                    break;
                case PhaseMode.Exclude:
                    AddInfo(result.Exclude, state.Phase, state.Field);
                    break;
                case PhaseMode.Include:
                    AddInfo(result.Include, state.Phase, state.Field);
                    break;
            }
        }

        void AddInfo(List<SearchStringParseInfo> target, string phase, string field) {
            target.Add(new SearchStringParseInfo(phase, field));
        }
    }
}
