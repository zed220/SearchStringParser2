using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchStringParser {
    public class SearchStringParser2 {
        public SearchStringParseResult Parse(string searchText, SearchStringParseSettings settings) {
            SearchStringParseResult result = new SearchStringParseResult();
            var builder = new SearchStringPhaseBuilder(settings);
            if(searchText != null) {
                int i = 0;
                Func<char?> getNextChar = () => {
                    if(i < searchText.Length - 1)
                        return searchText[i + 1];
                    return null;
                };
                bool ignoreGrouping = false;
                for(i = 0; i < searchText.Length; i++) {
                    switch(builder.Add(searchText[i], getNextChar, i, ignoreGrouping)) {
                        case SearchStringParseState.Completed:
                            builder.ApplyAndFlush(result, ref i, ref ignoreGrouping);
                            break;
                    }
                }
            }
            return result;
        }
    }

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
                PhaseInfos.Clear();
            }

            public void StartGroup(int index) {
                if(GroupStarted)
                    throw new InvalidOperationException();
                GroupStarted = true;
                GroupStartIndex = PhaseMode == PhaseMode.Default ? index : index - 1;
                if(Field != null)
                    GroupStartIndex -= Field.Length + 1;
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
            public void Build(SearchStringParseSettings settings, bool addSeparator) {
                if(Phase == string.Empty && Field != null && !GroupFinished) {
                    Phase = Field + settings.SpecificFieldModificator;
                    Field = null;
                }
                if(GroupStarted && !GroupFinished)
                    Phase = settings.GroupModificator + Phase;
                if(Phase == String.Empty && GroupFinished) {
                    GroupStarted = false;
                    GroupFinished = false;
                    Phase = settings.GroupModificator.ToString() + settings.GroupModificator;
                }
                string phaseStr = String.Empty;
                SearchModificator modificator = SearchModificator.None;
                switch(PhaseMode) {
                    case PhaseMode.Exclude:
                        phaseStr += settings.ExcludeModificator;
                        if(Phase != string.Empty)
                            modificator = SearchModificator.Exclude;
                        break;
                    case PhaseMode.Include:
                        phaseStr += settings.IncludeModificator;
                        if(Phase != string.Empty)
                            modificator = SearchModificator.Include;
                        break;
                }
                if(Field != null) {
                    if(phaseStr != string.Empty)
                        PhaseInfos.Add(new PhaseInfo(phaseStr, modificator));
                    PhaseInfos.Add(new PhaseInfo(Field, SearchModificator.Field));
                    PhaseInfos.Add(new PhaseInfo(settings.SpecificFieldModificator.ToString()));
                    phaseStr = string.Empty;
                }
                if(GroupStarted && GroupFinished) {
                    BuildGroup(settings, addSeparator, phaseStr, modificator);
                    return;
                }
                phaseStr += Phase;
                if(addSeparator)
                    phaseStr += settings.PhaseSeparator;
                if(phaseStr == string.Empty)
                    return;
                PhaseInfos.Add(new PhaseInfo(phaseStr, modificator));
            }
            void BuildGroup(SearchStringParseSettings settings, bool addSeparator, string phaseStr, SearchModificator modificator) {
                if(phaseStr != String.Empty)
                    PhaseInfos.Add(new PhaseInfo(phaseStr, modificator));
                if(Phase != String.Empty) {
                    PhaseInfos.Add(new PhaseInfo(settings.GroupModificator.ToString(), SearchModificator.Group));
                    PhaseInfos.Add(new PhaseInfo(Phase, modificator, true));
                    PhaseInfos.Add(new PhaseInfo(settings.GroupModificator.ToString(), SearchModificator.Group));
                }
                else
                    PhaseInfos.Add(new PhaseInfo(settings.GroupModificator.ToString() + settings.GroupModificator, modificator));
                if(addSeparator)
                    PhaseInfos.Add(new PhaseInfo(settings.PhaseSeparator.ToString()));
                return;
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
                        if(state.Field == null) {
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
                }
                if(!state.GroupStarted) {
                    if(c == settings.PhaseSeparator) {
                        BuildPhase(true);
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
                        BuildPhase(false);
                        continue;
                    }
                }
                state.AddPhaseChar(c);
            }
            if(state.GroupStarted) {
                ParseString(searchText.Substring(state.GroupStartIndex), false);
                return;
            }
            BuildPhase(false);
        }

        void BuildPhase(bool addSeparator) {
            BuildPhaseCore(addSeparator);
            state.Flush();
        }
        void BuildPhaseCore(bool addSeparator) {
            state.Build(settings, addSeparator);
            result.PhaseInfos.AddRange(state.PhaseInfos);
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
