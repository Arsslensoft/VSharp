using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.AST;
using VSC.Base;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.Context
{

    /// <summary>
    /// Allows controlling which nodes are resolved by the resolve visitor.
    /// </summary>
    /// <seealso cref="ResolveVisitor"/>
    public interface IResolveVisitorNavigator
    {
        /// <summary>
        /// Notifies the navigator that an implicit conversion was applied.
        /// </summary>
        /// <param name="expression">The expression that was resolved.</param>
        /// <param name="result">The resolve result of the expression.</param>
        /// <param name="conversion">The conversion applied to the expressed.</param>
        /// <param name="targetType">The target type of the conversion.</param>
        void ProcessConversion(Expression expression, ResolveResult result, Conversion conversion, IType targetType);
    }


   public class ResolveContext
    {
       internal VSharpResolver _resolver;
    //   readonly Dictionary<Expression, ConversionWithTargetType> conversionDict = new Dictionary<Expression, ConversionWithTargetType>();
       

       IResolveVisitorNavigator navigator;


       public VSharpResolver Resolver { get { return _resolver; } }
       public void CreateResolver(ICompilation c)
       {
           _resolver = new VSharpResolver(c);
       }
       public ResolveContext()
       {

       }
       public ResolveContext(ICompilation comp, CompilerContext cctx)
       {
           _resolver = new VSharpResolver(comp);
           Compiler = cctx;
       }
       public CompilerContext Compiler { get; set; }


       public ResolveResult Resolve(Expression expr)
       {
           return null;

       }
    //   public ResolveResult Resolve(BinaryExpression expr)
    //   {
    //       expr._resolved_left =   Resolve(expr._left);
    //       expr._resolved_right = Resolve(expr._right);
    //       ResolveResult rr = _resolver.ResolveBinaryOperator(expr._operator, expr._resolved_left, expr._resolved_right);
    //       ProcessConversionsInBinaryOperatorResult(expr._left, expr._right, rr);
    //       return rr;
    //   }
    //   public bool TypeDefinitionExists(TypeDeclarationName name, bool ispartial)
    //   {
    //       int parameters = 0;
    //       if (name._opt_type_parameter_list._type_parameters != null)
    //           parameters = name._opt_type_parameter_list._type_parameters.Count();

  
    //       return false;
    //   }

    //   #region Types
    //   public static KnownTypeCode GetTypeCodeForPrimitiveType(string keyword)
    //   {
    //       switch (keyword)
    //       {
    //           case "string":
    //               return KnownTypeCode.String;
    //           case "int":
    //               return KnownTypeCode.Int32;
    //           case "uint":
    //               return KnownTypeCode.UInt32;
    //           case "object":
    //               return KnownTypeCode.Object;
    //           case "bool":
    //               return KnownTypeCode.Boolean;
    //           case "sbyte":
    //               return KnownTypeCode.SByte;
    //           case "byte":
    //               return KnownTypeCode.Byte;
    //           case "short":
    //               return KnownTypeCode.Int16;
    //           case "ushort":
    //               return KnownTypeCode.UInt16;
    //           case "long":
    //               return KnownTypeCode.Int64;
    //           case "ulong":
    //               return KnownTypeCode.UInt64;
    //           case "float":
    //               return KnownTypeCode.Single;
    //           case "double":
    //               return KnownTypeCode.Double;
    //           case "decimal":
    //               return KnownTypeCode.Decimal;
    //           case "char":
    //               return KnownTypeCode.Char;
    //           case "void":
    //               return KnownTypeCode.Void;
    //           default:
    //               return KnownTypeCode.None;
    //       }
    //   }
    //   IType ResolveTypeSymbol(VSC.AST.Type type, NameLookupMode lookupMode = NameLookupMode.Type, bool isattribute = false)
    //   {
    //       return ResolveType(type,lookupMode, isattribute).type;
    //   }
    //   List<IType> ResolveTypeArguments(IEnumerable<VSC.AST.Type> typeArguments)
    //   {
    //       List<IType> result = new List<IType>();
    //    if (typeArguments != null)
    //       foreach (VSC.AST.Type typeArgument in typeArguments)
    //           result.Addition(ResolveTypeSymbol(typeArgument));
           
    //       return result;
    //   }
    
   
    //   public ResolveResult ResolveType(SimpleNameExpr type, NameLookupMode lookupMode = NameLookupMode.Type, bool isattribute = false)
    //   {
    //       var typeArguments = ResolveTypeArguments(type._opt_type_argument_list._type_arguments); 
     
    //       if (string.IsNullOrEmpty(type._identifier._Identifier))
    //           return new TypeResolveResult(SpecialTypeSpec.UnboundTypeArgument);


    //       ResolveResult rr = _resolver.LookupSimpleNameOrTypeName(type._identifier._Identifier, typeArguments, lookupMode);
    //       if (isattribute)
    //       {
    //           var withSuffix = _resolver.LookupSimpleNameOrTypeName(type._identifier._Identifier + "Attribute", typeArguments, lookupMode);
    //           if (AttributeTypeReference.PreferAttributeTypeWithSuffix(rr.Type, withSuffix.Type, _resolver.Compilation))
    //               return withSuffix;
    //       }
    //       return rr;
    //   }
    //   public ResolveResult ResolveType(MemberType type, NameLookupMode lookupMode = NameLookupMode.Type, bool isattribute = false)
    //   {
    //       if (type._type_expression_or_array != null)
    //           return ResolveType(type._type_expression_or_array, lookupMode, isattribute);
    //       else return new TypeResolveResult(_resolver.Compilation.FindType(KnownTypeCode.Void));
    //   }
    //   public ResolveResult ResolveType(VSC.AST.Type type, NameLookupMode lookupMode = NameLookupMode.Type, bool isattribute = false)
    //   {
    //       return ResolveType(type._type_expression_or_array, lookupMode, isattribute) ;
    //   }
    //   public ResolveResult ResolveType(BuiltinType type, NameLookupMode lookupMode = NameLookupMode.Type, bool isattribute = false)
    //   {
    //       KnownTypeCode typeCode = GetTypeCodeForPrimitiveType(type._Keyword);
    //       return new TypeResolveResult(_resolver.Compilation.FindType(typeCode));
    //   }
    //   public ResolveResult ResolveType(BuiltinTypeExpression type, NameLookupMode lookupMode = NameLookupMode.Type, bool isattribute = false)
    //  {
    //      TypeResolveResult trr = ResolveType(type._builtin_type, lookupMode, isattribute) as TypeResolveResult;
    //      if (trr != null)
    //      {
    //          if (type._isnullable)
    //              trr.type = NullableType.Create(_resolver.Compilation, trr.type);
    //          else if (type._pointer_stars != null)
    //              foreach (var p in type._pointer_stars)
    //                  trr.type = new PointerTypeSpec(trr.type);
              
    //          return trr;
    //      }
    //      else return new TypeResolveResult(SpecialTypeSpec.UnboundTypeArgument);
    //  }
    //   public ResolveResult ResolveType(TypeExpression type, NameLookupMode lookupMode = NameLookupMode.Type, bool isattribute = false)
    //   {
    //       if (type._builtin_type_expression != null)
    //           return ResolveType(type._builtin_type_expression, lookupMode, isattribute);

    //       TypeResolveResult trr = ResolveType(type._package_or_type_expr, lookupMode, isattribute) as TypeResolveResult;
    //       if (trr != null)
    //       {
    //           if (type._isnullable)
    //               trr.type = NullableType.Create(_resolver.Compilation, trr.type);
    //           else if (type._pointer_stars != null)
    //               foreach (var p in type._pointer_stars)
    //                   trr.type = new PointerTypeSpec(trr.type);

    //           return trr;
    //       }
    //       else return new TypeResolveResult(SpecialTypeSpec.UnboundTypeArgument);
    //   }
    //   public ResolveResult ResolveType(TypeExpressionOrArray type, NameLookupMode lookupMode = NameLookupMode.Type, bool isattribute = false)
    //   {
    //       TypeResolveResult trr = ResolveType(type._type_expression, lookupMode, isattribute) as TypeResolveResult;
    //       if (trr != null)
    //       {
    //           if (type._rank_specifiers != null)
    //               foreach (var p in type._rank_specifiers)
    //                   trr.type = new ArrayType(_resolver.Compilation, trr.type, p._dimension);

    //           return trr;
    //       }
    //       else return new TypeResolveResult(SpecialTypeSpec.UnboundTypeArgument);
    //   }
    //   public ResolveResult ResolveType(PackageOrTypeExpr type, NameLookupMode lookupMode = NameLookupMode.Type, bool isattribute = false)
    //   {
    //       if (type._qualified_alias_member != null)
    //           return ResolveType(type._qualified_alias_member, lookupMode, isattribute);
    //       else if (type._package_or_type_expr != null)
    //       {
    //           var typeArguments = ResolveTypeArguments(type._simple_name_expr._opt_type_argument_list._type_arguments);
    //           ResolveResult trr = ResolveType(type._package_or_type_expr, lookupMode, isattribute);


    //           ResolveResult rr = _resolver.ResolveMemberAccess(trr, type._simple_name_expr._identifier._Identifier, typeArguments, lookupMode);
    //           if (isattribute)
    //           {
    //               var withSuffix = _resolver.ResolveMemberAccess(trr, type._simple_name_expr._identifier._Identifier + "Attribute", typeArguments, lookupMode);
    //               if (AttributeTypeReference.PreferAttributeTypeWithSuffix(rr.Type, withSuffix.Type, _resolver.Compilation))
    //                   return withSuffix;
    //           }
             
    //           return rr;
    //       }
    //       else return ResolveType(type._simple_name_expr, lookupMode, isattribute);
    //   }
    //   public ResolveResult ResolveType(QualifiedAliasMember type, NameLookupMode lookupMode = NameLookupMode.Type, bool isattribute = false)
    //   {
    //      var trr = _resolver.ResolveAlias(type._identifier._Identifier);
    //      var typeArguments = ResolveTypeArguments(type._simple_name_expr._opt_type_argument_list._type_arguments);
      
    //      ResolveResult rr = _resolver.ResolveMemberAccess(trr, type._simple_name_expr._identifier._Identifier, typeArguments, lookupMode);
    //      if (isattribute)
    //      {
    //          var withSuffix = _resolver.ResolveMemberAccess(trr, type._simple_name_expr._identifier._Identifier + "Attribute", typeArguments, lookupMode);
    //          if (AttributeTypeReference.PreferAttributeTypeWithSuffix(rr.Type, withSuffix.Type, _resolver.Compilation))
    //              return withSuffix;
    //      }

    //      return rr;
    //   }
    //   public ResolveResult ResolveType(ExplicitInterface type, NameLookupMode lookupMode = NameLookupMode.Type, bool isattribute = false)
    //   {
    //       var typeArguments = ResolveTypeArguments(type._opt_type_argument_list._type_arguments);
    //       ResolveResult trr = ResolveType(type._explicit_interface, lookupMode, isattribute);


    //       ResolveResult rr = _resolver.ResolveMemberAccess(trr, type._identifier._Identifier, typeArguments, lookupMode);
    //       if (isattribute)
    //       {
    //           var withSuffix = _resolver.ResolveMemberAccess(trr, type._identifier._Identifier + "Attribute", typeArguments, lookupMode);
    //           if (AttributeTypeReference.PreferAttributeTypeWithSuffix(rr.Type, withSuffix.Type, _resolver.Compilation))
    //               return withSuffix;
    //       }

    //       return rr;
    //   }
    //    #endregion

    //   #region Conversion

    //   internal struct ConversionWithTargetType
    //   {
    //       public readonly Conversion Conversion;
    //       public readonly IType TargetType;

    //       public ConversionWithTargetType(Conversion conversion, IType targetType)
    //       {
    //           this.Conversion = conversion;
    //           this.TargetType = targetType;
    //       }
    //   }
     
    //   /// <summary>
    //   /// Convert 'rr' to the target type using the specified conversion.
    //   /// </summary>
    //   void ProcessConversion(Expression expr, ResolveResult rr, VSC.TypeSystem.Conversion conversion, IType targetType)
    //   {
    //       //AnonymousFunctionConversion afc = conversion as AnonymousFunctionConversion;
    //       //if (afc != null)
    //       //{
             
    //       //    if (afc.Hypothesis != null)
    //       //        afc.Hypothesis.MergeInto(this, afc.ReturnType);
    //       //    if (afc.ExplicitlyTypedLambda != null)
    //       //        afc.ExplicitlyTypedLambda.ApplyReturnType(this, afc.ReturnType);
            
    //       //}
    //       if (expr != null &&  conversion != Conversion.IdentityConversion)
    //       {
    //           navigator.ProcessConversion(expr, rr, conversion, targetType);
    //           conversionDict[expr] = new ConversionWithTargetType(conversion, targetType);
    //       }
    //   }



    //   /// <summary>
    //   /// Convert 'rr' to the target type.
    //   /// </summary>
    //   void ProcessConversion(Expression expr, ResolveResult rr, IType targetType)
    //   {
    //       if (expr == null)
    //           return;
    //       ProcessConversion(expr, rr, _resolver.conversions.ImplicitConversion(rr, targetType), targetType);
    //   }
    //    void ProcessConversionResult(Expression expr, ConversionResolveResult rr)
    //   {
    //       if (rr != null && !(rr is CastResolveResult))
    //           ProcessConversion(expr, rr.Input, rr.Conversion, rr.Type);
    //   }

    //   void ProcessConversionResults(IEnumerable<Expression> expr, IEnumerable<ResolveResult> conversionResolveResults)
    //   {
    //       Debug.Assert(expr.Count() == conversionResolveResults.Count());
    //       using (var e1 = expr.GetEnumerator())
    //       {
    //           using (var e2 = conversionResolveResults.GetEnumerator())
    //           {
    //               while (e1.MoveNext() && e2.MoveNext())
    //               {
    //                   ProcessConversionResult(e1.Current, e2.Current as ConversionResolveResult);
    //               }
    //           }
    //       }
    //   }
    //public  ResolveResult ProcessConversionsInBinaryOperatorResult(Expression left, Expression right, ResolveResult rr)
    //   {
    //       OperatorResolveResult orr = rr as OperatorResolveResult;
    //       if (orr != null && orr.Operands.Count == 2)
    //       {
    //           ProcessConversionResult(left, orr.Operands[0] as ConversionResolveResult);
    //           ProcessConversionResult(right, orr.Operands[1] as ConversionResolveResult);
    //       }
    //       else
    //       {
    //           InvocationResolveResult irr = rr as InvocationResolveResult;
    //           if (irr != null && irr.Arguments.Count == 2)
    //           {
    //               ProcessConversionResult(left, irr.Arguments[0] as ConversionResolveResult);
    //               ProcessConversionResult(right, irr.Arguments[1] as ConversionResolveResult);
    //           }
    //       }
    //       return rr;
    //   }
    //   #endregion 
    }
}
