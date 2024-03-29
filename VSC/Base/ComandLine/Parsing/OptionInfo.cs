using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

using VSC.Base.CommandLine.Infrastructure;


namespace VSC.Base.CommandLine.Parsing
{
    [DebuggerDisplay("ShortName = {ShortName}, LongName = {LongName}")]
    internal sealed class OptionInfo
    {
        private readonly CultureInfo _parsingCulture;
        private readonly BaseOptionAttribute _attribute;
        private readonly PropertyInfo _property;
        private readonly PropertyWriter _propertyWriter;
        private readonly bool _required;
        private readonly char? _shortName;
        private readonly string _longName;
        private readonly string _mutuallyExclusiveSet;
        private readonly object _defaultValue;
        private readonly bool _hasDefaultValue;

        public OptionInfo(BaseOptionAttribute attribute, PropertyInfo property, CultureInfo parsingCulture)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException("attribute", SR.ArgumentNullException_AttributeCannotBeNull);
            }

            if (property == null)
            {
                throw new ArgumentNullException("property", SR.ArgumentNullException_PropertyCannotBeNull);
            }

            _required = attribute.Required;
            _shortName = attribute.ShortName;
            _longName = attribute.LongName;
            _mutuallyExclusiveSet = attribute.MutuallyExclusiveSet;
            _defaultValue = attribute.DefaultValue;
            _hasDefaultValue = attribute.HasDefaultValue;
            _attribute = attribute;
            _property = property;
            _parsingCulture = parsingCulture;
            _propertyWriter = new PropertyWriter(_property, _parsingCulture);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionInfo"/> class. Used for unit testing purpose.
        /// </summary>
        /// <param name="shortName">Option short name.</param>
        /// <param name="longName">Option long name.</param>
        internal OptionInfo(char? shortName, string longName)
        {
            _shortName = shortName;
            _longName = longName;
        }

        public char? ShortName
        {
            get { return _shortName; }
        }

        public string LongName
        {
            get { return _longName; }
        }

        public string MutuallyExclusiveSet
        {
            get { return _mutuallyExclusiveSet; }
        }

        public bool Required
        {
            get { return _required; }
        }

        public bool IsBoolean
        {
            get { return _property.PropertyType == typeof(bool); }
        }

        public bool IsArray
        {
            get { return _property.PropertyType.IsArray; }
        }

        public bool IsAttributeArrayCompatible
        {
            get { return _attribute is OptionArrayAttribute; }
        }

        public bool IsDefined
        {
            get; set;
        }

        public bool ReceivedValue
        {
            get; private set;
        }

        public bool HasBothNames
        {
            get
            {
                return _shortName != null && _longName != null;
            }
        }

        public bool HasParameterLessCtor
        {
            get; set;
        }

        public object GetValue(object target)
        {
            return _property.GetValue(target, null);
        }

        public object CreateInstance(object target)
        {
            object instance = null;

            try
            {
                instance = Activator.CreateInstance(_property.PropertyType);

                _property.SetValue(target, instance, null);
            }
            catch (Exception e)
            {
                throw new ParserException(SR.CommandLineParserException_CannotCreateInstanceForVerbCommand, e);
            }

            return instance;
        }

        public bool SetValue(string value, object options)
        {
            if (_attribute is OptionListAttribute)
            {
                return SetValueList(value, options);
            }

            if (ReflectionHelper.IsNullableType(_property.PropertyType))
            {
                return ReceivedValue = _propertyWriter.WriteNullable(value, options);
            }

            return ReceivedValue = _propertyWriter.WriteScalar(value, options);
        }

        public bool SetValue(IList<string> values, object options)
        {
            var elementType = _property.PropertyType.GetElementType();
            var array = Array.CreateInstance(elementType, values.Count);

            for (int i = 0; i < array.Length; i++)
            {
                try
                {
                    array.SetValue(Convert.ChangeType(values[i], elementType, _parsingCulture), i);
                    _property.SetValue(options, array, null);
                }
                catch (FormatException)
                {
                    return false;
                }
            }

            return ReceivedValue = true;
        }

        public bool SetValue(bool value, object options)
        {
            _property.SetValue(options, value, null);
            return ReceivedValue = true;
        }

        public void SetDefault(object options)
        {
            if (_hasDefaultValue)
            {
                try
                {
                    _property.SetValue(options, _defaultValue, null);
                }
                catch (Exception e)
                {
                    throw new ParserException("Bad default value.", e);
                }
            }
        }

        private bool SetValueList(string value, object options)
        {
            _property.SetValue(options, new List<string>(), null);
            var fieldRef = (IList<string>)_property.GetValue(options, null);
            var values = value.Split(((OptionListAttribute)_attribute).Separator);
            foreach (var item in values)
            {
                fieldRef.Add(item);
            }

            return ReceivedValue = true;
        }
    }
}