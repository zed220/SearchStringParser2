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

        public SearchStringPhaseBuilder(SearchStringParseSettings settings) {
            this.settings = settings;
            Flush();
        }

        public void ApplyAndFlush(SearchStringParseResult result) {
            var r = MakeResult();
            result.Regular.AddRange(r.Regular);
            result.Exclude.AddRange(r.Exclude);
            result.Include.AddRange(r.Include);
            result.PhaseInfos.AddRange(r.PhaseInfos);
            Flush();
        }

        SearchStringParseResult MakeResult() {
            BoundaryValues();
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
        }
        void FillSearchResult(SearchStringParseResult result) {
            if(phase == String.Empty)
                return;
            if(include)
                result.Include.Add(new SearchStringParseInfo(phase));
            else if(exclude)
                result.Exclude.Add(new SearchStringParseInfo(phase));
            else
                result.Regular.Add(new SearchStringParseInfo(phase));
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
            result.PhaseInfos.AddRange(GetPhase(modificator));
            if(hasSpace)
                result.PhaseInfos.Add(new PhaseInfo(settings.PhaseSeparator.ToString()));
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

        void Flush() {
            phase = string.Empty;
            hasSpace = false;
            include = false;
            exclude = false;
            groupStarted = false;
            groupFinished = false;
        }

        public SearchStringParseState Add(char c, Func<char?> tryGetNextChar) {
            if(c == settings.PhaseSeparator) {
                if(IsPhaseEnded()) {
                    hasSpace = true;
                    return SearchStringParseState.Completed;
                }
            }
            if(phase == string.Empty && !groupStarted) {
                if(c == settings.IncludeModificator) {
                    include = true;
                    return SearchStringParseState.Calculating;
                }
                if(c == settings.ExcludeModificator) {
                    exclude = true;
                    return SearchStringParseState.Calculating;
                }
                if(c == settings.GroupModificator) {
                    if(!groupStarted) {
                        groupStarted = true;
                        return SearchStringParseState.Calculating;
                    }
                }
            }
            if(c == settings.GroupModificator) {
                char? nextC = tryGetNextChar();
                if(!nextC.HasValue || nextC.Value == settings.PhaseSeparator) {
                    groupFinished = true;
                    return SearchStringParseState.Completed;
                }
            }
            phase += c;
            return SearchStringParseState.Calculating;
        }

        bool IsPhaseEnded() {
            if(groupStarted && !groupFinished)
                return false;
            return true;
        }
    }
}
