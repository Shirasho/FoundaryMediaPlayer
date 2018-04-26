using System.Windows.Input;
using Prism.Mvvm;

namespace FoundaryMediaPlayer.Controls.Models
{
    /// <summary>
    /// The base class for custom control models.
    /// </summary>
    public abstract class ControlModelBase : BindableBase
    {
        private bool _bIsEnabled;
        private string _Text;
        private ICommand _Command;

        /// <summary>
        /// Whether the control is enabled.
        /// </summary>
        public bool bIsEnabled
        {
            get => _bIsEnabled;
            set => SetProperty(ref _bIsEnabled, value);
        }

        /// <summary>
        /// The text value of the control.
        /// </summary>
        public string Text
        {
            get => _Text;
            set => SetProperty(ref _Text, value);
        }

        /// <summary>
        /// The command to execute when the control is clicked.
        /// </summary>
        public ICommand Command
        {
            get => _Command;
            set => SetProperty(ref _Command, value);
        }
    }
}
