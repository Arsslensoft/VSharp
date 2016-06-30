using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using VSC.Base;

namespace VSC.TypeSystem.Resolver
{
	/// <summary>
	/// Represents a unary/binary/ternary operator invocation.
	/// </summary>
	public class OperatorResolveResult : ResolveResult
	{
		readonly ExpressionType operatorType;
		readonly IMethod userDefinedOperatorMethod;
		readonly IList<ResolveResult> operands;
		readonly bool isLiftedOperator;
		
		public OperatorResolveResult(IType resultType, ExpressionType operatorType, params ResolveResult[] operands)
			: base(resultType)
		{
			if (operands == null)
				throw new ArgumentNullException("operands");
			this.operatorType = operatorType;
			this.operands = operands;
		}
		
		public OperatorResolveResult(IType resultType, ExpressionType operatorType, IMethod userDefinedOperatorMethod, bool isLiftedOperator, IList<ResolveResult> operands)
			: base(resultType)
		{
			if (operands == null)
				throw new ArgumentNullException("operands");
			this.operatorType = operatorType;
			this.userDefinedOperatorMethod = userDefinedOperatorMethod;
			this.isLiftedOperator = isLiftedOperator;
			this.operands = operands;
		}
		
		/// <summary>
		/// Gets the operator type.
		/// </summary>
		public ExpressionType OperatorType {
			get { return operatorType; }
		}
		
		/// <summary>
		/// Gets the operands.
		/// </summary>
		public IList<ResolveResult> Operands {
			get { return operands; }
		}
		
		/// <summary>
		/// Gets the user defined operator method.
		/// Returns null if this is a predefined operator.
		/// </summary>
		public IMethod UserDefinedOperatorMethod {
			get { return userDefinedOperatorMethod; }
		}
		
		/// <summary>
		/// Gets whether this is a lifted operator.
		/// </summary>
		public bool IsLiftedOperator {
			get { return isLiftedOperator; }
		}
		
		public override IEnumerable<ResolveResult> GetChildResults()
		{
			return operands;
		}
	}
}
