using System;
using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    [Serializable]
    public class PropertyDeclaration : PropertyOrIndexer
    {

        Expression expr;
        public Expression Initializer
        {
            get
            {
                return expr;
            }
            set
            {
                expr = value;
            }
        }
        public PropertyDeclaration(TypeContainer parent, FullNamedExpression type, Modifiers mod,
            MemberName name, VSharpAttributes attrs)
            : base(parent, type, mod, parent is InterfaceDeclaration ? AllowedModifiersInterface :
                parent is StructDeclaration? AllowedModifiersStruct :
                    AllowedModifiersClass, name, attrs, SymbolKind.Property)
        {

        }
        public PropertyBackingField BackingField { get; private set; }
        // TODO:Resolve Automatic property
        //TODO: Remove Apply Modifiers (All Is*** must evaluate modifiers)

        void CreateAutomaticProperty(ResolveContext rc)
        {
            // Create backing field
            BackingField = new PropertyBackingField(this, Initializer == null && Setter == null);
           

            if (Initializer != null)
            {
                BackingField.Initializer = expr;
                BackingField.ModFlags |= Modifiers.READONLY;
    
            }
            BackingField.DoResolve(rc);

            MemberExpressionStatement fe = null;
              
           
            if ((BackingField.ModFlags & Modifiers.STATIC) == 0)
                fe = new MemberExpressionStatement(new SelfReference(rc.CurrentTypeDefinition, Location), BackingField.ResolvedField, BackingField.ResolvedMemberType);
            else fe = new MemberExpressionStatement(new TypeExpression(rc.CurrentTypeDefinition, Location), BackingField.ResolvedField, BackingField.ResolvedMemberType);

            ////
            //// Create get block but we careful with location to
            //// emit only single sequence point per accessor. This allow
            //// to set a breakpoint on it even with no user code
            ////
            //(Getter as GetterDeclaration).Block = new ToplevelBlock(rc, ParametersCompiled.EmptyReadOnlyParameters, Location.Null);
            //Return r = new Return(fe, Getter.Location);
            //(Getter as GetterDeclaration).Block.AddStatement(r);
    

            //// Create set block
            //if (Setter != null)
            //{
            //    (Setter as MethodCore).Block = new ToplevelBlock(Compiler, Setter.ParameterInfo, Location.Null);
            //    Assign a = new SimpleAssign(fe, new SimpleName("value", Location.Null), Location.Null);
            //    (Setter as MethodCore).Block.AddStatement(new StatementExpression(a, Set.Location));
            //    Setter.ModFlags |= Modifiers.COMPILER_GENERATED;
            //}
        }
        public override bool DoResolve(ResolveContext resolver)
        {
          bool ok =  base.DoResolve(resolver);
            AutoImplemented = AccessorFirst.Block == null && (AccessorSecond == null || AccessorSecond.Block == null) &&
                (ModFlags & (Modifiers.ABSTRACT | Modifiers.EXTERN)) == 0;

            // initializer check
            if (Initializer != null)
            {
                if (!AutoImplemented)
                    resolver.Report.Error(220, Location, "`{0}': Only auto-implemented properties can have initializers",
                        GetSignatureForError());

                if (IsInterfaceMember)
                    resolver.Report.Error(221, Location, "`{0}': Properties inside interfaces cannot have initializers",
                        GetSignatureForError());

            }

            if (AutoImplemented)
            {
                ModFlags |= Modifiers.AutoProperty;
                if (Getter == null)
                {
                   resolver.Report.Error(222, Location, "Auto-implemented property `{0}' must have get accessor",
                        GetSignatureForError());
                    return false;
                }

                CreateAutomaticProperty(resolver);
            }
       

            if (AccessorSecond == null)
            {
                PropertyMethod pm;
                if (AccessorFirst is GetterDeclaration)
                    pm = new SetterDeclaration(this, 0, ParametersCompiled.EmptyReadOnlyParameters, null, Location);
                else
                    pm = new GetterDeclaration(this, 0, null, Location);

                Parent.AddNameToContainer(pm, pm.MemberName.Basename);
            }

            if (!ResolveAccessors(resolver))
                return false;


            if (!CheckBase(resolver))
                return false;

            return ok;
        }

        public bool ResolveAccessors(ResolveContext rc)
        {
            //TODO:Resolve Accessors
            //resolver = resolver.WithCurrentMember(ResolvedProperty);
            //ResolveWithCurrentContext(resolver);
            return false;
        }

    }
    public class PropertyBackingField : FieldDeclaration
    {
        readonly PropertyDeclaration property;
        const Modifiers DefaultModifiers = Modifiers.BACKING_FIELD | Modifiers.COMPILER_GENERATED | Modifiers.PRIVATE | Modifiers.DEBUGGER_HIDDEN;

        public PropertyBackingField(PropertyDeclaration p, bool readOnly)
				: base (p.Parent, p.type_expr, DefaultModifiers | (p.ModFlags & (Modifiers.STATIC)),
				new MemberName ("<" + p.GetFullName (p.MemberName) + ">k__BackingField", p.Location), null)
			{
				this.property = p;
				if (readOnly)
					ModFlags |= Modifiers.READONLY;
			}
        public PropertyDeclaration OriginalProperty
        {
            get
            {
                return property;
            }
        }

        public override string GetSignatureForError()
        {
            return property.GetSignatureForError();
        }
    }
}