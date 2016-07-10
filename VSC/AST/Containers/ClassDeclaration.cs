using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    public sealed class ClassDeclaration : ClassOrStructDeclaration
    {  
        /// <summary>
        ///   Modifiers allowed in a class declaration
        /// </summary>
        const Modifiers AllowedModifiers =
            Modifiers.NEW |
            Modifiers.PUBLIC |
            Modifiers.PROTECTED |
            Modifiers.INTERNAL |
            Modifiers.PRIVATE |
            Modifiers.ABSTRACT |
            Modifiers.SEALED |
            Modifiers.STATIC;
        public ClassDeclaration(TypeContainer parent, MemberName name, Modifiers mod, VSharpAttributes attrs, Location l, CompilationSourceFile file)
            : base(parent, mod, AllowedModifiers, name, attrs, l, TypeKind.Class,file)
        {

        }

        public override void ResolveWithCurrentContext(ResolveContext rc)
        {
            base.ResolveWithCurrentContext(rc);

            // modifiers check
            if (ResolvedTypeDefinition.IsSealed && ResolvedTypeDefinition.IsAbstract)
                rc.Report.Error(152, Location, "`{0}': an abstract class cannot be sealed or static", GetSignatureForError());

            if ((mod_flags & Modifiers.STATIC) == Modifiers.STATIC && (mod_flags & Modifiers.SEALED) == Modifiers.SEALED)
                rc.Report.Error(153, Location, "`{0}': a class cannot be both static and sealed", GetSignatureForError());

            
            // classes cannot derive from a special type
            if (ResolvedBaseType.IsKnownType(KnownTypeCode.Delegate) || ResolvedBaseType.IsKnownType(KnownTypeCode.MulticastDelegate) || ResolvedBaseType.IsKnownType(KnownTypeCode.Enum) || ResolvedBaseType.IsKnownType(KnownTypeCode.ValueType))
                rc.Report.Error(195, Location, "`{0}' cannot derive from special class `{1}'",
                    GetSignatureForError(), ResolvedBaseType.ToString());


            // members check
            if (IsStatic)
            {
                if (PrimaryConstructorParameters != null)
                {
                    rc.Report.Error(154, Location, "`{0}': Static classes cannot have primary constructor", GetSignatureForError());
                    PrimaryConstructorParameters = null;
                }
            }
        }
    }
}