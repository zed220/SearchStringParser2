using System;
using System.Collections.Generic;

namespace SearchStringParser {
    public class PhaseInfo {
        public PhaseInfo(string text, SearchModificator modificator = SearchModificator.None) {
            if(String.IsNullOrEmpty(text))
                throw new InvalidOperationException();
            Text = text;
            Modificator = modificator;
        }

        public string Text { get; }
        public SearchModificator Modificator { get; }

        public override bool Equals(object obj) {
            var info = obj as PhaseInfo;
            return info != null &&
                   Text == info.Text &&
                   Modificator == info.Modificator;
        }

        public override int GetHashCode() {
            var hashCode = -2137812222;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Text);
            hashCode = hashCode * -1521134295 + Modificator.GetHashCode();
            return hashCode;
        }
    }
}
