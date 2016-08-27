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
    public class PencilModel : IModel
    {
        private InkCanvas CurrentInkCanvas;

        public PencilModel()
        {
            CurrentInkCanvas = new InkCanvas();
        }

        public override void RemoveHandle()
        {
            base.RemoveHandle();

            ContainerClass.MousePosition = null;

            if (CurrentInkCanvas != null)
            {
                CurrentInkCanvas.EditingMode = InkCanvasEditingMode.None;
                CurrentInkCanvas = null;
            }

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
                    InkCanvas pencil = new InkCanvas();
                    pencil.Background = null;

                    Grid grid = GetPackedGrid<InkCanvas>(pencil);

                    CurrentInkCanvas = pencil;

                    this.Publish<CurrentGrid>(new CurrentGrid(grid));
                    ContainerClass.LastGrid = grid;
                }
            }
        }

        public override void Transform(Shape shape)
        { }
    }
}
