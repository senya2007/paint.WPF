using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Paint.Interface
{
    public abstract class IModel : IHandleModel
    {
        public IModel()
        {
            TemporaryDots = new List<Grid>();
        }

        public string Name { get; set; }
        public List<Grid> TemporaryDots { get; set; }

        public virtual void Transform(Shape shape)
        {
            Point currentPosition = Mouse.GetPosition(this.CurrentWindow.Canvas);
            Point shapeMousePosition = Mouse.GetPosition(shape);

            shape.Margin = new Thickness(0, 0, 0, 0);

            double calculateX = 0;
            double calculateY = 0;

            if (shape.ActualWidth > shapeMousePosition.X)
            {
                calculateX = currentPosition.X - shapeMousePosition.X;
            }

            if (shape.ActualHeight > shapeMousePosition.Y)
            {
                calculateY = currentPosition.Y - shapeMousePosition.Y;
            }
            //                 Текущая позиция мыши   Актуальная ширина                          Позиция мыши при захвате фигуры
            Canvas.SetLeft(shape, currentPosition.X - (shape.ActualWidth - (shape.ActualWidth - ContainerClass.PositionFromShape.Value.X)));
            Canvas.SetTop(shape, currentPosition.Y - (shape.ActualHeight - (shape.ActualHeight - ContainerClass.PositionFromShape.Value.Y)));
        }
       
        public Grid GetPackedGrid<T>(T model) where T : FrameworkElement
        {
            Canvas canvas = new Canvas();
            canvas.Children.Add(model);

            Grid grid = new Grid();
            grid.Children.Add(canvas);

            return grid;
        }
    }
}
