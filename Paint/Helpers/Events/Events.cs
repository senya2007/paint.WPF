using Paint.Helpers.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Paint.Helpers.Events
{
    public class DeleteVisibilityButton
    {
        public Visibility Visibility { get; set; }
    }

    public class CurrentGrid
    {
        public CurrentGrid(Grid grid)
        {
            Grid = grid;
        }

        public Grid Grid { get; set; }
    }

    public class SelectedLayer
    {
        public Layer Layer { get; set; }

        public SelectedLayer(Layer layer)
        {
            Layer = layer;
        }
    }

    public class SelectedColorFill
    {
        public SolidColorBrush Color { get; set; }

        public SelectedColorFill(SolidColorBrush color)
        {
            Color = color;
        }
    }

    public class DeleteLayer
    {
        public Layer CurrentLayer { get; set; }

        public DeleteLayer(Layer currentLayer)
        {
            CurrentLayer = currentLayer;
        }
    }

    public class MoveLayer
    {
        public ActionWithLayer Action { get; set; }

        public MoveLayer(ActionWithLayer action)
        {
            Action = action;
        }
    }

    public class MixLayers
    {}

    public class AddLayer
    {
        public Layer Layer { get; set; }
        public int IndexToMove { get; set; }

        public AddLayer(Layer layer, int indexToMove)
        {
            Layer = layer;
            IndexToMove = indexToMove;
        }
    }

    public class IsEnableMoveAndMixButtons
    {
        public ObservableCollection<Layer> Layers { get; set; }

        public IsEnableMoveAndMixButtons(ObservableCollection<Layer> layers)
        {
            Layers = layers;
        }
    }
}
