using System;
using System.Collections.Generic;
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
            if (Result != null && !Result.IsError)
                return this;

            Result = Resolve(rc);
   
            return this;
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
		
        public override ResolveResult Resolve(ResolveContext rc)
        {
           
            if (Result == null || Result.IsError)
            {

                TypeNameExpression target = expr.DoResolve(rc) as TypeNameExpression;               
                ResolveResult targetRR = target.Resolve(rc);
                if (targetRR.IsError)
                    return targetRR;


                IList<IType> typeArgs = typeArgumentsrefs.Resolve(rc.CurrentTypeResolveContext);
                Result = LookForAttribute
                    ? rc.ResolveMemberAccess(targetRR, name + "Attribute", typeArgs, lookupMode)
                    : rc.ResolveMemberAccess(targetRR, name, typeArgs, lookupMode);

                if ((Result == null || Result.IsError) && LookForAttribute)
                    Result = rc.ResolveMemberAccess(targetRR, name, typeArgs, lookupMode);

            }
            if (Result.IsError)
               rc.Report.Error(148, loc, "Type `{0}' does not contain a definition for `{1}' and no extension method `{1}' of type `{0}' could be found.", expr.GetSignatureForError(), GetSignatureForError());           
           
            LookForAttribute = false;
            return Result;
        }
		
        public override IType ResolveType(ResolveContext resolver)
        {
            TypeResolveResult trr = Resolve(resolver) as TypeResolveResult;
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

        public override IConstantValue BuilConstantValue(bool isAttributeConstant)
        {
            string memberName = Name;
            if (LeftExpression is ITypeReference)
            {
                // handle "int.MaxValue"
                return new ConstantMemberReference(
                    LeftExpression as ITypeReference, 
                    memberName,
                   TypeArgumentsReferences);
            }
            Constant v =LeftExpression.BuilConstantValue(isAttributeConstant) as Constant;
            
            if (v == null)
                return null;
            return new ConstantMemberReference(
                v, memberName,
               TypeArgumentsReferences);
        }
    }
}