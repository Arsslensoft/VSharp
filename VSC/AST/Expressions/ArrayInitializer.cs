using System.Collections.Generic;

namespace VSC.AST
{
    public class ArrayInitializer : Expression
    {
        List<Expression> elements;
        BlockVariable variable;

        public ArrayInitializer(List<Expression> init, Location loc)
        {
            elements = init;
            this.loc = loc;
        }

        public ArrayInitializer(int count, Location loc)
            : this(new List<Expression>(count), loc)
        {
        }

        public ArrayInitializer(Location loc)
            : this(4, loc)
        {
        }

        #region Properties

        public int Count
        {
            get { return elements.Count; }
        }

        public List<Expression> Elements
        {
            get
            {
                return elements;
            }
        }

        public Expression this[int index]
        {
            get
            {
                return elements[index];
            }
        }

        public BlockVariable VariableDeclaration
        {
            get
            {
                return variable;
            }
            set
            {
                variable = value;
            }
        }

        #endregion

        public void Add(Expression expr)
        {
            elements.Add(expr);
        }
    }
}