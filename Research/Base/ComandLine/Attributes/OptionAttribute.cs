using System;
using VSC.Base.CommandLine.Parsing;

namespace VSC.Base.CommandLine
{
    /// <summary>
    /// Models an option specification.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class OptionAttribute : BaseOptionAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VSC.Base.CommandLine.OptionAttribute"/> class.
        /// The default long name will be inferred from target property.
        /// </summary>
        public OptionAttribute()
        {
            AutoLongName = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VSC.Base.CommandLine.OptionAttribute"/> class.
        /// </summary>
        /// <param name="shortName">The short name of the option..</param>
        public OptionAttribute(char shortName)
            : base(shortName, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VSC.Base.CommandLine.OptionAttribute"/> class.
        /// </summary>
        /// <param name="longName">The long name of the option.</param>
        public OptionAttribute(string longName)
            : base(null, longName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VSC.Base.CommandLine.OptionAttribute"/> class.
        /// </summary>
        /// <param name="shortName">The short name of the option.</param>
        /// <param name="longName">The long name of the option or null if not used.</param>
        public OptionAttribute(char shortName, string longName)
            : base(shortName, longName)
        {
        }

        /// <summary>
        /// Helper factory method for testing purpose.
        /// </summary>
        /// <returns>An <see cref="OptionInfo"/> instance.</returns>
        internal OptionInfo CreateOptionInfo()
        {
            return new OptionInfo(ShortName, LongName);
        }
    }
}