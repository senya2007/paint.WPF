using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Paint.Helpers
{
    public class Layer
    {
        public string Name { get; set; }

        public Grid Grid { get; set; }
        public Grid LastGrid { get; set; }
        public Grid NextGrid { get; set; }

        public Layer LastLayer { get; set; }
        public Layer NextLayer { get; set; }

        public int _zIndex = 1;
        public int ZIndex
        {
            get { return _zIndex; }
            set
            {
                _zIndex = value;
                Panel.SetZIndex(this.Grid, _zIndex);
            }
        }

        public int GetZIndex
        {
            get { return Panel.GetZIndex(this.Grid); }
        }

        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                if (_isChecked == true)
                {
                    Grid.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    Grid.Visibility = System.Windows.Visibility.Hidden;
                }
            }
        }
    }
}
