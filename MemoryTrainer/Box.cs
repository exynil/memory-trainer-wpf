using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MemoryTrainer
{
    public class Box
    {
        public Box(int id)
        {
            Id = id;
            Position = new Point();
            Ellipse = new Ellipse
            {
                Width = 200,
                Height = 200,
                Fill = null,
                Stroke = null
            };
            Text = new TextBlock
            {
                Text = Id.ToString(),
                Foreground = new SolidColorBrush(Colors.WhiteSmoke),
                FontSize = 150,
                FontWeight = FontWeights.Medium
            };
            Text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Text.Arrange(new Rect(Text.DesiredSize));
        }

        public int Id { get; set; }
        public static int Current { get; set; }
        public Point Position { get; set; }
        public Ellipse Ellipse { get; set; }
        public TextBlock Text { get; set; }

        public void Show()
        {
            Ellipse.Fill = null;
            Ellipse.Stroke = null;
        }

        public void Hide()
        {
            Ellipse.Dispatcher?.Invoke(() =>
            {
                Ellipse.Fill = new SolidColorBrush(Color.FromRgb(20, 20, 25));
                Ellipse.Stroke = Brushes.SpringGreen;
            });
        }

        public void Error()
        {
            Ellipse.Fill = null;
            Ellipse.Stroke = null;
            Text.Foreground = new SolidColorBrush(Colors.Crimson);
        }

        public void SetSize(double ellipseSize, double fontSize)
        {
            Ellipse.Width = ellipseSize;
            Ellipse.Height = ellipseSize;
            Text.FontSize = fontSize;
        }
    }
}