using System;
using System.Collections.Generic;
using Interfaces;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Converters.PropertyValueConverters;
using UnityMvvmToolkit.Core.Interfaces;

namespace Models
{
    public class CalcModel
    {
        private readonly FloatToStrConverter _converter;
        private readonly IReadOnlyDictionary<char, IMathOperation> _mathOperations;

        private bool _hasOperation;
        private bool _isResultState;

        private int _number1Length;
        private int _number2Length;

        private readonly IProperty<string> _result;
        private readonly IProperty<string> _expression;

        public CalcModel(IAppContext appContext)
        {
            _result = new ObservableProperty<string>(string.Empty);
            _expression = new ObservableProperty<string>(string.Empty);

            _converter = appContext.Resolve<FloatToStrConverter>();
            _mathOperations = appContext.Resolve<IReadOnlyDictionary<char, IMathOperation>>();
        }

        public IReadOnlyProperty<string> Result => _result;
        public IReadOnlyProperty<string> Expression => _expression;

        public bool HasOperation => _hasOperation;
        public bool HasFirstNumber => _number1Length > 0;
        public bool HasSecondNumber => _number2Length > 0;

        public void AddNumber(string number)
        {
            if (_isResultState)
            {
                _isResultState = false;
                _number1Length = 1;

                _expression.Value = number;

                return;
            }

            _expression.Value += number;

            if (_hasOperation)
            {
                _number2Length++;
                _result.Value = Calculate(_expression.Value);
            }
            else
            {
                _number1Length++;
            }
        }

        public void AddOperation(string operation)
        {
            if (_hasOperation)
            {
                return;
            }

            if (operation.Length != 1)
            {
                throw new InvalidOperationException(nameof(operation));
            }

            _hasOperation = true;
            _isResultState = false;

            _expression.Value += operation;
        }

        public void Calculate()
        {
            if (string.IsNullOrWhiteSpace(_expression.Value))
            {
                return;
            }

            var result = Calculate(_expression.Value);

            _hasOperation = false;
            _isResultState = true;

            _number1Length = result.Length;
            _number2Length = 0;

            _result.Value = string.Empty;
            _expression.Value = result;
        }

        public void Clear()
        {
            _hasOperation = false;
            _isResultState = false;

            _number1Length = 0;
            _number2Length = 0;

            _result.Value = string.Empty;
            _expression.Value = string.Empty;
        }

        private string Calculate(ReadOnlySpan<char> expression)
        {
            var number1 = expression.Slice(0, _number1Length);
            var number2 = expression.Slice(_number1Length + 1, _number2Length);
            var operation = expression.Slice(_number1Length, 1)[0];

            return _converter.Convert(_mathOperations[operation].Calculate(number1, number2));
        }
    }
}