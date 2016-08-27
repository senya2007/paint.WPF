using Paint.Helpers.Events;
using Paint.View;
using Paint.ViewModel;
using PubSub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace Paint
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        MainViewModel context;
        public MainWindow()
        {
            InitializeComponent();
            context = new MainViewModel(this);
            DataContext = context;
        }

        private void ColorFill_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var brush = new SolidColorBrush((Color)(Color.SelectedItem as PropertyInfo).GetValue(null, null));

            this.Publish<SelectedColorFill>(new SelectedColorFill(brush));
        }

    }
}
