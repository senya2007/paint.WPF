using Paint.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
using Paint.Helpers;
using PubSub;
using Paint.Helpers.Events;

namespace Paint.Model
{
    public class RectangleModel : IModel
    {
        public override void RemoveHandle()
        {
            base.RemoveHandle();

            ContainerClass.MousePosition = null;

            if (ContainerClass.LastGrid != null)
            {
                ContainerClass.LastGrid = null;
            }
        }


        public override void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && ContainerClass.MousePosition.HasValue)
            {
                Point currentPosition = Mouse.GetPosition(this.CurrentWindow.Canvas);

                if (ContainerClass.LastGrid == null)
                {
                    Rectangle rectangle = new Rectangle();
                    rectangle.Width = 50;
                    rectangle.Height = 50;
                    rectangle.Fill = new SolidColorBrush(Colors.White);
                    rectangle.Stroke = new SolidColorBrush(Colors.Black);
                    rectangle.StrokeThickness = 1;
                    rectangle.Margin = new Thickness(currentPosition.X, currentPosition.Y,0,0);

                    Grid grid = GetPackedGrid<Rectangle>(rectangle);

                    this.Publish<CurrentGrid>(new CurrentGrid(grid));

                    ContainerClass.LastGrid = grid;
                    ContainerClass.LastShape = rectangle;
                    ContainerClass.MousePosition = currentPosition;
                }
                else
                {
                    if (this.CurrentWindow.GetLayerGridElement() != null)
                    {
                        double currentHeight = currentPosition.Y - ContainerClass.MousePosition.Value.Y;
                        double currentWidth = currentPosition.X - ContainerClass.MousePosition.Value.X;

                        Tuple<double, double> calculateHeightAndWidth = new Tuple<double, double>
                            (
                                     (currentHeight < 0) ? ((ContainerClass.LastShape.ActualHeight + currentHeight) < 50 ? 0 : currentHeight) : currentHeight,
                                     (currentWidth < 0) ? ((ContainerClass.LastShape.ActualWidth + currentWidth) < 50 ? 0 : currentWidth) : currentWidth
                            );

                        ContainerClass.LastShape.Height += calculateHeightAndWidth.Item1;
                        ContainerClass.LastShape.Width += calculateHeightAndWidth.Item2;

                        ContainerClass.MousePosition = currentPosition;
                    }
                }
            }
        }
    }
}
