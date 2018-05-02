using System;
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
using System.Windows.Input;
using System.Windows.Media;

namespace FoundaryMediaPlayer.Controls
{
    public sealed class ConsoleInput : RichTextBox
    {
        /// <summary>
        /// The terminal prompt to be displayed.
        /// </summary>
        public static readonly DependencyProperty PromptProperty = DependencyProperty.Register("Prompt",
            typeof(string),
            typeof(ConsoleInput),
            new PropertyMetadata(default(string), OnPromptChanged));

        /// <summary>
        /// The prompt of the terminal.
        /// </summary>
        public string Prompt
        {
            get { return (string)GetValue(PromptProperty); }
            set { SetValue(PromptProperty, value); }
        }

        /// <summary>
        /// The items to be displayed in the terminal window, e.g. an ObservableCollection.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource",
            typeof(IEnumerable),
            typeof(ConsoleInput),
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
            typeof(ConsoleInput),
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
            typeof(ConsoleInput),
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
            typeof(ConsoleInput),
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
        /// The current the editable line in the terminal, there is only one editable line in the terminal and this is at the bottom of the content.
        /// </summary>
        public static readonly DependencyProperty LineProperty = DependencyProperty.Register("Line",
            typeof(string),
            typeof(ConsoleInput),
            new PropertyMetadata(default(string)));

        /// <summary>
        /// The current editable line of the terminal (bottom line).
        /// </summary>
        public string Line
        {
            get { return (string)GetValue(LineProperty); }
            set { SetValue(LineProperty, value); }
        }

        /// <summary>
        /// The height of each line in the terminal window, optional field with a default value of 10.
        /// </summary>
        public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register("ItemHeight",
            typeof(int),
            typeof(ConsoleInput),
            new PropertyMetadata(10, OnItemHeightChanged));

        /// <summary>
        /// The individual line height for the bound items.
        /// </summary>
        public int ItemHeight
        {
            get { return (int)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        /// <summary>
        /// Event fired when the user presses the Enter key.
        /// </summary>
        public event EventHandler LineEntered;

        private Run _PromptInline { get; }
        private Paragraph _Paragraph { get; }
        private INotifyCollectionChanged _NotifyChanged { get; set; }
        private PropertyInfo _DisplayPathProperty { get; set; }
        private List<string> _Buffer { get; } = new List<string>();

        public ConsoleInput()
        {
            _Paragraph = new Paragraph
            {
                Margin = ItemsMargin,
                LineHeight = ItemHeight
            };

            IsUndoEnabled = false;
            _PromptInline = new Run(Prompt);
            Document = new FlowDocument(_Paragraph);

            AddPrompt();

            TextChanged += (s, e) =>
            {
                Line = AggregateAfterPrompt();
                ScrollToEnd();
            };

            DataObject.AddPastingHandler(this, PasteCommand);
            DataObject.AddCopyingHandler(this, CopyCommand);

            SetResourceReference(StyleProperty, "ConsoleInputDefault");
        }

        /// <summary>
        /// Processes every key pressed when the control has focus.
        /// </summary>
        /// <param name="args">The key pressed arguments.</param>
        protected override void OnPreviewKeyDown(KeyEventArgs args)
        {
            base.OnPreviewKeyDown(args);

            switch (args.Key)
            {
                case Key.A:
                    args.Handled = HandleSelectAllKeys();
                    break;
                case Key.X:
                case Key.C:
                case Key.V:
                    args.Handled = HandleCopyKeys(args);
                    break;
                case Key.Left:
                    args.Handled = HandleLeftKey();
                    break;
                case Key.Right:
                    break;
                case Key.PageDown:
                case Key.PageUp:
                    args.Handled = true;
                    break;
                case Key.Escape:
                    ClearAfterPrompt();
                    args.Handled = true;
                    break;
                case Key.Up:
                case Key.Down:
                    args.Handled = HandleUpDownKeys(args);
                    break;
                case Key.Delete:
                    args.Handled = HandleDeleteKey();
                    break;
                case Key.Back:
                    args.Handled = HandleBackspaceKey();
                    break;
                case Key.Enter:
                    HandleEnterKey();
                    args.Handled = true;
                    break;
                default:
                    args.Handled = HandleAnyOtherKey();
                    break;
            }
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

        private static void OnPromptChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue == args.OldValue)
            {
                return;
            }

            var terminal = (ConsoleInput)d;
            terminal.HandlePromptChanged((string)args.NewValue);
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue == args.OldValue)
            {
                return;
            }

            var terminal = (ConsoleInput)d;
            terminal.HandleItemsSourceChanged((IEnumerable)args.NewValue);
        }

