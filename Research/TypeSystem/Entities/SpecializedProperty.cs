using System;

namespace VSC.TypeSystem.Implementation
{
	/// <summary>
	/// Represents a specialized IProperty (property after type substitution).
	/// </summary>
	public class SpecializedProperty : SpecializedParameterizedMember, IProperty
	{
		readonly IProperty propertyDefinition;
		
		public SpecializedProperty(IProperty propertyDefinition, TypeParameterSubstitution substitution)
			: base(propertyDefinition)
		{
			this.propertyDefinition = propertyDefinition;
			AddSubstitution(substitution);
		}
		
		public bool CanGet {
			get { return propertyDefinition.CanGet; }
		}
		
		public bool CanSet {
			get { return propertyDefinition.CanSet; }
		}
		
		IMethod getter, setter;
		
		public IMethod Getter {
			get { return WrapAccessor(ref this.getter, propertyDefinition.Getter); }
		}
		
		public IMethod Setter {
			get { return WrapAccessor(ref this.setter, propertyDefinition.Setter); }
		}
		
		public bool IsIndexer {
			get { return propertyDefinition.IsIndexer; }
		}
	}
}
