using System;

using VSC.Base.CommandLine.Infrastructure;



namespace VSC.Base.CommandLine.Parsing
{
    internal sealed class OneCharStringEnumerator : IArgumentEnumerator
    {
        private readonly string _data;
        private string _currentElement;
        private int _index;

        public OneCharStringEnumerator(string value)
        {
            Assumes.NotNullOrEmpty(value, "value");
            _data = value;
            _index = -1;
        }

        public string Current
        {
            get
            {
                if (_index == -1)
                {
                    throw new InvalidOperationException();
                }

                if (_index >= _data.Length)
                {
                    throw new InvalidOperationException();
                }

                return _currentElement;
            }
        }

        public string Next
        {
            get
            {
                if (_index == -1)
                {
                    throw new InvalidOperationException();
                }

                if (_index > _data.Length)
                {
                    throw new InvalidOperationException();
                }

                if (IsLast)
                {
                    return null;
                }

                return _data.Substring(_index + 1, 1);
            }
        }

        public bool IsLast
        {
            get { return _index == _data.Length - 1; }
        }

        public bool MoveNext()
        {
            if (_index < (_data.Length - 1))
            {
                _index++;
                _currentElement = _data.Substring(_index, 1);
                return true;
            }

            _index = _data.Length;
            return false;
        }

        public string GetRemainingFromNext()
        {
            if (_index == -1)
            {
                throw new InvalidOperationException();
            }

            if (_index > _data.Length)
            {
                throw new InvalidOperationException();
            }

            return _data.Substring(_index + 1);
        }

        public bool MovePrevious()
        {
            throw new NotSupportedException();
        }
    }
}