        private static void OnItemsMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue == args.OldValue)
            {
                return;
            }

            var terminal = (ConsoleInput)d;
            terminal._Paragraph.Margin = (Thickness)args.NewValue;
        }

        private static void OnItemHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue == args.OldValue)
            {
                return;
            }

            var terminal = (ConsoleInput)d;
            terminal._Paragraph.LineHeight = (int)args.NewValue;
        }

        private static void OnLineConverterChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue == args.OldValue)
            {
                return;
            }

            var terminal = (ConsoleInput)d;
            terminal.HandleLineConverterChanged();
        }

        private static void OnDisplayPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue == args.OldValue)
            {
                return;
            }

            var terminal = (ConsoleInput)d;
            terminal._DisplayPathProperty = null;
        }

        private void CopyCommand(object sender, DataObjectCopyingEventArgs args)
        {
            if (!string.IsNullOrEmpty(Selection.Text))
            {
                args.DataObject.SetData(typeof(string), Selection.Text);
            }

            args.Handled = true;
        }

        private void PasteCommand(object sender, DataObjectPastingEventArgs args)
        {
            var text = (string)args.DataObject.GetData(typeof(string));

            if (!string.IsNullOrEmpty(text))
            {
                if (Selection.Start != Selection.End)
                {
                    Selection.Start.DeleteTextInRun(Selection.Text.Length);
                    Selection.Start.InsertTextInRun(text);

                    var selectionEnd = Selection.Start.GetPositionAtOffset(text.Length);
                    CaretPosition = selectionEnd;
                }
                else
                {
                    AddLine(text);
                }
            }

            args.CancelCommand();
            args.Handled = true;
        }

        private void HandlePromptChanged(string prompt)
        {
            if (_PromptInline == null)
            {
                return;
            }

            _PromptInline.Text = prompt;
        }

        private void HandleItemsSourceChanged(IEnumerable items)
        {
            if (items == null)
            {
                _Paragraph.Inlines.Clear();
                AddPrompt();

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
                        ReplaceItems(((IEnumerable)sender).Cast<object>().ToArray());
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

        private static TextPointer GetTextPointer(TextPointer textPointer, LogicalDirection direction)
        {
            var currentTextPointer = textPointer;
            while (currentTextPointer != null)
            {
                var nextPointer = currentTextPointer.GetNextContextPosition(direction);
                if (nextPointer == null)
                {
                    return null;
                }

                if (nextPointer.GetPointerContext(direction) == TextPointerContext.Text)
                {
                    return nextPointer;
                }

                currentTextPointer = nextPointer;
            }

            return null;
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
            var command = AggregateAfterPrompt();
            ClearAfterPrompt();
            _Paragraph.Inlines.Remove(_PromptInline);

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
            AddPrompt();
            _Paragraph.Inlines.Add(new Run(command));
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

        private bool HandleCopyKeys(KeyEventArgs args)
        {
            if (args.Key == Key.C)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    return false;
                }

                var promptEnd = _PromptInline.ContentEnd;
                var pos = CaretPosition.CompareTo(promptEnd);
                var selectionPos = Selection.Start.CompareTo(CaretPosition);

                return pos < 0 || selectionPos < 0;
            }

            if (args.Key == Key.X || args.Key == Key.V)
            {
                var promptEnd = _PromptInline.ContentEnd;

                var pos = CaretPosition.CompareTo(promptEnd);
                var selectionPos = Selection.Start.CompareTo(CaretPosition);

                return pos < 0 || selectionPos < 0;
            }

            return false;
        }

        private bool HandleSelectAllKeys()
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                Selection.Select(Document.ContentStart, Document.ContentEnd);

                return true;
            }

            return HandleAnyOtherKey();
        }

        private bool HandleUpDownKeys(KeyEventArgs args)
        {
            var pos = CaretPosition.CompareTo(_PromptInline.ContentEnd);

            if (pos < 0)
            {
                return false;
            }

            if (!_Buffer.Any())
            {
                return true;
            }

            ClearAfterPrompt();

            string existingLine;
            if (args.Key == Key.Down)
            {
                existingLine = _Buffer[_Buffer.Count - 1];
                _Buffer.RemoveAt(_Buffer.Count - 1);
                _Buffer.Insert(0, existingLine);
            }
            else
            {
                existingLine = _Buffer[0];
                _Buffer.RemoveAt(0);
                _Buffer.Add(existingLine);
            }

            AddLine(existingLine);

            return true;
        }

        private void HandleEnterKey()
        {
            var line = AggregateAfterPrompt();

            ClearAfterPrompt();

            Line = line;
            _Buffer.Insert(0, line);

            CaretPosition = Document.ContentEnd;

            OnLineEntered();
        }

        private bool HandleAnyOtherKey()
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                return false;
            }

            var promptEnd = _PromptInline.ContentEnd;
            var pos = CaretPosition.CompareTo(promptEnd);
            return pos < 0;
        }

        private bool HandleBackspaceKey()
        {
            var promptEnd = _PromptInline.ContentEnd;

            var textPointer = GetTextPointer(promptEnd, LogicalDirection.Forward);
            if (textPointer == null)
            {
                var pos = CaretPosition.CompareTo(promptEnd);

                if (pos <= 0)
                {
                    return true;
                }
            }
            else
            {
                var pos = CaretPosition.CompareTo(textPointer);
                if (pos <= 0)
                {
                    return true;
                }
            }

            return false;
        }

        private bool HandleLeftKey()
        {
            var promptEnd = _PromptInline.ContentEnd;

            var textPointer = GetTextPointer(promptEnd, LogicalDirection.Forward);
            if (textPointer == null)
            {
                var pos = CaretPosition.CompareTo(promptEnd);

                if (pos == 0)
                {
                    return true;
                }
            }
            else
            {
                var pos = CaretPosition.CompareTo(textPointer);
                if (pos == 0)
                {
                    return true;
                }
            }

            return false;
        }

        private bool HandleDeleteKey()
        {
            var pos = CaretPosition.CompareTo(_PromptInline.ContentEnd);

            return pos < 0;
        }

        private void OnLineEntered()
        {
            var handler = LineEntered;

            handler?.Invoke(this, EventArgs.Empty);
        }

        private void AddLine(string line)
        {
            CaretPosition = CaretPosition.DocumentEnd;

            var inline = new Run(line);
            _Paragraph.Inlines.Add(inline);

            CaretPosition = Document.ContentEnd;
        }

        private string AggregateAfterPrompt()
        {
            var inlineList = _Paragraph.Inlines.ToList();
            var promptIndex = inlineList.IndexOf(_PromptInline);

            return inlineList.Where((x, i) => i > promptIndex)
                .Where(x => x is Run)
                .Cast<Run>()
                .Select(x => x.Text)
                .Aggregate(string.Empty, (current, part) => current + part);
        }

        private void ClearAfterPrompt()
        {
            var inlineList = _Paragraph.Inlines.ToList();
            var promptIndex = inlineList.IndexOf(_PromptInline);

            foreach (var inline in inlineList.Where((x, i) => i > promptIndex))
            {
                _Paragraph.Inlines.Remove(inline);
            }
        }

        private void AddPrompt()
        {
            _Paragraph.Inlines.Add(_PromptInline);
            _Paragraph.Inlines.Add(new Run());

            CaretPosition = Document.ContentEnd;
        }
    }
}
