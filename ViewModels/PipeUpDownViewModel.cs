using AkryazTools.Messages;
using AkryazTools.Models;
using AkryazTools.ViewModels;
using Autodesk.Revit.DB;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using AkryazTools.Views;
using AkryazTools.External_Events;

namespace AkryazTools.ViewModels
{
    internal class PipeUpDownViewModel : ViewModelBase
    {
        public PipeUpDownWindow MainWindow { get; set; }

        public PipeUpDownViewModel()
        {
            //_document = document;

            MainWindow = new PipeUpDownWindow();
            MainWindow.DataContext = this;


            UpCommand = new RelayCommand(() => SetMepUpData(), CheckValuesEntered);
            LeftCommand = new RelayCommand(() => SetMepLeftData(), CheckValuesEntered);
            RightCommand = new RelayCommand(() => SetMepRightData(), CheckValuesEntered);
            DownCommand = new RelayCommand(() => SetMepDownData(), CheckValuesEntered);

            MainWindow.Show();

        }

        private void SetMepDownData()
        {
            PipeUpDownData.OffsetValue = double.Parse(Offset);
            PipeUpDownData.AngleValue = double.Parse(Angle);
            PipeUpDownData.Direction = Enum.MepReRouterEnum.Down;
            PipeUpDownExternalEvent.HandlerEvent.Raise();
            //CloseWindow();
        }

        private void SetMepRightData()
        {
            PipeUpDownData.OffsetValue = double.Parse(Offset);
            PipeUpDownData.AngleValue = double.Parse(Angle);
            PipeUpDownExternalEvent.HandlerEvent.Raise();
            PipeUpDownData.Direction = Enum.MepReRouterEnum.Right;
            //CloseWindow();
        }

        private void SetMepLeftData()
        {
            PipeUpDownData.OffsetValue = double.Parse(Offset);
            PipeUpDownData.AngleValue = double.Parse(Angle);
            PipeUpDownExternalEvent.HandlerEvent.Raise();
            PipeUpDownData.Direction = Enum.MepReRouterEnum.Left;
            //CloseWindow();
        }

        private void SetMepUpData()
        {
            PipeUpDownData.OffsetValue = double.Parse(Offset);
            PipeUpDownData.AngleValue = double.Parse(Angle);
            PipeUpDownExternalEvent.HandlerEvent.Raise();
            PipeUpDownData.Direction = Enum.MepReRouterEnum.Up;
            //CloseWindow();
        }

        private bool CheckValuesEntered()
        {
            var test1 = double.TryParse(Offset, out var offsetDouble) ;
            var test2 = double.TryParse(Angle, out var anglwDouble);

            if (test1 && test2)
            {
                if (offsetDouble != 0 && anglwDouble != 0)
                    return true;
            }

            return false;
        }

        private void SetPipeUpDownData()
        {
            PipeUpDownData.OffsetValue = double.Parse(Offset);
            PipeUpDownData.AngleValue = double.Parse(Angle);
            CloseWindow();
        }

        private void CloseWindow()
        {
            Messenger.Default.Send(new CloseMessage());
        }

        private string _offset;

        public string Offset
        {
            get { return _offset; }
            set 
            { 
                _offset = value;
                RaisePropertyChanged();
            }
        }

        private string _angle;

        public string Angle
        {
            get { return _angle; }
            set 
            {
                _angle = value;
                RaisePropertyChanged();
            }
        }

        public ICommand UpCommand { get; set; }
        public ICommand LeftCommand { get; set; }
        public ICommand RightCommand { get; set; }
        public ICommand DownCommand { get; set; }


    }
}