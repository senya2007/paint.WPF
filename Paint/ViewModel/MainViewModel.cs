using GalaSoft.MvvmLight;
using System.Windows.Input;
using System;
using System.Collections;
using Paint.Interface;
using System.Collections.Generic;
using Paint.Model;
using Paint.Helpers;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using Paint.Helpers.Handle;
using PubSub;
using System.Windows;
using System.ComponentModel;
using Paint.Helpers.Events;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Linq;
using System.IO;
using System.Windows.Markup;
using System.Reflection;

namespace Paint.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        /// 

        private MainWindow window;

        public MainViewModel(MainWindow window)
        {
            this.window = window;

            GetAllModels();

            Layers = new ObservableCollection<Layer>();

            MoveIsEnable = true;

            this.Subscribe<CurrentGrid>
                (data => SetCurrentGrid(data.Grid));

            this.Subscribe<SelectedLayer>
                (data => SetSelectedLayer(data.Layer));

            this.Subscribe<SelectedColorFill>
                (data => SetFill(data.Color));

            this.Subscribe<DeleteLayer>
                (data => DeleteLayerFromCollection(data.CurrentLayer));

            this.Subscribe<AddLayer>
                (data => AddLayerFromMix(data.Layer, data.IndexToMove));

            this.Subscribe<IsEnableMoveAndMixButtons>
                (data => SetIsEnableMixAndMoveButtons(data.Layers));

            SetStartLayer();
        }

        #region Commands

        public ICommand AddLayerCommand
        {
            get { return new DelegateCommand(AddLayer); }
        }

        public ICommand DeleteLayersCommand
        {
            get { return new DelegateCommand(DeleteLayers); }
        }

        public ICommand UpLayerCommand
        {
            get { return new DelegateCommand(UpLayer); }
        }

        public ICommand SaveButton
        {
            get { return new DelegateCommand(SaveInFile); }
        }

        public ICommand OpenButton
        {
            get { return new DelegateCommand(OpenImage); }
        }

        public ICommand MixLayersCommand
        {
            get { return new DelegateCommand(MixLayers); }
        }

        public ICommand DownLayerCommand
        {
            get { return new DelegateCommand(DownLayer); }
        }

        public ICommand CursorCommand
        {
            get { return new DelegateCommand(SetModel<CursorModel>); }
        }

        public ICommand PencilCommand
        {
            get { return new DelegateCommand(SetModel<PencilModel>); }
        }

        public ICommand LineCommand
        {
            get { return new DelegateCommand(SetModel<LineModel>); }
        }

        public ICommand EllipsCommand
        {
            get { return new DelegateCommand(SetModel<EllipseModel>); }
        }

        public ICommand RectangleCommand
        {
            get { return new DelegateCommand(SetModel<RectangleModel>); }
        }

        public ICommand BezierCommand
        {
            get { return new DelegateCommand(SetModel<BezierModel>); }
        }

        #endregion

        #region Methods

        private void SetIsEnableMixAndMoveButtons(ObservableCollection<Layer> layers)
        {
            if (layers != null)
            {
                SetIsEnableMoveLayer(layers);
                SetIsEnableMixLayersButton(layers);
            }
        }

        private void SetIsEnableMixLayersButton(ObservableCollection<Layer> layers)
        {
            MixLayersIsEnable = layers.Count == 2 ? true : false;
        }

        private void SetIsEnableMoveLayer(ObservableCollection<Layer> layers)
        {
            MoveIsEnable = layers.Count > 0 ? false : true;
        }

        private void AddLayerFromMix(Layer layer, int index)
        {
            Layers.Add(layer);
            RaisePropertyChanged("Layers");

            window.Canvas.Children.Insert(index, layer.Grid);
            CurrentLayer = layer;
            CurrentLayer.Grid = layer.Grid;
        }

        private void DeleteLayerFromCollection(Layer currentLayer)
        {
            Layers.Remove(currentLayer);
            window.Canvas.Children.Remove(currentLayer.Grid);
            RaisePropertyChanged("Layers");
        }

        private void SetStartLayer()
        {
            var layer = new Layer { Grid = new Grid() { Name = string.Format("Layer" + ContainerClass.NumberLayer) }, ZIndex = ContainerClass.NumberLayer, LastGrid = null, Name = string.Format("Layer" + ContainerClass.NumberLayer), IsChecked = true };
            Layers.Add(layer);
            RaisePropertyChanged("Layers");

            CurrentGrid = layer.Grid;
            window.Canvas.Children.Add(layer.Grid);
            CurrentLayer = layer;
            CurrentLayer.Grid = CurrentGrid;
            CurrentLayer.LastGrid = null;
            ContainerClass.NumberLayer++;
        }

        private void SetSelectedLayer(Layer layer)
        {
            CurrentGrid = layer.Grid;
            ContainerClass.SelectedGrid = layer.Grid;
            SetModel<CursorModel>();
        }

        private void SetCurrentGrid(Grid grid)
        {
            CurrentGrid.Children.Add(grid);
            ContainerClass.SelectedGrid = CurrentLayer.Grid;
        }

        private void CreateHandlers(IModel shape)
        {
            if (LastShape != null)
            {
                LastShape.RemoveHandle();
            }

            HandleCursor cursor = new HandleCursor();
            HandlePencil pencil = new HandlePencil();
            HandleLine line = new HandleLine();
            HandleEllipse ellipse = new HandleEllipse();
            HandleRectangle rectangle = new HandleRectangle();
            HandleBezier bezier = new HandleBezier();

            HandleFill fill = new HandleFill();

            HandleImage image = new HandleImage();

            cursor.Successor = pencil;
            pencil.Successor = line;
            line.Successor = ellipse;
            ellipse.Successor = rectangle;
            rectangle.Successor = bezier;
            bezier.Successor = fill;
            fill.Successor = image;

            cursor.Handle(shape);
        }

        private void AddLayer()
        {
            var layer = new Layer { Grid = new Grid() { Name = string.Format("Layer" + ContainerClass.NumberLayer) }, ZIndex = ContainerClass.NumberLayer, LastGrid = CurrentLayer.Grid, Name = string.Format("Layer" + ContainerClass.NumberLayer), IsChecked = true };

            AddLayer(layer);
        }

        private void AddLayer(Layer layer)
        {
            if (CurrentGrid != null)
            {
                var temporaryLastLayer = CurrentLayer;

                if (CurrentLayer != null)
                {
                    CurrentLayer.NextLayer = layer;
                    CurrentLayer.NextGrid = layer.Grid;
                }

                Layers.Add(layer);
                RaisePropertyChanged("Layers");
                window.Canvas.Children.Add(layer.Grid);
                CurrentGrid = layer.Grid;
                CurrentLayer = layer;
                CurrentLayer.LastLayer = temporaryLastLayer;
                CurrentLayer.Grid = layer.Grid;
                ContainerClass.NumberLayer++;
            }
        }

        private void UpLayer()
        {
            this.Publish<MoveLayer>(new MoveLayer(Helpers.Enums.ActionWithLayer.Up));
        }

        private void DownLayer()
        {
            this.Publish<MoveLayer>(new MoveLayer(Helpers.Enums.ActionWithLayer.Down));
        }

        private void MixLayers()
        {
            this.Publish<MixLayers>();
        }

        private void DeleteLayers()
        {
            DeleteButton = (DeleteButton == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
            this.Publish<DeleteVisibilityButton>();
        }

        private void OpenImage()
        {
            var openImage = new OpenFileDialog
            {
                Filter = "bmp files |*.bmp",
                FileName = "image"
            };

            var resultDialog = openImage.ShowDialog();

            if (resultDialog == true)
            {
                ContainerClass.OpenImageName = openImage.FileName;
            }

            SetModel<ImageModel>();
        }

        private void SaveInFile()
        {
            var saveFile = new SaveFileDialog
            {
                Filter = "bmp files |*.bmp",
                FileName = "image"
            };

            var resultDialog = saveFile.ShowDialog();

            if (resultDialog == true)
            {
                double
                x1 = window.Canvas.Margin.Left,
                x2 = window.Canvas.Margin.Top,
                x3 = window.Canvas.Margin.Right,
                x4 = window.Canvas.Margin.Bottom;

                if (saveFile.FileName == null) return;

                window.Canvas.Margin = new Thickness(0, 0, 0, 0);

                Size size = new Size(window.Canvas.Width, window.Canvas.Height);
                window.Canvas.Measure(size);
                window.Canvas.Arrange(new Rect(size));

                RenderTargetBitmap renderBitmap =
                 new RenderTargetBitmap(
                   (int)size.Width,
                   (int)size.Height,
                   96,
                   96,
                   PixelFormats.Default);

                renderBitmap.Render(window.Canvas);

                using (FileStream fs = File.Open(saveFile.FileName, FileMode.Create))
                {
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                    encoder.Save(fs);
                }

                window.Canvas.Margin = new Thickness(x1, x2, x3, x4);
            }
        }

        public void SetModel<T>() where T : IModel
        {
            if (Layers.Count != 0)
            {
                foreach (var model in AllModels)
                {
                    if (model is T)
                    {
                        if (CurrentShape != model)
                        {
                            LastShape = CurrentShape;
                            CurrentShape = model;
                        }
                        model.CurrentWindow = window;
                        CreateHandlers(model);
                    }
                }
            }
        }

        private void GetAllModels()
        {
            ModelsCollection collection = new ModelsCollection();
            AllModels = collection.AllModels;
        }

        public void SetFill(SolidColorBrush color)
        {
            ContainerClass.SelectedColorFill = color;
            SetModel<FillModel>();
        }

        #endregion

        #region Property

        public IEnumerable<IModel> AllModels { get; set; }
        public IModel CurrentShape { get; set; }
        public IModel LastShape { get; set; }

        public Grid _currentGrid;
        public Grid CurrentGrid
        {
            get { return _currentGrid; }
            set
            {
                _currentGrid = value;
            }
        }

        public Layer CurrentLayer { get; set; }

        private Visibility _deleteButton = Visibility.Collapsed;
        public Visibility DeleteButton
        {
            get { return _deleteButton; }
            set
            {
                _deleteButton = value;
                RaisePropertyChanged(() => DeleteButton);
            }
        }

        private bool _mixLayersIsEnable;
        public bool MixLayersIsEnable
        {
            get { return _mixLayersIsEnable; }
            set
            {
                _mixLayersIsEnable = value;

                RaisePropertyChanged("MixLayersIsEnable");
            }
        }

        private bool _moveIsEnable;
        public bool MoveIsEnable
        {
            get { return _moveIsEnable; }
            set
            {
                _moveIsEnable = value;

                RaisePropertyChanged("MoveIsEnable");
            }
        }

        private ObservableCollection<Layer> _layers;
        public ObservableCollection<Layer> Layers
        {
            get { return _layers; }
            set
            {
                _layers = value;

                RaisePropertyChanged("Layers");
            }
        }

        #endregion
    }
}