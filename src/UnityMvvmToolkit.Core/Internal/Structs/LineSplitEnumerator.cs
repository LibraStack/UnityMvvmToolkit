using System;
using System.Runtime.CompilerServices;

namespace UnityMvvmToolkit.Core.Internal.Structs
{
    internal ref struct LineSplitEnumerator
    {
        private int _index;
        private int _start;
        private ReadOnlySpan<char> _str;
        private readonly char _separator;
        private readonly bool _trimLines;

        internal LineSplitEnumerator(ReadOnlySpan<char> str, char separator, bool trimLines)
        {
            _str = str;
            _index = 0;
            _start = 0;
            _separator = separator;
            _trimLines = trimLines;

            Current = default;
        }

        public LineSplitData Current { get; private set; }

        public LineSplitEnumerator GetEnumerator() => this;

        public bool MoveNext()
        {
            var span = _str;
            if (span.Length == 0)
            {
                return false;
            }

            var index = span.IndexOf(_separator);
            if (index == -1)
            {
                _str = ReadOnlySpan<char>.Empty;
                Current = CreateNewLine(_index, _start, span);

                return true;
            }

            Current = CreateNewLine(_index, _start, span[..index]);

            _index++;
            _start += index + 1;
            _str = span.Slice(index + 1);

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private LineSplitData CreateNewLine(int index, int start, ReadOnlySpan<char> data)
        {
            return _trimLines
                ? new LineSplitData(index, start, data).Trim()
                : new LineSplitData(index, start, data);
        }
    }
}