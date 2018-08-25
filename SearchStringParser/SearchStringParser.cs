using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchStringParser {
    public class SearchStringParser {
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
}
