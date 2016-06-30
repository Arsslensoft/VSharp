using System;

namespace VSC.TypeSystem.Implementation
{
	/// <summary>
	/// References an existing assembly by name.
	/// </summary>
	[Serializable]
	public sealed class AssemblyReferenceSpec : IAssemblyReference, ISupportsInterning
	{
		public static readonly IAssemblyReference CurrentAssembly = new CurrentAssemblyReference();
		
		[Obsolete("The corlib is not always called 'mscorlib' (as returned by this property), but might be 'System.Runtime'.")]
		public static readonly IAssemblyReference Corlib = new AssemblyReferenceSpec("mscorlib");
		
		readonly string shortName;
		
		public AssemblyReferenceSpec(string assemblyName)
		{
			int pos = assemblyName != null ? assemblyName.IndexOf(',') : -1;
			if (pos >= 0)
				shortName = assemblyName.Substring(0, pos);
			else
				shortName = assemblyName;
		}
		
		public IAssembly Resolve(ITypeResolveContext context)
		{
			IAssembly current = context.CurrentAssembly;
			if (current != null && string.Equals(shortName, current.AssemblyName, StringComparison.OrdinalIgnoreCase))
				return current;
			foreach (IAssembly asm in context.Compilation.Assemblies) {
				if (string.Equals(shortName, asm.AssemblyName, StringComparison.OrdinalIgnoreCase))
					return asm;
			}
			return null;
		}
		
		public override string ToString()
		{
			return shortName;
		}
		
		int ISupportsInterning.GetHashCodeForInterning()
		{
			unchecked {
				return shortName.GetHashCode();
			}
		}
		
		bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
		{
			AssemblyReferenceSpec o = other as AssemblyReferenceSpec;
			return o != null && shortName == o.shortName;
		}
		
		[Serializable]
		sealed class CurrentAssemblyReference : IAssemblyReference
		{
			public IAssembly Resolve(ITypeResolveContext context)
			{
				IAssembly asm = context.CurrentAssembly;
				if (asm == null)
					throw new ArgumentException("A reference to the current assembly cannot be resolved in the compilation's global type resolve context.");
				return asm;
			}
		}
	}
}
