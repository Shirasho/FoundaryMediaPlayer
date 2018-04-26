using FluentAssertions;
using MahApps.Metro.Controls;
using Prism.Mvvm;

namespace FoundaryMediaPlayer.Contexts
{
    /// <summary>
    /// The base class for a window context.
    /// </summary>
    public abstract class WindowContext : BindableBase
    {
        private string _Title;

        /// <summary>
        /// The title of the window.
        /// </summary>
        public string Title
        {
            get { return _Title; }
            set { SetProperty(ref _Title, value); }
        }

        /// <summary>
        /// The owner of this context.
        /// </summary>
        public MetroWindow Owner { get; private set; }

        /// <summary>
        /// Sets the owner of this context.
        /// </summary>
        /// <param name="owner"></param>
        public virtual void SetOwner(MetroWindow owner)
        {
            Owner.Should().BeNull();
            owner.Should().NotBeNull();

            Owner = Owner;
        }
    }
}
