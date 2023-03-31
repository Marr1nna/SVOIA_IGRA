﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SvoiaIgra.ViewModel
{
    public abstract partial class ValidationBase : ViewModelBase, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> _errors = new();

        protected void AddError(string errorMessage, [CallerMemberName] string propertyName = "")
        {
            if (!_errors.TryGetValue(propertyName, out var errorList))
            {
                errorList = new List<string>();
                _errors.Add(propertyName, errorList);
            }
            errorList.Add(errorMessage);
            RaiseErrorsChanged(propertyName);
        }

        protected void ClearErrors([CallerMemberName] string propertyName = "")
        {
            if (_errors.Remove(propertyName))
            {
                RaiseErrorsChanged(propertyName);
            }
        }

        public bool HasErrors => _errors.Count > 0;

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                foreach (var errorPair in _errors)
                {
                    foreach (var error in errorPair.Value)
                        yield return error;
                }
            }
            else if (_errors.TryGetValue(propertyName, out List<string>? errorList))
            {
                foreach (var error in errorList)
                    yield return error;
            }
        }

        private void RaiseErrorsChanged(string propertyName)
        {
            RaisePropertyChanged(nameof(HasErrors));
            ErrorsChanged?.Invoke(this, new(propertyName));
        }
    }
}