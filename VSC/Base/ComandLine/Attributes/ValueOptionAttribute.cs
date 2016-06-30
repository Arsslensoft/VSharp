using System;


namespace VSC.Base.CommandLine
{
    /// <summary>
    /// Maps a single unnamed option to the target property. Values will be mapped in order of Index.
    /// This attribute takes precedence over <see cref="VSC.Base.CommandLine.ValueListAttribute"/> with which
    /// can coexist.
    /// </summary>
    /// <remarks>It can handle only scalar values. Do not apply to arrays or lists.</remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ValueOptionAttribute : Attribute
    {
        private readonly int _index;

        /// <summary>
        /// Initializes a new instance of the <see cref="VSC.Base.CommandLine.ValueOptionAttribute"/> class.
        /// </summary>
        /// <param name="index">The _index of the option.</param>
        public ValueOptionAttribute(int index)
        {
            _index = index;
        }

        /// <summary>
        /// Gets the position this option has on the command line.
        /// </summary>
        public int Index
        {
            get { return _index; }
        }
    }
}