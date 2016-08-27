using GalaSoft.MvvmLight;
using Paint.Helpers;
using Paint.Helpers.Events;
using Paint.ViewModel;
using PubSub;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Paint.Helpers.Enums;

namespace Paint.View
{
    /// <summary>
    /// Логика взаимодействия для Layers.xaml
    /// </summary>
    public partial class Layers : UserControl, INotifyPropertyChanged
    {
        public ObservableCollection<Layer> Items { get; set; }
        public static Layers control;
        public static bool StartupLayer;
        private ObservableCollection<Layer> CheckedLayersForMixInOneLayer { get; set; }
        public Layer SelectedLayer { get; set; }

        public Layers()
        {
            InitializeComponent();
            Items = new ObservableCollection<Layer>();
            CheckedLayersForMixInOneLayer = new ObservableCollection<Layer>();

            this.Subscribe<DeleteVisibilityButton>
                (data => ClickDeleteButton());

            this.Subscribe<MoveLayer>
                (data => ActionLayer(data.Action));

            this.Subscribe<MixLayers>
                (data => Mix2Layers());

            ItemsSource.Tag = DeleteVisibilityButton;
            ItemsSource.SelectionChanged += ItemsSource_SelectionChanged;
            StartupLayer = true;

            ((INotifyCollectionChanged)ItemsSource.Items).CollectionChanged += Layers_CollectionChanged;
        }

        #region Methods

        private void ActionLayer(ActionWithLayer action)
        {
            if (action == ActionWithLayer.Up)
            {
                UpZindexButton();
            }
            else
                if (action == ActionWithLayer.Down)
            {
                DownZindexButton();
            }
        }

