using VSC.TypeSystem;

namespace VSC.AST
{
    public class AnonymousTypeParameter : ShimExpression
    {
        public readonly string Name;
        
        public AnonymousTypeParameter(Expression initializer, string name, Location loc)
            : base(initializer)
        {
            this.Name = name;
            this.loc = loc;
        }

        public AnonymousTypeParameter(Parameter parameter)
            : base(new SimpleName(parameter.Name, parameter.Location))
        {
            this.Name = parameter.Name;
            this.loc = parameter.Location;
        }
        public override Expression DoResolve(TypeSystem.Resolver.ResolveContext rc)
        {
            Expression e = expr.DoResolve(rc);
            if (e == null)
                return null;

            if (e.eclass == ExprClass.MethodGroup)
            {
                rc.Report.Error(0, loc, "An anonymous type property `{0}' cannot be initialized with `{1}'",
                    Name, e.GetSignatureForError());
                return null;
            }

            ResolvedType = e.Type;
            if (ResolvedType.Kind == TypeKind.Void || ResolvedType.Kind == TypeKind.Pointer)
            {
                rc.Report.Error(0, loc, "An anonymous type property `{0}' cannot be initialized with `{1}'",
                    Name, ResolvedType.ToString());
                return null;
            }

            return e;
        }
        public override bool Equals(object o)
        {
            AnonymousTypeParameter other = o as AnonymousTypeParameter;
            return other != null && Name == other.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}