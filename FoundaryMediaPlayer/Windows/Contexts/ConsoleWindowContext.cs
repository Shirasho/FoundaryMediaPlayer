using System.Collections.ObjectModel;
using log4net;

namespace FoundaryMediaPlayer.Windows.Contexts
{
    public sealed class FConsoleWindowContext : AWindowContext
    {
        /// <inheritdoc />
        protected override ILog Logger { get; } = LogManager.GetLogger(nameof(FConsoleWindowContext));

        public ObservableCollection<string> Output { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> Input { get; } = new ObservableCollection<string>();
    }
}
