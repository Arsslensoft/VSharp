using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

using VSC.Base.CommandLine.Infrastructure;


namespace VSC.Base.CommandLine.Parsing
{
    /// <summary>
    /// Maps unnamed options to property using <see cref="VSC.Base.CommandLine.ValueOptionAttribute"/> and <see cref="VSC.Base.CommandLine.ValueListAttribute"/>.
    /// </summary>
    internal sealed class ValueMapper
    {
        private readonly CultureInfo _parsingCulture;
        private readonly object _target;
        private IList<string> _valueList;
        private ValueListAttribute _valueListAttribute;
        private IList<Pair<PropertyInfo, ValueOptionAttribute>> _valueOptionAttributeList;
        private int _valueOptionIndex;

        public ValueMapper(object target, CultureInfo parsingCulture)
        {
            _target = target;
            _parsingCulture = parsingCulture;
            InitializeValueList();
            InitializeValueOption();
        }

        public bool CanReceiveValues
        {
            get { return IsValueListDefined || IsValueOptionDefined; }
        }

        private bool IsValueListDefined
        {
            get { return _valueListAttribute != null; }
        }

        private bool IsValueOptionDefined
        {
            get { return _valueOptionAttributeList.Count > 0; }
        }

        public bool MapValueItem(string item)
        {
            if (IsValueOptionDefined &&
                _valueOptionIndex < _valueOptionAttributeList.Count)
            {
                var valueOption = _valueOptionAttributeList[_valueOptionIndex++];

                var propertyWriter = new PropertyWriter(valueOption.Left, _parsingCulture);

                return ReflectionHelper.IsNullableType(propertyWriter.Property.PropertyType) ?
                    propertyWriter.WriteNullable(item, _target) :
                    propertyWriter.WriteScalar(item, _target);
            }

            return IsValueListDefined && AddValueItem(item);
        }

        private bool AddValueItem(string item)
        {
            if (_valueListAttribute.MaximumElements == 0 ||
                _valueList.Count == _valueListAttribute.MaximumElements)
            {
                return false;
            }

            _valueList.Add(item);
            return true;
        }

        private void InitializeValueList()
        {
            _valueListAttribute = ValueListAttribute.GetAttribute(_target);
            if (IsValueListDefined)
            {
                _valueList = ValueListAttribute.GetReference(_target);
            }
        }

        private void InitializeValueOption()
        {
            var list = ReflectionHelper.RetrievePropertyList<ValueOptionAttribute>(_target);

            // default is index 0, so skip sorting if all have it
            _valueOptionAttributeList = list.All(x => x.Right.Index == 0)
                ? list : list.OrderBy(x => x.Right.Index).ToList();
        }
    }
}