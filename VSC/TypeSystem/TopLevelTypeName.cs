using System;
using System.Text;

namespace VSC.TypeSystem
{
	/// <summary>
	/// Holds the name of a top-level type.
	/// This struct cannot refer to nested classes.
	/// </summary>
	[Serializable]
	public struct TopLevelTypeName : IEquatable<TopLevelTypeName>
	{
		readonly string namespaceName;
		readonly string name;
		readonly int typeParameterCount;
		
		public TopLevelTypeName(string namespaceName, string name, int typeParameterCount = 0)
		{
			if (namespaceName == null)
				throw new ArgumentNullException("namespaceName");
			if (name == null)
				throw new ArgumentNullException("name");
			this.namespaceName = namespaceName;
			this.name = name;
			this.typeParameterCount = typeParameterCount;
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
		
		public string ReflectionName {
			get {
				StringBuilder b = new StringBuilder();
				if (!string.IsNullOrEmpty(namespaceName)) {
					b.Append(namespaceName);
					b.Append('.');
				}
				b.Append(name);
				if (typeParameterCount > 0) {
					b.Append('`');
					b.Append(typeParameterCount);
				}
				return b.ToString();
			}
		}
		
		public override string ToString()
		{
			return this.ReflectionName;
		}
		
		public override bool Equals(object obj)
		{
			return (obj is TopLevelTypeName) && Equals((TopLevelTypeName)obj);
		}
		
		public bool Equals(TopLevelTypeName other)
		{
			return this.namespaceName == other.namespaceName && this.name == other.name && this.typeParameterCount == other.typeParameterCount;
		}
		
		public override int GetHashCode()
		{
			return (name != null ? name.GetHashCode() : 0) ^ (namespaceName != null ? namespaceName.GetHashCode() : 0) ^ typeParameterCount;
		}
		
		public static bool operator ==(TopLevelTypeName lhs, TopLevelTypeName rhs)
		{
			return lhs.Equals(rhs);
		}
		
		public static bool operator !=(TopLevelTypeName lhs, TopLevelTypeName rhs)
		{
			return !lhs.Equals(rhs);
		}
	}
}
