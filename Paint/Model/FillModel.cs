using Paint.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Input;
using Paint.Helpers;
using System.Windows.Controls;
using System.Windows;

namespace Paint.Model
{
    public class FillModel : IModel
    {
        private Shape _shape;

        public override void MouseDownHandle(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                ContainerClass.MousePosition = Mouse.GetPosition(this.CurrentWindow.Canvas);

                try
                {
                    if (ContainerClass.SelectedGrid != null)
                    {
                        var selectedLayerGrid = HelpMethods.GetElementFromMouseOver<Grid>(ContainerClass.SelectedGrid);
                        if (selectedLayerGrid != null)
                        {
                            var selectedCanvasElement = HelpMethods.GetElementFromMouseOver<Grid>((Grid)selectedLayerGrid);

                            if (selectedCanvasElement != null)
                            {
                                if (selectedCanvasElement is InkCanvas)
                                {
                                    foreach (var stroke in ((InkCanvas)selectedCanvasElement).Strokes)
                                    {
                                        stroke.DrawingAttributes.Color = ContainerClass.SelectedColorFill.Color;
                                    }
                                }
                                else
                                {
                                    var elementFromCanvas = HelpMethods.GetElementFromMouseOver<Canvas>((Canvas)selectedCanvasElement);

                                    if (elementFromCanvas is Ellipse)
                                    {

                                        _shape = (Ellipse)elementFromCanvas;
                                        _shape.Fill = ContainerClass.SelectedColorFill;
                                        ContainerClass.PositionFromShape = Mouse.GetPosition(_shape);
                                    }
                                    else if (elementFromCanvas is Rectangle)
                                    {
                                        _shape = (Rectangle)elementFromCanvas;
                                        _shape.Fill = ContainerClass.SelectedColorFill;
                                        ContainerClass.PositionFromShape = Mouse.GetPosition(_shape);
                                    }
                                    else if (elementFromCanvas is Line)
                                    {
                                        _shape = (Line)elementFromCanvas;
                                        _shape.Stroke = ContainerClass.SelectedColorFill;
                                        ContainerClass.PositionFromShape = Mouse.GetPosition(_shape);
                                    }
                                    else if (elementFromCanvas is Path)
                                    {
                                        _shape = (Path)elementFromCanvas;
                                        _shape.Stroke = ContainerClass.SelectedColorFill;
                                        ContainerClass.PositionFromShape = Mouse.GetPosition(_shape);
                                    }
                                }
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error Fill. Select Cursor and after select Fill");
                }
            }
        }

        public override void Transform(Shape shape)
        { }
    }
}
