using System;
using System.Diagnostics.Contracts;

namespace VSC.TypeSystem
{
	public interface INamedElement
	{
		/// <summary>
		/// Gets the fully qualified name of the class the return type is pointing to.
		/// </summary>
		/// <returns>
		/// "System.Int32[]" for int[]<br/>
		/// "System.Collections.Generic.List" for List&lt;string&gt;
		/// "System.Environment.SpecialFolder" for Environment.SpecialFolder
		/// </returns>
		string FullName { get; }
		
		/// <summary>
		/// Gets the short name of the class the return type is pointing to.
		/// </summary>
		/// <returns>
		/// "Int32[]" for int[]<br/>
		/// "List" for List&lt;string&gt;
		/// "SpecialFolder" for Environment.SpecialFolder
		/// </returns>
		string Name { get; }
		
		/// <summary>
		/// Gets the full reflection name of the element.
		/// </summary>
		/// <remarks>
		/// For types, the reflection name can be parsed back into a ITypeReference by using
		/// <see cref="ReflectionHelper.ParseReflectionName(string)"/>.
		/// </remarks>
		/// <returns>
		/// "System.Int32[]" for int[]<br/>
		/// "System.Int32[][,]" for V# int[,][]<br/>
		/// "System.Collections.Generic.List`1[[System.String]]" for List&lt;string&gt;
		/// "System.Environment+SpecialFolder" for Environment.SpecialFolder
		/// </returns>
		string ReflectionName { get; }
		
		/// <summary>
		/// Gets the full name of the namespace containing this entity.
		/// </summary>
		string Namespace { get; }
	}
}
