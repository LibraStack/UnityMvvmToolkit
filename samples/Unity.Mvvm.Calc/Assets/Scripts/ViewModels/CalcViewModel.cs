using Interfaces;
using Models;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace ViewModels
{
    public class CalcViewModel : IBindingContext
    {
        private readonly CalcModel _model;

        public CalcViewModel(IAppContext appContext)
        {
            _model = appContext.Resolve<CalcModel>();

            NumberCommand = new Command<string>(OnEnterNumber);
            OperationCommand = new Command<string>(OnEnterOperation, IsOperationsEnabled);
            CalculateCommand = new Command(OnCalculate, IsCalculateEnabled);
            ClearCommand = new Command(OnClear, IsClearEnabled);
        }

        public IReadOnlyProperty<string> Result => _model.Result;
        public IReadOnlyProperty<string> Expression => _model.Expression;

        public ICommand<string> NumberCommand { get; }
        public ICommand<string> OperationCommand { get; }
        public ICommand CalculateCommand { get; }
        public ICommand ClearCommand { get; }

        private bool IsOperationsEnabled() => !_model.HasOperation && _model.HasFirstNumber && !_model.HasSecondNumber;
        private bool IsCalculateEnabled() => _model.HasOperation && _model.HasFirstNumber && _model.HasSecondNumber;
        private bool IsClearEnabled() => _model.Expression.Value.Length > 0;

        private void OnEnterNumber(string number)
        {
            _model.AddNumber(number);
            RaiseCanExecuteChanged();
        }

        private void OnEnterOperation(string operation)
        {
            _model.AddOperation(operation);
            RaiseCanExecuteChanged();
        }

        private void OnCalculate()
        {
            _model.Calculate();
            RaiseCanExecuteChanged();
        }

        private void OnClear()
        {
            _model.Clear();
            RaiseCanExecuteChanged();
        }

        private void RaiseCanExecuteChanged()
        {
            OperationCommand.RaiseCanExecuteChanged();
            CalculateCommand.RaiseCanExecuteChanged();
            ClearCommand.RaiseCanExecuteChanged();
        }
    }
}