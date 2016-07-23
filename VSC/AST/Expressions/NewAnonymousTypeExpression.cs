using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    class AnonymousTypeMember
    {
        public readonly Expression Expression;
        public readonly Expression Initializer;

        public AnonymousTypeMember(Expression expression, Expression initializer)
        {
            this.Expression = expression;
            this.Initializer = initializer;
        }
    }
    public class NewAnonymousTypeExpression : NewExpression
    {
        static readonly AnonymousTypeParameter[] EmptyParameters = new AnonymousTypeParameter[0];

        List<AnonymousTypeParameter> parameters;
        readonly TypeContainer parent;

        public NewAnonymousTypeExpression(List<AnonymousTypeParameter> parameters, TypeContainer parent, Location loc)
            : base (null, null, loc)
        {
            this.parameters = parameters;
            this.parent = parent;
        }

        public List<AnonymousTypeParameter> Parameters {
            get {
                return this.parameters;
            }
        }
        AnonymousTypeClass CreateAnonymousType(ResolveContext ec, IList<AnonymousTypeParameter> parameters)
        {
            AnonymousTypeClass type = parent.Module.GetAnonymousType(parameters);
            if (type != null)
                return type;

            type = AnonymousTypeClass.Create(parent, parameters, loc);
            if (type == null)
                return null;

            int errors = ec.Report.Errors;
            type.DoResolve(ec);
            if ((ec.Report.Errors - errors) == 0)
                parent.Module.AddAnonymousType(type);
 

            return type;
        }
        AnonymousTypeClass anonymous_type;
        public override VSC.AST.Expression DoResolve(VSC.TypeSystem.Resolver.ResolveContext rc)
        {
            if (parameters == null)
            {
                anonymous_type = CreateAnonymousType(rc, EmptyParameters);
                RequestedType = new TypeExpression(anonymous_type, loc);
                return base.DoResolve(rc);
            }

            bool error = false;
            arguments = new Arguments(parameters.Count);
            for (int i = 0; i < parameters.Count; ++i)
            {
                Expression e = parameters[i].DoResolve(rc);
                if (e == null)
                {
                    error = true;
                    continue;
                }

                arguments.Add(new Argument(e, e.Location));
            }

            if (error)
                return null;

            anonymous_type = CreateAnonymousType(rc, parameters);
            if (anonymous_type == null)
                return null;


            return base.DoResolve(rc);
        }
    }
}