using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using VSC.Base;
using Expression = VSC.AST.Expression;

namespace VSC.TypeSystem.Resolver
{
	/// <summary>
	/// Represents a unary/binary/ternary operator invocation.
	/// </summary>
	public class OperatorExpression : Expression
	{
		readonly ExpressionType operatorType;
		readonly IMethod userDefinedOperatorMethod;
		readonly IList<Expression> operands;
		readonly bool isLiftedOperator;
		
		public OperatorExpression(IType resultType, ExpressionType operatorType, params Expression[] operands)
			: base(resultType)
		{
			if (operands == null)
				throw new ArgumentNullException("operands");
			this.operatorType = operatorType;
			this.operands = operands;
		}
		
		public OperatorExpression(IType resultType, ExpressionType operatorType, IMethod userDefinedOperatorMethod, bool isLiftedOperator, IList<Expression> operands)
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
		public IList<Expression> Operands {
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
		
		
	}
}
