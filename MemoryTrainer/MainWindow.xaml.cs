using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace MemoryTrainer
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        private string _message;
        private double _progressMaximum;
        private string _resultMessage;
        private int _stopwatch;
        private string _visualMessage;

        public MainWindow()
        {
            InitializeComponent();
        }

        public List<Box> Boxes { get; set; }
        public int EllipseSize { get; set; }
        public int TextSize { get; set; }
        public Random Random { get; set; }

        public int Stopwatch
        {
            get => _stopwatch;
            set
            {
                _stopwatch = value;
                OnPropertyChanged();
            }
        }

        public double ProgressMaximum
        {
            get => _progressMaximum;
            set
            {
                _progressMaximum = value;
                OnPropertyChanged();
            }
        }

        public string VisualMessage
        {
            get => _visualMessage;
            set
            {
                _visualMessage = value;
                OnPropertyChanged();
            }
        }

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public string ResultMessage
        {
            get => _resultMessage;
            set
            {
                _resultMessage = value;
                OnPropertyChanged();
            }
        }

        public DispatcherTimer Timer { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Log.Clear();
            KeyboardController.Start();
            KeyboardController.BlockInput(true);
            Boxes = new List<Box>();
            Random = new Random();
            Timer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(1)};
            Timer.Tick += TimerOnTick;
            ShowMessage("🖰", "Нажмите ПКМ чтобы начать.\nESC - чтобы выйти.", "");
        }

        private void TimerOnTick(object sender, EventArgs e)
        {
            Stopwatch++;
            if (Stopwatch < ProgressMaximum) return;
            Timer.Stop();
            Stopwatch = 0;
            MainCanvas.Children.Clear();
            ShowMessage("⌛", "Нажмите ПКМ чтобы начать сначала.\nESC - чтобы выйти.", "— Время истекло.");
        }

        public double GetDistance(Point a, Point b)
        {
            var xDistance = b.X - a.X;
            var yDistance = b.Y - a.Y;

            return Math.Sqrt(Math.Pow(xDistance, 2) + Math.Pow(yDistance, 2));
        }

        public void AddParticle()
        {
            var box = new Box(Boxes.Count);
            box.Ellipse.MouseDown += EllipseOnMouseDown;
            Boxes.Add(box);
            Canvas.SetLeft(box.Text, box.Position.X - box.Text.ActualWidth / 2);
            Canvas.SetTop(box.Text, box.Position.Y - box.Text.ActualHeight / 2);
            Canvas.SetLeft(box.Ellipse, box.Position.X - box.Ellipse.Width / 2);
            Canvas.SetTop(box.Ellipse, box.Position.Y - box.Ellipse.Height / 2);
            MainCanvas.Children.Add(box.Text);
            MainCanvas.Children.Add(box.Ellipse);
        }

        private void EllipseOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            foreach (var box in Boxes)
            {
                if (!box.Ellipse.Equals(sender)) continue;
                if (box.Id == Box.Current)
                {
                    Box.Current++;
                    box.Show();
                    if (box.Id == Boxes.Count - 1)
                    {
                        Timer.Stop();
                        Stopwatch = 0;
                        AddParticle();
                        ProgressMaximum = Boxes.Count * 30;
                        Mix();
                        Box.Current = 0;
                        Reduce();
                        HideNumbers();
                    }
                }
                else
                {
                    Timer.Stop();
                    Stopwatch = 0;
                    box.Error();
                    ShowMessage("¯\\_(ツ)_/¯", "Нажмите ПКМ чтобы начать сначала.\nESC - чтобы выйти.", "");
                }

                break;
            }
        }

        public void Mix()
        {
            var screenWidth = (int) SystemParameters.PrimaryScreenWidth;
            var screenHeight = (int) SystemParameters.PrimaryScreenHeight;
            for (var i = 0; i < Boxes.Count; i++)
            {
                var position = new Point
                {
                    X = Random.Next((int) Boxes[i].Ellipse.Width, screenWidth - (int) Boxes[i].Ellipse.Width),
                    Y = Random.Next((int) Boxes[i].Ellipse.Height, screenHeight - (int) Boxes[i].Ellipse.Height)
                };

                for (var j = 0; j < i; j++)
                    if (GetDistance(position, Boxes[j].Position) - Boxes[i].Ellipse.Width < 0)
                    {
                        position.X = Random.Next((int) Boxes[i].Ellipse.Width,
                            screenWidth - (int) Boxes[i].Ellipse.Width);
                        position.Y = Random.Next((int) Boxes[i].Ellipse.Height,
                            screenHeight - (int) Boxes[i].Ellipse.Height);
                        j = -1;
                    }

                Boxes[i].Position = position;
                Canvas.SetLeft(Boxes[i].Text, position.X - Boxes[i].Text.ActualWidth / 2);
                Canvas.SetTop(Boxes[i].Text, position.Y - Boxes[i].Text.ActualHeight / 2);
                Canvas.SetLeft(Boxes[i].Ellipse, position.X - Boxes[i].Ellipse.Width / 2);
                Canvas.SetTop(Boxes[i].Ellipse, position.Y - Boxes[i].Ellipse.Height / 2);
            }
        }

        public void HideNumbers()
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                foreach (var b in Boxes) b.Hide();
                Timer.Start();
            });
        }

        public void Reduce()
        {
            EllipseSize -= 5;
            TextSize -= 5;
            foreach (var b in Boxes) b.SetSize(EllipseSize, TextSize);
        }

        public void StartNewGame()
        {
            if (Timer.IsEnabled)
            {
                Timer.Stop();
                Stopwatch = 0;
            }

            ClearMessage();
            MainCanvas.Children.Clear();
            Boxes.Clear();
            Box.Current = 0;
            EllipseSize = 200;
            TextSize = 150;
            AddParticle();
            ProgressMaximum = Boxes.Count * 30;
            Mix();
            HideNumbers();
        }

        private void ClearMessage()
        {
            VisualMessage = "";
            Message = "";
            ResultMessage = "";
        }

        private void MainWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed) StartNewGame();
        }

        public void ShowMessage(string visualMessage, string message, string resultMessage)
        {
            VisualMessage = visualMessage;
            Message = message;
            ResultMessage = resultMessage;
        }

        private void MainWindow_OnDeactivated(object sender, EventArgs e)
        {
            Background = new SolidColorBrush(Color.FromRgb(30, 30, 35));
        }
    }
}