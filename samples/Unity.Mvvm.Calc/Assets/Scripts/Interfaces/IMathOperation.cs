using System;

namespace Interfaces
{
    public interface IMathOperation
    {
        float Calculate(ReadOnlySpan<char> number1, ReadOnlySpan<char> number2);
    }
}