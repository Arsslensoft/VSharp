// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Text;
using VSC.Context;

namespace VSC.TypeSystem.Interfaces
{
	/// <summary>
	/// Static helper methods for reflection names.
	/// </summary>
	public static class ReflectionHelper
	{
		/// <summary>
		/// A reflection class used to represent <c>null</c>.
		/// </summary>
		public sealed class Null {}
		
		/// <summary>
		/// A reflection class used to represent <c>dynamic</c>.
		/// </summary>
		public sealed class Dynamic {}
		
		/// <summary>
		/// A reflection class used to represent an unbound type argument.
		/// </summary>
		public sealed class UnboundTypeArgument {}
	
		

		
		#region SplitTypeParameterCountFromReflectionName
		/// <summary>
		/// Removes the ` with type parameter count from the reflection name.
		/// </summary>
		/// <remarks>Do not use this method with the full name of inner classes.</remarks>
		public static string SplitTypeParameterCountFromReflectionName(string reflectionName)
		{
			int pos = reflectionName.LastIndexOf('`');
			if (pos < 0) {
				return reflectionName;
			} else {
				return reflectionName.Substring(0, pos);
			}
		}
		
		/// <summary>
		/// Removes the ` with type parameter count from the reflection name.
		/// </summary>
		/// <remarks>Do not use this method with the full name of inner classes.</remarks>
		public static string SplitTypeParameterCountFromReflectionName(string reflectionName, out int typeParameterCount)
		{
			int pos = reflectionName.LastIndexOf('`');
			if (pos < 0) {
				typeParameterCount = 0;
				return reflectionName;
			} else {
				string typeCount = reflectionName.Substring(pos + 1);
				if (int.TryParse(typeCount, out typeParameterCount))
					return reflectionName.Substring(0, pos);
				else
					return reflectionName;
			}
		}
		#endregion
		
		#region TypeCode support
		/// <summary>
		/// Retrieves a built-in type using the specified type code.
		/// </summary>
		public static IType FindType(this ICompilation compilation, TypeCode typeCode)
		{
			return compilation.FindType((KnownTypeCode)typeCode);
		}
		
		/// <summary>
		/// Creates a reference to the specified type.
		/// </summary>
		/// <param name="typeCode">The type to be converted.</param>
		/// <returns>Returns the type reference.</returns>
		public static ITypeReference ToTypeReference(this TypeCode typeCode)
		{
			return KnownTypeReference.Get((KnownTypeCode)typeCode);
		}
		
		/// <summary>
		/// Gets the type code for the specified type, or TypeCode.Empty if none of the other type codes match.
		/// </summary>
		public static TypeCode GetTypeCode(IType type)
		{
			ITypeDefinition def = type as ITypeDefinition;
			if (def != null) {
				KnownTypeCode typeCode = def.KnownTypeCode;
				if (typeCode <= KnownTypeCode.String && typeCode != KnownTypeCode.Void)
					return (TypeCode)typeCode;
				else
					return TypeCode.Empty;
			}
			return TypeCode.Empty;
		}
		#endregion
	
	}
}
