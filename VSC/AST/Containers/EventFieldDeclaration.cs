using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.TypeSystem;

namespace VSC.AST
{
    public sealed class EventFieldDeclaration : EventDeclaration
    {
        public List<EventFieldDeclaration> Declarators = new List<EventFieldDeclaration>();
        public Expression Initializer;
        public EventFieldDeclaration(TypeContainer parent, FullNamedExpression type, Modifiers mod_flags, MemberName name, VSharpAttributes attrs)
            : base (parent, type, mod_flags, name, attrs,true)
        {
    

        }

        public EventFieldDeclaration(EventFieldDeclaration decl, string ident, Location loc)
            :base(decl.Parent, decl.type_expr, decl.mod_flags,new MemberName(ident,loc), decl.attribs, true)
        {
            

        }

    }
}
