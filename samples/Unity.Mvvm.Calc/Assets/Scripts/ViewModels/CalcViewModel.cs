using System;
using Interfaces;
using Models;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

namespace ViewModels
{
    public class CalcViewModel : ViewModel
    {
        private readonly CalcModel _model;

        public CalcViewModel(IAppContext appContext)
        {
            _model = appContext.Resolve<CalcModel>();

            NumberCommand = new Command<ReadOnlyMemory<char>>(OnEnterNumber);
            OperationCommand = new Command<ReadOnlyMemory<char>>(OnEnterOperation, IsOperationsEnabled);
            CalculateCommand = new Command(OnCalculate, IsCalculateEnabled);
            ClearCommand = new Command(OnClear, IsClearEnabled);
        }

        public string Result => _model.Result;
        public string Expression => _model.Expression;

        public ICommand<ReadOnlyMemory<char>> NumberCommand { get; }
        public ICommand<ReadOnlyMemory<char>> OperationCommand { get; }
        public ICommand CalculateCommand { get; }
        public ICommand ClearCommand { get; }

        private bool IsOperationsEnabled() => !_model.HasOperation && _model.HasFirstNumber && !_model.HasSecondNumber;
        private bool IsCalculateEnabled() => _model.HasOperation && _model.HasFirstNumber && _model.HasSecondNumber;
        private bool IsClearEnabled() => _model.Expression.Length > 0;

        private void OnEnterNumber(ReadOnlyMemory<char> number)
        {
            _model.AddNumber(number.Span);
            RaisePropertiesChanged();
            RaiseCanExecuteChanged();
        }

        private void OnEnterOperation(ReadOnlyMemory<char> operation)
        {
            _model.AddOperation(operation.Span);
            RaisePropertiesChanged();
            RaiseCanExecuteChanged();
        }

        private void OnCalculate()
        {
            _model.Calculate();
            RaisePropertiesChanged();
            RaiseCanExecuteChanged();
        }

        private void OnClear()
        {
            _model.Clear();
            RaisePropertiesChanged();
            RaiseCanExecuteChanged();
        }

        private void RaisePropertiesChanged()
        {
            OnPropertyChanged(nameof(Result));
            OnPropertyChanged(nameof(Expression));
        }

        private void RaiseCanExecuteChanged()
        {
            OperationCommand.RaiseCanExecuteChanged();
            CalculateCommand.RaiseCanExecuteChanged();
            ClearCommand.RaiseCanExecuteChanged();
        }
    }
}