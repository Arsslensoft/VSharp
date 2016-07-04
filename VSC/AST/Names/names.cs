using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.Base;
using VSC.Context;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    public abstract class FullNamedExpression : Expression
    {
        public virtual ITypeReference ToTypeReference(InterningProvider intern)
        {
            return this as ITypeReference;
        }
    }
    public abstract class TypeNameExpression : FullNamedExpression , ITypeReference
    {
      protected  string name;
        protected TypeArguments targs;
        public NameLookupMode lookupMode;
        protected TypeNameExpression(string name, Location l)
        {
            this.name = name;
            loc = l;
        }
        protected TypeNameExpression(string name, TypeArguments targs, Location l)
        {
            this.name = name;
            this.targs = targs;
            loc = l;
        }
        protected TypeNameExpression(string name, int arity, Location l)
            : this(name, new UnboundTypeArguments(arity, l), l)
        {
        }

        #region ITypeReference
        /// <summary>
        /// Resolves the reference and returns the ResolveResult.
        /// </summary>
        public abstract ResolveResult Resolve(VSharpResolver resolver);

        /// <summary>
        /// Returns the type that is referenced; or an <c>UnknownType</c> if the type isn't found.
        /// </summary>
        public abstract IType ResolveType(VSharpResolver resolver);

        /// <summary>
        /// Returns the namespace that is referenced; or null if no such namespace is found.
        /// </summary>
        public INamespace ResolveNamespace(VSharpResolver resolver)
        {
            NamespaceResolveResult nrr = Resolve(resolver) as NamespaceResolveResult;
            return nrr != null ? nrr.Namespace : null;
        }

        IType ITypeReference.Resolve(ITypeResolveContext context)
        {
            // Strictly speaking, we might have to resolve the type in a nested compilation, similar
            // to what we're doing with ConstantExpression.
            // However, in almost all cases this will work correctly - if the resulting type is only available in the
            // nested compilation and not in this, we wouldn't be able to map it anyways.
            var ctx = context as VSharpTypeResolveContext;
            if (ctx == null)
            {
                ctx = new VSharpTypeResolveContext(context.CurrentAssembly ?? context.Compilation.MainAssembly, null, context.CurrentTypeDefinition, context.CurrentMember);
            }
            return ResolveType(new VSharpResolver(ctx));

            // A potential issue might be this scenario:

            // Assembly 1:
            //  class A { public class Nested {} }

            // Assembly 2: (references asm 1)
            //  class B : A {}

            // Assembly 3: (references asm 1 and 2)
            //  class C { public B.Nested Field; }

            // Assembly 4: (references asm 1 and 3, but not 2):
            //  uses C.Field;

            // Here we would not be able to resolve 'B.Nested' in the compilation of assembly 4, as type B is missing there.
        }
        #endregion
        #region Properties

        public int Arity
        {
            get
            {
                return targs == null ? 0 : targs.Count;
            }
        }

        public bool HasTypeArguments
        {
            get
            {
                return targs != null && !targs.IsEmpty;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public TypeArguments TypeArguments
        {
            get
            {
                return targs;
            }
        }

        #endregion

        public override string GetSignatureForError()
        {
            if (targs != null)
            {
                return Name + "<" + targs.GetSignatureForError() + ">";
            }

            return Name;
        }

    }
    /// <summary>
	///   Expression that evaluates to a type
	/// </summary>
    public abstract class TypeExpr : FullNamedExpression, ITypeReference
    {
        public abstract   IType Resolve(ITypeResolveContext context);
      
        public override bool Equals(object obj)
        {
            TypeExpr tobj = obj as TypeExpr;
            if (tobj == null)
                return false;

            return Type == tobj.Type;
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode();
        }
    }
    /// <summary>
    ///   Fully resolved Expression that already evaluated to a type
    /// </summary>
    public class TypeExpression : TypeExpr
    {
        public override IType Resolve(ITypeResolveContext ctx)
        {
            return type.Resolve(ctx);
        }
        public TypeExpression(ITypeReference t, Location l)
        {
         
            Type = t;
            loc = l;
        }
        public override void AcceptVisitor(IVisitor vis)
        {
            vis.Visit(this);
        }
    }
    public class VarTypeExpression : SimpleName
    {
        public VarTypeExpression(Location loc)
            : base("var", loc)
        {
        }


    }
   // <summary>
	//   This class is used to "construct" the type during a typecast
	//   operation.
	// </summary>
    public class ComposedType : TypeExpr
    {
        FullNamedExpression left;
        ComposedTypeSpecifier spec;
        public override IType Resolve(ITypeResolveContext ctx)
        {
            throw new NotSupportedException();
        }
        public ComposedType(FullNamedExpression left, ComposedTypeSpecifier spec)
        {
            if (spec == null)
                throw new ArgumentNullException("spec");

            this.left = left;
            this.spec = spec;
            this.loc = left.Location;
        }
        public override void AcceptVisitor(IVisitor vis)
        {
            vis.Visit(this);
        }
    }
    //
    // Holds additional type specifiers like ?, *, []
    //
    public class ComposedTypeSpecifier
    {
        public static readonly ComposedTypeSpecifier SingleDimension = new ComposedTypeSpecifier(1, Location.Null);

        public readonly int Dimension;
        public readonly Location Location;

        public ComposedTypeSpecifier(int specifier, Location loc)
        {
            this.Dimension = specifier;
            this.Location = loc;
        }

        #region Properties
        public bool IsNullable
        {
            get
            {
                return Dimension == -1;
            }
        }

        public bool IsPointer
        {
            get
            {
                return Dimension == -2;
            }
        }

        public ComposedTypeSpecifier Next { get; set; }

        #endregion

        public static ComposedTypeSpecifier CreateArrayDimension(int dimension, Location loc)
        {
            return new ComposedTypeSpecifier(dimension, loc);
        }

        public static ComposedTypeSpecifier CreateNullable(Location loc)
        {
            return new ComposedTypeSpecifier(-1, loc);
        }

        public static ComposedTypeSpecifier CreatePointer(Location loc)
        {
            return new ComposedTypeSpecifier(-2, loc);
        }

        public string GetSignatureForError()
        {
            string s =
                IsPointer ? "*" :
                IsNullable ? "?" :
                GetPostfixSignature(Dimension);

            return Next != null ? s + Next.GetSignatureForError() : s;
        }

        public static string GetPostfixSignature(int rank)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            for (int i = 1; i < rank; i++)
            {
                sb.Append(",");
            }
            sb.Append("]");

            return sb.ToString();
        }

    }


	/// <summary>
	///   Implements the member access expression
	/// </summary>
    [Serializable]
    public class MemberAccess : TypeNameExpression, ISupportsInterning
    {
        protected Expression expr;

        public MemberAccess(Expression expr, string id, NameLookupMode lookupMode = NameLookupMode.Type)
            : base(id, expr.Location)
        {
            this.expr = expr;
            this.typeArgumentsrefs = EmptyList<ITypeReference>.Instance;
            this.lookupMode = lookupMode;
        }
        public MemberAccess(Expression expr, string identifier, Location loc, NameLookupMode lookupMode = NameLookupMode.Type)
            : base(identifier, loc)
        {
            this.expr = expr;
            this.typeArgumentsrefs = EmptyList<ITypeReference>.Instance;
            this.lookupMode = lookupMode;
        }
        public MemberAccess(Expression expr, string identifier, TypeArguments args, Location loc, NameLookupMode lookupMode = NameLookupMode.Type)
            : base(identifier, args, loc)
        {
            this.expr = expr;
            this.typeArgumentsrefs = targs != null ? targs.ToTypeReferences(CompilerContext.InternProvider) : EmptyList<ITypeReference>.Instance.ToList();
            this.lookupMode = lookupMode;
        }
        public MemberAccess(Expression expr, string identifier, int arity, Location loc, NameLookupMode lookupMode = NameLookupMode.Type)
            : base(identifier, arity, loc)
        {
            this.expr = expr;
            this.typeArgumentsrefs =  EmptyList<ITypeReference>.Instance;
            this.lookupMode = lookupMode;
        }

        public Expression LeftExpression
        {
            get
            {
                return expr;
            }
        }

        public override string GetSignatureForError()
        {
            return expr.GetSignatureForError() + "." + base.GetSignatureForError();
        }

        public override void AcceptVisitor(IVisitor vis)
        {
            vis.Visit(this);
        }

        #region ITypeReference
		protected readonly IList<ITypeReference> typeArgumentsrefs;
	
	
			
		public IList<ITypeReference> TypeArgumentsReferences {
            get { return typeArgumentsrefs; }
		}
		
		public NameLookupMode LookupMode {
			get { return lookupMode; }
		}
		
		/// <summary>
		/// Adds a suffix to the identifier.
		/// Does not modify the existing type reference, but returns a new one.
		/// </summary>
        public MemberAccess AddSuffix(string suffix)
		{
            return new MemberAccess(expr, name + suffix, targs, Location, lookupMode);
		}
		
		public override ResolveResult Resolve(VSharpResolver resolver)
		{
            TypeNameExpression target = expr as TypeNameExpression;
			ResolveResult targetRR = target.Resolve(resolver);
			if (targetRR.IsError)
				return targetRR;
			IList<IType> typeArgs = typeArgumentsrefs.Resolve(resolver.CurrentTypeResolveContext);
			return resolver.ResolveMemberAccess(targetRR, name, typeArgs, lookupMode);
		}
		
		public override IType ResolveType(VSharpResolver resolver)
		{
			TypeResolveResult trr = Resolve(resolver) as TypeResolveResult;
            return trr != null ? trr.Type : new UnknownTypeSpec(null, name, typeArgumentsrefs.Count);
		}
		
		public override string ToString()
		{
            if (typeArgumentsrefs.Count == 0)
                return expr.ToString() + "." + name;
			else
                return expr.ToString() + "." + name + "<" + string.Join(",", typeArgumentsrefs) + ">";
		}
		
		int ISupportsInterning.GetHashCodeForInterning()
		{
			int hashCode = 0;
			unchecked {
				hashCode += 1000000007 * expr.GetHashCode();
                hashCode += 1000000033 * name.GetHashCode();
                hashCode += 1000000087 * typeArgumentsrefs.GetHashCode();
				hashCode += 1000000021 * (int)lookupMode;
			}
			return hashCode;
		}
		
		bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
		{
			MemberAccess o = other as MemberAccess;
            return o != null && this.expr == o.expr
                && this.name == o.name && this.typeArgumentsrefs == o.typeArgumentsrefs
				&& this.lookupMode == o.lookupMode;
		}

        #endregion

    }

    /// <summary>
    /// Looks up an alias (identifier in front of :: operator).
    /// </summary>
    /// <remarks>
    /// The member lookup performed by the :: operator is handled
    /// by <see cref="MemberTypeOrNamespaceReference"/>.
    /// </remarks>
    [Serializable]
    public sealed class AliasNamespace : TypeNameExpression, ISupportsInterning
    {

        public AliasNamespace(string identifier, Location l)
            : base(identifier,l)
        {
       
        }

     
        public override ResolveResult Resolve(VSharpResolver resolver)
        {
            return resolver.ResolveAlias(name);
        }

        public override IType ResolveType(VSharpResolver resolver)
        {
            // alias cannot refer to types
            return SpecialTypeSpec.UnknownType;
        }

        public override string ToString()
        {
            return name + "::";
        }

        int ISupportsInterning.GetHashCodeForInterning()
        {
            return name.GetHashCode();
        }

        bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
        {
            AliasNamespace anr = other as AliasNamespace;
            return anr != null && this.name == anr.name;
        }
    }
    /// <summary>
    ///   Implements the qualified-alias-member (::) expression.
    /// </summary>
    public class QualifiedAlias : MemberAccess
    {

        readonly string alias;

        public QualifiedAlias(string alias, string identifier, Location l, NameLookupMode lookupMode = NameLookupMode.Type)
            : base(null, identifier, l, lookupMode)
		{
			this.alias = alias;
		}

        public QualifiedAlias(string alias, string identifier, TypeArguments targs, Location l, NameLookupMode lookupMode = NameLookupMode.Type)
			: base (null, identifier, targs, l, lookupMode)
		{
			this.alias = alias;
      
		}

        public QualifiedAlias(string alias, string identifier, int arity, Location l, NameLookupMode lookupMode = NameLookupMode.Type)
			: base (null, identifier, arity, l,lookupMode)
		{
			this.alias = alias;
		}

		public string Alias {
			get {
				return alias;
			}
		}
  
        public override string GetSignatureForError()
        {
            string name = Name;
            if (targs != null)
                name = Name + "<" + targs.GetSignatureForError() + ">";


            return alias + "::" + name;
        }

        public override void AcceptVisitor(IVisitor vis)
        {
            vis.Visit(this);
        }

        #region ITypeReference
        public override ResolveResult Resolve(VSharpResolver resolver)
        {
            AliasNamespace target = new AliasNamespace(alias, Location);
            ResolveResult targetRR = target.Resolve(resolver);
            if (targetRR.IsError)
                return targetRR;
            IList<IType> typeArgs = typeArgumentsrefs.Resolve(resolver.CurrentTypeResolveContext);
            return resolver.ResolveMemberAccess(targetRR, name, typeArgs, lookupMode);
        }
        public override string ToString()
        {
            return GetSignatureForError();
        }
        public override IType ResolveType(VSharpResolver resolver)
        {
            TypeResolveResult trr = Resolve(resolver) as TypeResolveResult;
            return trr != null ? trr.Type : new UnknownTypeSpec(Alias, name, typeArgumentsrefs.Count);
        }
        #endregion
    }
    /// <summary>
    /// Represents a simple V# name. (a single non-qualified identifier with an optional list of type arguments)
    /// </summary>
    [Serializable]
    public class SimpleName : TypeNameExpression, ISupportsInterning
    {
        public SimpleName(string name, Location l, NameLookupMode lookupMode = NameLookupMode.Type)
            : base(name, l)
        {
            this.typeArgumentsrefs = EmptyList<ITypeReference>.Instance;
            this.lookupMode = lookupMode;
        }

        public SimpleName(string name, TypeArguments args, Location l, NameLookupMode lookupMode = NameLookupMode.Type)
            : base(name, args, l)
        {
            this.typeArgumentsrefs = args != null ? args.ToTypeReferences(CompilerContext.InternProvider) : EmptyList<ITypeReference>.Instance.ToList();
            this.lookupMode = lookupMode;
        }

        public SimpleName(string name, int arity, Location l, NameLookupMode lookupMode = NameLookupMode.Type)
            : base(name, arity, l)
        {
            this.typeArgumentsrefs = EmptyList<ITypeReference>.Instance;
            this.lookupMode = lookupMode;
        }

    
        public override void AcceptVisitor(IVisitor vis)
        {
            vis.Visit(this);
        }


        #region ITypeReference
        readonly IList<ITypeReference> typeArgumentsrefs;
    

   
        public IList<ITypeReference> TypeArgumentsReferences
        {
            get { return typeArgumentsrefs; }
        }

        public NameLookupMode LookupMode
        {
            get { return lookupMode; }
        }

        /// <summary>
        /// Adds a suffix to the identifier.
        /// Does not modify the existing type reference, but returns a new one.
        /// </summary>
        public SimpleName AddSuffix(string suffix)
        {
            return new SimpleName(name + suffix, this.targs,Location, lookupMode);
        }

        public override ResolveResult Resolve(VSharpResolver resolver)
        {
            var typeArgs = typeArgumentsrefs.Resolve(resolver.CurrentTypeResolveContext);
            return resolver.LookupSimpleNameOrTypeName(name, typeArgs, lookupMode);
        }

        public override IType ResolveType(VSharpResolver resolver)
        {
            TypeResolveResult trr = Resolve(resolver) as TypeResolveResult;
            return trr != null ? trr.Type : new UnknownTypeSpec(null, name,Arity);
        }

        public override string ToString()
        {
            if (typeArgumentsrefs.Count == 0)
                return name;
            else
                return name + "<" + string.Join(",", typeArgumentsrefs) + ">";
        }

        int ISupportsInterning.GetHashCodeForInterning()
        {
            int hashCode = 0;
            unchecked
            {
                hashCode += 1000000021 * name.GetHashCode();
                hashCode += 1000000033 * typeArgumentsrefs.GetHashCode();
                hashCode += 1000000087 * (int)lookupMode;
            }
            return hashCode;
        }

        bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
        {
            SimpleName o = other as SimpleName;
            return o != null && this.name == o.Name
                && this.typeArgumentsrefs == o.typeArgumentsrefs && this.lookupMode == o.lookupMode;
        }
        #endregion
    }

    //
    // Better name would be DottenName
    //
    [DebuggerDisplay("{GetSignatureForError()}")]
    public class MemberName
    {
        public static readonly MemberName Null = new MemberName("");

        public readonly string Name;
        public TypeParameters TypeParameters;
        public readonly FullNamedExpression ExplicitInterface;
        public readonly Location Location;

        public readonly MemberName Left;

        public MemberName(string name)
            : this(name, Location.Null)
        { }

        public MemberName(string name, Location loc)
            : this(null, name, loc)
        { }

        public MemberName(string name, TypeParameters tparams, Location loc)
        {
            this.Name = name;
            this.Location = loc;

            this.TypeParameters = tparams;
        }

        public MemberName(string name, TypeParameters tparams, FullNamedExpression explicitInterface, Location loc)
            : this(name, tparams, loc)
        {
            this.ExplicitInterface = explicitInterface;
        }

        public MemberName(MemberName left, string name, Location loc)
        {
            this.Name = name;
            this.Location = loc;
            this.Left = left;
        }

        public MemberName(MemberName left, string name, FullNamedExpression explicitInterface, Location loc)
            : this(left, name, loc)
        {
            this.ExplicitInterface = explicitInterface;
        }

        public MemberName(MemberName left, MemberName right)
        {
            this.Name = right.Name;
            this.Location = right.Location;
            this.TypeParameters = right.TypeParameters;
            this.Left = left;
        }

        public int Arity
        {
            get
            {
                return TypeParameters == null ? 0 : TypeParameters.Count;
            }
        }

        public bool IsGeneric
        {
            get
            {
                return TypeParameters != null;
            }
        }

        public string Basename
        {
            get
            {
                if (TypeParameters != null)
                    return MakeName(Name, TypeParameters);
                return Name;
            }
        }

        public void CreateMetadataName(StringBuilder sb)
        {
            if (Left != null)
                Left.CreateMetadataName(sb);

            if (sb.Length != 0)
            {
                sb.Append(".");
            }

            sb.Append(Basename);
        }

        public string GetSignatureForDocumentation()
        {
            var s = Basename;

            if (ExplicitInterface != null)
                s = ExplicitInterface.GetSignatureForError() + "." + s;

            if (Left == null)
                return s;

            return Left.GetSignatureForDocumentation() + "." + s;
        }

        public string GetSignatureForError()
        {
            string s = TypeParameters == null ? null : "<" + TypeParameters.GetSignatureForError() + ">";
            s = Name + s;

            if (ExplicitInterface != null)
                s = ExplicitInterface.GetSignatureForError() + "." + s;

            if (Left == null)
                return s;

            return Left.GetSignatureForError() + "." + s;
        }

        public override bool Equals(object other)
        {
            return Equals(other as MemberName);
        }

        public bool Equals(MemberName other)
        {
            if (this == other)
                return true;
            if (other == null || Name != other.Name)
                return false;

            if ((TypeParameters != null) &&
                (other.TypeParameters == null || TypeParameters.Count != other.TypeParameters.Count))
                return false;

            if ((TypeParameters == null) && (other.TypeParameters != null))
                return false;

            if (Left == null)
                return other.Left == null;

            return Left.Equals(other.Left);
        }

        public override int GetHashCode()
        {
            int hash = Name.GetHashCode();
            for (MemberName n = Left; n != null; n = n.Left)
                hash ^= n.Name.GetHashCode();

            if (TypeParameters != null)
                hash ^= TypeParameters.Count << 5;

            return hash & 0x7FFFFFFF;
        }

        public static string MakeName(string name, TypeParameters args)
        {
            if (args == null)
                return name;

            return name + "`" + args.Count;
        }
    }

    public class SimpleMemberName
    {
        public string Value;
        public Location Location;

        public SimpleMemberName(string name, Location loc)
        {
            this.Value = name;
            this.Location = loc;
        }
    }
}
