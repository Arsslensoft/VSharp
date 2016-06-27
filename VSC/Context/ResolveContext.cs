
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.AST;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.Context
{


    public class BlockContext : ResolveContext
    {

    }
    public class SymbolResolveContext
    {
        public bool ResolveOne(params IResolve[] resolvable)
        {
            foreach (IResolve r in resolvable)
                if (r != null)
                    return r.Resolve(this);


            return false;
        }
        public CompilerContext Compiler { get; set; } 
        internal UsingScope usingScope;
        VSharpUnresolvedTypeDefinition currentTypeDefinition;
        UnresolvedMethodSpec currentMethod;

        InterningProvider interningProvider = new SimpleInterningProvider();
        internal VSharpUnresolvedFile unresolvedFile;
        public VSharpUnresolvedFile UnresolvedFile
        {
            get { return unresolvedFile; }
        }
        /// <summary>
        /// Gets/Sets the interning provider to use.
        /// The default value is a new <see cref="SimpleInterningProvider"/> instance.
        /// </summary>
        public InterningProvider InterningProvider
        {
            get { return interningProvider; }
            set
            {
                if (interningProvider == null)
                    throw new ArgumentNullException();
                interningProvider = value;
            }
        }


        public SymbolResolveContext(string filename, CompilerContext cp)
        {
            this.Compiler = cp;
            this.unresolvedFile = new VSharpUnresolvedFile();
            this.unresolvedFile.FileName = filename;
            this.usingScope = unresolvedFile.RootUsingScope;
            IsInMemberAccess = false;
        }


        public DomRegion MakeRegion(Location start, Location end)
        {
            return new DomRegion(unresolvedFile.FileName, start.Line, start.Column, end.Line, end.Column);
        }

        public bool IsInMemberAccess { get; set; }

        #region Types
        public static KnownTypeCode GetTypeCodeForPrimitiveType(string keyword)
        {
            switch (keyword)
            {
                case "string":
                    return KnownTypeCode.String;
                case "int":
                    return KnownTypeCode.Int32;
                case "uint":
                    return KnownTypeCode.UInt32;
                case "object":
                    return KnownTypeCode.Object;
                case "bool":
                    return KnownTypeCode.Boolean;
                case "sbyte":
                    return KnownTypeCode.SByte;
                case "byte":
                    return KnownTypeCode.Byte;
                case "short":
                    return KnownTypeCode.Int16;
                case "ushort":
                    return KnownTypeCode.UInt16;
                case "long":
                    return KnownTypeCode.Int64;
                case "ulong":
                    return KnownTypeCode.UInt64;
                case "float":
                    return KnownTypeCode.Single;
                case "double":
                    return KnownTypeCode.Double;
                case "decimal":
                    return KnownTypeCode.Decimal;
                case "char":
                    return KnownTypeCode.Char;
                case "void":
                    return KnownTypeCode.Void;
                default:
                    return KnownTypeCode.None;
            }
        }
        public ITypeReference ConvertTypeReference(BuiltinType type, NameLookupMode lookupMode = NameLookupMode.Type)
        {
            KnownTypeCode typeCode = GetTypeCodeForPrimitiveType(type._Keyword);
            if (typeCode == KnownTypeCode.None)
                return new UnknownTypeSpec(null, type._Keyword);
            else
                return KnownTypeReference.Get(typeCode);
         
        }
        public ITypeReference ConvertTypeReference(VSC.AST.Type sne, NameLookupMode lookupMode, InterningProvider interningProvider = null)
        {
            if (interningProvider == null)
                interningProvider = InterningProvider.Dummy;

            return ConvertTypeReference(sne._type_expression_or_array, lookupMode, interningProvider);
        }
        public ITypeReference ConvertTypeReference(VSC.AST.MemberType sne, NameLookupMode lookupMode, InterningProvider interningProvider = null)
        {
            if (interningProvider == null)
                interningProvider = InterningProvider.Dummy;
            if(sne._type_expression_or_array != null)
                    return ConvertTypeReference(sne._type_expression_or_array, lookupMode, interningProvider);
            else return KnownTypeReference.Get(KnownTypeCode.Void); 
        }
        public ITypeReference ConvertTypeReference(VSC.AST.TypeExpressionOrArray sne, NameLookupMode lookupMode, InterningProvider interningProvider = null)
        {
            if (interningProvider == null)
                interningProvider = InterningProvider.Dummy;
            ITypeReference target = ConvertTypeReference(sne, lookupMode, interningProvider);
            foreach (var a in sne._rank_specifiers)
                target = interningProvider.Intern(new ArrayTypeReference(target, a._dimension));

            return target;

        }
        public ITypeReference ConvertTypeReference(VSC.AST.QualifiedAliasMember p, NameLookupMode lookupMode, InterningProvider interningProvider = null)
        {
            ITypeReference target = null;
            if (interningProvider == null)
                interningProvider = InterningProvider.Dummy;

            TypeOrNamespaceReference t;
             SimpleNameExpr st = p._simple_name_expr;
                if (st != null)
                    t = interningProvider.Intern(new AliasNamespaceReference(interningProvider.Intern(st._identifier._Identifier)));
                else
                    return SpecialTypeSpec.UnknownType;      
            var typeArguments = new List<ITypeReference>();
            foreach (var ta in st._opt_type_argument_list._type_arguments)
                typeArguments.Add(ConvertTypeReference(ta,lookupMode, interningProvider));
            
            string memberName = interningProvider.Intern(p._identifier._Identifier);
            return interningProvider.Intern(new MemberTypeOrNamespaceReference(t, memberName, interningProvider.InternList(typeArguments), lookupMode));
        }
        public ITypeReference ConvertTypeReference(VSC.AST.PackageOrTypeExpr p, NameLookupMode lookupMode, InterningProvider interningProvider = null)
        {
        
            if (interningProvider == null)
                interningProvider = InterningProvider.Dummy;
            if (p._package_or_type_expr != null && p._simple_name_expr != null)
            {
                var target = ConvertTypeReference(p._package_or_type_expr, lookupMode, interningProvider) as TypeOrNamespaceReference;
                var typeArguments = new List<ITypeReference>();
                foreach (var ta in p._simple_name_expr._opt_type_argument_list._type_arguments)
                    typeArguments.Add(ConvertTypeReference(ta,lookupMode, interningProvider));

                string memberName = interningProvider.Intern(p._simple_name_expr._identifier._Identifier);
              
                return interningProvider.Intern(new MemberTypeOrNamespaceReference(target, memberName, interningProvider.InternList(typeArguments), lookupMode));
            }
            else if (p._simple_name_expr != null)
                return ConvertTypeReference(p._simple_name_expr, lookupMode, interningProvider);
            else
                return ConvertTypeReference(p._qualified_alias_member, lookupMode, interningProvider);


        }
        public ITypeReference ConvertTypeReference(VSC.AST.TypeExpression type, NameLookupMode lookupMode, InterningProvider interningProvider = null)
        {
            if (interningProvider == null)
                interningProvider = InterningProvider.Dummy;
            ITypeReference t = null;
            if (type._builtin_type_expression != null)
                return ConvertTypeReference(type._builtin_type_expression, lookupMode, interningProvider);
            else
            {
                t = ConvertTypeReference(type._package_or_type_expr, lookupMode, interningProvider);
           
                if (type._isnullable)
                    t = interningProvider.Intern(NullableType.Create(t));
                else if (type._pointer_stars != null)
                {
                    foreach (var p in type._pointer_stars)
                        t = interningProvider.Intern(new PointerTypeReference(t));
                }


                return t;
            }
        }
        public ITypeReference ConvertTypeReference(VSC.AST.BuiltinTypeExpression type, NameLookupMode lookupMode, InterningProvider interningProvider = null)
        {
            if (interningProvider == null)
                interningProvider = InterningProvider.Dummy;
             ITypeReference t =ConvertTypeReference( type,lookupMode, interningProvider);

             if (type._isnullable)
                 t = interningProvider.Intern(NullableType.Create(t));
             else if (type._pointer_stars != null)
             {
                foreach(var p in type._pointer_stars)
                     t = interningProvider.Intern(new PointerTypeReference(t));
             }


             return t;
        }
        public ITypeReference ConvertTypeReference(SimpleNameExpr sne,NameLookupMode lookupMode, InterningProvider interningProvider = null)
        {
            if (interningProvider == null)
                interningProvider = InterningProvider.Dummy;
            var typeArguments = new List<ITypeReference>();
            if (sne._opt_type_argument_list._type_arguments != null)
            {
                foreach (var ta in sne._opt_type_argument_list._type_arguments)
                    typeArguments.Add(ConvertTypeReference(ta,lookupMode, interningProvider));
                
            }
            string identifier = sne._identifier._Identifier;
            if (typeArguments.Count == 0 && string.IsNullOrEmpty(identifier))
              // empty SimpleType is used for typeof(List<>).
                return SpecialTypeSpec.UnboundTypeArgument;
          
            var t = new SimpleTypeOrNamespaceReference(identifier, interningProvider.InternList(typeArguments), lookupMode);
            return interningProvider.Intern(t);
        }
        #endregion

        #region Modifiers
        static void ApplyModifiers(UnresolvedTypeDefinitionSpec td, VSC.TypeSystem.Modifiers modifiers)
        {
            td.Accessibility = GetAccessibility(modifiers) ?? (td.DeclaringTypeDefinition != null ? Accessibility.Private : Accessibility.Internal);
            td.IsAbstract = (modifiers & (VSC.TypeSystem.Modifiers.ABSTRACT | VSC.TypeSystem.Modifiers.STATIC)) != 0;
            td.IsSealed = (modifiers & (VSC.TypeSystem.Modifiers.SEALED | VSC.TypeSystem.Modifiers.STATIC)) != 0;
            td.IsShadowing = (modifiers & VSC.TypeSystem.Modifiers.NEW) != 0;
        }
        static void ApplyModifiers(UnresolvedMemberSpec m, VSC.TypeSystem.Modifiers modifiers)
        {
            // members from interfaces are always Public+Abstract. (NOTE: 'new' modifier is valid in interfaces as well.)
            if (m.DeclaringTypeDefinition.Kind == TypeKind.Interface)
            {
                m.Accessibility = Accessibility.Public;
                m.IsAbstract = true;
                m.IsShadowing = (modifiers & VSC.TypeSystem.Modifiers.NEW) != 0;
                return;
            }
            m.Accessibility = GetAccessibility(modifiers) ?? Accessibility.Private;
            m.IsAbstract = (modifiers & VSC.TypeSystem.Modifiers.ABSTRACT) != 0;
            m.IsOverride = (modifiers & VSC.TypeSystem.Modifiers.OVERRIDE) != 0;
            m.IsSealed = (modifiers & VSC.TypeSystem.Modifiers.SEALED) != 0;
            m.IsShadowing = (modifiers & VSC.TypeSystem.Modifiers.NEW) != 0;
            m.IsStatic = (modifiers & VSC.TypeSystem.Modifiers.STATIC) != 0;
            m.IsVirtual = (modifiers & VSC.TypeSystem.Modifiers.VIRTUAL) != 0;
        }
        static Accessibility? GetAccessibility(VSC.TypeSystem.Modifiers modifiers)
        {
            switch (modifiers & VSC.TypeSystem.Modifiers.AccessibilityMask)
            {
                case VSC.TypeSystem.Modifiers.PRIVATE:
                    return Accessibility.Private;
                case VSC.TypeSystem.Modifiers.FRIEND:
                    return Accessibility.Internal;
                case VSC.TypeSystem.Modifiers.PROTECTED | VSC.TypeSystem.Modifiers.FRIEND:
                    return Accessibility.ProtectedOrInternal;
                case VSC.TypeSystem.Modifiers.PROTECTED:
                    return Accessibility.Protected;
                case VSC.TypeSystem.Modifiers.PUBLIC:
                    return Accessibility.Public;
                default:
                    return null;
            }
        }
        #endregion


        //TODO:Add Constant folding support
        #region Constant Values
        IConstantValue ConvertConstantValue(ITypeReference targetType, Expression expression)
        {
            return ConvertConstantValue(targetType, expression, currentTypeDefinition, currentMethod, usingScope, interningProvider);
        }

        internal static IConstantValue ConvertConstantValue(
            ITypeReference targetType, Expression expression,
            IUnresolvedTypeDefinition parentTypeDefinition, IUnresolvedMethod parentMethodDefinition, UsingScope parentUsingScope,
            InterningProvider interningProvider)
        {

            ConstantExpression c = expression as ConstantExpression;
            if (c == null)
                return new ErrorConstantValue(targetType);
            PrimitiveConstantExpression pc = c as PrimitiveConstantExpression;
            if (pc != null && pc.Type == targetType)
                // Save memory by directly using a SimpleConstantValue.
                return interningProvider.Intern(new SimpleConstantValue(targetType, pc.Value));
       
            // cast to the desired type
            return interningProvider.Intern(new ConstantCast(targetType, c, true));
        }
        IConstantValue ConvertAttributeArgument(Expression expression)
        {
            if (expression is ConstantExpression)
                return expression as ConstantExpression;

            return null;
        }
        #endregion

        #region Attributes
       public void ConvertAttributes(IList<IUnresolvedAttribute> outputList, IEnumerable<AttributeSection> attributes)
        {
            foreach (AttributeSection section in attributes)
            {
                ConvertAttributes(outputList, section);
            }
        }

       public void ConvertAttributes(IList<IUnresolvedAttribute> outputList, AttributeSection attributeSection)
        {
            foreach (VSC.AST.Attribute attr in attributeSection._attribute_list)
                outputList.Add(ConvertAttribute(attr));
            
        }

    

        VSharpAttribute ConvertAttribute(AST.Attribute attr)
        {
            DomRegion region = MakeRegion(attr);

            ITypeReference type = ConvertTypeReference(attr._package_or_type_expr, NameLookupMode.Type,interningProvider);
            List<IConstantValue> positionalArguments = null;
            List<KeyValuePair<string, IConstantValue>> namedCtorArguments = null;
            List<KeyValuePair<string, IConstantValue>> namedArguments = null;
            if (attr._opt_argument_list != null)
            {
                foreach (var arg in attr._opt_argument_list._argument_list)
                {
                        if (arg._named_argument_expression != null)
                        {
                            string name = interningProvider.Intern(arg._named_argument_expression._identifier._Identifier);
                            if (namedArguments == null)
                                namedArguments = new List<KeyValuePair<string, IConstantValue>>();
                            namedArguments.Add(new KeyValuePair<string, IConstantValue>(name, ConvertAttributeArgument(arg._named_argument_expression._expression)));
                        }
                        else if (arg._non_simple_argument != null)
                        {
                            if (positionalArguments == null)
                                positionalArguments = new List<IConstantValue>();
                            positionalArguments.Add(ConvertAttributeArgument(arg._non_simple_argument._expression));
                        }
                        else
                        {
                            if (positionalArguments == null)
                                positionalArguments = new List<IConstantValue>();
                            positionalArguments.Add(ConvertAttributeArgument(arg._expression));
                        }
                    
                }
            }
            return new VSharpAttribute(type, region, interningProvider.InternList(positionalArguments), namedCtorArguments, namedArguments);
        }
        #endregion

        #region Parameters
        UnresolvedParameterSpec ConvertParameter(FixedParameter pd)
        {
            UnresolvedParameterSpec p = new UnresolvedParameterSpec(ConvertTypeReference(pd._member_type, NameLookupMode.Type), interningProvider.Intern(pd._identifier._Identifier));
            p.Region = MakeRegion(pd);
            if (pd._opt_attributes._attribute_sections != null)
                ConvertAttributes(p.Attributes, pd._opt_attributes._attribute_sections);

            if (pd._opt_parameter_modifier._parameter_modifier != null)
            {
                switch (pd._opt_parameter_modifier._parameter_modifier._Modifier)
                {
                    case VSC.TypeSystem.ParameterModifier.Ref:
                        p.IsRef = true;
                        p.Type = interningProvider.Intern(new ByReferenceTypeReference(p.Type));
                        break;
                    case VSC.TypeSystem.ParameterModifier.Out:
                        p.IsOut = true;
                        p.Type = interningProvider.Intern(new ByReferenceTypeReference(p.Type));
                        break;
                }
            }
            if (pd._expression != null)
                p.DefaultValue = ConvertConstantValue(p.Type, pd._expression);

            return p;
       
        }
        UnresolvedParameterSpec ConvertParameter(ParameterArray pd)
        {
            UnresolvedParameterSpec p = new UnresolvedParameterSpec(ConvertTypeReference(pd._type, NameLookupMode.Type), interningProvider.Intern(pd._identifier._Identifier));
            p.Region = MakeRegion(pd);
            if (pd._opt_attributes._attribute_sections != null)
                ConvertAttributes(p.Attributes, pd._opt_attributes._attribute_sections);

            p.IsParams = true;
            if (pd._expression != null)
                p.DefaultValue = ConvertConstantValue(p.Type, pd._expression);

            return p;

        }
        void ConvertParameters(IList<IUnresolvedParameter> outputList, IEnumerable<FixedParameter> parameters, ParameterArray par = null)
        {
            foreach ( FixedParameter pd in parameters)
                outputList.Add(interningProvider.Intern(ConvertParameter(pd)));
            if(par != null)
                  outputList.Add(interningProvider.Intern(ConvertParameter(par)));
        }

        internal IList<ITypeReference> GetParameterTypes(IEnumerable<FixedParameter> parameters, InterningProvider interningProvider)
        {
            List<ITypeReference> result = new List<ITypeReference>();
            foreach (FixedParameter pd in parameters)
            {
                ITypeReference type = ConvertTypeReference(pd._member_type,NameLookupMode.Type, interningProvider);


                if(pd._opt_parameter_modifier._parameter_modifier != null)
                    if (pd._opt_parameter_modifier._parameter_modifier._Modifier == VSC.TypeSystem.ParameterModifier.Ref || pd._opt_parameter_modifier._parameter_modifier._Modifier == VSC.TypeSystem.ParameterModifier.Out)
                    type = interningProvider.Intern(new ByReferenceTypeReference(type));


                result.Add(type);
            }
            return result;
        }
        #endregion
       public DomRegion MakeRegion(Semantic node)
        {
            if (node == null )
                return DomRegion.Empty;
            else
                return MakeRegion(node.Location,Location.Null);
        }
       public DomRegion MakeRegion(Semantic node,Semantic end)
       {
           if (node == null)
               return DomRegion.Empty;
           else
               return MakeRegion(node.Location, end.Location);
       }
        //internal static Location GetStartLocationAfterAttributes(Semantic node)
        //{
        //    AstNode child = node.FirstChild;
        //    // Skip attributes and comments between attributes for the purpose of
        //    // getting a declaration's region.
        //    while (child != null && (child is AttributeSection || child.NodeType == NodeType.Whitespace))
        //        child = child.NextSibling;
        //    return (child ?? node).StartLocation;
        //}

       
    }
   public class ResolveContext
    {
       internal VSharpResolver _resolver;
     
       public VSharpResolver Resolver { get { return _resolver; } }
       public void CreateResolver(ICompilation c)
       {
           _resolver = new VSharpResolver(c);
       }
    

       public ResolveContext()
       {

       }
       public ResolveContext(ICompilation comp)
       {
           _resolver = new VSharpResolver(comp);
       }




    }
}
