using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;

namespace VSC.AST
{
    /// <summary>
    /// Default implementation of <see cref="IUnresolvedField"/>.
    /// </summary>
    [Serializable]
    public class FieldDeclaration : MemberContainer, IUnresolvedField
    {
      
        public List<FieldDeclaration> Declarators = new List<FieldDeclaration>();
       	// <summary>
		//   Modifiers allowed in a class declaration
		// </summary>
		const Modifiers AllowedFieldModifiers =
			Modifiers.NEW |
			Modifiers.PUBLIC |
			Modifiers.PROTECTED |
			Modifiers.INTERNAL |
			Modifiers.PRIVATE |
			Modifiers.STATIC |
			Modifiers.READONLY;

        IConstantValue constantValue;

        protected override void FreezeInternal()
        {
            FreezableHelper.Freeze(constantValue);
            base.FreezeInternal();
        }

          // For declarators
        public FieldDeclaration(FieldDeclaration baseconstant, MemberName name, Modifiers allowed)
            : this(baseconstant.Parent, baseconstant.ReturnType, baseconstant.mod_flags,allowed, baseconstant.member_name,baseconstant.attributes)
        {
            
        }
        public FieldDeclaration(FieldDeclaration baseconstant, MemberName name)
            : this(baseconstant.Parent, baseconstant.ReturnType, baseconstant.mod_flags, AllowedFieldModifiers, baseconstant.member_name, baseconstant.attributes)
        {

        }


        public FieldDeclaration(TypeContainer parent, FullNamedExpression type, Modifiers mods, MemberName name, VSharpAttributes attr)
            : this(parent, type, mods, AllowedFieldModifiers, name, attr)
        {

        }

        public FieldDeclaration(TypeContainer parent, FullNamedExpression type, Modifiers mods, Modifiers allowed, MemberName name, VSharpAttributes attr)
            : this(parent, name.Name)
        {
            Parent = parent;
            CheckModifiersAndSetNames(mods, allowed, Modifiers.PRIVATE, name);
            if (attr != null)
                foreach (var a in attr.Attrs)
                    this.attributes.Add(a);


            this.returnType = type as ITypeReference;
 

        }
              public FieldDeclaration(TypeContainer parent, ITypeReference type, Modifiers mods, Modifiers allowed, MemberName name, VSharpAttributes attr)
            : this(parent, name.Name)
        {
            Parent = parent;
            CheckModifiersAndSetNames(mods, allowed, Modifiers.PRIVATE, name);

            if (attr != null)
                foreach (var a in attr.Attrs)
                    this.attributes.Add(a);


            this.returnType = type;


        }
        public FieldDeclaration(TypeContainer parent, ITypeReference type, Modifiers mods, Modifiers allowed, MemberName name, IList<IUnresolvedAttribute> attr)
            : this(parent, name.Name)
        {
            Parent = parent;
            CheckModifiersAndSetNames(mods & ~Modifiers.STATIC, allowed, Modifiers.PRIVATE, name);

            if (attr != null)
                foreach (var a in attr)
                    this.attributes.Add(a);


            this.returnType = type;


        }

        public FieldDeclaration()
        {
            this.SymbolKind = SymbolKind.Field;
        }

        public FieldDeclaration(IUnresolvedTypeDefinition declaringType, string name)
        {
            type_expr = declaringType as FullNamedExpression;
            this.SymbolKind = SymbolKind.Field;
            this.DeclaringTypeDefinition = declaringType;
            this.Name = name;
            if (declaringType != null)
                this.UnresolvedFile = declaringType.UnresolvedFile;
        }

        public bool IsConst
        {
            get { return constantValue != null && !IsFixed; }
        }

        public bool IsReadOnly
        {
            get { return flags[FlagFieldIsReadOnly]; }
            set
            {
                ThrowIfFrozen();
                flags[FlagFieldIsReadOnly] = value;
            }
        }

        public bool IsVolatile
        {
            get { return flags[FlagFieldIsVolatile]; }
            set
            {
                ThrowIfFrozen();
                flags[FlagFieldIsVolatile] = value;
            }
        }

        public bool IsFixed
        {
            get { return flags[FlagFieldIsFixedSize]; }
            set
            {
                ThrowIfFrozen();
                flags[FlagFieldIsFixedSize] = value;
            }
        }

        public IConstantValue ConstantValue
        {
            get { return constantValue; }
            set
            {
                ThrowIfFrozen();
                constantValue = value;
            }
        }

        public override IMember CreateResolved(ITypeResolveContext context)
        {
            return new ResolvedFieldSpec(this, context);
        }

        IField IUnresolvedField.Resolve(ITypeResolveContext context)
        {
            return (IField)Resolve(context);
        }
    }
}
