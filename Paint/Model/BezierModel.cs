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
using Paint.Helpers.Events;
using PubSub;

namespace Paint.Model
{
    public class BezierModel : IModel
    {
        public BezierModel()
        {
            BezierDots = new List<Point>();
        }

        private List<Point> BezierDots;

        public override void RemoveHandle()
        {
            base.RemoveHandle();

            ContainerClass.MousePosition = null;

            if (ContainerClass.LastGrid != null)
            {
                ContainerClass.LastGrid = null;
            }

            if (TemporaryDots.Count != 0)
            {
                DeleteTemporaryDots();
            }

            BezierDots.Clear();
        }

        public void DeleteTemporaryDots()
        {
            foreach (var item in TemporaryDots)
            {
                this.CurrentWindow.Canvas.Children.Remove(item);
            }

            TemporaryDots.Clear();
        }

        public void CreateTemporaryDot(Point point)
        {
            Ellipse ellispse = new Ellipse();
            ellispse.Width = 5;
            ellispse.Height = 5;
            ellispse.Fill = new SolidColorBrush(System.Windows.Media.Colors.White);
            ellispse.Stroke = new SolidColorBrush(Colors.Black);
            ellispse.StrokeThickness = 1;
            ellispse.Margin = new Thickness(point.X, point.Y, 0, 0);

            Grid grid = new Grid();
            grid.Children.Add(ellispse);

            TemporaryDots.Add(grid);
            this.CurrentWindow.Canvas.Children.Add(grid);
        }

        public override void MouseDownHandle(object sender, MouseButtonEventArgs e)
        {
            base.MouseDownHandle(sender, e);

            Point currentPosition = Mouse.GetPosition(this.CurrentWindow.Canvas);

            if (ContainerClass.LastGrid == null)
            {
                if (BezierDots.Count != 4)
                {
                    BezierDots.Add(currentPosition);
                    CreateTemporaryDot(currentPosition);
                }

                if (BezierDots.Count == 4)
                {
                    DeleteTemporaryDots();

                    BezierSegment bezier = new BezierSegment
                    {
                        Point1 = BezierDots[1],
                        Point2 = BezierDots[2],
                        Point3 = BezierDots[3]
                    };

                    PathFigure figure = new PathFigure();
                    figure.StartPoint = BezierDots[0];
                    figure.Segments.Add(bezier);

                    Path path = new Path();
                    path.Stroke = new SolidColorBrush(Colors.Black);
                    path.StrokeThickness = 2;
                    path.Data = new PathGeometry(new PathFigure[] { figure });

                    Grid grid = GetPackedGrid<Path>(path);

                    this.Publish<CurrentGrid>(new CurrentGrid(grid));

                    ContainerClass.LastGrid = grid;
                    ContainerClass.LastShape = path;
                    ContainerClass.MousePosition = currentPosition;
                }
            }
        }
    }
}
