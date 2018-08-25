using System;

namespace SearchStringParser {
    class SearchStringPhaseBuilder {
        readonly SearchStringParseSettings settings;

        string phase = string.Empty;
        bool hasSpace = false;
        bool include = false;

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
        }
        void FillSearchResult(SearchStringParseResult result) {
            if(phase == String.Empty)
                return;
            if(include)
                result.Include.Add(new SearchStringParseInfo(phase));
            else
                result.Regular.Add(new SearchStringParseInfo(phase));
        }
        void FillPhases(SearchStringParseResult result) {
            if(include)
                result.PhaseInfos.Add(new PhaseInfo(settings.IncludeModificator.ToString(), SearchModificator.Include));
            if(phase != String.Empty)
                result.PhaseInfos.Add(new PhaseInfo(phase, include ? SearchModificator.Include : SearchModificator.None));
            if(hasSpace)
                result.PhaseInfos.Add(new PhaseInfo(settings.PhaseSeparator.ToString()));
        }

        void Flush() {
            phase = string.Empty;
            hasSpace = false;
            include = false;
        }

        public SearchStringParseState Add(char c) {
            if(c == settings.PhaseSeparator) {
                if(IsPhaseEnded()) {
                    hasSpace = true;
                    return SearchStringParseState.Completed;
                }
            }
            if(c == settings.IncludeModificator) {
                if(phase == string.Empty) {
                    include = true;
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
