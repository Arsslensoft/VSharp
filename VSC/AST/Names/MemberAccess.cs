using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using VSC.Base;
using VSC.Context;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    /// <summary>
    ///   Implements the member access expression
    /// </summary>
    [Serializable]
    public class MemberAccess : TypeNameExpression, ISupportsInterning
    {
        protected Expression expr;

        public MemberAccess(Expression expr, string id, NameLookupMode lookupMode = NameLookupMode.Type)
            : base(id, expr.Location)
        {
            this.expr = expr;
            this.typeArgumentsrefs = EmptyList<ITypeReference>.Instance;
            this.lookupMode = lookupMode;
        }
        public MemberAccess(Expression expr, string identifier, Location loc, NameLookupMode lookupMode = NameLookupMode.Type)
            : base(identifier, loc)
        {
            this.expr = expr;
            this.typeArgumentsrefs = EmptyList<ITypeReference>.Instance;
            this.lookupMode = lookupMode;
        }
        public MemberAccess(Expression expr, string identifier, TypeArguments args, Location loc, NameLookupMode lookupMode = NameLookupMode.Type)
            : base(identifier, args, loc)
        {
            this.expr = expr;
            this.typeArgumentsrefs = targs != null ? targs.ToTypeReferences(CompilerContext.InternProvider) : EmptyList<ITypeReference>.Instance.ToList();
            this.lookupMode = lookupMode;
        }
        public MemberAccess(Expression expr, string identifier, int arity, Location loc, NameLookupMode lookupMode = NameLookupMode.Type)
            : base(identifier, arity, loc)
        {
            this.expr = expr;
            this.typeArgumentsrefs =  EmptyList<ITypeReference>.Instance;
            this.lookupMode = lookupMode;
        }

        public Expression LeftExpression
        {
            get
            {
                return expr;
            }
        }

        public override Expression DoResolve(ResolveContext rc)
        {
      
           return Resolve(rc);

        }

        public override string GetSignatureForError()
        {
            return expr.GetSignatureForError() + "." + base.GetSignatureForError();
        }

        public override void AcceptVisitor(IVisitor vis)
        {
            vis.Visit(this);
        }

        #region ITypeReference
        protected readonly IList<ITypeReference> typeArgumentsrefs;
	
	
			
        public IList<ITypeReference> TypeArgumentsReferences {
            get { return typeArgumentsrefs; }
        }
		
        public NameLookupMode LookupMode {
            get { return lookupMode; }
        }
		
        /// <summary>
        /// Adds a suffix to the identifier.
        /// Does not modify the existing type reference, but returns a new one.
        /// </summary>
        public MemberAccess AddSuffix(string suffix)
        {
            return new MemberAccess(expr, name + suffix, targs, Location, lookupMode);
        }

        public override Expression Resolve(ResolveContext rc)
        {
            if (_resolved)
                return this;
            else
            {
                TypeNameExpression target = expr.DoResolve(rc) as TypeNameExpression;
                Expression targetRR = target.Resolve(rc);
                IList<IType> typeArgs = typeArgumentsrefs.Resolve(rc.CurrentTypeResolveContext);
                if (LookForAttribute)
                {
                    var wa = ResolveMemberAccess(rc, targetRR, name + "Attribute", typeArgs, lookupMode);
                    if (wa == null || wa.IsError)
                    {
                        wa = ResolveMemberAccess(rc, targetRR, name, typeArgs, lookupMode);
                        if (wa == null || wa.IsError)
                            rc.Report.Error(148, loc, "Type `{0}' does not contain a definition for `{1}' and no extension method `{1}' of type `{0}' could be found.", expr.GetSignatureForError(), GetSignatureForError());
                    }
                    LookForAttribute = false;
                    return wa;
                }
                else
                {
                    var wa = ResolveMemberAccess(rc, targetRR, name, typeArgs, lookupMode);
                    if (wa == null || wa.IsError)
                        rc.Report.Error(148, loc, "Type `{0}' does not contain a definition for `{1}' and no extension method `{1}' of type `{0}' could be found.", expr.GetSignatureForError(), GetSignatureForError());

                    return wa;
                }
            }
        }
		
        public override IType ResolveType(ResolveContext resolver)
        {
            Expression trr = Resolve(resolver) as Expression;
            return trr != null ? trr.Type : new UnknownTypeSpec(null, name, typeArgumentsrefs.Count);
        }
		
        public override string ToString()
        {
            if (typeArgumentsrefs.Count == 0)
                return expr.ToString() + "." + name;
            else
                return expr.ToString() + "." + name + "<" + string.Join(",", typeArgumentsrefs) + ">";
        }
		
        int ISupportsInterning.GetHashCodeForInterning()
        {
            int hashCode = 0;
            unchecked {
                hashCode += 1000000007 * expr.GetHashCode();
                hashCode += 1000000033 * name.GetHashCode();
                hashCode += 1000000087 * typeArgumentsrefs.GetHashCode();
                hashCode += 1000000021 * (int)lookupMode;
            }
            return hashCode;
        }
		
        bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
        {
            MemberAccess o = other as MemberAccess;
            return o != null && this.expr == o.expr
                   && this.name == o.name && this.typeArgumentsrefs == o.typeArgumentsrefs
                   && this.lookupMode == o.lookupMode;
        }

        #endregion
        public Expression ResolveMemberAccess(ResolveContext rc,VSC.AST.Expression target, string identifier, IList<IType> typeArguments, NameLookupMode lookupMode = NameLookupMode.Expression)
        {
            // V# 4.0 spec: §7.6.4

            bool parameterizeResultType = !(typeArguments.Count != 0 && typeArguments.All(t => t.Kind == TypeKind.UnboundTypeArgument));
            AliasNamespace nrr = target as AliasNamespace;
            if (nrr != null)
                return ResolveMemberAccessOnNamespace(nrr, identifier, typeArguments, parameterizeResultType);
            
            // TODO:Dynamic Resolution
            //if (target.Type.Kind == TypeKind.Dynamic)
            //    return new DynamicMemberResolveResult(target, identifier);

            MemberLookup lookup = rc.CreateMemberLookup(lookupMode);
            Expression result;
            switch (lookupMode)
            {
                case NameLookupMode.Expression:
                    result = lookup.Lookup(target, identifier, typeArguments, isInvocation: false);
                    break;
                case NameLookupMode.InvocationTarget:
                    result = lookup.Lookup(target, identifier, typeArguments, isInvocation: true);
                    break;
                case NameLookupMode.Type:
                case NameLookupMode.TypeInUsingDeclaration:
                case NameLookupMode.BaseTypeReference:
                    // Don't do the UnknownMemberResolveResult/MethodGroupResolveResult processing,
                    // it's only relevant for expressions.
                    return lookup.LookupType(target.Type, identifier, typeArguments, parameterizeResultType);
                default:
                    throw new NotSupportedException("Invalid value for NameLookupMode");
            }
            if (result is UnknownMemberExpression)
            {
                // We intentionally use all extension methods here, not just the eligible ones.
                // Proper eligibility checking is only possible for the full invocation
                // (after we know the remaining arguments).
                // The eligibility check in GetExtensionMethods is only intended for code completion.
                var extensionMethods = rc.GetExtensionMethods(identifier, typeArguments);
                if (extensionMethods.Count > 0)
                {
                    return new MethodGroupExpression(target, identifier, EmptyList<MethodListWithDeclaringType>.Instance, typeArguments)
                    {
                        extensionMethods = extensionMethods
                    };
                }
            }
            else
            {
                MethodGroupExpression mgrr = result as MethodGroupExpression;
                if (mgrr != null)
                {
                    Debug.Assert(mgrr.extensionMethods == null);
                    // set the values that are necessary to make MethodGroupResolveResult.GetExtensionMethods() work
                    mgrr.resolver = rc;
                }
            }
            return result;
        }
        Expression ResolveMemberAccessOnNamespace(AliasNamespace nrr, string identifier, IList<IType> typeArguments, bool parameterizeResultType)
        {
            if (typeArguments.Count == 0)
            {
                INamespace childNamespace = nrr.Namespace.GetChildNamespace(identifier);
                if (childNamespace != null)
                    return new AliasNamespace(childNamespace, Location);
            }
            ITypeDefinition def = nrr.Namespace.GetTypeDefinition(identifier, typeArguments.Count);
            if (def != null)
            {
                if (parameterizeResultType && typeArguments.Count > 0)
                    return new TypeExpression(new ParameterizedTypeSpec(def, typeArguments),loc);
                else
                    return new TypeExpression(def, loc);
            }
            return ErrorResult;
        }
        //public override IConstantValue BuilConstantValue(bool isAttributeConstant)
        //{
        //    string memberName = Name;
        //    if (LeftExpression is ITypeReference)
        //    {
        //        // handle "int.MaxValue"
        //        return new ConstantMemberReference(
        //            LeftExpression as ITypeReference, 
        //            memberName,
        //           TypeArgumentsReferences);
        //    }
        //    Constant v =LeftExpression.BuilConstantValue(isAttributeConstant) as Constant;
            
        //    if (v == null)
        //        return null;
        //    return new ConstantMemberReference(
        //        v, memberName,
        //       TypeArgumentsReferences);
        //}
    }
}