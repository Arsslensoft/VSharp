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

    class InternalType : TypeSpec
    {
        public static readonly InternalType AnonymousMethod = new InternalType("anonymous method");
        public static readonly InternalType NullLiteral = new InternalType("null");
        public static readonly InternalType FakeInternalType = new InternalType("<fake$type>");
        public static readonly InternalType Namespace = new InternalType("<namespace>");
        public static readonly InternalType ErrorType = new InternalType("<error>");
        public static readonly InternalType VarOutType = new InternalType("var out");

        readonly string name;

        InternalType(string name)
            : base(MemberKind.InternalCompilerType, null, Modifiers.PUBLIC)
        {
            this.name = name;
        }

        #region Properties

        public override int Arity
        {
            get
            {
                return 0;
            }
        }
     
        public override string Name
        {
            get
            {
                return name;
            }
        }
        #endregion

     

    
    }
    public class BuiltinTypes
    {
        public readonly BuiltinTypeSpec Object;
        public readonly BuiltinTypeSpec ValueType;
        public readonly BuiltinTypeSpec Attribute;

        public readonly BuiltinTypeSpec Int;
        public readonly BuiltinTypeSpec UInt;
        public readonly BuiltinTypeSpec Long;
        public readonly BuiltinTypeSpec ULong;
        public readonly BuiltinTypeSpec Float;
        public readonly BuiltinTypeSpec Double;
        public readonly BuiltinTypeSpec Char;
        public readonly BuiltinTypeSpec Short;
        public readonly BuiltinTypeSpec Decimal;
        public readonly BuiltinTypeSpec Bool;
        public readonly BuiltinTypeSpec SByte;
        public readonly BuiltinTypeSpec Byte;
        public readonly BuiltinTypeSpec UShort;
        public readonly BuiltinTypeSpec String;

        public readonly BuiltinTypeSpec Enum;
        public readonly BuiltinTypeSpec Delegate;
        public readonly BuiltinTypeSpec MulticastDelegate;
        public readonly BuiltinTypeSpec Void;
        public readonly BuiltinTypeSpec Array;
        public readonly BuiltinTypeSpec Type;
        public readonly BuiltinTypeSpec IEnumerator;
        public readonly BuiltinTypeSpec IEnumerable;
        public readonly BuiltinTypeSpec IDisposable;
        public readonly BuiltinTypeSpec IntPtr;
        public readonly BuiltinTypeSpec UIntPtr;
        public readonly BuiltinTypeSpec RuntimeFieldHandle;
        public readonly BuiltinTypeSpec RuntimeTypeHandle;
        public readonly BuiltinTypeSpec Exception;

        //
        // These are internal buil-in types which depend on other
        // build-in type (mostly object)
        //
        public readonly BuiltinTypeSpec Dynamic;

        // Predefined operators tables
        public readonly TypeSpec[][] OperatorsUnary;
        public readonly TypeSpec[] OperatorsUnaryMutator;

        public readonly TypeSpec[] BinaryPromotionsTypes;

        readonly BuiltinTypeSpec[] types;

        public BuiltinTypes()
        {
            Object = new BuiltinTypeSpec(MemberKind.Class, "System", "Object", BuiltinTypeSpec.Type.Object);
            ValueType = new BuiltinTypeSpec(MemberKind.Class, "System", "ValueType", BuiltinTypeSpec.Type.ValueType);
            Attribute = new BuiltinTypeSpec(MemberKind.Class, "System", "Attribute", BuiltinTypeSpec.Type.Attribute);

            Int = new BuiltinTypeSpec(MemberKind.Struct, "System", "Int32", BuiltinTypeSpec.Type.Int);
            Long = new BuiltinTypeSpec(MemberKind.Struct, "System", "Int64", BuiltinTypeSpec.Type.Long);
            UInt = new BuiltinTypeSpec(MemberKind.Struct, "System", "UInt32", BuiltinTypeSpec.Type.UInt);
            ULong = new BuiltinTypeSpec(MemberKind.Struct, "System", "UInt64", BuiltinTypeSpec.Type.ULong);
            Byte = new BuiltinTypeSpec(MemberKind.Struct, "System", "Byte", BuiltinTypeSpec.Type.Byte);
            SByte = new BuiltinTypeSpec(MemberKind.Struct, "System", "SByte", BuiltinTypeSpec.Type.SByte);
            Short = new BuiltinTypeSpec(MemberKind.Struct, "System", "Int16", BuiltinTypeSpec.Type.Short);
            UShort = new BuiltinTypeSpec(MemberKind.Struct, "System", "UInt16", BuiltinTypeSpec.Type.UShort);

            IEnumerator = new BuiltinTypeSpec(MemberKind.Interface, "System.Collections", "IEnumerator", BuiltinTypeSpec.Type.IEnumerator);
            IEnumerable = new BuiltinTypeSpec(MemberKind.Interface, "System.Collections", "IEnumerable", BuiltinTypeSpec.Type.IEnumerable);
            IDisposable = new BuiltinTypeSpec(MemberKind.Interface, "System", "IDisposable", BuiltinTypeSpec.Type.IDisposable);

            Char = new BuiltinTypeSpec(MemberKind.Struct, "System", "Char", BuiltinTypeSpec.Type.Char);
            String = new BuiltinTypeSpec(MemberKind.Class, "System", "String", BuiltinTypeSpec.Type.String);
            Float = new BuiltinTypeSpec(MemberKind.Struct, "System", "Single", BuiltinTypeSpec.Type.Float);
            Double = new BuiltinTypeSpec(MemberKind.Struct, "System", "Double", BuiltinTypeSpec.Type.Double);
            Bool = new BuiltinTypeSpec(MemberKind.Struct, "System", "Boolean", BuiltinTypeSpec.Type.Bool);
            IntPtr = new BuiltinTypeSpec(MemberKind.Struct, "System", "IntPtr", BuiltinTypeSpec.Type.IntPtr);
            UIntPtr = new BuiltinTypeSpec(MemberKind.Struct, "System", "UIntPtr", BuiltinTypeSpec.Type.UIntPtr);

            MulticastDelegate = new BuiltinTypeSpec(MemberKind.Class, "System", "MulticastDelegate", BuiltinTypeSpec.Type.MulticastDelegate);
            Delegate = new BuiltinTypeSpec(MemberKind.Class, "System", "Delegate", BuiltinTypeSpec.Type.Delegate);
            Enum = new BuiltinTypeSpec(MemberKind.Class, "System", "Enum", BuiltinTypeSpec.Type.Enum);
            Array = new BuiltinTypeSpec(MemberKind.Class, "System", "Array", BuiltinTypeSpec.Type.Array);
            Void = new BuiltinTypeSpec(MemberKind.Void, "System", "Void", BuiltinTypeSpec.Type.Other);
            Type = new BuiltinTypeSpec(MemberKind.Class, "System", "Type", BuiltinTypeSpec.Type.Type);
            Exception = new BuiltinTypeSpec(MemberKind.Class, "System", "Exception", BuiltinTypeSpec.Type.Exception);
            RuntimeFieldHandle = new BuiltinTypeSpec(MemberKind.Struct, "System", "RuntimeFieldHandle", BuiltinTypeSpec.Type.Other);
            RuntimeTypeHandle = new BuiltinTypeSpec(MemberKind.Struct, "System", "RuntimeTypeHandle", BuiltinTypeSpec.Type.Other);
       

            types = new BuiltinTypeSpec[] {
				Object, ValueType, Attribute,
				Int, UInt, Long, ULong, Float, Double, Char, Short, Decimal, Bool, SByte, Byte, UShort, String,
				Enum, Delegate, MulticastDelegate, Void, Array, Type, IEnumerator, IEnumerable, IDisposable,
				IntPtr, UIntPtr, RuntimeFieldHandle, RuntimeTypeHandle, Exception };
        }

        public BuiltinTypeSpec[] AllTypes
        {
            get
            {
                return types;
            }
        }

    
    }
    public class BuiltinTypeSpec : TypeSpec
    {
        public enum Type
        {
            None = 0,

            // Ordered carefully for fast compares
            FirstPrimitive = 1,
            Bool = 1,
            Byte = 2,
            SByte = 3,
            Char = 4,
            Short = 5,
            UShort = 6,
            Int = 7,
            UInt = 8,
            Long = 9,
            ULong = 10,
            Float = 11,
            Double = 12,
            LastPrimitive = 12,

            IntPtr = 14,
            UIntPtr = 15,

            Object = 16,
            Dynamic = 17,
            String = 18,
            Type = 19,

            ValueType = 20,
            Enum = 21,
            Delegate = 22,
            MulticastDelegate = 23,
            Array = 24,

            IEnumerator,
            IEnumerable,
            IDisposable,
            Exception,
            Attribute,
            Other,
        }


        readonly Type type;
        readonly string ns;
        readonly string name;


        public BuiltinTypeSpec(MemberKind kind, string ns, string name, Type builtinKind)
            : base(kind, null, Modifiers.PUBLIC)
        {
            this.type = builtinKind;
            this.ns = ns;
            this.name = name;
        }

        #region Properties

        public override int Arity
        {
            get
            {
                return 0;
            }
        }

        public override BuiltinTypeSpec.Type BuiltinType
        {
            get
            {
                return type;
            }
        }

        public string FullName
        {
            get
            {
                return ns + '.' + name;
            }
        }

        public override string Name
        {
            get
            {
                return name;
            }
        }

        public string Namespace
        {
            get
            {
                return ns;
            }
        }

        #endregion
        public static bool IsPrimitiveType(TypeSpec type)
        {
            return type.BuiltinType >= Type.FirstPrimitive && type.BuiltinType <= Type.LastPrimitive;
        }


    }

    //
    // Member details which are same between all member
    // specifications
    //
    public interface IMemberDefinition
    {
 
        string Name { get; }
        bool IsImported { get; }

        string[] ConditionalConditions();
        ObsoleteAttribute GetAttributeObsolete();
        void SetIsAssigned();
        void SetIsUsed();
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

            IsAccessor = 1 ,		// Method is an accessor
            IsGeneric = 1 << 1,		// Member contains type arguments

        }


        protected Modifiers modifiers;
        public StateFlags state;
        protected IMemberDefinition definition;
        public readonly MemberKind Kind;
        protected TypeSpec declaringType;

#if DEBUG
        static int counter;
        public int ID = counter++;
#endif

        protected MemberSpec(MemberKind kind, TypeSpec declaringType, IMemberDefinition definition, Modifiers modifiers)
        {
            this.Kind = kind;
            this.declaringType = declaringType;
            this.definition = definition;
            this.modifiers = modifiers;

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

        public IMemberDefinition MemberDefinition
        {
            get
            {
                return definition;
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

  public  class TypeSpec : MemberSpec
    {
      public bool IsPointer { get; set; }
      public bool IsNullableType;

      public TypeSpec(MemberKind kind, TypeSpec declaringType, Modifiers modifiers)
          : base(kind, declaringType,null, modifiers)
      {
          this.declaringType = declaringType;

      }
        public virtual BuiltinTypeSpec.Type BuiltinType
        {
            get
            {
                return BuiltinTypeSpec.Type.None;
            }
        }
    }
}
