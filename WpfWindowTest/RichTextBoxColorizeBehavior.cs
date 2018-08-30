using SearchStringParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace WpfWindowTest {
    public class RichTextBoxColorizeBehavior : Behavior<RichTextBox> {
        public Style RunRegularStyle { get; set; }
        public Style RunIncludeStyle { get; set; }
        public Style RunExcludeStyle { get; set; }
        public Style RunRegularGroupStyle { get; set; }
        public Style RunIncludeGroupStyle { get; set; }
        public Style RunExcludeGroupStyle { get; set; }
        public Style RunGroupQuoteStyle { get; set; }
        public Style RunFieldStyle { get; set; }

        protected override void OnAttached() {
            base.OnAttached();
            AssociatedObject.PreviewKeyDown += AssociatedObject_PreviewKeyDown;
            AssociatedObject.TextChanged += AssociatedObject_TextChanged;
        }

        bool locked = false;
        Paragraph paragraph { get { return AssociatedObject.Document.Blocks.FirstBlock as Paragraph; } }

        void AssociatedObject_TextChanged(object sender, TextChangedEventArgs e) {
            if(locked)
                return;
            if(paragraph == null)
                return;
            if(paragraph.Inlines.Count == 0)
                return;
            locked = true;
            List<Inline> inlines = new List<Inline>();
            StringBuilder sb = new StringBuilder();
            foreach(Run inline in paragraph.Inlines) {
                sb.Append(inline.Text);
            }
            SearchStringParseResult result = new SearchStringParser.SearchStringParser().Parse(sb.ToString(), SearchStringParseSettings.Default);
            paragraph.Inlines.Clear();
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
                        run.Style = info.Grouped ? RunExcludeGroupStyle : RunExcludeStyle;
                        break;
                    case SearchModificator.Field:
                        run.Style = RunFieldStyle;
                        break;
                    case SearchModificator.Group:
                        run.Style = RunGroupQuoteStyle;
                        break;
                }
                paragraph.Inlines.Add(run);
            }
            AssociatedObject.CaretPosition = AssociatedObject.Document.ContentEnd;
            locked = false; 
        }

        void AssociatedObject_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            if(e.Key == Key.Enter)
                e.Handled = true;
        }
    }
}
