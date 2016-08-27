using Paint.Helpers.Events;
using Paint.Interface;
using PubSub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Paint.Model
{
    public class ImageModel : IModel
    {
        public override void ConnectHandle()
        {
            if (ContainerClass.OpenImageName != string.Empty && ContainerClass.OpenImageName != null)
            {
                Image image = new Image();
                image.Source = new BitmapImage(new Uri(ContainerClass.OpenImageName));

                Grid grid = GetPackedGrid<Image>(image);

                this.Publish<CurrentGrid>(new CurrentGrid(grid));
            }
        }

        public override void Transform(Shape shape)
        { }
    }
}
