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
        NONE = 0,
        PROTECTED = 0x0001,
        PUBLIC = 0x0002,
        PRIVATE = 0x0004,
        INTERNAL = 0x0008,
        NEW = 0x0010,
        ABSTRACT = 0x0020,
        SEALED = 0x0040,
        STATIC = 0x0080,
        READONLY = 0x0100,
        VIRTUAL = 0x0200,
        OVERRIDE = 0x0400,
        EXTERN = 0x0800,
        SUPERSEDE = 0x1000,
        PARTIAL = 0x2000,
        TOP = 0x4000,

        //
        // Compiler specific flags
        //
        PROPERTY_CUSTOM = 0x10000,

        DEFAULT_ACCESS_MODIFIER = 0x40000,
        METHOD_EXTENSION = 0x80000,
        COMPILER_GENERATED = 0x100000,
        BACKING_FIELD = 0x200000,

        AccessibilityMask = PUBLIC | PROTECTED | INTERNAL | PRIVATE,
        AllowedExplicitImplFlags = EXTERN,
    }
}
