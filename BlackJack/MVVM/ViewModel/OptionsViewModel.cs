﻿using BlackJack.Core;
using GameCardLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using UtilitiesLib;

namespace BlackJack.MVVM.ViewModel
{
    class OptionsViewModel : ObservableObject
    {
        Helper helper;
        GameManager _gameManager = GameManager.Instance;

        private int _numberOfPlayers = 4;
        private int _numberOfDecks = 4;

        private int _minNumberOfPlayers = 2;
        private int _maxNumberOfPlayers = 4;
        private int _minNumberOfDecks = 1;
        private int _maxNumberOfDecks = 4;

        private bool _optionsSaved;

        public int NumberOfPlayers
        {
            get
            {
                return _numberOfPlayers;
            }
            set
            {
                _numberOfPlayers = value;
                OnPropertyChanged();
            }
        }

        public int NumberOfDecks
        {
            get
            {
                return _numberOfDecks;
            }
            set
            {
                _numberOfDecks = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Holds an Execute and CanExecute method, which executes respectively checks if the execution is possible. In this case will be used for saving data on the OptionsView
        /// </summary>
        public ICommand SaveCommand { get; }
        public bool OptionsSaved { get => _optionsSaved; set => _optionsSaved = value; }

        public OptionsViewModel()
        {
            SaveCommand = new RelayCommand(ExecuteSave, CanExecuteSave);
            helper = new Helper();
        }


        public bool CanExecuteSave(object parameter)
        {
            //returns true if
            return OptionsValueCheck();
        }

        private void ExecuteSave(object parameter)
        {
            if (CanExecuteSave(parameter))
            {
                _gameManager.InitilizeGame(_numberOfDecks, _numberOfPlayers);

                OptionsSaved = true;
            }
            else
            {
                MessageBox.Show("Please use valid integers as input");
            }
        }

        /// <summary>
        /// Returns true if the values asked for in OptionsView are within range of the program rules
        /// </summary>
        private bool OptionsValueCheck()
        {
            bool correctAmountPlayers = helper.WithinMinMaxValueCheck(_minNumberOfPlayers, _maxNumberOfPlayers, _numberOfPlayers);
            bool correctAmountDecks = helper.WithinMinMaxValueCheck(_minNumberOfDecks, _maxNumberOfDecks, _numberOfDecks);

            if (correctAmountPlayers && correctAmountDecks)
            {
                Debug.WriteLine("OptionsValueCheck = true");
                return true;
            }
            return false;
        }


   
    }
}
