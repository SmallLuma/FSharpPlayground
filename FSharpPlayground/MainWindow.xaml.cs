﻿using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FSharpPlayground
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : SourceChord.FluentWPF.AcrylicWindow
    {
        const string fileFilter = 
            "F# Code (*.fs;*.fsx)|*.fs;*.fsx|F# Source Code (*.fs)|*.fs|F# Script (*.fsx)|*.fsx|All Files (*.*)|*.*";

        public MainWindow()
        {
            InitializeComponent();

            // 读取设置
            Width = Settings.Default.WindowWidth;
            Height = Settings.Default.WindowHeight;
            editorWidthWhenOutputShown = Settings.Default.EditorWidth;
            FSharpEditor.Text = Settings.Default.Code;

            // 写入设置
            Closed += (o, e) =>
            {
                Settings.Default.WindowWidth = Width;
                Settings.Default.WindowHeight = Height;
                Settings.Default.EditorWidth = editorWidthWhenOutputShown.GetValueOrDefault(Width / 2);
                Settings.Default.Code = Settings.Default.Code;
                Settings.Default.Save();
            };

            EditorCol.Width = new GridLength(Width);

            // 寻找Fira Code字体
            InstalledFontCollection fonts = new InstalledFontCollection();
            foreach (var family in fonts.Families)
            {
                if(family.Name.StartsWith("Fira Code"))
                {
                    FSharpEditor.FontFamily = new FontFamily(family.Name);
                    Output.FontFamily = new FontFamily(family.Name);
                    break;
                }
            }
        }

        private void NewDocument(object sender = null, RoutedEventArgs e = null)
        {
            FSharpEditor.Clear();
        }

        private void OpenDocument(object sender = null, RoutedEventArgs e = null)
        {
            using(var f = new System.Windows.Forms.OpenFileDialog()
            {
                Filter = fileFilter,
                Multiselect = false,
                ReadOnlyChecked = true
            })
            {
                f.FileOk += (s,e2) => {
                    if (!e2.Cancel)
                        FSharpEditor.Text = System.IO.File.ReadAllText(f.FileName);
                };

                f.ShowDialog();
            }
        }

        private void SaveDocument(object sender = null, RoutedEventArgs e = null)
        {
            using (var f = new System.Windows.Forms.SaveFileDialog()
            {
                Filter = fileFilter
            })
            {
                f.FileOk += (s, e2) => {
                    if (!e2.Cancel)
                        System.IO.File.WriteAllText(f.FileName, FSharpEditor.Text);
                };

                f.ShowDialog();
            }
        }

        private double? editorWidthWhenOutputShown;
        private void HideOrShowOutput(object sender = null, RoutedEventArgs e = null)
        {
            if(EditorOutputSplitter.Visibility == Visibility.Visible)
            {
                editorWidthWhenOutputShown = EditorCol.ActualWidth;
                EditorCol.Width = new GridLength(EditorGrid.ActualWidth);
                EditorOutputSplitter.Visibility = Visibility.Collapsed;
                Output.Visibility = Visibility.Collapsed;
                HideOrShowOutputButton.Content = "Show Output";
            }
            else
            {
                var w = editorWidthWhenOutputShown.GetValueOrDefault(EditorGrid.ActualWidth / 2);
                EditorCol.Width = new GridLength(w);
                if (w >= Width * 0.9)
                {
                    EditorCol.Width = new GridLength(Width / 2);
                    editorWidthWhenOutputShown = Width / 2;
                }
                EditorOutputSplitter.Visibility = Visibility.Visible;
                Output.Visibility = Visibility.Visible;
                HideOrShowOutputButton.Content = "Hide Output";
            }
        }
    }
}
