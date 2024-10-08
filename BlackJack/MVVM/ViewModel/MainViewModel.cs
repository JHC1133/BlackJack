﻿using BlackJack.Core;
using BlackJack.MVVM.View;
using GameCardLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BlackJack.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {

        GameManager _gameManager = GameManager.Instance;

        #region MVVM
        public RelayCommand OptionsViewCommand { get; set; }
        public RelayCommand PlayViewCommand { get; set; }
        public RelayCommand QuitCommand { get; set; }
        public RelayCommand StatsViewCommand { get; set; }


        public StatsViewModel StatsVM { get; set; }
        public OptionsViewModel OptionsVM { get; set; }
        public PlayViewModel PlayVM { get; set; }

        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }
        #endregion


        public MainViewModel()
        {
            OptionsVM = new OptionsViewModel();
            PlayVM = new PlayViewModel();
            StatsVM = new StatsViewModel();

            CurrentView = OptionsVM; // Setting the default view
           
            // Lambda expressions 

            OptionsViewCommand = new RelayCommand(o =>
            {
                CurrentView = OptionsVM;
            });

            PlayViewCommand = new RelayCommand(o =>
            {
                CurrentView = PlayVM;
                _gameManager.BlackJackCheck();
            });

            StatsViewCommand = new RelayCommand(o =>
            {
                CurrentView = StatsVM;
            });

            QuitCommand = new RelayCommand(o =>
            {

                _gameManager.UpdateStatistics();
                _gameManager.UpdateGame();
                //Application.Current.Shutdown();

            });
        }
    }
}
