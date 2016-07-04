using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.TypeSystem;

namespace VSC.AST
{
    public class AliasIdentifier
    {
        public string Value;
        public Location Location;

        public AliasIdentifier(string name, Location loc)
        {
            this.Value = name;
            this.Location = loc;
        }
    }
    public class Import
    {
        readonly TypeNameExpression expr;
        readonly Location loc;

        public Import(TypeNameExpression expr, Location loc)
        {
            this.expr = expr;
            this.loc = loc;
        }

        #region Properties

        public virtual AliasIdentifier Alias
        {
            get
            {
                return null;
            }
        }

        public Location Location
        {
            get
            {
                return loc;
            }
        }

        public TypeNameExpression NamespaceExpression
        {
            get
            {
                return expr;
            }
        }

   

        #endregion

        public string GetSignatureForError()
        {
            return expr.GetSignatureForError();
        }
      
     
    }
   public class ImportPackage :Import, IResolve,IAstNode
    {
       public ImportPackage(TypeNameExpression expr, Location loc)
			: base (expr, loc)
		{
		}


       public object DoResolve(Context.ResolveContext rc)
       {
           throw new NotImplementedException();
       }

       public bool Resolve(Context.ResolveContext rc)
       {
           throw new NotImplementedException();
       }

       public virtual void AcceptVisitor(IVisitor visitor)
       {
           visitor.Visit(this); 
       }
    }
   public class ImportPackageAlias : ImportPackage
   {
       readonly AliasIdentifier alias;

       public ImportPackageAlias(AliasIdentifier alias, TypeNameExpression expr, Location loc)
			: base (expr, loc)
		{
			this.alias = alias;
		}

		public override AliasIdentifier Alias {
			get {
				return alias;
			}
		}

        public override void AcceptVisitor(IVisitor visitor)
        {
            visitor.Visit(this);
        }
   }
}
