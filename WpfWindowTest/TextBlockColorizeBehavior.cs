using SearchStringParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interactivity;

namespace WpfWindowTest {
    public class TextBlockColorizeBehavior : Behavior<TextBlock> {
        public static readonly DependencyProperty SourceTextBoxProperty = 
            DependencyProperty.Register("SourceTextBox", typeof(TextBox), typeof(TextBlockColorizeBehavior), new PropertyMetadata(null));

        public Style RunRegularStyle { get; set; }
        public Style RunIncludeStyle { get; set; }
        public Style RunExcludeStyle { get; set; }
        public Style RunFieldStyle { get; set; }

        public TextBox SourceTextBox {
            get { return (TextBox)GetValue(SourceTextBoxProperty); }
            set { SetValue(SourceTextBoxProperty, value); }
        }

        protected override void OnAttached() {
            base.OnAttached();
            SourceTextBox.TextChanged += SourceTextBox_TextChanged;
        }
        protected override void OnDetaching() {
            base.OnDetaching();
            SourceTextBox.TextChanged -= SourceTextBox_TextChanged;
        }

        void SourceTextBox_TextChanged(object sender, TextChangedEventArgs e) {
            AssociatedObject.Inlines.Clear();
            SearchStringParseResult result = SearchStringParser.SearchStringParser.Parse(SourceTextBox.Text, SearchStringParseSettings.Default);
            foreach(var info in result.PhaseInfos) {
                var run = new Run(info.Text);
                switch(info.Modificator) {
                    case SearchModificator.None:
                        run.Style = RunRegularStyle;
                        break;
                    case SearchModificator.Include:
                        run.Style = RunIncludeStyle;
                        break;
                    case SearchModificator.Exclude:
                        run.Style = RunExcludeStyle;
                        break;
                    case SearchModificator.Field:
                        run.Style = RunFieldStyle;
                        break;
                }
                AssociatedObject.Inlines.Add(run);
            }
        }
    }
}