using System;
using VSC.Base;

namespace VSC.TypeSystem.Resolver
{
	/// <summary>
	/// Represents an explicitly applied conversion (CastExpression or AsExpression)
	/// (a result belonging to an AST node; not implicitly inserted 'between' nodes).
	/// </summary>
	class CastResolveResult : ConversionResolveResult
	{
		// The reason this class exists is that for code like this:
		//    int i = ...;
		//    long n = 0;
		//    n = n + (long)i;
		// The resolver will produce (and process) an CastResolveResult for the cast,
		// (with Conversion = implicit numeric conversion)
		// and then pass it into ResolveContext.ResolveBinaryOperator().
		// That method normally wraps input arguments into another conversion
		// (the implicit conversion applied by the operator).
		// However, identity conversions do not cause the creation of ConversionResolveResult instances,
		// so the OperatorResolveResult's argument will be the CastResolveResult
		// of the cast.
		// Without this class (and instead using ConversionResolveResult for both purposes),
		// it would be hard for the conversion-processing code
		// in the ResolveVisitor to distinguish the existing conversion from the CastExpression
		// from an implicit conversion introduced by the binary operator.
		// This would cause the conversion to be processed yet again.
		// The following unit tests would fail without this class:
		//  * CastTests.ExplicitConversion_In_Assignment
		//  * FindReferencesTest.FindReferencesForOpImplicitInAssignment_ExplicitCast
		//  * CS0029InvalidConversionIssueTests.ExplicitConversionFromUnknownType
		
		public CastResolveResult(ConversionResolveResult rr)
			: base(rr.Type, rr.Input, rr.Conversion, rr.CheckForOverflow)
		{
		}
		
		public CastResolveResult(IType targetType, ResolveResult input, Conversion conversion, bool checkForOverflow)
			: base(targetType, input, conversion, checkForOverflow)
		{
		}
	}
}
