using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Paint
{
    public static class ContainerClass
    {
        public static Point? MousePosition { get; set; }
        public static Grid SelectedGrid { get; set; }
        public static Grid LastGrid { get; set; }
        public static Shape LastShape { get; set; }

        public static int NumberLayer { get; set; } = 1;

        public static SolidColorBrush SelectedColorFill { get; set; }

        public static string OpenImageName { get; set; }

        public static Point? PositionFromShape { get; set; }
    }
}
