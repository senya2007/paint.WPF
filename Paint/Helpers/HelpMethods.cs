using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Paint.Helpers
{
    public static class HelpMethods
    {
        public static UIElement GetElementFromMouseOver<T>(T item) where T: Panel
        {
            var ellipse = item.Children.OfType<UIElement>().Where(e => e.Visibility == Visibility.Visible && e.IsMouseOver);
            if(ellipse != null)
            {
                return ellipse.FirstOrDefault();
            }
            return null;
        }

        public static UIElement GetElementFrom<T>(T item) where T : Panel
        {
            var ellipse = item.Children.OfType<UIElement>();
            if (ellipse != null)
            {
                return ellipse.FirstOrDefault();
            }
            return null;
        }

        public static List<UIElement> GetElementsFrom<T>(T item) where T : Panel
        {
            var elements = item.Children.OfType<UIElement>();
            if (elements != null)
            {
                return elements.ToList();
            }
            return null;
        }

        public static UIElement GetLayerGridElement(this MainWindow window)
        {
            var elems = window.Canvas.Children.OfType<UIElement>().Where(e => e.Visibility == Visibility.Visible && e.IsMouseOver);
            return elems.DefaultIfEmpty(null).First();
        }

        public static IEnumerable<UIElement> GetElements(this MainWindow window)
        {
            var elem = window.GetLayerGridElement();
            if (elem != null)
            {
                var el = ((Grid)elem).Children.OfType<UIElement>();
                return el;
            }
            return null;
        }
    }
}
