using System;
using Interfaces;

namespace MathOperations
{
    public class MultiplyOperation : IMathOperation
    {
        public float Calculate(ReadOnlySpan<char> number1, ReadOnlySpan<char> number2)
        {
            return float.Parse(number1) * float.Parse(number2);
        }
    }
}