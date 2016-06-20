using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSC.TypeSystem
{
    [Flags]
    public enum Modifiers
    {
        PROTECTED = 0x0001,
        PUBLIC = 0x0002,
        PRIVATE = 0x0004,
        FRIEND = 0x0008,
        NEW = 0x0010,
        ABSTRACT = 0x0020,
        SEALED = 0x0040,
        STATIC = 0x0080,
        READONLY = 0x0100,
        VIRTUAL = 0x0200,
        OVERRIDE = 0x0400,
        EXTERN = 0x0800,
        SUPERSEDE = 0x1000,
        UNSAFE = 0x2000,
        TOP = 0x4000,

        //
        // Compiler specific flags
        //
        PROPERTY_CUSTOM = 0x10000,

        DEFAULT_ACCESS_MODIFIER = 0x40000,
        METHOD_EXTENSION = 0x80000,
        COMPILER_GENERATED = 0x100000,
        BACKING_FIELD = 0x200000,

        AccessibilityMask = PUBLIC | PROTECTED | FRIEND | PRIVATE,
        AllowedExplicitImplFlags = UNSAFE | EXTERN,
    }

    static class ModifiersExtensions
    {
        public static string AccessibilityName(Modifiers mod)
        {
            switch (mod & Modifiers.AccessibilityMask)
            {
                case Modifiers.PUBLIC:
                    return "public";
                case Modifiers.PROTECTED:
                    return "protected";
                case Modifiers.PROTECTED | Modifiers.FRIEND:
                    return "protected friend";
                case Modifiers.FRIEND:
                    return "friend";
                case Modifiers.PRIVATE:
                    return "private";
                default:
                    throw new NotImplementedException(mod.ToString());
            }
        }
        static public string Name(Modifiers i)
        {
            string s = "";

            switch (i)
            {
                case Modifiers.NEW:
                    s = "new"; break;
                case Modifiers.PUBLIC:
                    s = "public"; break;
                case Modifiers.PROTECTED:
                    s = "protected"; break;
                case Modifiers.FRIEND:
                    s = "friend"; break;
                case Modifiers.PRIVATE:
                    s = "private"; break;
                case Modifiers.ABSTRACT:
                    s = "abstract"; break;
                case Modifiers.SEALED:
                    s = "sealed"; break;
                case Modifiers.STATIC:
                    s = "static"; break;
                case Modifiers.READONLY:
                    s = "readonly"; break;
                case Modifiers.VIRTUAL:
                    s = "virtual"; break;
                case Modifiers.OVERRIDE:
                    s = "override"; break;
                case Modifiers.EXTERN:
                    s = "extern"; break;
                case Modifiers.SUPERSEDE:
                    s = "supersede"; break;
                case Modifiers.UNSAFE:
                    s = "unsafe"; break;
   
            }

            return s;
        }

        //
        // Used by custom property accessors to check whether @modA is more restrictive than @modB
        //
        public static bool IsRestrictedModifier(Modifiers modA, Modifiers modB)
        {
            Modifiers flags = 0;

            if ((modB & Modifiers.PUBLIC) != 0)
            {
                flags = Modifiers.PROTECTED | Modifiers.FRIEND | Modifiers.PRIVATE;
            }
            else if ((modB & Modifiers.PROTECTED) != 0)
            {
                if ((modB & Modifiers.FRIEND) != 0)
                    flags = Modifiers.PROTECTED | Modifiers.FRIEND;

                flags |= Modifiers.PRIVATE;
            }
            else if ((modB & Modifiers.FRIEND) != 0)
                flags = Modifiers.PRIVATE;

            return modB != modA && (modA & (~flags)) == 0;
        }
        // <summary>
        //   Checks the object @mod modifiers to be in @allowed.
        //   Returns the new mask.  Side effect: reports any
        //   incorrect attributes. 
        // </summary>
        public static Modifiers Check(Modifiers allowed, Modifiers mod, Modifiers def_access, Location l, Report Report)
        {
            int invalid_flags = (~(int)allowed) & ((int)mod & ((int)Modifiers.TOP - 1));
            int i;

            if (invalid_flags == 0)
            {
                //
                // If no accessibility bits provided
                // then provide the defaults.
                //
                if ((mod & Modifiers.AccessibilityMask) == 0)
                {
                    mod |= def_access;
                    if (def_access != 0)
                        mod |= Modifiers.DEFAULT_ACCESS_MODIFIER;
                    return mod;
                }

                return mod;
            }

            for (i = 1; i < (int)Modifiers.TOP; i <<= 1)
            {
                if ((i & invalid_flags) == 0)
                    continue;

                Error_InvalidModifier((Modifiers)i, l, Report);
            }

            return allowed & mod;
        }
        static void Error_InvalidModifier(Modifiers mod, Location l, Report Report)
        {
            Report.Error(106, l, "The modifier `{0}' is not valid for this item",
                Name(mod));
        }
    }
}
