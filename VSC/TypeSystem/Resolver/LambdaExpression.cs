using System;
using System.Collections.Generic;
using VSC.AST;
using VSC.Base;

namespace VSC.TypeSystem.Resolver
{
	/// <summary>
	/// Represents an anonymous method or lambda expression.
	/// Note: the lambda has no type.
	/// To retrieve the delegate type, look at the anonymous function conversion.
	/// </summary>
	public abstract class LambdaExpression : Expression
	{
		protected LambdaExpression() : base(SpecialTypeSpec.UnknownType)
		{
		}
		
		/// <summary>
		/// Gets whether there is a parameter list.
		/// This property always returns true for V# 3.0-lambdas, but may return false
		/// for V# 2.0 anonymous methods.
		/// </summary>
		public abstract bool HasParameterList { get; }
		
		/// <summary>
		/// Gets whether this lambda is using the V# 2.0 anonymous method syntax.
		/// </summary>
		public abstract bool IsAnonymousMethod { get; }
		
		/// <summary>
		/// Gets whether the lambda parameters are implicitly typed.
		/// </summary>
		/// <remarks>This property returns false for anonymous methods without parameter list.</remarks>
		public abstract bool IsImplicitlyTyped { get; }
		
		/// <summary>
		/// Gets whether the lambda is async.
		/// </summary>
		public abstract bool IsAsync { get; }
		
		/// <summary>
		/// Gets the return type inferred when the parameter types are inferred to be <paramref name="parameterTypes"/>
		/// </summary>
		/// <remarks>
		/// This method determines the return type inferred from the lambda body, which is used as part of V# type inference.
		/// Use the <see cref="ReturnType"/> property to retrieve the actual return type as determined by the target delegate type.
		/// </remarks>
		public abstract IType GetInferredReturnType(IType[] parameterTypes);
		
		/// <summary>
		/// Gets the list of parameters.
		/// </summary>
		public abstract IList<IParameter> Parameters { get; }
		
		/// <summary>
		/// Gets the return type of the lambda.
		/// 
		/// If the lambda is async, the return type includes <code>Task&lt;T&gt;</code>
		/// </summary>
		public abstract IType ReturnType { get; }
		
		/// <summary>
		/// Gets whether the lambda body is valid for the given parameter types and return type.
		/// </summary>
		/// <returns>
		/// Produces a conversion with <see cref="Conversion.IsAnonymousFunctionConversion"/>=<c>true</c> if the lambda is valid;
		/// otherwise returns <see cref="Conversion.None"/>.
		/// </returns>
		public abstract Conversion IsValid(IType[] parameterTypes, IType returnType, VSharpConversions conversions);
		
		/// <summary>
		/// Gets the resolve result for the lambda body.
		/// Returns a resolve result for 'void' for statement lambdas.
		/// </summary>
		public abstract Expression Body { get; }
		
	
	}
}
