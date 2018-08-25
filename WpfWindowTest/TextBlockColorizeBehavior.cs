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
        public static readonly DependencyProperty ResultListBoxProperty =
            DependencyProperty.Register("ResultListBox", typeof(ListBox), typeof(TextBlockColorizeBehavior), new PropertyMetadata(null));

        public Style RunRegularStyle { get; set; }
        public Style RunIncludeStyle { get; set; }
        public Style RunExcludeStyle { get; set; }
        public Style RunRegularGroupStyle { get; set; }
        public Style RunIncludeGroupStyle { get; set; }
        public Style RunExcludeGroupStyle { get; set; }
        public Style RunGroupQuoteStyle { get; set; }
        public Style RunFieldStyle { get; set; }

        public TextBox SourceTextBox {
            get { return (TextBox)GetValue(SourceTextBoxProperty); }
            set { SetValue(SourceTextBoxProperty, value); }
        }
        public ListBox ResultListBox {
            get { return (ListBox)GetValue(ResultListBoxProperty); }
            set { SetValue(ResultListBoxProperty, value); }
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
            SearchStringParseResult result = new SearchStringParser.SearchStringParser().Parse(SourceTextBox.Text, SearchStringParseSettings.Default);
            foreach(var info in result.PhaseInfos) {
                var run = new Run(info.Text);
                switch(info.Modificator) {
                    case SearchModificator.None:
                        run.Style = info.Grouped ? RunRegularGroupStyle : RunRegularStyle;
                        break;
                    case SearchModificator.Include:
                        run.Style = info.Grouped ? RunIncludeGroupStyle : RunIncludeStyle;
                        break;
                    case SearchModificator.Exclude:
                        run.Style = info.Grouped ?  RunExcludeGroupStyle : RunExcludeStyle;
                        break;
                    case SearchModificator.Field:
                        run.Style = RunFieldStyle;
                        break;
                    case SearchModificator.Group:
                        run.Style = RunGroupQuoteStyle;
                        break;
                }
                AssociatedObject.Inlines.Add(run);
                ResultListBox.ItemsSource = result.PhaseInfos;
            }
        }
    }
}