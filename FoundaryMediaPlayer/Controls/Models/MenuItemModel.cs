using System.Collections.ObjectModel;

namespace FoundaryMediaPlayer.Controls.Models
{
    /// <summary>
    /// A backing model for menu items.
    /// </summary>
    public class MenuItemModel : ControlModelBase
    {
        private ObservableCollection<MenuItemModel> _Children = new ObservableCollection<MenuItemModel>();
        
        /// <summary>
        /// A collection of children this menu item has.
        /// </summary>
        public ObservableCollection<MenuItemModel> Children
        {
            get => _Children;
            set => SetProperty(ref _Children, value);
        }
    }
}
