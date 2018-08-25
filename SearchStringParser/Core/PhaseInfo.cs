﻿using System;
using System.Collections.Generic;

namespace SearchStringParser {
    public class PhaseInfo {
        public PhaseInfo(string text, SearchModificator modificator = SearchModificator.None, bool grouped = false) {
            if(String.IsNullOrEmpty(text))
                throw new ArgumentNullException();
            Text = text;
            Modificator = modificator;
            Grouped = grouped;
        }

        public string Text { get; }
        public SearchModificator Modificator { get; }
        public bool Grouped { get; }

        public override bool Equals(object obj) {
            var info = obj as PhaseInfo;
            return info != null &&
                   Text == info.Text &&
                   Modificator == info.Modificator &&
                   Grouped == info.Grouped;
        }
    }
}
