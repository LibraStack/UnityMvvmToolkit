using System;

namespace UnityMvvmToolkit.Common.Internal
{
    internal readonly ref struct LineSplitData
    {
        public LineSplitData(int start, ReadOnlySpan<char> data)
        {
            Data = data;
            Start = start;
            Length = data.Length;
        }

        public int Start { get; }
        public int Length { get; }
        public ReadOnlySpan<char> Data { get; }

        public bool IsEmptyOrWhiteSpace => Data.IsEmpty || Data.IsWhiteSpace();

        public LineSplitData Trim()
        {
            var start = Start + (Length - Data.TrimStart().Length);

            return new LineSplitData(start, Data.Trim());
        }
    }
}