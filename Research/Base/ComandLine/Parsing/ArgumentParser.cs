using System;
using System.Collections.Generic;
using VSC.Base.CommandLine.Extensions;

namespace VSC.Base.CommandLine.Parsing
{
    internal abstract class ArgumentParser
    {
        protected ArgumentParser()
        {
            PostParsingState = new List<ParsingError>();
        }

        public List<ParsingError> PostParsingState
        {
            get; private set;
        }

        public static bool CompareShort(string argument, char? option, bool caseSensitive)
        {
            return string.Compare(
                argument,
                ToOption(option),
                caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) == 0;
        }

        public static bool CompareLong(string argument, string option, bool caseSensitive)
        {
            return string.Compare(
                argument,
                ToOption(option),
                caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) == 0;
        }

        public static ArgumentParser Create(string argument, bool ignoreUnknownArguments = false)
        {
            if (argument.IsNumeric())
            {
                return null;
            }

            if (IsDash(argument))
            {
                return null;
            }

            if (IsLongOption(argument))
            {
                return new LongOptionParser(ignoreUnknownArguments);
            }

            if (IsShortOption(argument))
            {
                return new OptionGroupParser(ignoreUnknownArguments);
            }

            return null;
        }

        public static bool IsInputValue(string argument)
        {
            if (argument.IsNumeric())
            {
                return true;
            }

            if (argument.Length > 0)
            {
                return IsDash(argument) || !IsShortOption(argument);
            }

            return true;
        }

        public abstract PresentParserState Parse(IArgumentEnumerator argumentEnumerator, OptionMap map, object options);

        /// <summary>
        /// Helper method for testing purpose.
        /// </summary>
        /// <param name="ae">An argument enumerator instance.</param>
        /// <returns>The next input value.</returns>
        internal static IList<string> InternalWrapperOfGetNextInputValues(IArgumentEnumerator ae)
        {
            return GetNextInputValues(ae);
        }

        protected static IList<string> GetNextInputValues(IArgumentEnumerator ae)
        {
            IList<string> list = new List<string>();

            while (ae.MoveNext())
            {
                if (IsInputValue(ae.Current))
                {
                    list.Add(ae.Current);
                }
                else
                {
                    break;
                }
            }

            if (!ae.MovePrevious())
            {
                throw new ParserException();
            }

            return list;
        }

        protected static PresentParserState BooleanToParserState(bool value)
        {
            return BooleanToParserState(value, false);
        }

        protected static PresentParserState BooleanToParserState(bool value, bool addMoveNextIfTrue)
        {
            if (value && !addMoveNextIfTrue)
            {
                return PresentParserState.Success;
            }

            if (value)
            {
                return PresentParserState.Success | PresentParserState.MoveOnNextElement;
            }

            return PresentParserState.Failure;
        }

        protected static void EnsureOptionAttributeIsArrayCompatible(OptionInfo option)
        {
            if (!option.IsAttributeArrayCompatible)
            {
                throw new ParserException();
            }
        }

        protected static void EnsureOptionArrayAttributeIsNotBoundToScalar(OptionInfo option)
        {
            if (!option.IsArray && option.IsAttributeArrayCompatible)
            {
                throw new ParserException();
            }
        }

        protected void DefineOptionThatViolatesFormat(OptionInfo option)
        {
            PostParsingState.Add(new ParsingError(option.ShortName, option.LongName, true));
        }

        private static string ToOption(string value)
        {
            return string.Concat("--", value);
        }

        private static string ToOption(char? value)
        {
            return string.Concat("-", value);
        }

        private static bool IsDash(string value)
        {
            return string.CompareOrdinal(value, "-") == 0;
        }

        private static bool IsShortOption(string value)
        {
            return value[0] == '-';
        }

        private static bool IsLongOption(string value)
        {
            return value[0] == '-' && value[1] == '-';
        }
    }
}