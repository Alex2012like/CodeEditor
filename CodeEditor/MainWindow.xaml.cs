using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System.Xml;
using ICSharpCode.AvalonEdit.Document;
using System.Windows.Input;
using System.Reflection;
using ICSharpCode.AvalonEdit.Editing;

namespace CodeEditor
{
    public partial class MainWindow : Window
    {
        private string currentFilePath;

        public MainWindow()
        {
            InitializeComponent();
            InitializeHighlighting();
            CodeEditor.TextArea.Caret.PositionChanged += CaretPositionChanged;

            CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, SaveCommandExecuted, SaveCommandCanExecute));
        }

        private void InitializeHighlighting()
        {
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("CodeEditor.Resources.CSharp-Mode.xshd"))
            {
                if (s != null)
                {
                    using (XmlReader reader = new XmlTextReader(s))
                    {
                        CodeEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                    }
                }
            }
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                currentFilePath = openFileDialog.FileName;
                CodeEditor.Text = File.ReadAllText(currentFilePath);
                FileTabControl.Items.Clear();
                TabItem tabItem = new TabItem();
                tabItem.Header = System.IO.Path.GetFileName(currentFilePath);
                tabItem.Content = CodeEditor;
                FileTabControl.Items.Add(tabItem);
            }
        }

        public void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(currentFilePath))
            {
                File.WriteAllText(currentFilePath, CodeEditor.Text);
            }
            else
            {
                SaveAsFile_Click(sender, e);
            }
        }

        private void SaveAsFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                currentFilePath = saveFileDialog.FileName;
                File.WriteAllText(currentFilePath, CodeEditor.Text);
                FileTabControl.Items.Clear();
                TabItem tabItem = new TabItem();
                tabItem.Header = System.IO.Path.GetFileName(currentFilePath);
                tabItem.Content = CodeEditor;
                FileTabControl.Items.Add(tabItem);
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void CaretPositionChanged(object sender, EventArgs e)
        {
            int line = CodeEditor.TextArea.Caret.Line;
            int column = CodeEditor.TextArea.Caret.Column;
            StatusBarInfo.Content = $"Row: {line}, Column: {column}";
        }

        private void SaveCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFile_Click(sender, e);
        }

        private void SaveCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
    }
}