using AkryazTools.External_Events;
using AkryazTools.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AkryazTools.ViewModels
{
    internal class TestViewModel : ViewModelBase
    {
        public TestWindow MainWindow { get; set; }

        public TestViewModel()
        {
            MainWindow = new TestWindow();
            MainWindow.DataContext = this;

            HelloWorldCommand = new RelayCommand(() => ShowMessage(), true);

            MainWindow.Show();
        }

        private void ShowMessage()
        {
            TestExternalEvent.HandlerEvent.Raise();
        }

        public ICommand HelloWorldCommand { get; set; }

    }
}
