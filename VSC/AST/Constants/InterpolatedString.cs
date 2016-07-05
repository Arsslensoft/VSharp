using System.Collections.Generic;

namespace VSC.AST
{
    public class InterpolatedString : Expression
    {
        readonly StringLiteral start, end;
        List<Expression> interpolations;
        Arguments arguments;

        public InterpolatedString(StringLiteral start, List<Expression> interpolations, StringLiteral end)
        {
            this.start = start;
            this.end = end;
            this.interpolations = interpolations;
            loc = start.Location;
        }

    }
}