using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace XO.Controls
{
    public partial class ConfettiControl : UserControl
    {
        private readonly DispatcherTimer _timer;
        private readonly Random _random;
        private readonly SolidColorBrush[] _colors = {
            Brushes.Red, Brushes.Green, Brushes.Blue, 
            Brushes.Yellow, Brushes.Purple, Brushes.Orange
        };

        public ConfettiControl()
        {
            InitializeComponent();
            _random = new Random();
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };
            _timer.Tick += Timer_Tick;
        }

        public static readonly DependencyProperty IsActiveProperty = 
            DependencyProperty.Register("IsActive", typeof(bool), typeof(ConfettiControl), 
                new PropertyMetadata(false, OnIsActiveChanged));

        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }

        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ConfettiControl control)
            {
                if ((bool)e.NewValue)
                {
                    control.StartConfetti();
                }
                else
                {
                    control.StopConfetti();
                }
            }
        }

        private void StartConfetti()
        {
            ConfettiCanvas.Children.Clear();
            _timer.Start();
        }

        private void StopConfetti()
        {
            _timer.Stop();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < 5; i++)
            {
                var confetti = new Rectangle
                {
                    Width = _random.Next(5, 15),
                    Height = _random.Next(5, 15),
                    Fill = _colors[_random.Next(_colors.Length)],
                    Opacity = 0.7 + _random.NextDouble() * 0.3
                };

                Canvas.SetLeft(confetti, _random.NextDouble() * ActualWidth);
                Canvas.SetTop(confetti, -20);
                ConfettiCanvas.Children.Add(confetti);

                var animation = new System.Windows.Media.Animation.DoubleAnimation
                {
                    To = ActualHeight + 20,
                    Duration = TimeSpan.FromSeconds(2 + _random.NextDouble() * 3),
                    AccelerationRatio = 0.1,
                    DecelerationRatio = 0.1
                };

                animation.Completed += (s, _) => ConfettiCanvas.Children.Remove(confetti);
                confetti.BeginAnimation(Canvas.TopProperty, animation);

                // Rotation animation
                var rotateAnimation = new System.Windows.Media.Animation.DoubleAnimation
                {
                    To = _random.Next(360),
                    Duration = TimeSpan.FromSeconds(1 + _random.NextDouble() * 2)
                };
                var rotateTransform = new RotateTransform();
                confetti.RenderTransform = rotateTransform;
                confetti.RenderTransformOrigin = new Point(0.5, 0.5);
                rotateTransform.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);
            }
        }
    }
}