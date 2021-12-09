using AkryazTools.Messages;
using AkryazTools.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AkryazTools.Views
{
    /// <summary>
    /// Interaction logic for RoomsToViewsWindow.xaml
    /// </summary>
    public partial class RoomsToViewsWindow : Window
    {
        public RoomsToViewsWindow(Autodesk.Revit.DB.Document document)
        {
            InitializeComponent();
            this.DataContext = new RoomsToViewViewModel(document);
            Messenger.Default.Register<CloseMessage>(this, (msg) => WindowClose(msg));
        }
        private void WindowClose(CloseMessage obj)
        {
            this.Close();
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[.][0-9]+$|^[0-9]*[.]{0,1}[0-9]*$");
            //e.Handled = regex.IsMatch(e.Text);
            e.Handled = !regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            var window = (Window)((FrameworkElement)sender).TemplatedParent;
            window.Close();
        }

        private void MaximizeRestoreClick(object sender, RoutedEventArgs e)
        {
            var window = (Window)((FrameworkElement)sender).TemplatedParent;
            if (window.WindowState == System.Windows.WindowState.Normal)
            {
                window.WindowStyle = WindowStyle.SingleBorderWindow;
                window.WindowState = System.Windows.WindowState.Maximized;
            }
            else
            {
                window.WindowStyle = WindowStyle.SingleBorderWindow;
                window.WindowState = System.Windows.WindowState.Normal;
            }
        }

        private void MinimizeClick(object sender, RoutedEventArgs e)
        {
            var window = (Window)((FrameworkElement)sender).TemplatedParent;
            window.WindowStyle = WindowStyle.SingleBorderWindow;
            window.WindowState = System.Windows.WindowState.Minimized;
        }
    }
}
