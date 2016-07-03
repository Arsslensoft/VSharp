using System;

namespace VSC.TypeSystem.Implementation
{
	/// <summary>
	/// Represents a specialized IField (field after type substitution).
	/// </summary>
	public class SpecializedField : SpecializedMember, IField
	{
		readonly IField fieldDefinition;
		
		public SpecializedField(IField fieldDefinition, TypeParameterSubstitution substitution)
			: base(fieldDefinition)
		{
			this.fieldDefinition = fieldDefinition;
			AddSubstitution(substitution);
		}
		
		public bool IsReadOnly {
			get { return fieldDefinition.IsReadOnly; }
		}
		
		public bool IsVolatile {
			get { return fieldDefinition.IsVolatile; }
		}
		
		IType IVariable.Type {
			get { return this.ReturnType; }
		}
		
		public bool IsConst {
			get { return fieldDefinition.IsConst; }
		}

		public bool IsFixed {
			get { return fieldDefinition.IsFixed; }
		}

		public object ConstantValue {
			get { return fieldDefinition.ConstantValue; }
		}
	}
}
