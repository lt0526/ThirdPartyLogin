using Cimbalino.Toolkit.Extensions;
using LoginThird.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace LoginThird
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CustomPage : Page
    {
        private const float REFRESH_ICON_MAX_OFFSET_Y = 36.0f;

        private Compositor _compositor;

        private ScrollViewer _scrollViewer;

        private bool _isAtTop = true;

        private Visual _refreshIconVisual;
        private Visual _borderVisual;

        private ExpressionAnimation _rotationAnimation, _opacityAnimation, _offsetAnimation;
        private ScalarKeyFrameAnimation _resetAnimation, _loadingAnimation;
        private float _refreshIconOffsetY;

        private bool _refresh;

        private DateTime _pulledDownTime, _restoredTime;

        public ObservableCollection<School> Items { set; get; } = new ObservableCollection<School>();
        int newItemCount = 1;

        public CustomPage()
        {
            this.InitializeComponent();

            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

            for (int i = 1; i <= 100; i++)
            {
                Items.Add(new School { Name = "Item" + i });
            }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        private void ItemsControl_Loaded(object sender, RoutedEventArgs e)
        {
            _scrollViewer = MyItemsControl.GetVisualDescendents<ScrollViewer>().FirstOrDefault();

            if (_scrollViewer != null)
            {
                _scrollViewer.IsScrollInertiaEnabled = true;

                _scrollViewer.DirectManipulationStarted += OnDirectManipStarted;
                _scrollViewer.DirectManipulationCompleted += OnDirectManipCompleted;


                CompositionPropertySet _scrollerViewerManipulation = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(_scrollViewer);

                _rotationAnimation = _compositor.CreateExpressionAnimation();
                _rotationAnimation.SetScalarParameter("MyMultiplier", 10.0f);
                _rotationAnimation.SetScalarParameter("MyMaxDegree", 400.0f);
                _rotationAnimation.SetReferenceParameter("MyScrollManipulation", _scrollerViewerManipulation);
                _rotationAnimation.Expression = "min(max(0, MyScrollManipulation.Translation.Y) * MyMultiplier, MyMaxDegree)";

                _opacityAnimation = _compositor.CreateExpressionAnimation();
                _opacityAnimation.SetScalarParameter("MyDivider", 30.0f);
                _opacityAnimation.SetReferenceParameter("MyScrollManipulation", _scrollerViewerManipulation);
                _opacityAnimation.Expression = "min(max(0, MyScrollManipulation.Translation.Y) / MyDivider, 1)";

                _offsetAnimation = _compositor.CreateExpressionAnimation();
                _offsetAnimation.SetScalarParameter("MyDivider", 30.0f);
                _offsetAnimation.SetScalarParameter("MyMaxOffsetY", REFRESH_ICON_MAX_OFFSET_Y);
                _offsetAnimation.SetReferenceParameter("MyScrollManipulation", _scrollerViewerManipulation);
                _offsetAnimation.Expression = "(min(max(0, MyScrollManipulation.Translation.Y) / MyDivider, 1)) * MyMaxOffsetY";

                _resetAnimation = _compositor.CreateScalarKeyFrameAnimation();
                _resetAnimation.InsertKeyFrame(1.0f, 0.0f);

                _loadingAnimation = _compositor.CreateScalarKeyFrameAnimation();
                _loadingAnimation.InsertKeyFrame(1.0f, 360);
                _loadingAnimation.Duration = TimeSpan.FromMilliseconds(800);
                _loadingAnimation.IterationBehavior = AnimationIterationBehavior.Forever;

                _refreshIconVisual = ElementCompositionPreview.GetElementVisual(RefreshIcon);
                _refreshIconVisual.CenterPoint = new Vector3(Convert.ToSingle(RefreshIcon.ActualWidth / 2), Convert.ToSingle(RefreshIcon.ActualHeight / 2), 0);

                // Get the ListView's inner Border's Visual.
                var border = (Border)VisualTreeHelper.GetChild(MyItemsControl, 0);
                _borderVisual = ElementCompositionPreview.GetElementVisual(border);

                _refreshIconVisual.StartAnimation(nameof(_refreshIconVisual.RotationAngleInDegrees), _rotationAnimation);
                _refreshIconVisual.StartAnimation(nameof(_refreshIconVisual.Opacity), _opacityAnimation);
                _refreshIconVisual.StartAnimation($"{nameof(_refreshIconVisual.Offset)}.{nameof(_refreshIconVisual.Offset.Y)}", _offsetAnimation);
                _borderVisual.StartAnimation($"{nameof(_borderVisual.Offset)}.{nameof(_borderVisual.Offset.Y)}", _offsetAnimation);

            }
        }

        private async void OnDirectManipCompleted(object sender, object e)
        {
            Windows.UI.Xaml.Media.CompositionTarget.Rendering -= OnCompositionTargetRendering;

            var cancelled = (_restoredTime - _pulledDownTime) > TimeSpan.FromMilliseconds(250);

            if (_refresh)
            {
                if (cancelled)
                    StartResetAnimations();
                else
                    await StartLoadingAnimation(StartResetAnimations);
            }
        }

        private void OnDirectManipStarted(object sender, object e)
        {
            Windows.UI.Xaml.Media.CompositionTarget.Rendering += OnCompositionTargetRendering;

            _refresh = false;
        }

        private async Task StartLoadingAnimation(Action completed)
        {
            var modes = _scrollViewer.ManipulationMode;


            await Task.Delay(100);

            _refreshIconVisual.StartAnimation(nameof(_refreshIconVisual.RotationAngleInDegrees), _loadingAnimation);

            _scrollViewer.ManipulationMode = Windows.UI.Xaml.Input.ManipulationModes.None;


            await Task.Delay(1500);

            Items.Insert(0, new School { Name = "New Items" + newItemCount++ });

            completed();

            _scrollViewer.ManipulationMode = modes;
        }

        private void StartResetAnimations()
        {
            var batch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);

            batch.Completed += (s, e) =>
            {
                _refreshIconVisual.StartAnimation(nameof(_refreshIconVisual.RotationAngleInDegrees), _rotationAnimation);
                _refreshIconVisual.StartAnimation(nameof(_refreshIconVisual.Opacity), _opacityAnimation);
                _refreshIconVisual.StartAnimation($"{nameof(_refreshIconVisual.Offset)}.{nameof(_refreshIconVisual.Offset.Y)}", _offsetAnimation);
                _borderVisual.StartAnimation($"{nameof(_borderVisual.Offset)}.{nameof(_borderVisual.Offset.Y)}", _offsetAnimation);
            };

            _borderVisual.StartAnimation($"{nameof(_borderVisual.Offset)}.{nameof(_borderVisual.Offset.Y)}", _resetAnimation);
            _refreshIconVisual.StartAnimation(nameof(_refreshIconVisual.Opacity), _resetAnimation);

            batch.End();
        }

        private void OnCompositionTargetRendering(object sender, object e)
        {

            _refreshIconVisual.StopAnimation($"{nameof(_refreshIconVisual.Offset)}.{nameof(_refreshIconVisual.Offset.Y)}");

            _refreshIconOffsetY = _refreshIconVisual.Offset.Y;
            if (!_refresh)
            {
                _refresh = _refreshIconOffsetY == REFRESH_ICON_MAX_OFFSET_Y;
            }

            if (_refreshIconOffsetY == REFRESH_ICON_MAX_OFFSET_Y)
            {
                _pulledDownTime = DateTime.Now;

                // Stop the Opacity animation on the RefreshIcon and the Offset.Y animation on the Border (ScrollViewer's host)
                _refreshIconVisual.StopAnimation(nameof(_refreshIconVisual.Opacity));
                _borderVisual.StopAnimation($"{nameof(_borderVisual.Offset)}.{nameof(_borderVisual.Offset.Y)}");
            }

            if (_refresh && _refreshIconOffsetY <= 1)
            {
                _restoredTime = DateTime.Now;
            }

            _refreshIconVisual.StartAnimation($"{nameof(_refreshIconVisual.Offset)}.{nameof(_refreshIconVisual.Offset.Y)}", _offsetAnimation);
        }
    }
}
