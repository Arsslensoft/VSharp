using System;
using System.Collections.Generic;
using VSC.TypeSystem.Resolver;

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


        public override Expression DoResolve(ResolveContext rc)
        {
            var current_field = rc.CurrentMember as FieldContainer;
            TypeExpression type;
            if (current_field != null && rc.CurrentAnonymousMethod == null)
                type = new TypeExpression(current_field.ReturnType, current_field.Location);
            else if (variable != null)
            {
                if (variable.TypeExpression is VarTypeExpression)
                {
                    rc.Report.Error(0, loc, "An implicitly typed local variable declarator cannot use an array initializer");
                    return EmptyExpression.Null;
                }

                type = new TypeExpression(variable.Variable.Type, variable.Variable.Location);
            }
            else
            {
                throw new NotImplementedException("Unexpected array initializer context");
            }

            return new ArrayCreation(type, this).DoResolve(rc);
        }
    }
}