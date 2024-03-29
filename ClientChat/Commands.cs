﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace ClientChat
{
    internal class Command : ICommand
    {
        private Action Method;

        public Command(Action method)
        {
            Method = method;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return Method != null;
        }

        public void Execute(object parameter)
        {
            CanExecuteChanged?.Invoke(this, null);
            Method.Invoke();
        }
    }

    internal class Command<T> : ICommand
    {
        private Action<T> Method;

        public Command(Action<T> method)
        {
            Method = method;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return Method != null;
        }

        public void Execute(object parameter)
        {
            CanExecuteChanged?.Invoke(this, null);
            Method.Invoke((T)parameter);
        }
    }
}
