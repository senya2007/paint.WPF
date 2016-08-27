using Paint.Helpers;
using Paint.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Paint.Model
{
    public class CursorModel : IModel
    {
        private IModel Model;
        private Shape _shape;

        public override void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && (ContainerClass.SelectedGrid != null))
            {
                if (_shape != null)
                {
                    Model.Transform(_shape);
                }
            }
        }


        public override void MouseDownHandle(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                ContainerClass.MousePosition = Mouse.GetPosition(this.CurrentWindow.Canvas);
                UIElement selectedCanvasElement = null;

                if (ContainerClass.SelectedGrid != null)
                {
                    var selectedLayerGrid = HelpMethods.GetElementFromMouseOver<Grid>(ContainerClass.SelectedGrid);
                    if (selectedLayerGrid != null)
                    {
                        selectedCanvasElement = HelpMethods.GetElementFromMouseOver<Grid>((Grid)selectedLayerGrid);

                        if (selectedCanvasElement != null)
                        {
                            try
                            {
                                if (selectedCanvasElement is InkCanvas || selectedCanvasElement is Image)
                                {
                                    //pencilHandler and ImageHandler
                                }
                                else
                                {
                                    var elementFromCanvas = HelpMethods.GetElementFromMouseOver<Canvas>((Canvas)selectedCanvasElement);

                                    if (elementFromCanvas is Ellipse)
                                    {
                                        SetModel<EllipseModel>();

                                        _shape = (Ellipse)elementFromCanvas;
                                        ContainerClass.PositionFromShape = Mouse.GetPosition(_shape);
                                    }
                                    else if (elementFromCanvas is Rectangle)
                                    {
                                        SetModel<RectangleModel>();
                                        _shape = (Rectangle)elementFromCanvas;
                                        ContainerClass.PositionFromShape = Mouse.GetPosition(_shape);
                                    }
                                    else if (elementFromCanvas is Line)
                                    {
                                        SetModel<LineModel>();
                                        _shape = (Line)elementFromCanvas;
                                        ContainerClass.PositionFromShape = Mouse.GetPosition(_shape);
                                    }
                                    else if (elementFromCanvas is Path)
                                    {
                                        SetModel<BezierModel>();
                                        _shape = (Path)elementFromCanvas;
                                        ContainerClass.PositionFromShape = Mouse.GetPosition(_shape);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }
                if (selectedCanvasElement == null)
                {
                    SetModel<CursorModel>();
                }
            }

        }

        private void SetModel<T>() where T : IModel, new()
        {
            T model = new T();
            model.CurrentWindow = this.CurrentWindow;
            Model = model;
        }

        public override void Transform(Shape shape)
        { }
    }
}
