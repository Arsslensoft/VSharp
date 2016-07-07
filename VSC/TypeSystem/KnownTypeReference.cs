using System;

namespace VSC.TypeSystem
{
    /// <summary>
	/// Contains well-known type references.
	/// </summary>
	[Serializable]
	public sealed class KnownTypeReference : ITypeReference
	{
        internal const int KnownTypeCodeCount = (int)KnownTypeCode.IDisposable + 1;
		
		static readonly KnownTypeReference[] knownTypeReferences = new KnownTypeReference[KnownTypeCodeCount] {
			null, // None
			new KnownTypeReference(KnownTypeCode.Object,   "Std", "Object", baseType: KnownTypeCode.None),
			new KnownTypeReference(KnownTypeCode.DBNull,   "Std", "DBNull"),
			new KnownTypeReference(KnownTypeCode.Boolean,  "Std", "Boolean",  baseType: KnownTypeCode.ValueType),
			new KnownTypeReference(KnownTypeCode.Char,     "Std", "Char",     baseType: KnownTypeCode.ValueType),
			new KnownTypeReference(KnownTypeCode.SByte,    "Std", "SByte",    baseType: KnownTypeCode.ValueType),
			new KnownTypeReference(KnownTypeCode.Byte,     "Std", "Byte",     baseType: KnownTypeCode.ValueType),
			new KnownTypeReference(KnownTypeCode.Int16,    "Std", "Int16",    baseType: KnownTypeCode.ValueType),
			new KnownTypeReference(KnownTypeCode.UInt16,   "Std", "UInt16",   baseType: KnownTypeCode.ValueType),
			new KnownTypeReference(KnownTypeCode.Int32,    "Std", "Int32",    baseType: KnownTypeCode.ValueType),
			new KnownTypeReference(KnownTypeCode.UInt32,   "Std", "UInt32",   baseType: KnownTypeCode.ValueType),
			new KnownTypeReference(KnownTypeCode.Int64,    "Std", "Int64",    baseType: KnownTypeCode.ValueType),
			new KnownTypeReference(KnownTypeCode.UInt64,   "Std", "UInt64",   baseType: KnownTypeCode.ValueType),
			new KnownTypeReference(KnownTypeCode.Single,   "Std", "Single",   baseType: KnownTypeCode.ValueType),
			new KnownTypeReference(KnownTypeCode.Double,   "Std", "Double",   baseType: KnownTypeCode.ValueType),
			new KnownTypeReference(KnownTypeCode.Decimal,  "Std", "Decimal",  baseType: KnownTypeCode.ValueType),
			new KnownTypeReference(KnownTypeCode.DateTime, "Std", "DateTime", baseType: KnownTypeCode.ValueType),
			null,
			new KnownTypeReference(KnownTypeCode.String,    "Std", "String"),
			new KnownTypeReference(KnownTypeCode.Void,      "Std", "Void"),
			new KnownTypeReference(KnownTypeCode.Type,      "Std", "Type"),
			new KnownTypeReference(KnownTypeCode.Array,     "Std", "Array"),
			new KnownTypeReference(KnownTypeCode.Attribute, "Std", "Attribute"),
			new KnownTypeReference(KnownTypeCode.ValueType, "Std", "ValueType"),
			new KnownTypeReference(KnownTypeCode.Enum,      "Std", "Enum", baseType: KnownTypeCode.ValueType),
			new KnownTypeReference(KnownTypeCode.Delegate,  "Std", "Delegate"),
			new KnownTypeReference(KnownTypeCode.MulticastDelegate, "Std", "MulticastDelegate", baseType: KnownTypeCode.Delegate),
			new KnownTypeReference(KnownTypeCode.Exception, "Std", "Exception"),
			new KnownTypeReference(KnownTypeCode.IntPtr,    "Std", "IntPtr", baseType: KnownTypeCode.ValueType),
			new KnownTypeReference(KnownTypeCode.UIntPtr,   "Std", "UIntPtr", baseType: KnownTypeCode.ValueType),
			new KnownTypeReference(KnownTypeCode.IEnumerable,    "Std.Collections", "IEnumerable"),
			new KnownTypeReference(KnownTypeCode.IEnumerator,    "Std.Collections", "IEnumerator"),
			new KnownTypeReference(KnownTypeCode.IEnumerableOfT, "Std.Collections.Generic", "IEnumerable", 1),
			new KnownTypeReference(KnownTypeCode.IEnumeratorOfT, "Std.Collections.Generic", "IEnumerator", 1),
			new KnownTypeReference(KnownTypeCode.ICollection,    "Std.Collections", "ICollection"),
			new KnownTypeReference(KnownTypeCode.ICollectionOfT, "Std.Collections.Generic", "ICollection", 1),
			new KnownTypeReference(KnownTypeCode.IList,          "Std.Collections", "IList"),
			new KnownTypeReference(KnownTypeCode.IListOfT,       "Std.Collections.Generic", "IList", 1),

			new KnownTypeReference(KnownTypeCode.IReadOnlyCollectionOfT, "Std.Collections.Generic", "IReadOnlyCollection", 1),
			new KnownTypeReference(KnownTypeCode.IReadOnlyListOfT, "Std.Collections.Generic", "IReadOnlyList", 1),
			new KnownTypeReference(KnownTypeCode.NullableOfT, "Std", "Nullable", 1, baseType: KnownTypeCode.ValueType),
			new KnownTypeReference(KnownTypeCode.IDisposable, "Std", "IDisposable"),
		
		};
		
