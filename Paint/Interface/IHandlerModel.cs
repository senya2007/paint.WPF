using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Paint.Interface
{
    public abstract class IHandleModel
    {
        public MainWindow CurrentWindow { get; set; }

        public virtual void ConnectHandle()
        {
            Mouse.AddMouseDownHandler(this.CurrentWindow.Canvas, MouseDownHandle);
            Mouse.AddMouseUpHandler(this.CurrentWindow.Canvas, MouseUpHandle);
            Mouse.AddMouseMoveHandler(this.CurrentWindow.Canvas, MouseMoveHandler);
        }

        public virtual void RemoveHandle()
        {
            Mouse.RemoveMouseDownHandler(this.CurrentWindow.Canvas, MouseDownHandle);
            Mouse.RemoveMouseUpHandler(this.CurrentWindow.Canvas, MouseUpHandle);
            Mouse.RemoveMouseMoveHandler(this.CurrentWindow.Canvas, MouseMoveHandler);
        }

        public virtual void MouseUpHandle(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                ContainerClass.MousePosition = null;
            }
        }

        public virtual void MouseDownHandle(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                ContainerClass.MousePosition = Mouse.GetPosition(this.CurrentWindow.Canvas);
            }
        }

        public virtual void MouseMoveHandler(object sender, MouseEventArgs e)
        { }

    }
}
