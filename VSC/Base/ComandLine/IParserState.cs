using System.Collections.Generic;


namespace VSC.Base.CommandLine
{
    /// <summary>
    /// Represents the parser state.
    /// </summary>
    public interface IParserState
    {
        /// <summary>
        /// Gets errors occurred during parsing.
        /// </summary>
        IList<ParsingError> Errors { get; }
    }
}