		/// <summary>
		/// Gets the known type reference for the specified type code.
		/// Returns null for KnownTypeCode.None.
		/// </summary>
		public static KnownTypeReference Get(KnownTypeCode typeCode)
		{
			return knownTypeReferences[(int)typeCode];
		}
		
		/// <summary>
		/// Gets a type reference pointing to the <c>object</c> type.
		/// </summary>
		public static readonly KnownTypeReference Object = Get(KnownTypeCode.Object);
		
		/// <summary>
		/// Gets a type reference pointing to the <c>System.DBNull</c> type.
		/// </summary>
		public static readonly KnownTypeReference DBNull = Get(KnownTypeCode.DBNull);
		
		/// <summary>
		/// Gets a type reference pointing to the <c>bool</c> type.
		/// </summary>
		public static readonly KnownTypeReference Boolean = Get(KnownTypeCode.Boolean);
		
		/// <summary>
		/// Gets a type reference pointing to the <c>char</c> type.
		/// </summary>
		public static readonly KnownTypeReference Char = Get(KnownTypeCode.Char);
		
		/// <summary>
		/// Gets a type reference pointing to the <c>sbyte</c> type.
		/// </summary>
		public static readonly KnownTypeReference SByte = Get(KnownTypeCode.SByte);
		
		/// <summary>
		/// Gets a type reference pointing to the <c>byte</c> type.
		/// </summary>
		public static readonly KnownTypeReference Byte = Get(KnownTypeCode.Byte);
		
		/// <summary>
		/// Gets a type reference pointing to the <c>short</c> type.
		/// </summary>
		public static readonly KnownTypeReference Int16 = Get(KnownTypeCode.Int16);
		
		/// <summary>
		/// Gets a type reference pointing to the <c>ushort</c> type.
		/// </summary>
		public static readonly KnownTypeReference UInt16 = Get(KnownTypeCode.UInt16);
		
		/// <summary>
		/// Gets a type reference pointing to the <c>int</c> type.
		/// </summary>
		public static readonly KnownTypeReference Int32 = Get(KnownTypeCode.Int32);
		
		/// <summary>
		/// Gets a type reference pointing to the <c>uint</c> type.
		/// </summary>
		public static readonly KnownTypeReference UInt32 = Get(KnownTypeCode.UInt32);
		
		/// <summary>
		/// Gets a type reference pointing to the <c>long</c> type.
		/// </summary>
		public static readonly KnownTypeReference Int64 = Get(KnownTypeCode.Int64);
		
		/// <summary>
		/// Gets a type reference pointing to the <c>ulong</c> type.
		/// </summary>
		public static readonly KnownTypeReference UInt64 = Get(KnownTypeCode.UInt64);
		
		/// <summary>
		/// Gets a type reference pointing to the <c>float</c> type.
		/// </summary>
		public static readonly KnownTypeReference Single = Get(KnownTypeCode.Single);
		
