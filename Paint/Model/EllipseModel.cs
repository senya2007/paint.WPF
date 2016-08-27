using Paint.Helpers;
using Paint.Helpers.Events;
using Paint.Interface;
using PubSub;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Paint.Model
{
    public class EllipseModel : IModel
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
                    Ellipse ellispse = new Ellipse();
                    ellispse.Width = 50;
                    ellispse.Height = 50;
                    ellispse.Fill = new SolidColorBrush(System.Windows.Media.Colors.White);
                    ellispse.Stroke = new SolidColorBrush(Colors.Black);
                    ellispse.StrokeThickness = 1;
                    ellispse.Margin = new Thickness(currentPosition.X, currentPosition.Y, 0, 0);

                    Grid grid = GetPackedGrid<Ellipse>(ellispse);

                    this.Publish<CurrentGrid>(new CurrentGrid(grid));

                    ContainerClass.LastGrid = grid;
                    ContainerClass.LastShape = ellispse;
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
