using Paint.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Paint.Interface;

namespace Paint.Model
{
    public class HandleFill : HandlerShapes
    {
        public override void Handle(IModel shape)
        {
            if (shape is FillModel)
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
