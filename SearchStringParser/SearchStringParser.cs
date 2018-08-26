using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchStringParser {
    public class SearchStringParser {

        #region Inner Types
        enum PhaseState { None, Include, Exclude }
        enum GroupingState { None, Started, Finished, Invalid }
        enum SearchStringParseState {
            Calculating, Completed
        }
        class ParsingState {
            readonly string searchString;

            int currentIndex = -1;
            int savedIndex = -1;

            public SearchStringParseSettings Settings { get; }
            public char? CurrentChar { get; private set; }
            public char? NextChar { get; private set; }

            public ParsingState(string searchString, SearchStringParseSettings settings) {
                this.searchString = searchString;
                Settings = settings;
            }
            public bool MoveNext() {
                currentIndex++;
                UpdateState();
                return CurrentChar.HasValue;
            }
            public void SaveIndex() {
                savedIndex = currentIndex;
            }
            public void RestoreIndex() {
                currentIndex = savedIndex - 1;
            }

            void UpdateState() {
                CurrentChar = GetChar(currentIndex);
                NextChar = GetChar(currentIndex + 1);
            }
            char? GetChar(int index) {
                if(index < searchString.Length)
                    return searchString[index];
                return null;
            }
        }
        #endregion
        #region Inner Fields

        SearchStringParseSettings settings;
        string phase = string.Empty;
        bool hasSpace = false;
        PhaseState phaseState = PhaseState.None;
        GroupingState groupState = GroupingState.None;
        string field = null;
        ParsingState state;

        #endregion


        public SearchStringParseResult Parse(string searchText, SearchStringParseSettings settings) {
            SearchStringParseResult result = new SearchStringParseResult();
            if(searchText == null)
                return result;
            this.settings = settings;
            state = new ParsingState(searchText, settings);
            while(state.MoveNext()) {
                switch(Add(state.CurrentChar.Value)) {
                    case SearchStringParseState.Completed:
                        ApplyAndFlush(result);
                        break;
                }
            }
            FlushAll();
            return result;
        }

        void ApplyAndFlush(SearchStringParseResult result) {
            if(DetectUnfinishedGroup()) {
                FlushForUnfinishedGroup();
                return;
            }
            BoundaryValues();
            var r = MakeResult();
            result.Regular.AddRange(r.Regular);
            result.Exclude.AddRange(r.Exclude);
            result.Include.AddRange(r.Include);
            result.PhaseInfos.AddRange(r.PhaseInfos);
            FlushAll();
        }
        bool DetectUnfinishedGroup() {
            if(groupState == GroupingState.Started) {
                state.RestoreIndex();
                return true;
            }
            return false;
        }

        SearchStringParseResult MakeResult() {
            var r = new SearchStringParseResult();
            FillSearchResult(r);
            FillPhases(r);
            return r;
        }
        void BoundaryValues() {
            if(groupState == GroupingState.Finished && phase == string.Empty) {
                groupState = GroupingState.None;
                phase = settings.GroupModificator.ToString() + settings.GroupModificator;
            }
            if(field != null && phase == string.Empty) {
                phase = field + settings.SpecificFieldModificator;
                field = null;
            }
            if(phase != string.Empty)
                return;
            switch(phaseState) {
                case PhaseState.Include:
                    phase += settings.IncludeModificator;
                    break;
                case PhaseState.Exclude:
                    phase += settings.ExcludeModificator;
                    break;
            }
            phaseState = PhaseState.None;
        }
        void FillSearchResult(SearchStringParseResult result) {
            if(phase == string.Empty)
                return;
            var info = new SearchStringParseInfo(phase, field);
            switch(phaseState) {
                case PhaseState.None:
                    result.Regular.Add(info);
                    break;
                case PhaseState.Include:
                    result.Include.Add(info);
                    break;
                case PhaseState.Exclude:
                    result.Exclude.Add(info);
                    break;
            }
        }
        void FillPhases(SearchStringParseResult result) {
            SearchModificator modificator = SearchModificator.None;
            string modificatorStr = string.Empty;
            switch(phaseState) {
                case PhaseState.Include:
                    modificator = SearchModificator.Include;
                    modificatorStr = settings.IncludeModificator.ToString();
                    break;
                case PhaseState.Exclude:
                    modificator = SearchModificator.Exclude;
                    modificatorStr = settings.ExcludeModificator.ToString();
                    break;
            }
            if(modificator != SearchModificator.None)
                result.PhaseInfos.Add(new PhaseInfo(modificatorStr, modificator));
            result.PhaseInfos.AddRange(GetFieldPhase());
            result.PhaseInfos.AddRange(GetPhase(modificator));
            if(hasSpace)
                result.PhaseInfos.Add(new PhaseInfo(settings.PhaseSeparator.ToString()));
        }
        List<PhaseInfo> GetFieldPhase() {
            var result = new List<PhaseInfo>();
            if(field == null)
                return result;
            result.Add(new PhaseInfo(field, SearchModificator.Field));
            result.Add(new PhaseInfo(settings.SpecificFieldModificator.ToString()));
            return result;
        }
        List<PhaseInfo> GetPhase(SearchModificator modificator) {
            var result = new List<PhaseInfo>();
            if(groupState == GroupingState.Finished)
                result.Add(new PhaseInfo(settings.GroupModificator.ToString(), SearchModificator.Group));
            if(phase != string.Empty)
                result.Add(new PhaseInfo(phase, modificator, groupState == GroupingState.Finished));
            if(groupState == GroupingState.Finished)
                result.Add(new PhaseInfo(settings.GroupModificator.ToString(), SearchModificator.Group));
            return result;
        }

        void FlushForUnfinishedGroup() {
            phase = string.Empty;
            hasSpace = false;
            groupState = GroupingState.Invalid;
        }
        void FlushAll() {
            FlushForUnfinishedGroup();
            groupState = GroupingState.None;
            phaseState = PhaseState.None;
            field = null;
        }

        SearchStringParseState Add(char c) {
            if(c == settings.PhaseSeparator) {
                if(IsPhaseEnded()) {
                    hasSpace = true;
                    return SearchStringParseState.Completed;
                }
            }
            if(phase == string.Empty && groupState == GroupingState.None) {
                if(phaseState == PhaseState.None && field == null) {
                    if(c == settings.IncludeModificator) {
                        phaseState = PhaseState.Include;
                        return GetState();
                    }
                    if(c == settings.ExcludeModificator) {
                        phaseState = PhaseState.Exclude;
                        return GetState();
                    }
                }
                if(c == settings.GroupModificator && groupState == GroupingState.None) {
                    groupState = GroupingState.Started;
                    state.SaveIndex();
                    return GetState();
                }
            }
            if(c == settings.GroupModificator && groupState == GroupingState.Started) {
                if(!state.NextChar.HasValue || state.NextChar.Value == settings.PhaseSeparator) {
                    groupState = GroupingState.Finished;
                    return SearchStringParseState.Completed;
                }
            }
            if(c == settings.SpecificFieldModificator && field == null && phase != string.Empty && groupState == GroupingState.None) {
                field = phase;
                phase = string.Empty;
                return GetState();
            }
            phase += c;
            return GetState();
        }
        SearchStringParseState GetState() {
            return state.NextChar.HasValue ? SearchStringParseState.Calculating : SearchStringParseState.Completed;
        }

        bool IsPhaseEnded() {
            if(groupState == GroupingState.Started)
                return false;
            return true;
        }
    }
}
