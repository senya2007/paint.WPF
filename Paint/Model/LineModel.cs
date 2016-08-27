using Paint.Helpers;
using Paint.Helpers.Events;
using Paint.Interface;
using PubSub;
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

namespace Paint.Model
{
    public class LineModel : IModel
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
                    Line line = new Line();
                    line.X1 = currentPosition.X;
                    line.Y1 = currentPosition.Y;
                    line.SnapsToDevicePixels = true;
                    line.StrokeThickness = 2;
                    line.Stroke = new SolidColorBrush(Colors.Black);

                    Grid grid = GetPackedGrid<Line>(line);

                    this.Publish<CurrentGrid>(new CurrentGrid(grid));
                    ContainerClass.LastGrid = grid;
                    ContainerClass.LastShape = line;
                    ContainerClass.MousePosition = currentPosition;
                }
                else
                {
                    if (ContainerClass.LastShape is Line)
                    {
                        try
                        {
                            ((Line)ContainerClass.LastShape).X2 = currentPosition.X;
                            ((Line)ContainerClass.LastShape).Y2 = currentPosition.Y;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Line error! Select Cursor and after select Line");

                            Log.Write(ex);
                        }
                    }
                }
            }
        }
    }
}
