using Paint.Interface;
using Paint.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint.Helpers.Handle
{
    public class HandleEllipse : HandlerShapes
    {
        public override void Handle(IModel shape)
        {
            if (shape is EllipseModel)
            {
                shape.ConnectHandle();
            }
            else if (Successor != null)
            {
                Successor.Handle(shape);
            }
        }
    }
}
