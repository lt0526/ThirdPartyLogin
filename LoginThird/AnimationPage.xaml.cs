using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace LoginThird
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AnimationPage : Page
    {
        DispatcherTimer timer;

        private Point currentPoint;

        private Point oldPoint;

        private List<Line> lineList;

        public AnimationPage()
        {
            this.InitializeComponent();

            timer = new DispatcherTimer();
            timer.Tick += drawLine;
            timer.Interval = TimeSpan.FromMilliseconds(10);

            lineList = new List<Line>();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        private void LinearBtn_Click(object sender, RoutedEventArgs e)
        {
            LinearStoryboard.Begin();
            resetLineState();
        }

        private void SplineBtn_Click(object sender, RoutedEventArgs e)
        {
            SplineStoryboard.Begin();
            resetLineState();
        }

        private void BounceBtn_Click(object sender, RoutedEventArgs e)
        {
            BounceStoryboard.Begin();
            resetLineState();
        }

        private void BackEaseBtn_Click(object sender, RoutedEventArgs e)
        {
            BackEasingStoryboard.Begin();
            resetLineState();
        }

        private void ResetPosition_Click(object sender, RoutedEventArgs e)
        {

            LinearStoryboard.Stop();
            SplineStoryboard.Stop();
            BounceStoryboard.Stop();
            BackEasingStoryboard.Stop();
            CircleEasingStoryboard.Stop();
            ElasticEasingStoryboard.Stop();
            SineEasingStoryboard.Stop();

            MyRect.Margin = new Thickness(0, 200, 0, 0);
            RectTranslate.X = 0;
            RectTranslate.Y = 0;
        }

        private void CircleBtn_Click(object sender, RoutedEventArgs e)
        {
            CircleEasingStoryboard.Begin();
            resetLineState();
        }

        private void ElasticBtn_Click(object sender, RoutedEventArgs e)
        {
            ElasticEasingStoryboard.Begin();
            resetLineState();
        }

        private void SineBtn_Click(object sender, RoutedEventArgs e)
        {
            SineEasingStoryboard.Begin();
            resetLineState();
        }

        private void drawLine(object sender, object e)
        {
            currentPoint = new Point(25 + RectTranslate.X, 225 + RectTranslate.Y);

            Line line = new Line() { X1 = currentPoint.X, Y1 = currentPoint.Y, X2 = oldPoint.X, Y2 = oldPoint.Y };
            line.Stroke = new SolidColorBrush(Colors.Blue);
            line.StrokeThickness = 5;
            line.StrokeLineJoin = PenLineJoin.Round;
            line.StrokeStartLineCap = PenLineCap.Round;
            line.StrokeEndLineCap = PenLineCap.Round;

            MyGrid.Children.Add(line);
            Grid.SetRow(line, 2);

            oldPoint = currentPoint;

            lineList.Add(line);
        }

        private void resetLineState()
        {
            foreach (Line l in lineList)
            {
                MyGrid.Children.Remove(l);
            }
            lineList.Clear();

            timer.Start();
            oldPoint = new Point(25, 225);
        }

        private void Linear_Completed(object sender, object e)
        {
            timer.Stop();
        }

        private void Spline_Completed(object sender, object e)
        {
            timer.Stop();
        }

        private void Back_Completed(object sender, object e)
        {
            timer.Stop();
        }

        private void Bounce_Completed(object sender, object e)
        {
            timer.Stop();
        }

        private void Circle_Completed(object sender, object e)
        {
            timer.Stop();
        }

        private void Elastic_Completed(object sender, object e)
        {
            timer.Stop();
        }

        private void Sine_Completed(object sender, object e)
        {
            timer.Stop();
        }

        private void PushBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CustomPage));
        }
    }
}
