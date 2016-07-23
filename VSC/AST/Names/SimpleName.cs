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
    /// Represents a simple V# name. (a single non-qualified identifier with an optional list of type arguments)
    /// </summary>
    [Serializable]
    public class SimpleName : TypeNameExpression, ISupportsInterning
    {    
        public SimpleName(string name, Location l, NameLookupMode lookupMode = NameLookupMode.Type)
            : base(name, l)
        {
            this.typeArgumentsrefs = EmptyList<ITypeReference>.Instance;
            this.lookupMode = lookupMode;
        }

        public SimpleName(string name, TypeArguments args, Location l, NameLookupMode lookupMode = NameLookupMode.Type)
            : base(name, args, l)
        {
            this.typeArgumentsrefs = args != null ? args.ToTypeReferences(CompilerContext.InternProvider) : EmptyList<ITypeReference>.Instance.ToList();
            this.lookupMode = lookupMode;
        }

        public SimpleName(string name, int arity, Location l, NameLookupMode lookupMode = NameLookupMode.Type)
            : base(name, arity, l)
        {
            this.typeArgumentsrefs = EmptyList<ITypeReference>.Instance;
            this.lookupMode = lookupMode;
        }

    
        public override void AcceptVisitor(IVisitor vis)
        {
            vis.Visit(this);
        }

        // TODO: Move it to MemberCore
        public static string GetMemberType(MemberContainer mc)
        {
            if (mc is PropertyDeclaration)
                return "property";
            if (mc is IndexerDeclaration)
                return "indexer";
            if (mc is EnumMemberDeclaration)
                return "enum";
            if (mc is FieldDeclaration)
                return "field";
            if (mc is MethodCore)
                return "method";
          
            if (mc is EventBase)
                return "event";

            return "type";
        }
        public override Expression DoResolve(ResolveContext rc)
        {
            return Resolve(rc);
        }

        #region ITypeReference
        readonly IList<ITypeReference> typeArgumentsrefs;
    

   
        public IList<ITypeReference> TypeArgumentsReferences
        {
            get { return typeArgumentsrefs; }
        }

        public NameLookupMode LookupMode
        {
            get { return lookupMode; }
        }

        /// <summary>
        /// Adds a suffix to the identifier.
        /// Does not modify the existing type reference, but returns a new one.
        /// </summary>
        public SimpleName AddSuffix(string suffix)
        {
            return new SimpleName(name + suffix, this.targs,Location, lookupMode);
        }
        //TODO:Resolve
        public override Expression Resolve(ResolveContext rc)
        {
            if (_resolved)
                return this;

            var typeArgs = typeArgumentsrefs.Resolve(rc.CurrentTypeResolveContext);
            if (LookForAttribute)
            {
                var wa = LookupSimpleNameOrTypeName(rc, name + "Attribute", typeArgs, lookupMode);
                if (wa == null || wa.IsError)
                {
                     wa  = LookupSimpleNameOrTypeName(rc,name, typeArgs, lookupMode);
                      if (wa == null || wa.IsError)
                          rc.Report.Error(6, loc, "This name `{0}' does not exist in the current context", name);
                }
                        LookForAttribute = false;


                        if (wa is LocalVariableExpression && typeArgumentsrefs.Count > 0)
                            rc.Report.Error(8, loc, "The name `{0}' does not need type arguments because it's a local name", name);
                return wa;

               
             }
            else
            {   
                var wa  = LookupSimpleNameOrTypeName(rc,name, typeArgs, lookupMode);
                      if (wa == null || wa.IsError)
                          rc.Report.Error(6, loc, "This name `{0}' does not exist in the current context", name);

                      if (wa is LocalVariableExpression && typeArgumentsrefs.Count > 0)
                          rc.Report.Error(8, loc, "The name `{0}' does not need type arguments because it's a local name", name);
                    return wa;
            }
          
            return this;
        }

        public override IType ResolveType(ResolveContext resolver)
        {
            TypeExpression trr = Resolve(resolver) as TypeExpression;
            return trr != null ? trr.Type : new UnknownTypeSpec(null, name,Arity);
        }

        public override string ToString()
        {
            if (typeArgumentsrefs.Count == 0)
                return name;
            else
                return name + "<" + string.Join(",", typeArgumentsrefs) + ">";
        }

        int ISupportsInterning.GetHashCodeForInterning()
        {
            int hashCode = 0;
            unchecked
            {
                hashCode += 1000000021 * name.GetHashCode();
                hashCode += 1000000033 * typeArgumentsrefs.GetHashCode();
                hashCode += 1000000087 * (int)lookupMode;
            }
            return hashCode;
        }

        bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
        {
            SimpleName o = other as SimpleName;
            return o != null && this.name == o.Name
                   && this.typeArgumentsrefs == o.typeArgumentsrefs && this.lookupMode == o.lookupMode;
        }
        #endregion

        public Expression LookupSimpleNameOrTypeName(ResolveContext rc, string identifier, IList<IType> typeArguments, NameLookupMode lookupMode)
        {
            // V# 4.0 spec: §3.8 Namespace and type names; §7.6.2 Simple Names

            if (identifier == null)
                throw new ArgumentNullException("identifier");
            if (typeArguments == null)
                throw new ArgumentNullException("typeArguments");

            int k = typeArguments.Count;

            if (k == 0)
            {
                if (lookupMode == NameLookupMode.Expression || lookupMode == NameLookupMode.InvocationTarget)
                {
                    // Look in local variables
                    foreach (IVariable v in rc.LocalVariables)
                    {
                        if (v.Name == identifier)
                            return new LocalVariableExpression(v, Location);
             
                    }
                    // Look in parameters of current method
                    IParameterizedMember parameterizedMember = rc.CurrentMember as IParameterizedMember;
                    if (parameterizedMember != null)
                    {
                        foreach (IParameter p in parameterizedMember.Parameters)
                        {
                            if (p.Name == identifier)
                                return new LocalVariableExpression(p, Location);
                      
                        }
                    }
                }

                // look in type parameters of current method
                IMethod m = rc.CurrentMember as IMethod;
                if (m != null)
                {
                    foreach (ITypeParameter tp in m.TypeParameters)
                    {
                        if (tp.Name == identifier)
                            return new TypeExpression(tp,Location);
                    }
                }
            }

            bool parameterizeResultType = !(typeArguments.Count != 0 && typeArguments.All(t => t.Kind == TypeKind.UnboundTypeArgument));

            Expression r = null;
            if (rc.currentTypeDefinitionCache != null)
            {
                Dictionary<string, Expression> cache = null;
                bool foundInCache = false;
                if (k == 0)
                {
                    switch (lookupMode)
                    {
                        case NameLookupMode.Expression:
                            cache = rc.currentTypeDefinitionCache.SimpleNameLookupCacheExpression;
                            break;
                        case NameLookupMode.InvocationTarget:
                            cache = rc.currentTypeDefinitionCache.SimpleNameLookupCacheInvocationTarget;
                            break;
                        case NameLookupMode.Type:
                            cache = rc.currentTypeDefinitionCache.SimpleTypeLookupCache;
                            break;
                    }
                    if (cache != null)
                    {
                        lock (cache)
                            foundInCache = cache.TryGetValue(identifier, out r);
                    }
                }
                if (foundInCache)
                {
                    r = (r != null ? r.ShallowClone() : null);
                }
                else
                {
                    r = LookInCurrentType(rc, identifier, typeArguments, lookupMode, parameterizeResultType);
                    if (cache != null)
                    {
                        // also cache missing members (r==null)
                        lock (cache)
                            cache[identifier] = r;
                    }
                }
                if (r != null)
                    return r;
            }

            if (rc.context.CurrentUsingScope == null)
            {
                // If no using scope was specified, we still need to look in the global namespace:
                r = LookInUsingScopeNamespace(rc,null, rc.compilation.RootNamespace, identifier, typeArguments, parameterizeResultType);
            }
            else
            {
                if (k == 0 && lookupMode != NameLookupMode.TypeInUsingDeclaration)
                {
                    if (rc.context.CurrentUsingScope.ResolveCache.TryGetValue(identifier, out r))
                        r = (r != null ? r.ShallowClone() : null);
                    else
                    {
                        r = LookInCurrentUsingScope(rc, identifier, typeArguments, false, false);
                        rc.context.CurrentUsingScope.ResolveCache.TryAdd(identifier, r);
                    }
                }
                else
                    r = LookInCurrentUsingScope(rc,identifier, typeArguments, lookupMode == NameLookupMode.TypeInUsingDeclaration, parameterizeResultType);
                
            }
            if (r != null)
                return r;

            if (typeArguments.Count == 0 && identifier == "dynamic")
                return new TypeExpression(SpecialTypeSpec.Dynamic as IType, Location);
            else
                return ErrorResult;
         
          
        }
        Expression LookInCurrentType(ResolveContext rc,string identifier, IList<IType> typeArguments, NameLookupMode lookupMode, bool parameterizeResultType)
        {
            int k = typeArguments.Count;
            MemberLookup lookup = rc.CreateMemberLookup(lookupMode);
            // look in current type definitions
            for (ITypeDefinition t = rc.CurrentTypeDefinition; t != null; t = t.DeclaringTypeDefinition)
            {
                if (k == 0)
                {
                    // Look for type parameter with that name
                    var typeParameters = t.TypeParameters;
                    // Look at all type parameters, including those copied from outer classes,
                    // so that we can fetch the version with the correct owner.
                    for (int i = 0; i < typeParameters.Count; i++)
                    {
                        if (typeParameters[i].Name == identifier)
                            return new TypeExpression(typeParameters[i],  Location);
                    }
                }

                if (lookupMode == NameLookupMode.BaseTypeReference && t == rc.CurrentTypeDefinition)
                {
                    // don't look in current type when resolving a base type reference
                    continue;
                }

                Expression r;
                if (lookupMode == NameLookupMode.Expression || lookupMode == NameLookupMode.InvocationTarget)
                {
                    Expression targetResolveResult = (t == rc.CurrentTypeDefinition ? (Expression)new SelfReference(t,Location) : new TypeExpression(t,Location));
                    r = lookup.Lookup(targetResolveResult, identifier, typeArguments, lookupMode == NameLookupMode.InvocationTarget);
                }
                else
                    r = lookup.LookupType(t, identifier, typeArguments, parameterizeResultType);

                if (!(r is UnknownMemberExpression)) // but do return AmbiguousMemberResolveResult
                    return r;
            }
            return null;
        }
        Expression LookInCurrentUsingScope(ResolveContext rc,string identifier, IList<IType> typeArguments, bool isInUsingDeclaration, bool parameterizeResultType)
        {
            // look in current namespace definitions
            ResolvedUsingScope currentUsingScope = rc.CurrentUsingScope;
            for (ResolvedUsingScope u = currentUsingScope; u != null; u = u.Parent)
            {
                var resultInNamespace = LookInUsingScopeNamespace(rc,u, u.Namespace, identifier, typeArguments, parameterizeResultType);
                if (resultInNamespace != null)
                    return resultInNamespace;
                // then look for aliases:
                if (typeArguments.Count == 0)
                {
                    if (u.ExternAliases.Contains(identifier))
                        return ResolveExternAlias(rc,identifier);
                    if (!(isInUsingDeclaration && u == currentUsingScope))
                    {
                        foreach (var pair in u.UsingAliases)
                        {
                            if (pair.Key == identifier)
                                return pair.Value.ShallowClone();
                            
                        }
                    }
                }
                // finally, look in the imported namespaces:
                if (!(isInUsingDeclaration && u == currentUsingScope))
                {
                    IType firstResult = null;
                    foreach (var importedNamespace in u.Usings)
                    {
                        ITypeDefinition def = importedNamespace.GetTypeDefinition(identifier, typeArguments.Count);
                        if (def != null)
                        {
                            IType resultType;
                            if (parameterizeResultType && typeArguments.Count > 0)
                                resultType = new ParameterizedTypeSpec(def, typeArguments);
                            else
                                resultType = def;

                            if (firstResult == null || !TopLevelTypeDefinitionIsAccessible(rc,firstResult.GetDefinition()))
                            {
                                if (TopLevelTypeDefinitionIsAccessible(rc, resultType.GetDefinition()))
                                    firstResult = resultType;
                            }
                            else if (TopLevelTypeDefinitionIsAccessible(rc, def))
                            {
                                rc.Report.Error(7, loc, "The name `{0}' is ambigious", name);
                                return new TypeExpression(firstResult, Location); // ambigious
                            }
                        }
                    }
                    if (firstResult != null)
                        return new TypeExpression(firstResult, Location);
                }
                // if we didn't find anything: repeat lookup with parent namespace
            }
            return null;
        }
        bool TopLevelTypeDefinitionIsAccessible(ResolveContext rc,ITypeDefinition typeDef)
        {
            if (typeDef.IsInternal)
                return typeDef.ParentAssembly.InternalsVisibleTo(rc.compilation.MainAssembly);
        
            return true;
        }
        Expression LookInUsingScopeNamespace(ResolveContext rc, ResolvedUsingScope usingScope, INamespace n, string identifier, IList<IType> typeArguments, bool parameterizeResultType)
        {
            if (n == null)
                return null;
            // first look for a namespace
            int k = typeArguments.Count;
            if (k == 0)
            {
                INamespace childNamespace = n.GetChildNamespace(identifier);
                if (childNamespace != null)
                {
                    if (usingScope != null && usingScope.HasAlias(identifier))
                    {
                        rc.Report.Error(7, loc, "The name `{0}' is ambigious", name);
                        return new TypeExpression((IType) new UnknownTypeSpec(null, identifier), Location); // ambigious
                    }

                    return new AliasNamespace(childNamespace,Location);
                }
            }
            // then look for a type
            ITypeDefinition def = n.GetTypeDefinition(identifier, k);
            if (def != null)
            {
                IType result = def;
                if (parameterizeResultType && k > 0)
                    result = new ParameterizedTypeSpec(def, typeArguments);

                if (usingScope != null && usingScope.HasAlias(identifier))
                {
                    rc.Report.Error(7, loc, "The name `{0}' is ambigious", name);
                    return new TypeExpression((IType) new UnknownTypeSpec(null, identifier), Location); // ambigious
                }
                else
                    return new TypeExpression(result, Location);
            }
            return null;
        }
        /// <summary>
        /// Looks up an alias (identifier in front of :: operator)
        /// </summary>
        public Expression ResolveAlias(ResolveContext rc,string identifier)
        {
            if (identifier == "global")
                return new AliasNamespace(rc.compilation.RootNamespace, Location);

            for (ResolvedUsingScope n = rc.CurrentUsingScope; n != null; n = n.Parent)
            {
                if (n.ExternAliases.Contains(identifier))
                    return ResolveExternAlias(rc,identifier);
                foreach (var pair in n.UsingAliases)
                {
                    if (pair.Key == identifier)
                    {
                        return (Expression)(pair.Value as AliasNamespace) ?? ErrorResult;
                    }
                }
            }
            return ErrorResult;
        }
        Expression ResolveExternAlias(ResolveContext rc, string alias)
        {
            INamespace ns = rc.compilation.GetNamespaceForExternAlias(alias);
            if (ns != null)
                return new AliasNamespace(ns,loc);
            else
                return ErrorResult;
        }
        
        
        //public override IConstantValue BuilConstantValue(bool isAttributeConstant)
        //{
        //    return new ConstantIdentifierReference(Name, TypeArgumentsReferences);
        //}
    }
}