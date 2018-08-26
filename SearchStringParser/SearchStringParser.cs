using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchStringParser {
    public class SearchStringParser {
        public SearchStringParseResult Parse(string searchText, SearchStringParseSettings settings) {
            SearchStringParseResult result = new SearchStringParseResult();
            if(searchText == null)
                return result;
            var builder = new SearchStringPhaseBuilder(settings);
            int i = 0;
            bool ignoreGrouping = false;
            Func<char?> getNextChar = () => {
                if(i < searchText.Length - 1)
                    return searchText[i + 1];
                return null;
            };
            for(i = 0; i < searchText.Length; i++) {
                switch(builder.Add(searchText[i], getNextChar, i, ignoreGrouping)) {
                    case SearchStringParseState.Completed:
                        builder.ApplyAndFlush(result, ref i, ref ignoreGrouping);
                        break;
                }
            }
            return result;
        }
    }
}
