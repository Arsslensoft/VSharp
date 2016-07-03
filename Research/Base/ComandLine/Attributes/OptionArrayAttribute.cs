using System;


namespace VSC.Base.CommandLine
{
    /// <summary>
    /// Models an option that can accept multiple values as separated arguments.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class OptionArrayAttribute : BaseOptionAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VSC.Base.CommandLine.OptionArrayAttribute"/> class.
        /// The default long name will be inferred from target property.
        /// </summary>
        public OptionArrayAttribute()
        {
            AutoLongName = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VSC.Base.CommandLine.OptionArrayAttribute"/> class.
        /// </summary>
        /// <param name="shortName">The short name of the option.</param>
        public OptionArrayAttribute(char shortName)
            : base(shortName, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VSC.Base.CommandLine.OptionArrayAttribute"/> class.
        /// </summary>
        /// <param name="longName">The long name of the option.</param>
        public OptionArrayAttribute(string longName)
            : base(null, longName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VSC.Base.CommandLine.OptionArrayAttribute"/> class.
        /// </summary>
        /// <param name="shortName">The short name of the option.</param>
        /// <param name="longName">The long name of the option or null if not used.</param>
        public OptionArrayAttribute(char shortName, string longName)
            : base(shortName, longName)
        {
        }
    }
}