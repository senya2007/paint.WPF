using Paint.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint.Helpers
{
    public abstract class HandlerShapes
    {
        public HandlerShapes Successor { get; set; }
        public abstract void Handle(IModel shape);
    }
}
