using System;
using System.Collections.Generic;
using Interfaces;
using UnityMvvmToolkit.Core.Converters.ValueConverters;

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

        private string _result;
        private string _expression;

        public CalcModel(IAppContext appContext)
        {
            _result = string.Empty;
            _expression = string.Empty;

            _converter = appContext.Resolve<FloatToStrConverter>();
            _mathOperations = appContext.Resolve<IReadOnlyDictionary<char, IMathOperation>>();
        }

        public string Result => _result;
        public string Expression => _expression;

        public bool HasOperation => _hasOperation;
        public bool HasFirstNumber => _number1Length > 0;
        public bool HasSecondNumber => _number2Length > 0;

        public void AddNumber(ReadOnlySpan<char> numberSpan)
        {
            if (_isResultState)
            {
                _isResultState = false;
                _number1Length = 1;

                _expression = numberSpan.ToString();

                return;
            }

            _expression += numberSpan.ToString();

            if (_hasOperation)
            {
                _number2Length++;
                _result = Calculate(_expression);
            }
            else
            {
                _number1Length++;
            }
        }

        public void AddOperation(ReadOnlySpan<char> operationSpan)
        {
            if (_hasOperation)
            {
                return;
            }

            if (operationSpan.Length != 1)
            {
                throw new InvalidOperationException(nameof(operationSpan));
            }

            _hasOperation = true;
            _isResultState = false;

            _expression += operationSpan[0];
        }

        public void Calculate()
        {
            if (string.IsNullOrWhiteSpace(_expression))
            {
                return;
            }

            var result = Calculate(_expression);

            _hasOperation = false;
            _isResultState = true;

            _number1Length = result.Length;
            _number2Length = 0;

            _result = string.Empty;
            _expression = result;
        }

        public void Clear()
        {
            _hasOperation = false;
            _isResultState = false;

            _number1Length = 0;
            _number2Length = 0;

            _result = string.Empty;
            _expression = string.Empty;
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