        private void Layers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                    if (ItemsSource.SelectedItem == null)
                    {
                        ItemsSource.SelectedItem = Items.FirstOrDefault();
                    }
                    else
                    {
                        ItemsSource.SelectedItem = Items.LastOrDefault();
                    }
            }
        }

        private void ItemsSource_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedLayer = (Layer)ItemsSource.SelectedItem;
            if (SelectedLayer != null)
            {
                this.Publish<SelectedLayer>(new SelectedLayer(SelectedLayer));
            }
        }

        private void ClickDeleteButton()
        {
            DeleteVisibilityButton = (DeleteVisibilityButton == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
            ItemsSource.Tag = DeleteVisibilityButton;
        }


        private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            control = d as Layers;

            if (e.OldValue != null)
            {
                var collection = (INotifyCollectionChanged)e.OldValue;
                collection.CollectionChanged -= control.LayersCollectionChanged;
            }
            if (e.NewValue != null)
            {
                var collection = (ObservableCollection<Layer>)e.NewValue;
                collection.CollectionChanged += control.LayersCollectionChanged;

                if (StartupLayer)
                {
                    StartupLayer = false;
                    AddItem(control, collection);
                }
            }
        }

        private void LayersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ObservableCollection<Layer> collection = new ObservableCollection<Layer>();
            if (e.NewItems != null)
            {
                foreach (var newLayer in e.NewItems)
                {
                    collection.Add((Layer)newLayer);
                }

                AddItem(this, collection);
            }
        }

        private static void AddItem(Layers userControl, ObservableCollection<Layer> collection)
        {
            if (collection.Count > 0)
            {
                foreach (var item in collection)
                {
                    userControl.Items.Add(item);
                }
            }
        }

        private static void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var items = sender as ObservableCollection<Layer>;
            control.Items.Clear();

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                AddItem(control, items);
            }
        }

        private void DeleteButton(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var deleteLayer = button.DataContext as Layer;

            DeleteLayer(deleteLayer);

        }

        public void DeleteLayer(Layer layer)
        {
            if (layer != null)
            {
                if (layer.LastLayer != null)
                {
                    layer.LastLayer.NextGrid = layer.NextGrid;
                    layer.LastLayer.NextLayer = layer.NextLayer;
                }
                if (layer.NextLayer != null)
                {
                    layer.NextLayer.LastGrid = layer.LastGrid;
                    layer.NextLayer.LastLayer = layer.LastLayer;
                }
                RaisePropertyChanged("SelectedLayer");
                Items.Remove(layer);
            }

            this.Publish<DeleteLayer>(new DeleteLayer(layer));
        }

        private void DownZindexButton()
        {
            if (SelectedLayer.NextGrid != null)
            {
                var nextZindexGrid = Panel.GetZIndex(SelectedLayer.NextGrid);
                var selectedZindexGrid = Panel.GetZIndex(SelectedLayer.Grid);

                if (nextZindexGrid > selectedZindexGrid)
                {
                    var indexSelectedItem = GetIndexSelectedLayer(); 

                    Layer LayerBeforeSelectedLayer = null;
                    Layer LayerAfterSelectedLayer = (Layer)ItemsSource.Items.GetItemAt(indexSelectedItem + 1);

                    if (indexSelectedItem - 1 >= 0)
                    {
                        LayerBeforeSelectedLayer = (Layer)ItemsSource.Items.GetItemAt(indexSelectedItem - 1);
                    }
                    if (LayerBeforeSelectedLayer != null)
                    {
                        LayerBeforeSelectedLayer.NextGrid = LayerAfterSelectedLayer.Grid;
                        LayerBeforeSelectedLayer.NextLayer = LayerAfterSelectedLayer;
                    }


                    var selectedNextGrid = SelectedLayer.NextGrid;
                    var selectedNextLayer = SelectedLayer.NextLayer;
                    var nextLayerInNextLayer = SelectedLayer.NextLayer.NextLayer;
                    var nextGridInNextLayer = SelectedLayer.NextLayer.NextGrid;

                    var currentGrid = SelectedLayer.LastGrid;
                    var currentLayer = SelectedLayer.LastLayer;

                    LayerAfterSelectedLayer.LastGrid = currentGrid;
                    LayerAfterSelectedLayer.LastLayer = currentLayer;

                    SetSelectedLayer(nextZindexGrid, selectedNextGrid, selectedNextLayer, nextLayerInNextLayer,  nextGridInNextLayer);

                    SetLastLayerInSelectedLayer();
                    
                    Panel.SetZIndex(SelectedLayer.Grid, nextZindexGrid);
                    Panel.SetZIndex(SelectedLayer.LastGrid, selectedZindexGrid);

                    var selectedLayer = GetIndexSelectedLayer();
                    Items.Move(selectedLayer, selectedLayer + 1);
                }
            }
        }

        private int GetIndexSelectedLayer()
        {
            return ItemsSource.Items.IndexOf(SelectedLayer);
        }

        private void SetLastLayerInSelectedLayer()
        {
            SelectedLayer.LastLayer.NextLayer = SelectedLayer;
            SelectedLayer.LastLayer.NextGrid = SelectedLayer.Grid;
        }

        private void UpZindexButton()
        {
            if (SelectedLayer.LastGrid != null)
            {
                var lastZindexGrid = Panel.GetZIndex(SelectedLayer.LastGrid);
                var selectedZindexGrid = Panel.GetZIndex(SelectedLayer.Grid);

                if (lastZindexGrid < selectedZindexGrid)
                {
                    var indexSelectedItem = GetIndexSelectedLayer();

                    Layer LayerBeforeSelectedLayer = (Layer)ItemsSource.Items.GetItemAt(indexSelectedItem - 1);
                    Layer LayerAfterSelectedLayer = null;


                    if (ItemsSource.Items.Count > indexSelectedItem + 1)
                    {
                        LayerAfterSelectedLayer = (Layer)ItemsSource.Items.GetItemAt(indexSelectedItem + 1);
                    }
                    if (LayerAfterSelectedLayer != null)
                    {
                        LayerAfterSelectedLayer.LastGrid = LayerBeforeSelectedLayer.Grid;
                        LayerAfterSelectedLayer.LastLayer = LayerBeforeSelectedLayer;
                    }

                    var tempCurrentGrid = SelectedLayer.Grid;
                    var currentName = SelectedLayer.Name;


                    var lastLayerInCurrentLayer = SelectedLayer.LastLayer;
                    var gridInLastLayer = SelectedLayer.LastLayer.Grid;
                    var lastGridInLastLayer = SelectedLayer.LastLayer.LastGrid;
                    var lastLayerInLastLayer = SelectedLayer.LastLayer.LastLayer;
                    var currentLayer = SelectedLayer.NextLayer;
                    var currentGrid = SelectedLayer.NextGrid;

                    LayerBeforeSelectedLayer.NextGrid = currentGrid;
                    LayerBeforeSelectedLayer.NextLayer = currentLayer;

                    SetSelectedLayer(lastZindexGrid, lastGridInLastLayer, lastLayerInLastLayer, lastLayerInCurrentLayer, gridInLastLayer);

                    SetNextLayerInSelectedLayer();
                    

                    Panel.SetZIndex(SelectedLayer.Grid, lastZindexGrid);
                    Panel.SetZIndex(SelectedLayer.NextGrid, selectedZindexGrid);

                    var selectedLayer = GetIndexSelectedLayer();
                    Items.Move(selectedLayer, selectedLayer - 1);
                }
            }
        }

        private void SetNextLayerInSelectedLayer()
        {
            SelectedLayer.NextLayer.LastLayer = SelectedLayer;
            SelectedLayer.NextLayer.LastGrid = SelectedLayer.Grid;
        }

        private void SetSelectedLayer(int ZindexGrid, Grid lastGrid, Layer lastLayer, Layer nextLayer, Grid nextGrid)
        {
            SelectedLayer.ZIndex = ZindexGrid;
            SelectedLayer.LastGrid = lastGrid;
            SelectedLayer.LastLayer = lastLayer;
            SelectedLayer.NextLayer = nextLayer;
            SelectedLayer.NextGrid = nextGrid;
        }

        private void CheckedLayerToMix(object sender, RoutedEventArgs e)
        {
            var contex = sender as System.Windows.Controls.Primitives.ToggleButton;

            Layer checkedLayer = (Layer)contex.DataContext;

            if (CheckedLayersForMixInOneLayer.Count == 0)
            {
                CheckedLayersForMixInOneLayer.Add(checkedLayer);

                this.Publish<IsEnableMoveAndMixButtons>(new IsEnableMoveAndMixButtons(CheckedLayersForMixInOneLayer));
            }
            else
                if (CheckedLayersForMixInOneLayer.Count == 1)
            {
                var layer = CheckedLayersForMixInOneLayer.FirstOrDefault();

                var indexFirstLayer = ItemsSource.Items.IndexOf(layer);

                Layer beforeFirstLayer = null;
                Layer afterFirstLayer = null;

                if (indexFirstLayer + 1 < ItemsSource.Items.Count)
                {
                    afterFirstLayer = (Layer)ItemsSource.Items.GetItemAt(indexFirstLayer + 1);
                }

                if (indexFirstLayer - 1 >= 0)
                {
                    beforeFirstLayer = (Layer)ItemsSource.Items.GetItemAt(indexFirstLayer - 1);
                }

                if ((checkedLayer == beforeFirstLayer) || (checkedLayer == afterFirstLayer))
                {
                    CheckedLayersForMixInOneLayer.Add(checkedLayer);

                    this.Publish<IsEnableMoveAndMixButtons>(new IsEnableMoveAndMixButtons(CheckedLayersForMixInOneLayer));
                }
                else
                {
                    contex.IsChecked = false;
                }
            }
            else
            {
                contex.IsChecked = false;
            }
        }

        private void UncheckedLayerToMix(object sender, RoutedEventArgs e)
        {
            var contex = sender as System.Windows.Controls.Primitives.ToggleButton;

            Layer uncheckedLayer = (Layer)contex.DataContext;

            if (uncheckedLayer != null)
            {
                CheckedLayersForMixInOneLayer.Remove(uncheckedLayer);

                this.Publish<IsEnableMoveAndMixButtons>(new IsEnableMoveAndMixButtons(CheckedLayersForMixInOneLayer));
            }
        }

        private void Mix2Layers()
        {
            if (CheckedLayersForMixInOneLayer.Count == 2)
            {
                var sortedCollection = CheckedLayersForMixInOneLayer.OrderBy(x => x.GetZIndex).ToList();

                var firstLayer = sortedCollection[0];
                var secondLayer = sortedCollection[1];


                Layer resultLayer = new Layer()
                {
                    LastGrid = firstLayer.LastGrid,
                    LastLayer = firstLayer.LastLayer,
                    Name = string.Format("{0} + {1}", firstLayer.Name, secondLayer.Name),
                    NextGrid = secondLayer.NextGrid,
                    NextLayer = secondLayer.NextLayer
                };

                List<Grid> gridIntoFirstGrid = null;
                List<Grid> gridIntoSecondGrid = null;

                gridIntoFirstGrid = (firstLayer.Grid.Children.Count > 0 ? HelpMethods.GetElementsFrom<Grid>(firstLayer.Grid).ConvertAll(x => (Grid)x) : new List<Grid>() { new Grid() });
                gridIntoSecondGrid = (secondLayer.Grid.Children.Count > 0 ? HelpMethods.GetElementsFrom<Grid>(secondLayer.Grid).ConvertAll(x => (Grid)x) : new List<Grid>() { new Grid() });

                var resultGrid = gridIntoFirstGrid.Concat(gridIntoSecondGrid);
                var zIndexResultGrid = firstLayer.ZIndex;

                firstLayer.Grid.Children.Clear();
                secondLayer.Grid.Children.Clear();

                int indexOfFirstLayer = Items.IndexOf(firstLayer);

                Grid grid = new Grid();

                foreach (var item in resultGrid)
                {
                    grid.Children.Add(item);
                }

                resultLayer.Grid = grid;
                resultLayer.ZIndex = zIndexResultGrid;
                resultLayer.IsChecked = true;

                int index = Items.IndexOf(firstLayer);

                DeleteLayer(firstLayer);
                DeleteLayer(secondLayer);

                this.Publish<AddLayer>(new AddLayer(resultLayer, index));

                Items.Move(Items.IndexOf(resultLayer), index);

                if (resultLayer.LastLayer != null)
                {
                    resultLayer.LastLayer.NextGrid = resultLayer.Grid;
                    resultLayer.LastLayer.NextLayer = resultLayer;
                }

                if (resultLayer.NextLayer != null)
                {
                    resultLayer.NextLayer.LastGrid = resultLayer.Grid;
                    resultLayer.NextLayer.LastLayer = resultLayer;
                }

                CheckedLayersForMixInOneLayer.Clear();

                this.Publish<IsEnableMoveAndMixButtons>(new IsEnableMoveAndMixButtons(CheckedLayersForMixInOneLayer));
            }
        }
        #endregion

        public ObservableCollection<Layer> ContainerLayers
        {
            get { return (ObservableCollection<Layer>)GetValue(ContainerLayersProperty); }
            set { SetValue(ContainerLayersProperty, value); }
        }

        public static DependencyProperty ContainerLayersProperty =
            DependencyProperty.Register("ContainerLayers", typeof(ObservableCollection<Layer>), typeof(Layers), new PropertyMetadata(null, new PropertyChangedCallback(OnChanged)));


        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private Visibility _deleteVisibilityButton = Visibility.Collapsed;
        public Visibility DeleteVisibilityButton
        {
            get { return _deleteVisibilityButton; }
            set
            {
                _deleteVisibilityButton = value;
                RaisePropertyChanged("DeleteVisibilityButton");
            }
        }
    }
}
