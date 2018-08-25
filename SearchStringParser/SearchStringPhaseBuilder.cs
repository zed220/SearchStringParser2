using System;
using System.Collections.Generic;

namespace SearchStringParser {
    class SearchStringPhaseBuilder {
        readonly SearchStringParseSettings settings;

        string phase = string.Empty;
        bool hasSpace = false;
        bool include = false;
        bool exclude = false;
        bool groupStarted = false;
        bool groupFinished = false;
        int groupIndex = -1;
        string field = null;

        public SearchStringPhaseBuilder(SearchStringParseSettings settings) {
            this.settings = settings;
            FlushAll();
        }

        public void ApplyAndFlush(SearchStringParseResult result, ref int index, ref bool ignoreGrouping) {
            if(DetectUnfinishedGroup(ref index, ref ignoreGrouping)) {
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
        bool DetectUnfinishedGroup(ref int index, ref bool ignoreGrouping) {
            if(groupStarted && !groupFinished) {
                index = groupIndex - 1;
                ignoreGrouping = true;
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
            if(include && phase == string.Empty) {
                include = false;
                phase += settings.IncludeModificator;
            }
            if(exclude && phase == string.Empty) {
                exclude = false;
                phase += settings.ExcludeModificator;
            }
            if(groupStarted && groupFinished && phase == string.Empty) {
                groupStarted = false;
                groupFinished = false;
                phase = settings.GroupModificator.ToString() + settings.GroupModificator;
            }
            if(field != null && phase == string.Empty) {
                phase = field + settings.SpecificFieldModificator;
                field = null;
            }
        }
        void FillSearchResult(SearchStringParseResult result) {
            if(phase == String.Empty)
                return;
            if(include)
                result.Include.Add(new SearchStringParseInfo(phase, field));
            else if(exclude)
                result.Exclude.Add(new SearchStringParseInfo(phase, field));
            else
                result.Regular.Add(new SearchStringParseInfo(phase, field));
        }
        void FillPhases(SearchStringParseResult result) {
            SearchModificator modificator = SearchModificator.None;
            string modificatorStr = string.Empty;
            if(include) {
                modificator = SearchModificator.Include;
                modificatorStr = settings.IncludeModificator.ToString();
            }
            if(exclude) {
                modificator = SearchModificator.Exclude;
                modificatorStr = settings.ExcludeModificator.ToString();
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
            bool grouped = groupStarted && groupFinished;
            if(groupStarted && groupFinished)
                result.Add(new PhaseInfo(settings.GroupModificator.ToString(), SearchModificator.Group));
            if(phase != string.Empty)
                result.Add(new PhaseInfo(phase, modificator, grouped));
            if(groupStarted && groupFinished)
                result.Add(new PhaseInfo(settings.GroupModificator.ToString(), SearchModificator.Group));
            return result;
        }

        void FlushForUnfinishedGroup() {
            phase = string.Empty;
            hasSpace = false;
            groupStarted = false;
            groupFinished = false;
            groupIndex = -1;
        }
        void FlushAll() {
            FlushForUnfinishedGroup();
            include = false;
            exclude = false;
            field = null;
        }

        public SearchStringParseState Add(char c, Func<char?> tryGetNextChar, int index, bool ignoreGrouping) {
            char? nextC = tryGetNextChar();
            if(c == settings.PhaseSeparator) {
                if(IsPhaseEnded()) {
                    hasSpace = true;
                    return SearchStringParseState.Completed;
                }
            }
            if(phase == string.Empty && !groupStarted) {
                if(!include && !exclude) {
                    if(c == settings.IncludeModificator) {
                        include = true;
                        return GetState(nextC);
                    }
                    if(c == settings.ExcludeModificator) {
                        exclude = true;
                        return GetState(nextC);
                    }
                }
                if(!ignoreGrouping && c == settings.GroupModificator && !groupStarted) {
                    groupStarted = true;
                    groupIndex = index;
                    return GetState(nextC);
                }
            }
            if(groupStarted && c == settings.GroupModificator) {
                if(!nextC.HasValue || nextC.Value == settings.PhaseSeparator) {
                    groupFinished = true;
                    return SearchStringParseState.Completed;
                }
            }
            if(phase != string.Empty && c == settings.SpecificFieldModificator) {
                field = phase;
                phase = string.Empty;
                return GetState(nextC);
            }
            phase += c;
            return GetState(nextC);
        }
        SearchStringParseState GetState(char? nextC) {
            return nextC.HasValue ? SearchStringParseState.Calculating : SearchStringParseState.Completed;
        }

        bool IsPhaseEnded() {
            if(groupStarted && !groupFinished)
                return false;
            return true;
        }
    }
}
