using System.Linq;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    public class StructDeclaration : ClassOrStructDeclaration
    {
        // <summary>
        //   Modifiers allowed in a struct declaration
        // </summary>
        const Modifiers AllowedModifiers =
            Modifiers.NEW       |
            Modifiers.PUBLIC    |
            Modifiers.PROTECTED |
            Modifiers.INTERNAL  |
            Modifiers.PRIVATE;


        public StructDeclaration(TypeContainer parent, MemberName name, Modifiers mod, VSharpAttributes attrs, Location l, CompilationSourceFile file)
            : base(parent, mod, AllowedModifiers, name, attrs, l, TypeKind.Struct,file)
        {
            mod_flags |= Modifiers.SEALED;
            IsSealed = true;
        }

        /*
         Algorithm:
         * The current struct is target
         * If a field is typeof struct
            * if the field has a member definition
                    *  check it for cycles with current target
         
         
         * */

        bool CheckFieldTypeCycle(IType type, ResolveContext rc)
        {
            var rts = type as ResolvedTypeDefinitionSpec;
            if (rts == null)
                return true;

            var fts = rts.Parts.First() as StructDeclaration;
            if (fts == null)
                return true;

            return fts.CheckForCycles(rc);
        }

     
        private bool Checking = false;
        /// <summary>
        ///  Performed on emit
        /// </summary>
        /// <param name="rc"></param>
        /// <returns></returns>
      public  bool CheckForCycles(ResolveContext rc)
        {
            if (Checking)
                return false;


            Checking = true;
            foreach (var member in Members)
            {
                var field = member as FieldDeclaration;
                if (field == null)
                    continue;

                IType ftype = field.ResolvedField.ReturnType;
                if (ftype.Kind != TypeKind.Struct)
                    continue;

                if (ftype.IsParameterized)
                    continue;

                foreach (var targ in ftype.TypeArguments)
                {
                    if (!CheckFieldTypeCycle(targ,rc))
                    {
                        rc.Report.Error(523, field.Location,
                            "Struct member `{0}' of type `{1}' causes a cycle in the struct layout",
                            field.GetSignatureForError(), ftype.ToString());
                        break;
                    }
                }

                //
                // Static fields of exactly same type are allowed
                //
                if (field.IsStatic && ftype == ResolvedTypeDefinition)
                    continue;

                if (!CheckFieldTypeCycle(ftype,rc))
                {
                    rc.Report.Error(523, field.Location,
                        "Struct member `{0}' of type `{1}' causes a cycle in the struct layout",
                        field.GetSignatureForError(), ftype.ToString());
                    break;
                }
            }

            Checking = false;
            return true;
        }

        public override void ResolveWithCurrentContext(ResolveContext rc)
        {
            base.ResolveWithCurrentContext(rc);
        }
    }
}