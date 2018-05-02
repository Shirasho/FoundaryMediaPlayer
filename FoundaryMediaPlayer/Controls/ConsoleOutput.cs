using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace FoundaryMediaPlayer.Controls
{
    public sealed class ConsoleOutput : RichTextBox
    {
        /// <summary>
        /// The items to be displayed in the terminal window, e.g. an ObservableCollection.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource",
            typeof(IEnumerable),
            typeof(ConsoleOutput),
            new PropertyMetadata(default(IEnumerable), OnItemsSourceChanged));

        /// <summary>
        /// The bound items to the terminal.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// The margin around the contents of the terminal window, optional field with a default value of 0.
        /// </summary>
        public static readonly DependencyProperty ItemsMarginProperty = DependencyProperty.Register("ItemsMargin",
            typeof(Thickness),
            typeof(ConsoleOutput),
            new PropertyMetadata(new Thickness(), OnItemsMarginChanged));

        /// <summary>
        /// The margin around the bound items.
        /// </summary>
        public Thickness ItemsMargin
        {
            get { return (Thickness)GetValue(ItemsMarginProperty); }
            set { SetValue(ItemsMarginProperty, value); }
        }

        /// <summary>
        /// The property name of the 'value' to be displayed, optional field which if null then ToString() is called on the
        /// bound instance.
        /// </summary>
        public static readonly DependencyProperty ItemDisplayPathProperty = DependencyProperty.Register("ItemDisplayPath",
            typeof(string),
            typeof(ConsoleOutput),
            new PropertyMetadata(default(string), OnDisplayPathChanged));

        /// <summary>
        /// The display path for the bound items.
        /// </summary>
        public string ItemDisplayPath
        {
            get { return (string)GetValue(ItemDisplayPathProperty); }
            set { SetValue(ItemDisplayPathProperty, value); }
        }

        /// <summary>
        /// The color converter for lines.
        /// </summary>
        public static readonly DependencyProperty LineColorConverterProperty = DependencyProperty.Register("LineColorConverter",
            typeof(IValueConverter),
            typeof(ConsoleOutput),
            new PropertyMetadata(null, OnLineConverterChanged));

        /// <summary>
        /// The error color for the bound items.
        /// </summary>
        public IValueConverter LineColorConverter
        {
            get { return (IValueConverter)GetValue(LineColorConverterProperty); }
            set { SetValue(LineColorConverterProperty, value); }
        }

        /// <summary>
        /// The height of each line in the terminal window, optional field with a default value of 10.
        /// </summary>
        public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register("ItemHeight",
            typeof(int),
            typeof(ConsoleOutput),
            new PropertyMetadata(10, OnItemHeightChanged));

        /// <summary>
        /// The individual line height for the bound items.
        /// </summary>
        public int ItemHeight
        {
            get { return (int)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        private Paragraph _Paragraph { get; }
        private INotifyCollectionChanged _NotifyChanged { get; set; }
        private PropertyInfo _DisplayPathProperty {get; set;}

        public ConsoleOutput()
        {
            _Paragraph = new Paragraph
            {
                Margin = ItemsMargin,
                LineHeight = ItemHeight
            };

            IsUndoEnabled = false;
            Document = new FlowDocument(_Paragraph);

            TextChanged += (s, e) => { ScrollToEnd(); };

            SetResourceReference(StyleProperty, "ConsoleOutputDefault");
        }

        /// <summary>
        /// Processes style changes for the terminal.
        /// </summary>
        /// <param name="oldStyle">The current style applied to the terminal.</param>
        /// <param name="newStyle">The new style to be applied to the terminal.</param>
        protected override void OnStyleChanged(Style oldStyle, Style newStyle)
        {
            base.OnStyleChanged(oldStyle, newStyle);

            if (ItemsSource != null)
            {
                using (DeclareChangeBlock())
                {
                    ReplaceItems(ItemsSource.Cast<object>().ToArray());
                }
            }
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue == args.OldValue)
            {
                return;
            }

            var terminal = (ConsoleOutput)d;
            terminal.HandleItemsSourceChanged((IEnumerable)args.NewValue);
        }

        private static void OnItemsMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue == args.OldValue)
            {
                return;
            }

            var terminal = (ConsoleOutput)d;
            terminal._Paragraph.Margin = (Thickness)args.NewValue;
        }

        private static void OnItemHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue == args.OldValue)
            {
                return;
            }

            var terminal = (ConsoleOutput)d;
            terminal._Paragraph.LineHeight = (int)args.NewValue;
        }

        private static void OnLineConverterChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue == args.OldValue)
            {
                return;
            }

            var terminal = (ConsoleOutput)d;
            terminal.HandleLineConverterChanged();
        }

        private static void OnDisplayPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue == args.OldValue)
            {
                return;
            }

            var terminal = (ConsoleOutput)d;
            terminal._DisplayPathProperty = null;
        }

        private void HandleItemsSourceChanged(IEnumerable items)
        {
            if (items == null)
            {
                _Paragraph.Inlines.Clear();

                return;
            }

            using (DeclareChangeBlock())
            {
                if (items is INotifyCollectionChanged changed)
                {
                    var notifyChanged = changed;
                    if (_NotifyChanged != null)
                    {
                        _NotifyChanged.CollectionChanged -= HandleItemsChanged;
                    }

                    _NotifyChanged = notifyChanged;
                    _NotifyChanged.CollectionChanged += HandleItemsChanged;

                    // ReSharper disable once PossibleMultipleEnumeration
                    var existingItems = items.Cast<object>().ToArray();
                    if (existingItems.Any())
                    {
                        ReplaceItems(existingItems);
                    }
                    else
                    {
                        ClearItems();
                    }
                }
                else
                {
                    // ReSharper disable once PossibleMultipleEnumeration
                    ReplaceItems(ItemsSource.Cast<object>().ToArray());
                }
            }
        }

        private void HandleLineConverterChanged()
        {
            using (DeclareChangeBlock())
            {
                foreach (var run in _Paragraph.Inlines
                    .Where(x => x is Run)
                    .Cast<Run>())
                {
                    run.Foreground = GetForegroundColor(run.Text);
                }
            }
        }

        private void HandleItemsChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            using (DeclareChangeBlock())
            {
                switch (args.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        AddItems(args.NewItems.Cast<object>().ToArray());
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        RemoveItems(args.OldItems.Cast<object>().ToArray());
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        ReplaceItems(((IEnumerable) sender).Cast<object>().ToArray());
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        RemoveItems(args.OldItems.Cast<object>().ToArray());
                        AddItems(args.NewItems.Cast<object>().ToArray());
                        break;
                }
            }
        }

        private Brush GetForegroundColor(object item)
        {
            if (LineColorConverter != null)
            {
                return (Brush)LineColorConverter.Convert(item, typeof(Brush), null, CultureInfo.InvariantCulture);
            }
            
            return Foreground;
        }

        private void ClearItems()
        {
            _Paragraph.Inlines.Clear();
        }

        private void ReplaceItems(object[] items)
        {
            _Paragraph.Inlines.Clear();
            
            AddItems(items);
        }

        private void AddItems(object[] items)
        {
            var inlines = items.SelectMany(x =>
            {
                var value = ExtractValue(x);

                var newInlines = new List<Inline>();
                using (var reader = new StringReader(value))
                {
                    var line = reader.ReadLine();
                    
                    newInlines.Add(new Run(line) { Foreground = GetForegroundColor(x) });
                    newInlines.Add(new LineBreak());
                }

                return newInlines;

            }).ToArray();

            _Paragraph.Inlines.AddRange(inlines);
            CaretPosition = CaretPosition.DocumentEnd;
        }

        private string ExtractValue(object item)
        {
            var displayPath = ItemDisplayPath;
            if (displayPath == null)
            {
                return item == null ? string.Empty : item.ToString();
            }

            if (_DisplayPathProperty == null)
            {
                _DisplayPathProperty = item.GetType().GetProperty(displayPath);
            }

            var value = _DisplayPathProperty?.GetValue(item, null);
            return value == null ? string.Empty : value.ToString();
        }

        private void RemoveItems(object[] items)
        {
            foreach (var item in items)
            {
                var value = ExtractValue(item);

                var run = _Paragraph.Inlines
                    .Where(x => x is Run)
                    .Cast<Run>()
                    .FirstOrDefault(x => x.Text == value);

                if (run != null)
                {
                    _Paragraph.Inlines.Remove(run);
                }
            }
        }
    }
}