		/// <summary>
		/// Gets a type reference pointing to the <c>double</c> type.
		/// </summary>
		public static readonly KnownTypeReference Double = Get(KnownTypeCode.Double);
		
		/// <summary>
		/// Gets a type reference pointing to the <c>decimal</c> type.
		/// </summary>
		public static readonly KnownTypeReference Decimal = Get(KnownTypeCode.Decimal);
		
		/// <summary>
		/// Gets a type reference pointing to the <c>System.DateTime</c> type.
		/// </summary>
		public static readonly KnownTypeReference DateTime = Get(KnownTypeCode.DateTime);
		
		/// <summary>
		/// Gets a type reference pointing to the <c>string</c> type.
		/// </summary>
		public static readonly KnownTypeReference String = Get(KnownTypeCode.String);
		
		/// <summary>
		/// Gets a type reference pointing to the <c>void</c> type.
		/// </summary>
		public static readonly KnownTypeReference Void = Get(KnownTypeCode.Void);
		
		/// <summary>
		/// Gets a type reference pointing to the <c>System.Type</c> type.
		/// </summary>
		public static readonly KnownTypeReference Type = Get(KnownTypeCode.Type);
		
		/// <summary>
		/// Gets a type reference pointing to the <c>System.Array</c> type.
		/// </summary>
		public static readonly KnownTypeReference Array = Get(KnownTypeCode.Array);
		
		/// <summary>
		/// Gets a type reference pointing to the <c>System.Attribute</c> type.
		/// </summary>
		public static readonly KnownTypeReference Attribute = Get(KnownTypeCode.Attribute);
		
		/// <summary>
		/// Gets a type reference pointing to the <c>System.ValueType</c> type.
		/// </summary>
		public static readonly KnownTypeReference ValueType = Get(KnownTypeCode.ValueType);
		
		/// <summary>
		/// Gets a type reference pointing to the <c>System.Enum</c> type.
		/// </summary>
		public static readonly KnownTypeReference Enum = Get(KnownTypeCode.Enum);
		
		/// <summary>
		/// Gets a type reference pointing to the <c>System.Delegate</c> type.
		/// </summary>
		public static readonly KnownTypeReference Delegate = Get(KnownTypeCode.Delegate);
		
		/// <summary>
		/// Gets a type reference pointing to the <c>System.MulticastDelegate</c> type.
		/// </summary>
		public static readonly KnownTypeReference MulticastDelegate = Get(KnownTypeCode.MulticastDelegate);
		
		/// <summary>
		/// Gets a type reference pointing to the <c>System.Exception</c> type.
		/// </summary>
		public static readonly KnownTypeReference Exception = Get(KnownTypeCode.Exception);
		
		/// <summary>
		/// Gets a type reference pointing to the <c>System.IntPtr</c> type.
		/// </summary>
		public static readonly KnownTypeReference IntPtr = Get(KnownTypeCode.IntPtr);
		
		/// <summary>
		/// Gets a type reference pointing to the <c>System.UIntPtr</c> type.
		/// </summary>
		public static readonly KnownTypeReference UIntPtr = Get(KnownTypeCode.UIntPtr);
		
		/// <summary>
		/// Gets a type reference pointing to the <c>System.Collections.IEnumerable</c> type.
		/// </summary>
		public static readonly KnownTypeReference IEnumerable = Get(KnownTypeCode.IEnumerable);
		
		/// <summary>
		/// Gets a type reference pointing to the <c>System.Collections.IEnumerator</c> type.
		/// </summary>
		public static readonly KnownTypeReference IEnumerator = Get(KnownTypeCode.IEnumerator);
		
		/// <summary>
		/// Gets a type reference pointing to the <c>System.Collections.Generic.IEnumerable{T}</c> type.
		/// </summary>
		public static readonly KnownTypeReference IEnumerableOfT = Get(KnownTypeCode.IEnumerableOfT);
		
		/// <summary>
		/// Gets a type reference pointing to the <c>System.Collections.Generic.IEnumerator{T}</c> type.
		/// </summary>
		public static readonly KnownTypeReference IEnumeratorOfT = Get(KnownTypeCode.IEnumeratorOfT);
		
		/// <summary>
		/// Gets a type reference pointing to the <c>System.Collections.ICollection</c> type.
		/// </summary>
		public static readonly KnownTypeReference ICollection = Get(KnownTypeCode.ICollection);
		
