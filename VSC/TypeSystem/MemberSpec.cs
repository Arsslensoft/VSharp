using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSC.TypeSystem
{
    [Flags]
    public enum MemberKind
    {
        Constructor = 1,
        Event = 1 << 1,
        Field = 1 << 2,
        Method = 1 << 3,
        Property = 1 << 4,
        Indexer = 1 << 5,
        Operator = 1 << 6,
        Destructor = 1 << 7,

        Class = 1 << 11,
        Struct = 1 << 12,
        Delegate = 1 << 13,
        Enum = 1 << 14,
        Interface = 1 << 15,
        TypeParameter = 1 << 16,

        ArrayType = 1 << 19,
        PointerType = 1 << 20,
        InternalCompilerType = 1 << 21,
        MissingType = 1 << 22,
        Void = 1 << 23,
        Namespace = 1 << 24,

        NestedMask = Class | Struct | Delegate | Enum | Interface,
        GenericMask = Method | Class | Struct | Delegate | Interface,
        MaskType = Constructor | Event | Field | Method | Property | Indexer | Operator | Destructor | NestedMask
    }

   	//
	// Base member specification. A member specification contains
	// member details which can alter in the context (e.g. generic instances)
	//
    public abstract class MemberSpec
    {
        [Flags]
        public enum StateFlags
        {
            IsAccessor = 1 << 1,		// Method is an accessor
            IsGeneric = 1 << 2		// Member contains type arguments

        }
        protected Modifiers modifiers;
        protected IMemberDefinition definition;
        public readonly MemberKind Kind;
        protected TypeSpec declaringType;
        public StateFlags state;



        static int counter;
        public int ID = counter++;

        protected MemberSpec(MemberKind kind, TypeSpec declaringType, Modifiers modifiers)
        {
            this.Kind = kind;
            this.declaringType = declaringType;
            this.definition = definition;
            this.modifiers = modifiers;

            if (kind == MemberKind.MissingType)
                state = StateFlags.MissingDependency;
            else
                state = StateFlags.Obsolete_Undetected | StateFlags.CLSCompliant_Undetected | StateFlags.MissingDependency_Undetected;
        }

        #region Properties

        public virtual int Arity
        {
            get
            {
                return 0;
            }
        }

        public TypeSpec DeclaringType
        {
            get
            {
                return declaringType;
            }
            set
            {
                declaringType = value;
            }
        }

        public Modifiers Modifiers
        {
            get
            {
                return modifiers;
            }
            set
            {
                modifiers = value;
            }
        }

        public virtual string Name
        {
            get
            {
                return definition.Name;
            }
        }

        public bool IsAbstract
        {
            get { return (modifiers & Modifiers.ABSTRACT) != 0; }
        }

        public bool IsAccessor
        {
            get
            {
                return (state & StateFlags.IsAccessor) != 0;
            }
            set
            {
                state = value ? state | StateFlags.IsAccessor : state & ~StateFlags.IsAccessor;
            }
        }

        //
        // Return true when this member is a generic in C# terms
        // A nested non-generic type of generic type will return false
        //
        public bool IsGeneric
        {
            get
            {
                return (state & StateFlags.IsGeneric) != 0;
            }
            set
            {
                state = value ? state | StateFlags.IsGeneric : state & ~StateFlags.IsGeneric;
            }
        }

        //
        // Returns true for imported members which are not compatible with C# language
        //
        public bool IsNotCSharpCompatible
        {
            get
            {
                return (state & StateFlags.IsNotCSharpCompatible) != 0;
            }
            set
            {
                state = value ? state | StateFlags.IsNotCSharpCompatible : state & ~StateFlags.IsNotCSharpCompatible;
            }
        }

        public bool IsPrivate
        {
            get { return (modifiers & Modifiers.PRIVATE) != 0; }
        }

        public bool IsPublic
        {
            get { return (modifiers & Modifiers.PUBLIC) != 0; }
        }

        public bool IsStatic
        {
            get
            {
                return (modifiers & Modifiers.STATIC) != 0;
            }
        }

        #endregion
    }
}
