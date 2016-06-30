using System.Collections.Generic;


namespace VSC.Base.CommandLine
{
    /// <summary>
    /// Models a type that records the parser state after parsing.
    /// </summary>
    public sealed class ParserState : IParserState
    {
        internal ParserState()
        {
            Errors = new List<ParsingError>();
        }

        /// <summary>
        /// Gets a list of parsing errors.
        /// </summary>
        /// <value>
        /// Parsing errors.
        /// </value>
        public IList<ParsingError> Errors { get; private set; }
    }
}