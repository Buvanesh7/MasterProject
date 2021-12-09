using AkryazTools.ExternalCommands;
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
    /// Interaction logic for PipeUpDownWindow.xaml
    /// </summary>
    public partial class PipeUpDownWindow : Window
    {
        public PipeUpDownWindow()
        {
            InitializeComponent();
            //this.DataContext = new PipeUpDownViewModel(document);

            Messenger.Default.Register<CloseMessage>(this, (msg) => WindowClose(msg));
        }

        private void WindowClose(CloseMessage obj)
        {
            this.Close();
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            //Regex regex = new Regex(@"^-?\d*\.{0,1}\d+$");
            Regex regex = new Regex("^[.][0-9]+$|^[0-9]*[.]{0,1}[0-9]*$");
            //e.Handled = regex.IsMatch(e.Text);
            //var check = !regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));
            e.Handled = !regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            var window = (Window)((FrameworkElement)sender).TemplatedParent;
            PipeUpDownCommand.ActiveWindow = null;
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
