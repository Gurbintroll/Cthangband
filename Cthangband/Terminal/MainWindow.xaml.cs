﻿using Cthangband.StaticData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Cthangband.Terminal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    internal partial class MainWindow : Window
    {
        public readonly Queue<char> KeyQueue = new Queue<char>();
        public TextBlock[][] Cells = new TextBlock[45][];
        private BackgroundImage _backgroundImage = Cthangband.Terminal.BackgroundImage.None;

        public MainWindow()
        {
            InitializeComponent();
            KeyDown += MainWindow_KeyDown;
            Closing += MainWindow_Closing;
            TextInput += MainWindow_TextInput;
        }

        public BackgroundImage BackgroundImage
        {
            set
            {
                if (value == _backgroundImage)
                {
                    return;
                }
                _backgroundImage = value;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                Uri uri;
                switch (value)
                {
                    case BackgroundImage.Splash:
                        uri = new Uri("pack://application:,,,/Resources/splash.png");
                        bitmapImage.UriSource = uri;
                        break;

                    case BackgroundImage.Normal:
                        uri = new Uri("pack://application:,,,/Resources/Background2.png");
                        bitmapImage.UriSource = uri;
                        break;

                    case BackgroundImage.Overhead:
                        uri = new Uri("pack://application:,,,/Resources/Background.png");
                        bitmapImage.UriSource = uri;
                        break;

                    case BackgroundImage.Paper:
                        uri = new Uri("pack://application:,,,/Resources/Paper.png");
                        bitmapImage.UriSource = uri;
                        break;

                    case BackgroundImage.Menu:
                        uri = new Uri("pack://application:,,,/Resources/Menu.png");
                        bitmapImage.UriSource = uri;
                        break;

                    case BackgroundImage.Tomb:
                        uri = new Uri("pack://application:,,,/Resources/Tomb.png");
                        bitmapImage.UriSource = uri;
                        break;

                    case BackgroundImage.Crown:
                        uri = new Uri("pack://application:,,,/Resources/Crown.png");
                        bitmapImage.UriSource = uri;
                        break;

                    case BackgroundImage.Sunset:
                        uri = new Uri("pack://application:,,,/Resources/Sunset.png");
                        bitmapImage.UriSource = uri;
                        break;

                    case BackgroundImage.Map:
                        uri = new Uri("pack://application:,,,/Resources/Map.png");
                        bitmapImage.UriSource = uri;
                        break;

                    case BackgroundImage.WildMap:
                        uri = new Uri("pack://application:,,,/Resources/WildMap.png");
                        bitmapImage.UriSource = uri;
                        break;
                }
                bitmapImage.EndInit();
                Background = new ImageBrush(bitmapImage);
            }
            get => _backgroundImage;
        }

        public void InitializeGrid(TerminalParameters parameters)
        {
            BackgroundImage = BackgroundImage.Menu;
            Content = null;
            UniformGrid grid = new UniformGrid();
            AddChild(grid);
            grid.Rows = Constants.ConsoleHeight;
            grid.Columns = Constants.ConsoleWidth;
            grid.Background = null;
            grid.IsHitTestVisible = false;
            grid.IsEnabled = false;
            grid.Cursor = Cursors.None;
            FontFamily family = new FontFamily(parameters.FontName);
            Cells = new TextBlock[Constants.ConsoleHeight][];
            for (int row = 0; row < Constants.ConsoleHeight; row++)
            {
                Cells[row] = new TextBlock[Constants.ConsoleWidth];
                for (int col = 0; col < Constants.ConsoleWidth; col++)
                {
                    Viewbox v = new Viewbox();
                    TextBlock x = new TextBlock();
                    v.Stretch = Stretch.Fill;
                    x.FontFamily = family;
                    x.FontWeight = parameters.FontBold ? FontWeights.Bold : FontWeights.Regular;
                    x.FontStyle = parameters.FontItalic ? FontStyles.Italic : FontStyles.Normal;
                    x.Text = new string(' ', 1);
                    x.Background = null;
                    x.Foreground = Brushes.White;
                    x.IsEnabled = false;
                    grid.Children.Add(v);
                    v.Child = x;
                    Cells[row][col] = x;
                }
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Tab:
                    KeyQueue.Enqueue('\x09');
                    break;

                case Key.Return:
                    KeyQueue.Enqueue('\x0D');
                    break;

                case Key.Escape:
                    KeyQueue.Enqueue('\x1B');
                    break;

                case Key.Prior:
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                    {
                        KeyQueue.Enqueue('.');
                    }
                    KeyQueue.Enqueue('9');
                    break;

                case Key.Next:
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                    {
                        KeyQueue.Enqueue('.');
                    }
                    KeyQueue.Enqueue('3');
                    break;

                case Key.End:
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                    {
                        KeyQueue.Enqueue('.');
                    }
                    KeyQueue.Enqueue('1');
                    break;

                case Key.Home:
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                    {
                        KeyQueue.Enqueue('.');
                    }
                    KeyQueue.Enqueue('7');
                    break;

                case Key.Left:
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                    {
                        KeyQueue.Enqueue('.');
                    }
                    KeyQueue.Enqueue('4');
                    break;

                case Key.Up:
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                    {
                        KeyQueue.Enqueue('.');
                    }
                    KeyQueue.Enqueue('8');
                    break;

                case Key.Right:
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                    {
                        KeyQueue.Enqueue('.');
                    }
                    KeyQueue.Enqueue('6');
                    break;

                case Key.Down:
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                    {
                        KeyQueue.Enqueue('.');
                    }
                    KeyQueue.Enqueue('2');
                    break;

                case Key.Clear:
                    KeyQueue.Enqueue('5');
                    break;

                default:
                    return;
            }
        }

        private void MainWindow_TextInput(object sender, TextCompositionEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Text) == false)
            {
                if (e.Text[0] < 255)
                {
                    KeyQueue.Enqueue(e.Text[0]);
                }
            }
        }
    }
}