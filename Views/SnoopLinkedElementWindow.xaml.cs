using AkryazTools.Messages;
using AkryazTools.Models;
using AkryazTools.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace AkryazTools.Views
{
    /// <summary>
    /// Interaction logic for SnoopLinkedElementWindow.xaml
    /// </summary>
    public partial class SnoopLinkedElementWindow : Window
    {
        public SnoopLinkedElementWindow(SnoopLinkedElementModel infoData)
        {
            InitializeComponent();
            this.DataContext = new SnoopLinkedElementViewModel(infoData);

            Messenger.Default.Register<CloseMessage>(this, (msg) => WindowClose(msg));
        }
        private void WindowClose(CloseMessage obj)
        {
            this.Close();
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
