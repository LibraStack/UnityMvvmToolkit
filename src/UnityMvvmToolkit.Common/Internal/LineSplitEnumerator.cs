using System;

namespace UnityMvvmToolkit.Common.Internal
{
    internal ref struct LineSplitEnumerator
    {
        private int _start;
        private ReadOnlySpan<char> _str;
        private readonly char _separator;

        public LineSplitEnumerator(ReadOnlySpan<char> str, char separator)
        {
            _str = str;
            _start = 0;
            _separator = separator;

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
                Current = new LineSplitData(_start, span);

                return true;
            }
            
            Current = new LineSplitData(_start, span.Slice(0, index));
            
            _str = span.Slice(index + 1);
            _start += index + 1;

            return true;
        }
    }
}