		/// <summary>
		/// Gets a type reference pointing to the <c>System.Collections.Generic.ICollection{T}</c> type.
		/// </summary>
		public static readonly KnownTypeReference ICollectionOfT = Get(KnownTypeCode.ICollectionOfT);
		
		/// <summary>
		/// Gets a type reference pointing to the <c>System.Collections.IList</c> type.
		/// </summary>
		public static readonly KnownTypeReference IList = Get(KnownTypeCode.IList);
		
		/// <summary>
		/// Gets a type reference pointing to the <c>System.Collections.Generic.IList{T}</c> type.
		/// </summary>
		public static readonly KnownTypeReference IListOfT = Get(KnownTypeCode.IListOfT);
		
		/// <summary>
		/// Gets a type reference pointing to the <c>System.Collections.Generic.IReadOnlyCollection{T}</c> type.
		/// </summary>
		public static readonly KnownTypeReference IReadOnlyCollectionOfT = Get(KnownTypeCode.IReadOnlyCollectionOfT);
		
		/// <summary>
		/// Gets a type reference pointing to the <c>System.Collections.Generic.IReadOnlyList{T}</c> type.
		/// </summary>
		public static readonly KnownTypeReference IReadOnlyListOfT = Get(KnownTypeCode.IReadOnlyListOfT);
		
		
		/// <summary>
		/// Gets a type reference pointing to the <c>System.Nullable{T}</c> type.
		/// </summary>
		public static readonly KnownTypeReference NullableOfT = Get(KnownTypeCode.NullableOfT);
		
		/// <summary>
		/// Gets a type reference pointing to the <c>System.IDisposable</c> type.
		/// </summary>
		public static readonly KnownTypeReference IDisposable = Get(KnownTypeCode.IDisposable);



		readonly KnownTypeCode knownTypeCode;
		readonly string namespaceName;
		readonly string name;
		readonly int typeParameterCount;
		internal readonly KnownTypeCode baseType;
		
		private KnownTypeReference(KnownTypeCode knownTypeCode, string namespaceName, string name, int typeParameterCount = 0, KnownTypeCode baseType = KnownTypeCode.Object)
		{
			this.knownTypeCode = knownTypeCode;
			this.namespaceName = namespaceName;
			this.name = name;
			this.typeParameterCount = typeParameterCount;
			this.baseType = baseType;
		}
		
		public KnownTypeCode KnownTypeCode {
			get { return knownTypeCode; }
		}
		
		public string Namespace {
			get { return namespaceName; }
		}
		
		public string Name {
			get { return name; }
		}
		
		public int TypeParameterCount {
			get { return typeParameterCount; }
		}
		
		public IType Resolve(ITypeResolveContext context)
		{
			return context.Compilation.FindType(knownTypeCode);
		}
		
		public override string ToString()
		{
			return GetVSharpNameByTypeCode(knownTypeCode) ?? (this.Namespace + "." + this.Name);
		}
		
		/// <summary>
		/// Gets the V# primitive type name from the known type code.
		/// Returns null if there is no primitive name for the specified type.
		/// </summary>
		public static string GetVSharpNameByTypeCode(KnownTypeCode knownTypeCode)
		{
			switch (knownTypeCode) {
				case KnownTypeCode.Object:
					return "object";
				case KnownTypeCode.Boolean:
					return "bool";
				case KnownTypeCode.Char:
					return "char";
				case KnownTypeCode.SByte:
					return "sbyte";
				case KnownTypeCode.Byte:
					return "byte";
				case KnownTypeCode.Int16:
					return "short";
				case KnownTypeCode.UInt16:
					return "ushort";
				case KnownTypeCode.Int32:
					return "int";
				case KnownTypeCode.UInt32:
					return "uint";
				case KnownTypeCode.Int64:
					return "long";
				case KnownTypeCode.UInt64:
					return "ulong";
				case KnownTypeCode.Single:
					return "float";
				case KnownTypeCode.Double:
					return "double";
				case KnownTypeCode.Decimal:
					return "decimal";
				case KnownTypeCode.String:
					return "string";
				case KnownTypeCode.Void:
					return "void";
				default:
					return null;
			}
		}
	}
}
