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

            public string Phase { get; private set; }
            public PhaseMode PhaseMode { get; private set; }
            public string Field { get; private set; }
            public bool GroupStarted { get; private set; }
            public int GroupStartIndex { get; private set; }
            public List<PhaseInfo> PhaseInfos { get; private set; } = new List<PhaseInfo>();

            public bool GroupFinished;

            public void Flush() {
                Phase = String.Empty;
                Field = null;
                PhaseMode = PhaseMode.Default;
                GroupStarted = false;
                GroupFinished = false;
            }

            public void StartGroup(int index) {
                if(GroupStarted)
                    throw new InvalidOperationException();
                GroupStarted = true;
                GroupStartIndex = PhaseMode == PhaseMode.Default ? index : index - 1;
                if(index < 0)
                    throw new InvalidOperationException();
            }
            public void AddPhaseChar(char c) {
                Phase += c;
            }
            public void SetPhaseMode(PhaseMode mode, char c) {
                if(PhaseMode != PhaseMode.Default)
                    throw new InvalidOperationException();
                if(mode == PhaseMode.Default)
                    throw new InvalidOperationException();
                PhaseMode = mode;
            }
            public void SetSpecificField(char c) {
                if(Phase == String.Empty) {
                    AddPhaseChar(c);
                    return;
                }
                Field = Phase;
                Phase = String.Empty;
            }
            public void Build(SearchStringParseSettings settings) {
                if(Phase == string.Empty) {
                    if(Field != null) {
                        Phase = Field + settings.SpecificFieldModificator;
                        Field = null;
                    }
                }
                if(GroupStarted && !GroupFinished)
                    Phase = settings.GroupModificator + Phase;
            }

        }

        #endregion

        readonly SearchStringParseSettings settings;
        readonly SearchStringParseResult result = new SearchStringParseResult();

        ParsingState state;

        SearchStringParser(SearchStringParseSettings settings) {
            this.settings = settings;
        }

        public static SearchStringParseResult Parse(string text, SearchStringParseSettings settings) {
            if(String.IsNullOrEmpty(text))
                return new SearchStringParseResult();
            if(String.IsNullOrWhiteSpace(text)) {
                var result = new SearchStringParseResult();
                result.PhaseInfos.Add(new PhaseInfo(text));
                return result;
            }
            var parser = new SearchStringParser(settings);
            parser.ParseString(text);
            return parser.result;
        }

        void ParseString(string searchText) {
            ParseString(searchText, true);
        }

        void ParseString(string searchText, bool ckeckGroup) {
            state = new ParsingState();
            for(int i = 0; i < searchText.Length; i++) {
                char c = searchText[i];
                char? next_c = i < searchText.Length - 1 ? (char?)searchText[i + 1] : null;
                if(state.Phase == string.Empty) {
                    if(!state.GroupStarted) {
                        if(ckeckGroup && c == settings.GroupModificator) {
                            state.StartGroup(i);
                            continue;
                        }
                        if(state.PhaseMode == PhaseMode.Default) {
                            if(c == settings.ExcludeModificator) {
                                state.SetPhaseMode(PhaseMode.Exclude, c);
                                continue;
                            }
                            if(c == settings.IncludeModificator) {
                                state.SetPhaseMode(PhaseMode.Include, c);
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
                        state.SetSpecificField(c);
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
                state.AddPhaseChar(c);
            }
            if(state.GroupStarted) {
                ParseString(searchText.Substring(state.GroupStartIndex), false);
                return;
            }
            BuildPhase();
        }

        void BuildPhase() {
            BuildPhaseCore();
            BuildPhaseInfos();
            state.Flush();
        }
        void BuildPhaseInfos() {
            result.PhaseInfos.AddRange(state.PhaseInfos);
        }
        void BuildPhaseCore() {
            state.Build(settings);
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
