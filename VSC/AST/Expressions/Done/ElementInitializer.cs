using System.Diagnostics;
using VSC.Context;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{	
    //
    // Empty statement expression
    //
    public sealed class EmptyExpressionStatement : ExpressionStatement
    {
        public static readonly EmptyExpressionStatement Instance = new EmptyExpressionStatement();

        private EmptyExpressionStatement()
        {
            loc = Location.Null;
        }

        public override Expression DoResolve(ResolveContext rc)
        {
            if (_resolved)
                return this;
            eclass = ExprClass.Value;
            ResolvedType = KnownTypeReference.Object.Resolve(rc);
            _resolved = true;
            return this;
        }

      
    }
    public class ElementInitializer : Assign
    {
        public readonly string Name;

        public ElementInitializer(string name, Expression initializer, Location loc)
            : base(null, initializer, loc)
        {
            this.Name = name;
        }

        public override VSC.AST.Expression DoResolve(VSC.TypeSystem.Resolver.ResolveContext rc)
        {
            if (_resolved)
                return this;
            if (source == null)
                return EmptyExpressionStatement.Instance;

            if (!ResolveElement(rc))
                return null;

            if (source is CollectionOrObjectInitializers)
            {
                Expression previous = rc.CurrentInitializerVariable;
                rc.CurrentInitializerVariable = target;
                source = source.DoResolve(rc);
                rc.CurrentInitializerVariable = previous;
           
                if (source == null)
                    return null;

                eclass = source.eclass;
                ResolvedType = source.Type;
                _resolved = true;
                return this;
            }

            return base.DoResolve(rc);
        }

        protected virtual bool ResolveElement(ResolveContext rc)
        {
            var t = rc.CurrentInitializerVariable.Type;


            Expression lhsRR = rc.ResolveIdentifierInObjectInitializer(Name);
           
                 if (lhsRR == null)
                 {
                rc.Report.Error(0, loc, "`{0}' does not contain a definition for `{1}'",
                    t.ToString(), Name);
                    return false;
                 }
             var me = lhsRR as MemberExpressionStatement;
            Debug.Assert(me != null);

           
            //TODO:Support  events for el init
             /*   if (me.Member.SymbolKind == SymbolKind.Event)
                {
                    me = me.ResolveMemberAccess(rc, null, null);
                }
                else*/ if (!(me.Member.SymbolKind == SymbolKind.Property || me.Member.SymbolKind == SymbolKind.Field))
                {
                    rc.Report.Error(0, loc,
                        "Member `{0}' cannot be initialized. An object initializer may only be used for fields, or properties",
                        me.Member.ToString());

                    return false;
                }

                if (me.Member.IsStatic)
                {
                    rc.Report.Error(0, loc,
                        "Static field or property `{0}' cannot be assigned in an object initializer",
                        me.GetSignatureForError());
                }

                target = me;        

            return true;
        }
    }
}