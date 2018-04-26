using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using FoundaryMediaPlayer.Events;

namespace FoundaryMediaPlayer.Controls
{
    /// <summary>
    /// Interaction logic for ExtendedSlider.xaml
    /// </summary>
    /// <remarks>
    /// A <see cref="Slider"/> control with additional exposed XAML bindings.
    /// </remarks>
    public partial class ExtendedSlider
    {
        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty SeekProperty { get; } = DependencyProperty.Register(nameof(Seek), typeof(ICommand), typeof(ExtendedSlider), new PropertyMetadata(OnSeekPropertyChanged));
        
        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty DragStartedProperty { get; } = DependencyProperty.Register(nameof(DragStarted), typeof(ICommand), typeof(ExtendedSlider), new PropertyMetadata(OnDragStartedPropertyChanged));
        
        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty DragDeltaProperty { get; } = DependencyProperty.Register(nameof(DragDelta), typeof(ICommand), typeof(ExtendedSlider), new PropertyMetadata(OnDragDeltaPropertyChanged));
        
        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty DragCompletedProperty { get; } = DependencyProperty.Register(nameof(DragCompleted), typeof(ICommand), typeof(ExtendedSlider), new PropertyMetadata(OnDragCompletedPropertyChanged));

        /// <summary>
        /// 
        /// </summary>
        [Bindable(true), Category("Action")]
        public ICommand Seek
        {
            get => (ICommand) GetValue(SeekProperty);
            set => SetValue(SeekProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        [Bindable(true), Category("Action")]
        public ICommand DragStarted
        {
            get => (ICommand)GetValue(DragStartedProperty);
            set => SetValue(DragStartedProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        [Bindable(true), Category("Action")]
        public ICommand DragDelta
        {
            get => (ICommand)GetValue(DragDeltaProperty);
            set => SetValue(DragDeltaProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        [Bindable(true), Category("Action")]
        public ICommand DragCompleted
        {
            get => (ICommand)GetValue(DragCompletedProperty);
            set => SetValue(DragCompletedProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public ExtendedSlider()
        {
            InitializeComponent();
        }

        private static void OnSeekPropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            var slider = (ExtendedSlider)target;
            slider.Seek = (ICommand) e.NewValue;
        }

        private static void OnDragStartedPropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            var slider = (ExtendedSlider)target;
            slider.DragStarted = (ICommand)e.NewValue;
        }

        private static void OnDragDeltaPropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            var slider = (ExtendedSlider)target;
            slider.DragDelta = (ICommand)e.NewValue;
        }

        private static void OnDragCompletedPropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            var slider = (ExtendedSlider)target;
            slider.DragCompleted = (ICommand)e.NewValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            OnSeek(new SliderSeekEventArgs(this, Value / Maximum));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSeek(SliderSeekEventArgs e)
        {
            Seek?.Execute(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnThumbDragStarted(DragStartedEventArgs e)
        {
            base.OnThumbDragStarted(e);
            DragStarted?.Execute(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnThumbDragDelta(DragDeltaEventArgs e)
        {
            base.OnThumbDragDelta(e);
            DragDelta?.Execute(new SliderDragDeltaEventArgs(e));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnThumbDragCompleted(DragCompletedEventArgs e)
        {
            base.OnThumbDragCompleted(e);
            DragCompleted?.Execute(this);
            OnSeek(new SliderSeekEventArgs(this, Value / Maximum));
        }
    }
}
