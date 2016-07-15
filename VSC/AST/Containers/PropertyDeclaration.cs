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
        void CreateAutomaticProperty(ResolveContext rc)
        {
            //// Create backing field
            //BackingField = new BackingFieldDeclaration(this, Initializer == null && Set == null);
            //if (!BackingField.Define())
            //    return;

            //if (Initializer != null)
            //{
            //    BackingField.Initializer = Initializer;
            //    Parent.RegisterFieldForInitialization(BackingField, new FieldInitializer(BackingField, Initializer, Location));
            //    BackingField.ModFlags |= Modifiers.READONLY;
            //}

            //Parent.PartialContainer.Members.Add(BackingField);

            //FieldExpr fe = new FieldExpr(BackingField, Location);
            //if ((BackingField.ModFlags & Modifiers.STATIC) == 0)
            //    fe.InstanceExpression = new CompilerGeneratedThis(Parent.CurrentType, Location);

            ////
            //// Create get block but we careful with location to
            //// emit only single sequence point per accessor. This allow
            //// to set a breakpoint on it even with no user code
            ////
            //Get.Block = new ToplevelBlock(rc, ParametersCompiled.EmptyReadOnlyParameters, Location.Null);
            //Return r = new Return(fe, Get.Location);
            //Get.Block.AddStatement(r);
            //Get.ModFlags |= Modifiers.COMPILER_GENERATED;

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
     
            if (!CheckBase(resolver))
                return false;

            return ok;
        }
    }
}