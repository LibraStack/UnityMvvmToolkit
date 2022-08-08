using System;

namespace UnityMvvmToolkit.Core.Internal.Structs
{
    internal readonly ref struct LineSplitData
    {
        internal LineSplitData(int index, int start, ReadOnlySpan<char> data)
        {
            Data = data;
            Index = index;
            Start = start;
            Length = data.Length;
        }

        public int Index { get; }
        public int Start { get; }
        public int Length { get; }
        public ReadOnlySpan<char> Data { get; }

        public LineSplitData Trim()
        {
            var start = Start + (Length - Data.TrimStart().Length);

            return new LineSplitData(Index, start, Data.Trim());
        }
    }
}