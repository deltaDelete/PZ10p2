using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;

namespace PZ10p2 {


    public class MainWindowViewModel : ViewModelBase {
        #region Команды

        public ICommand ItalicFontCommand { get; }
        public ICommand BoldFontCommand { get; }
        public ICommand OpenFileCommand { get; }
        public ICommand SaveFileCommand { get; }
        public ICommand UnderlineFontCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand AboutCommand { get; }
        public ICommand LineSpacingCommand { get; }

        #endregion

        public MainWindowViewModel() {
            OpenFileCommand = new Command(OpenFile);
            SaveFileCommand = new Command(SaveFile);
            BoldFontCommand = new Command(SetBold);
            ItalicFontCommand = new Command(SetItalic);
            UnderlineFontCommand = new Command(SetUnderline);
            AboutCommand = new Command(About);
            ExitCommand = new Command(() => Application.Current.Shutdown(0));
            LineSpacingCommand = new Command<string>(SetLineSpacing);
        }


        #region Свойства

        public RichTextBox RichTBox { get; set; }

        public List<FontFamily> FontFamilies { get; } = Fonts.SystemFontFamilies.OrderBy(f => f.Source).ToList();
        public List<double> FontSizes { get; } = Enumerable.Range(1, 72).Select(x => (double)x).ToList();
        public List<System.Reflection.PropertyInfo> FontColors { get; } = typeof(Colors)
            .GetProperties().ToList();

        public double SelectedFontSize {
            get => _selectedFontSize;
            set {
                SetPropertyIfChanged(ref _selectedFontSize, value);
                SetFontSize();
            }
        }

        public System.Reflection.PropertyInfo SelectedFontColor {
            get => _selectedFontColor;
            set {
                SetPropertyIfChanged(ref _selectedFontColor, value);
                SetColor();
            }
        }

        public FontFamily SelectedFontFamily {
            get => _selectedFontFamily;
            set {
                SetPropertyIfChanged(ref _selectedFontFamily, value);
                SetFontFamily();
            }
        }

        #endregion

        #region Поля

        private double _selectedFontSize;
        private FontFamily _selectedFontFamily;
        private System.Reflection.PropertyInfo _selectedFontColor;

        #endregion

        #region Методы

        private void SaveFile() {
            var dialog = new SaveFileDialog();
            dialog.Filter = "Text Files (*.txt)|*.txt|RichText Files (*.rtf)|*.rtf|XAML Files (*.xaml)|*.xaml|All files (*.*)|*.*";

            var result = dialog.ShowDialog();

            if (!(result.HasValue && result.Value)) {
                return;
            }

            var doc = new TextRange(RichTBox.Document.ContentStart, RichTBox.Document.ContentEnd);

            using var fs = File.Create(dialog.FileName);
            if (Path.GetExtension(dialog.FileName).ToLower() == ".rtf") {
                doc.Save(fs, DataFormats.Rtf);
            }
            else if (Path.GetExtension(dialog.FileName).ToLower() == ".txt") {
                doc.Save(fs, DataFormats.Text);
            }
            else {
                doc.Save(fs, DataFormats.Xaml);
            }
        }

        private void OpenFile() {
            var dialog = new OpenFileDialog();
            dialog.Filter = "RichText Files (*.rtf)|*.rtf|All files (*.*)|*.*";

            var result = dialog.ShowDialog();

            if (!(result.HasValue && result.Value)) {
                return;
            }

            var doc = new TextRange(RichTBox.Document.ContentStart, RichTBox.Document.ContentEnd);
            using var fs = new FileStream(dialog.FileName, FileMode.Open);
            if (Path.GetExtension(dialog.FileName).ToLower() == ".rtf") {
                doc.Load(fs, DataFormats.Rtf);
            }
            else if (Path.GetExtension(dialog.FileName).ToLower() == ".txt") {
                doc.Load(fs, DataFormats.Text);
            }
            else {
                doc.Load(fs, DataFormats.Xaml);
            }
        }

        private void SetBold() {
            var selected = RichTBox.Selection;
            if (selected.IsEmpty) {
                return;
            }

            FontWeight currentFontWeight = selected.GetPropertyValue(Inline.FontWeightProperty) is FontWeight weight
                ? weight
                : FontWeights.Normal;

            var newFontWeight = currentFontWeight == FontWeights.Bold ? FontWeights.Normal : FontWeights.Bold;

            selected.ApplyPropertyValue(Inline.FontWeightProperty, newFontWeight);
        }

        private void SetItalic() {
            var selected = RichTBox.Selection;
            if (selected.IsEmpty) {
                return;
            }

            FontStyle currentFontStyle = selected.GetPropertyValue(Inline.FontStyleProperty) is FontStyle style
                ? style
                : FontStyles.Italic;

            var newFontStyle = currentFontStyle == FontStyles.Italic
                ? FontStyles.Normal
                : FontStyles.Italic;

            selected.ApplyPropertyValue(Inline.FontStyleProperty, newFontStyle);
        }

        private void SetUnderline() {
            var selected = RichTBox.Selection;
            if (selected.IsEmpty) {
                return;
            }

            TextDecorationCollection currentTextDecoration = selected.GetPropertyValue(Inline.TextDecorationsProperty) is TextDecorationCollection td
                ? td
                : TextDecorations.Underline;

            var newTextDecoration = currentTextDecoration.Any(x => x.Location == TextDecorationLocation.Underline)
                ? new TextDecorationCollection()
                : TextDecorations.Underline;

            selected.ApplyPropertyValue(Inline.TextDecorationsProperty, newTextDecoration);
        }

        private void SetFontSize() {
            var selected = RichTBox.Selection;
            if (selected.IsEmpty) {
                return;
            }

            selected.ApplyPropertyValue(Inline.FontSizeProperty, _selectedFontSize);
        }

        private void SetFontFamily() {
            var selected = RichTBox.Selection;
            if (selected.IsEmpty) {
                return;
            }

            selected.ApplyPropertyValue(Inline.FontFamilyProperty, _selectedFontFamily);
        }

        private void About() {
            MessageBox.Show("Автор: Воронин Роман Витальевич группа ИС(ПРО)-31\nРазработано в рамках курса МДК 01.01");
        }

        private void SetColor() {
            var selected = RichTBox.Selection;
            if (selected.IsEmpty) {
                return;
            }

            selected.ApplyPropertyValue(Inline.ForegroundProperty, _selectedFontColor.Name);
        }

        private void SetLineSpacing(string height) {
            var selected = RichTBox.Selection;
            if (selected.IsEmpty) {
                return;
            }

            var heightValue = double.Parse(height.Replace('.', ','));

            MessageBox.Show(selected.GetPropertyValue(TextElement.FontSizeProperty).ToString());

            heightValue *= (double)selected.GetPropertyValue(TextElement.FontSizeProperty) * 1.3;

            selected.ApplyPropertyValue(Paragraph.LineHeightProperty, heightValue);
        }

        #endregion
    }
}