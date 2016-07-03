using System;


namespace VSC.Base.CommandLine
{
    /// <summary>
    /// Indicates that the property can receive an instance of type <see cref="VSC.Base.CommandLine.IParserState"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ParserStateAttribute : Attribute
    {
    }
}