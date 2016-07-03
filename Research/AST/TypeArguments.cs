using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;

namespace VSC.AST
{
    [Flags]
    public enum SpecialConstraint
    {
        None = 0,
        Constructor = 1 << 2,
        Class = 1 << 3,
        Struct = 1 << 4
    }
    public class SpecialContraintExpr : FullNamedExpression
    {
        public SpecialContraintExpr(SpecialConstraint constraint, Location loc)
        {
            this.loc = loc;
            this.Constraint = constraint;
        }
        public SpecialConstraint Constraint { get; private set; }
    }
    //
	// A set of parsed constraints for a type parameter
	//
    public class TypeParameterConstraints
    {
        readonly SimpleMemberName tparam;
        readonly List<FullNamedExpression> constraints;
        readonly Location loc;
        bool resolved;
        bool resolving;

        public TypeParameterConstraints(SimpleMemberName tparam, List<FullNamedExpression> constraints, Location loc)
        {
            this.tparam = tparam;
            this.constraints = constraints;
            this.loc = loc;
        }
        #region Properties

        public List<FullNamedExpression> TypeExpressions
        {
            get
            {
                return constraints;
            }
        }

        public Location Location
        {
            get
            {
                return loc;
            }
        }

        public SimpleMemberName TypeParameter
        {
            get
            {
                return tparam;
            }
        }

        #endregion
    }
    public class TypeParameters
    {
       public List<UnresolvedTypeParameterSpec> names;
        TypeParameterSpec[] types;

        public TypeParameters()
        {
            names = new List<UnresolvedTypeParameterSpec>();
        }

        public TypeParameters(int count)
        {
            names = new List<UnresolvedTypeParameterSpec>(count);
        }

        #region Properties

        public int Count
        {
            get
            {
                return names.Count;
            }
        }

        public TypeParameterSpec[] Types
        {
            get
            {
                return types;
            }
        }

        #endregion

        public void Add(UnresolvedTypeParameterSpec tparam)
        {
            names.Add(tparam);
        }

        public void Add(TypeParameters tparams)
        {
            names.AddRange(tparams.names);
        }



        public UnresolvedTypeParameterSpec this[int index]
        {
            get
            {
                return names[index];
            }
            set
            {
                names[index] = value;
            }
        }

        public UnresolvedTypeParameterSpec Find(string name)
        {
            foreach (var tp in names)
            {
                if (tp.Name == name)
                    return tp;
            }

            return null;
        }

        public string[] GetAllNames()
        {
            return names.Select(l => l.Name).ToArray();
        }

        public string GetSignatureForError()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Count; ++i)
            {
                if (i > 0)
                    sb.Append(',');

                var name = names[i];
                if (name != null)
                    sb.Append(name.ToString());
            }

            return sb.ToString();
        }


    }
    public class UnboundTypeArguments : TypeArguments
    {
        Location loc;

        public UnboundTypeArguments(int arity, Location loc)
            : base(new FullNamedExpression[arity])
        {
            this.loc = loc;
        }

        public override bool IsEmpty
        {
            get
            {
                return true;
            }
        }

       
    }
    public class TypeArguments
    {

        public List<ITypeReference> ToTypeReferences(InterningProvider intern)
        {
        List<ITypeReference> l = new List<ITypeReference>();
        foreach (var fe in args)
            l.Add(fe as ITypeReference);
        return l;
        }
        List<FullNamedExpression> args;
      //  TypeSpec[] atypes;

        public TypeArguments(params FullNamedExpression[] types)
        {
            this.args = new List<FullNamedExpression>(types);
        }

        public void Add(FullNamedExpression type)
        {
            args.Add(type);
        }


        public int Count
        {
            get
            {
                return args.Count;
            }
        }

        public virtual bool IsEmpty
        {
            get
            {
                return false;
            }
        }
        public List<FullNamedExpression> Arguments
        {
            get { return args; }
        }
        public string GetSignatureForError()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Count; ++i)
            {
                var expr = args[i];
                if (expr != null)
                    sb.Append(expr.GetSignatureForError());

                if (i + 1 < Count)
                    sb.Append(',');
            }

            return sb.ToString();
        }
    }
}
