using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace MaterialDesignThemes.Wpf
{
    public class Carousel : ItemsControl
    {
        private ScrollViewer _scrollViewer;

        public static readonly RoutedCommand TransitionForwardCommand = new RoutedCommand();
        public static readonly RoutedCommand TransitionBackwardCommand = new RoutedCommand();

        public Carousel()
        {
            CommandBindings.Add(new CommandBinding(TransitionForwardCommand, TransitionForwardHandler));
            CommandBindings.Add(new CommandBinding(TransitionBackwardCommand, TransitionBackwardHandler));
            Loaded += (s, e) =>
            {
                Transition(SelectedIndex, Orientation);
            };
        }

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                nameof(Orientation),      
                typeof(Orientation),           
                typeof(Carousel),       
                new PropertyMetadata(Orientation.Horizontal));

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public static readonly DependencyProperty IsDefaultButtonVisibleProperty =
            DependencyProperty.Register(
                nameof(IsDefaultButtonVisible),      
                typeof(bool),           
                typeof(Carousel),       
                new PropertyMetadata(true));

        public bool IsDefaultButtonVisible
        {
            get => (bool)GetValue(IsDefaultButtonVisibleProperty);
            set => SetValue(IsDefaultButtonVisibleProperty, value);
        }

        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(
                nameof(SelectedIndex),      
                typeof(int),           
                typeof(Carousel),       
                new PropertyMetadata(0));

        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(
                nameof(SelectedItem),      
                typeof(object),           
                typeof(Carousel));

        public object SelectedItem
        {
            get => (object)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _scrollViewer = GetTemplateChild("ScrollViewer") as ScrollViewer;
        }

        private void TransitionForwardHandler(object sender, RoutedEventArgs e)
        {
            if (_scrollViewer == null)
                return;
            var indexBefore = SelectedIndex;
            SelectedIndex = (SelectedIndex + 1) % Items.Count;

            Transition(SelectedIndex, Orientation);
        }

        private void TransitionBackwardHandler(object sender, RoutedEventArgs e)
        {
            if (_scrollViewer == null)
                return;
            SelectedIndex = ((SelectedIndex - 1) + Items.Count) % Items.Count;

            Transition(SelectedIndex, Orientation);
        }

        private void Transition(int index, Orientation orientation)
        {
            if (index < 0) return;
            if (index >= Items.Count) return;
            if (Items.Count == 0) return;

            if(orientation == Orientation.Horizontal)
                TransitionHorizontally(index);
            else
                TransitionVertically(index);

            SelectedItem = Items[index];
        }

        private void TransitionHorizontally(int index)
        {
            var container = (FrameworkElement)ItemContainerGenerator.ContainerFromIndex(index);
            if(container == null) return;

            var endPosition = _scrollViewer.HorizontalOffset <= (index * ActualWidth) ? index * ActualWidth : 0;

            var animation = new DoubleAnimation
            {
                From = _scrollViewer.HorizontalOffset,
                To = endPosition,
                Duration = TimeSpan.FromSeconds(.5),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut },
            };

            var storyboard = new Storyboard();
            Storyboard.SetTarget(animation, _scrollViewer);
            Storyboard.SetTargetProperty(animation, new PropertyPath(ScrollViewerBehavior.HorizontalOffsetProperty));
            storyboard.Children.Add(animation);
            storyboard.Begin();

        }

        private void TransitionVertically(int index)
        {
            var container = (FrameworkElement)ItemContainerGenerator.ContainerFromIndex(index);
            if(container == null) return;

            var endPosition = _scrollViewer.VerticalOffset <= (index * ActualHeight) ? index * ActualHeight : 0;

            var animation = new DoubleAnimation
            {
                From = _scrollViewer.VerticalOffset,
                To = endPosition,
                Duration = TimeSpan.FromSeconds(.5),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut },
            };

            var storyboard = new Storyboard();
            Storyboard.SetTarget(animation, _scrollViewer);
            Storyboard.SetTargetProperty(animation, new PropertyPath(ScrollViewerBehavior.VerticalOffsetProperty));
            storyboard.Children.Add(animation);
            storyboard.Begin();
        }
    }

    public static class ScrollViewerBehavior
    {
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.RegisterAttached("VerticalOffset", typeof(double), typeof(ScrollViewerBehavior), new PropertyMetadata(0.0, OnVerticalOffsetChanged));
        public static double GetVerticalOffset(DependencyObject obj) => (double)obj.GetValue(VerticalOffsetProperty);
        public static void SetVerticalOffset(DependencyObject obj, double value) => obj.SetValue(VerticalOffsetProperty, value);
        private static void OnVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is ScrollViewer scrollViewer)
                scrollViewer.ScrollToVerticalOffset((double)e.NewValue);
        }

        public static readonly DependencyProperty HorizontalOffsetProperty =
    DependencyProperty.RegisterAttached("HorizontalOffset", typeof(double), typeof(ScrollViewerBehavior), new PropertyMetadata(0.0, OnHorizontalOffsetChanged));
        public static double GetHorizontalOffset(DependencyObject obj) => (double)obj.GetValue(HorizontalOffsetProperty);
        public static void SetHorizontalOffset(DependencyObject obj, double value) => obj.SetValue(HorizontalOffsetProperty, value);
        private static void OnHorizontalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollViewer scrollViewer)
                scrollViewer.ScrollToHorizontalOffset((double)e.NewValue);
        }
    }

    public class CarouselItem : ContentControl
    {
        public CarouselItem()
        {
        }
    }
}
