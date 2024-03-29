﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Notes.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged, IViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected void RaiseAndSetIfChanged<T>(T currentValue, T newValue, Action<T> setter, [CallerMemberName] string name = null)
        {
            if (!EqualityComparer<T>.Default.Equals(currentValue, newValue))
            {
                setter(newValue);
                RaisePropertyChanged(name);
            }

        }

        protected void RaiseAndSetIfChanged<T>(ref T property, T newValue, [CallerMemberName] string name = null)
        {
            if (!EqualityComparer<T>.Default.Equals(property, newValue))
            {
                property = newValue;
                RaisePropertyChanged(name);
            }
        }
    }
}
