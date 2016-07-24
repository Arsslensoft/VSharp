using System;
using System.Collections.Generic;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    
    /// <summary>
    /// Default implementation of <see cref="IUnresolvedField"/>.
    /// </summary>
    [Serializable]
    public class FieldContainer : MemberContainer, IUnresolvedField
    {
        public List<FieldContainer> Declarators = new List<FieldContainer>();
        public ResolvedFieldSpec ResolvedField;

        public override IEntity ResolvedEntity
        {
            get { return ResolvedField; }
        }
        public override IType ResolvedMemberType
        {
            get { return ResolvedField.ReturnType; }
        }

        public bool IsInitializationRequired
        {
            get { return init != null; }
        }
      
        public FieldContainer(FieldContainer baseconstant, MemberName name,Modifiers allowed, SymbolKind sym)
            : this(baseconstant.Parent, baseconstant.TypeExpression, baseconstant.mod_flags, allowed, baseconstant.member_name, baseconstant.attribs,sym)
        {

        }
       
        public FieldContainer(TypeContainer parent, FullNamedExpression type, Modifiers mods, Modifiers allowed, MemberName name, VSharpAttributes attr,SymbolKind sym)
            : base(parent,type,mods, allowed, Modifiers.PRIVATE, name , attr,sym)
        {


            IsReadOnly = (mods & VSC.TypeSystem.Modifiers.READONLY) != 0;
            if ((mods & Modifiers.ABSTRACT) != 0)
                Report.Error(233, Location, "The modifier 'abstract' is not valid on fields. Try using a property instead");
        }


        protected Expression init = null;

        public virtual Expression Initializer
        {
            get { return init; }
            set
            {
                init = value;
                if (ConstantValue == null && init != null)
                {
                    ConstantValue = init as IConstantValue;
                    if (ConstantValue != null)
                        ConstantValue = new ImplicitCastExpression(type_expr, ConstantValue as Expression,init.Location);
                }
                else if (ConstantValue != null)
                {
                    ConstantValue = (ConstantValue as IConstantValue);
                    if (ConstantValue != null)
                        ConstantValue = new ImplicitCastExpression(type_expr, ConstantValue as Expression, init.Location);
                }
            }
        }
        IConstantValue constantValue;
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


        protected override void FreezeInternal()
        {
            FreezableHelper.Freeze(constantValue);
            base.FreezeInternal();
        }
        IField IUnresolvedField.Resolve(ITypeResolveContext context)
        {
            return (IField)Resolve(context);
        }
        /// <summary>
        /// Gets base method and its return type
        /// </summary>
        protected virtual IMember FindBaseMember(ResolveContext rc)
        {
            return (ResolvedEntity as ResolvedMemberSpec).FindBaseMembers();
        }
        protected override bool CheckBase(ResolveContext rc)
        {
            if (!base.CheckBase(rc))
                return false;

     
            bool overrides = false;
            var conflict_symbol = FindBaseMember(rc);

            if (conflict_symbol == null)
            {
                if ((mod_flags & Modifiers.NEW) != 0)
                    Report.Warning(234, 4, Location, "The member `{0}' does not hide an inherited member. The new keyword is not required",
                        GetSignatureForError());   
            }
            else
            {
                if ((mod_flags & (Modifiers.NEW | Modifiers.OVERRIDE | Modifiers.BACKING_FIELD)) == 0)
                    Report.Warning(235, 2, Location, "`{0}' hides inherited member `{1}'. Use the new keyword if hiding was intended",
                        GetSignatureForError(), conflict_symbol.ToString());


                if (conflict_symbol.IsAbstract)
                    Report.Error(236, Location, "`{0}' hides inherited abstract member `{1}'",
                        GetSignatureForError(), conflict_symbol.ToString());
                
            }

            return true;
        }
        protected override void CheckTypeDependency(ResolveContext rc)
        {
            base.CheckTypeDependency(rc);

            if (rc.IsStaticType(ResolvedMemberType))
                Report.Error(237, Location, "`{0}': cannot declare variables of static types",
                    name);

            if (!IsCompilerGenerated)
                CheckBase(rc);

            
        }
    }
}