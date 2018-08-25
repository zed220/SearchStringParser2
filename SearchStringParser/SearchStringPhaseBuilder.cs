using System;

namespace SearchStringParser {
    class SearchStringPhaseBuilder {
        readonly SearchStringParseSettings settings;

        string phase = string.Empty;
        bool hasSpace = false;
        bool include = false;
        bool exclude = false;

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
            if(phase != String.Empty)
                result.PhaseInfos.Add(new PhaseInfo(phase, modificator));
            if(hasSpace)
                result.PhaseInfos.Add(new PhaseInfo(settings.PhaseSeparator.ToString()));
        }

        void Flush() {
            phase = string.Empty;
            hasSpace = false;
            include = false;
            exclude = false;
        }

        public SearchStringParseState Add(char c) {
            if(c == settings.PhaseSeparator) {
                if(IsPhaseEnded()) {
                    hasSpace = true;
                    return SearchStringParseState.Completed;
                }
            }
            if(phase == string.Empty) {
                if(c == settings.IncludeModificator) {
                    include = true;
                    return SearchStringParseState.Calculating;
                }
                if(c == settings.ExcludeModificator) {
                    exclude = true;
                    return SearchStringParseState.Calculating;
                }
            }
            phase += c;
            return SearchStringParseState.Calculating;
        }

        bool IsPhaseEnded() {
            return true;
        }
    }
}
