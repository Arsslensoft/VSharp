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
    public class SymbolResolveContext
    {

        public static List<TypeContainer> DefaultTypes;
       
        
        static SymbolResolveContext()
        {
            DefaultTypes = new List<TypeContainer>();
        }

        public CompilerContext Compiler { get; set; }
        internal UsingScope usingScope;
        internal TypeContainer currentTypeDefinition;

        public TypeContainer DefaultTypeDefinition = null;
        internal MethodOrOperator currentMethod;

        internal InterningProvider interningProvider = new SimpleInterningProvider();
        internal CompilationSourceFile unresolvedFile;
        public CompilationSourceFile UnresolvedFile
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
            //this.unresolvedFile = new CompilationSourceFile();
            //this.unresolvedFile.FileName = filename;
            this.usingScope = unresolvedFile.RootUsingScope;
            DefaultTypeDefinition = CreateTypeDefinition("default", true);
            DefaultTypeDefinition.GlobalTypeDefinition = false;
            DefaultTypeDefinition.IsPartial = true;
            DefaultTypeDefinition.IsSealed = true;
            DefaultTypeDefinition.IsStatic = true;
            DefaultTypeDefinition.IsSynthetic = true;
           

            currentTypeDefinition = DefaultTypeDefinition;
            IsInMemberAccess = false;
        }
        public void AddDefaultType()
        {
            DefaultTypes.Add(DefaultTypeDefinition);
        }

        public DomRegion MakeRegion(Location start, Location end)
        {
            return new DomRegion(unresolvedFile.FileName, start.Line, start.Column, end.Line, end.Column);
        }

        public bool IsInMemberAccess { get; set; }


        public TypeContainer CreateTypeDefinition(string name, bool isglobal = false)
        {

            //TypeContainer newType;
            //if (currentTypeDefinition != null)
            //{
            //    newType = new TypeContainer(currentTypeDefinition, name);
            //    foreach (var typeParameter in currentTypeDefinition.TypeParameters)
            //        newType.TypeParameters.Addition(typeParameter);
            //    currentTypeDefinition.NestedTypes.Addition(newType);
            //}
            //else
            //{
            //    //newType = new TypeContainer(usingScope, name);
            //    //unresolvedFile.TopLevelTypeDefinitions.Addition(newType);
            //}
            //newType.UnresolvedFile = unresolvedFile;
            //newType.HasExtensionMethods = false; // gets set to true when an extension method is added
            return null;
        }


        //#region Types
        //public static KnownTypeCode GetTypeCodeForPrimitiveType(string keyword)
        //{
        //    switch (keyword)
        //    {
        //        case "string":
        //            return KnownTypeCode.String;
        //        case "int":
        //            return KnownTypeCode.Int32;
        //        case "uint":
        //            return KnownTypeCode.UInt32;
        //        case "object":
        //            return KnownTypeCode.Object;
        //        case "bool":
        //            return KnownTypeCode.Boolean;
        //        case "sbyte":
        //            return KnownTypeCode.SByte;
        //        case "byte":
        //            return KnownTypeCode.Byte;
        //        case "short":
        //            return KnownTypeCode.Int16;
        //        case "ushort":
        //            return KnownTypeCode.UInt16;
        //        case "long":
        //            return KnownTypeCode.Int64;
        //        case "ulong":
        //            return KnownTypeCode.UInt64;
        //        case "float":
        //            return KnownTypeCode.Single;
        //        case "double":
        //            return KnownTypeCode.Double;
        //        case "decimal":
        //            return KnownTypeCode.Decimal;
        //        case "char":
        //            return KnownTypeCode.Char;
        //        case "void":
        //            return KnownTypeCode.Void;
        //        default:
        //            return KnownTypeCode.None;
        //    }
        //}
        //public ITypeReference ConvertTypeReference(BuiltinType type, NameLookupMode lookupMode = NameLookupMode.Type)
        //{
        //    KnownTypeCode typeCode = GetTypeCodeForPrimitiveType(type._Keyword);
        //    if (typeCode == KnownTypeCode.None)
        //        return new UnknownTypeSpec(null, type._Keyword);
        //    else
        //        return KnownTypeReference.Get(typeCode);

        //}
        //public ITypeReference ConvertTypeReference(VSC.AST.Type sne, NameLookupMode lookupMode, InterningProvider interningProvider = null)
        //{
        //    if (interningProvider == null)
        //        interningProvider = InterningProvider.Dummy;

        //    return ConvertTypeReference(sne._type_expression_or_array, lookupMode, interningProvider);
        //}
        //public ITypeReference ConvertTypeReference(VSC.AST.MemberType sne, NameLookupMode lookupMode, InterningProvider interningProvider = null)
        //{
        //    if (interningProvider == null)
        //        interningProvider = InterningProvider.Dummy;
        //    if (sne._type_expression_or_array != null)
        //        return ConvertTypeReference(sne._type_expression_or_array, lookupMode, interningProvider);
        //    else return KnownTypeReference.Get(KnownTypeCode.Void);
        //}
        //public ITypeReference ConvertTypeReference(VSC.AST.TypeExpressionOrArray sne, NameLookupMode lookupMode, InterningProvider interningProvider = null)
        //{
        //    if (interningProvider == null)
        //        interningProvider = InterningProvider.Dummy;
        //    ITypeReference target = ConvertTypeReference(sne._type_expression, lookupMode, interningProvider);
        //    if (sne._rank_specifiers != null)
        //        foreach (var a in sne._rank_specifiers)
        //            target = interningProvider.Intern(new ArrayTypeReference(target, a._dimension));

        //    return target;

        //}
        //public ITypeReference ConvertTypeReference(VSC.AST.QualifiedAliasMember p, NameLookupMode lookupMode, InterningProvider interningProvider = null)
        //{
        //    ITypeReference target = null;
        //    if (interningProvider == null)
        //        interningProvider = InterningProvider.Dummy;

        //    TypeOrNamespaceReference t;
        //    SimpleNameExpr st = p._simple_name_expr;
        //    if (st != null)
        //        t = interningProvider.Intern(new AliasNamespaceReference(interningProvider.Intern(st._identifier._Identifier)));
        //    else
        //        return SpecialTypeSpec.UnknownType;
        //    var typeArguments = new List<ITypeReference>();
        //    foreach (var ta in st._opt_type_argument_list._type_arguments)
        //        typeArguments.Addition(ConvertTypeReference(ta, lookupMode, interningProvider));

        //    string memberName = interningProvider.Intern(p._identifier._Identifier);
        //    return interningProvider.Intern(new MemberTypeOrNamespaceReference(t, memberName, interningProvider.InternList(typeArguments), lookupMode));
        //}
        //public ITypeReference ConvertTypeReference(VSC.AST.PackageOrTypeExpr p, NameLookupMode lookupMode, InterningProvider interningProvider = null)
        //{

        //    if (interningProvider == null)
        //        interningProvider = InterningProvider.Dummy;
        //    if (p._package_or_type_expr != null && p._simple_name_expr != null)
        //    {
        //        var target = ConvertTypeReference(p._package_or_type_expr, lookupMode, interningProvider) as TypeOrNamespaceReference;
        //        var typeArguments = new List<ITypeReference>();
        //     if( p._simple_name_expr._opt_type_argument_list._type_arguments != null)
        //        foreach (var ta in p._simple_name_expr._opt_type_argument_list._type_arguments)
        //            typeArguments.Addition(ConvertTypeReference(ta, lookupMode, interningProvider));

        //        string memberName = interningProvider.Intern(p._simple_name_expr._identifier._Identifier);

        //        return interningProvider.Intern(new MemberTypeOrNamespaceReference(target, memberName, interningProvider.InternList(typeArguments), lookupMode));
        //    }
        //    else if (p._simple_name_expr != null)
        //        return ConvertTypeReference(p._simple_name_expr, lookupMode, interningProvider);
        //    else
        //        return ConvertTypeReference(p._qualified_alias_member, lookupMode, interningProvider);


        //}
        //public ITypeReference ConvertTypeReference(VSC.AST.TypeExpression type, NameLookupMode lookupMode, InterningProvider interningProvider = null)
        //{
        //    if (interningProvider == null)
        //        interningProvider = InterningProvider.Dummy;
        //    ITypeReference t = null;
        //    if (type._builtin_type_expression != null)
        //        return ConvertTypeReference(type._builtin_type_expression, lookupMode, interningProvider);
        //    else
        //    {
        //        t = ConvertTypeReference(type._package_or_type_expr, lookupMode, interningProvider);

        //        if (type._isnullable)
        //            t = interningProvider.Intern(NullableType.Create(t));
        //        else if (type._pointer_stars != null)
        //        {
        //            foreach (var p in type._pointer_stars)
        //                t = interningProvider.Intern(new PointerTypeReference(t));
        //        }


        //        return t;
        //    }
        //}
        //public ITypeReference ConvertTypeReference(VSC.AST.BuiltinTypeExpression type, NameLookupMode lookupMode, InterningProvider interningProvider = null)
        //{
        //    if (interningProvider == null)
        //        interningProvider = InterningProvider.Dummy;
        //    ITypeReference t = ConvertTypeReference(type._builtin_type, lookupMode);

        //    if (type._isnullable)
        //        t = interningProvider.Intern(NullableType.Create(t));
        //    else if (type._pointer_stars != null)
        //    {
        //        foreach (var p in type._pointer_stars)
        //            t = interningProvider.Intern(new PointerTypeReference(t));
        //    }


        //    return t;
        //}
        //public ITypeReference ConvertTypeReference(SimpleNameExpr sne, NameLookupMode lookupMode, InterningProvider interningProvider = null)
        //{
        //    if (interningProvider == null)
        //        interningProvider = InterningProvider.Dummy;
        //    var typeArguments = new List<ITypeReference>();
        //    if (sne._opt_type_argument_list._type_arguments != null)
        //    {
        //        foreach (var ta in sne._opt_type_argument_list._type_arguments)
        //            typeArguments.Addition(ConvertTypeReference(ta, lookupMode, interningProvider));

        //    }
        //    string identifier = sne._identifier._Identifier;
        //    if (typeArguments.Count == 0 && string.IsNullOrEmpty(identifier))
        //        // empty SimpleType is used for typeof(List<>).
        //        return SpecialTypeSpec.UnboundTypeArgument;

        //    var t = new SimpleTypeOrNamespaceReference(identifier, interningProvider.InternList(typeArguments), lookupMode);
        //    return interningProvider.Intern(t);
        //}
        //public ITypeReference ConvertTypeReference(ExplicitInterface sne, NameLookupMode lookupMode, InterningProvider interningProvider = null)
        //{
        //    if (interningProvider == null)
        //        interningProvider = InterningProvider.Dummy;
        //    var typeArguments = new List<ITypeReference>();
        //    if (sne._opt_type_argument_list._type_arguments != null)
        //    {
        //        foreach (var ta in sne._opt_type_argument_list._type_arguments)
        //            typeArguments.Addition(ConvertTypeReference(ta, lookupMode, interningProvider));

        //    }
        //    string identifier = sne._identifier._Identifier;
        //    if (typeArguments.Count == 0 && string.IsNullOrEmpty(identifier))
        //        // empty SimpleType is used for typeof(List<>).
        //        return SpecialTypeSpec.UnboundTypeArgument;

        //    var t = new SimpleTypeOrNamespaceReference(identifier, interningProvider.InternList(typeArguments), lookupMode);
        //    return interningProvider.Intern(t);
        //}
        //#endregion

        //#region Modifiers
        //public void ApplyModifiers(UnresolvedTypeDefinitionSpec td, VSC.TypeSystem.Modifiers modifiers)
        //{
        //    td.Accessibility = GetAccessibility(modifiers) ?? (td.DeclaringTypeDefinition != null ? Accessibility.Private : Accessibility.Internal);
        //    td.IsAbstract = (modifiers & (VSC.TypeSystem.Modifiers.ABSTRACT | VSC.TypeSystem.Modifiers.STATIC)) != 0;
        //    td.IsSealed = (modifiers & (VSC.TypeSystem.Modifiers.SEALED | VSC.TypeSystem.Modifiers.STATIC)) != 0;
        //    td.IsShadowing = (modifiers & VSC.TypeSystem.Modifiers.NEW) != 0;
        //    td.IsPartial = (modifiers & VSC.TypeSystem.Modifiers.PARTIAL) != 0;
        //}
        //public void ApplyModifiers(MemberContainer m, VSC.TypeSystem.Modifiers modifiers)
        //{
        //    // members from interfaces are always Public+Abstract. (NOTE: 'new' modifier is valid in interfaces as well.)
        //    if (m.DeclaringTypeDefinition.Kind == TypeKind.Interface)
        //    {
        //        m.Accessibility = Accessibility.Public;
        //        m.IsAbstract = true;
        //        m.IsShadowing = (modifiers & VSC.TypeSystem.Modifiers.NEW) != 0;
        //        return;
        //    }
        //    m.Accessibility = GetAccessibility(modifiers) ?? Accessibility.Private;
        //    m.IsAbstract = (modifiers & VSC.TypeSystem.Modifiers.ABSTRACT) != 0;
        //    m.IsOverride = (modifiers & VSC.TypeSystem.Modifiers.OVERRIDE) != 0;
        //    m.IsSealed = (modifiers & VSC.TypeSystem.Modifiers.SEALED) != 0;
        //    m.IsShadowing = (modifiers & VSC.TypeSystem.Modifiers.NEW) != 0;
        //    m.IsStatic = (modifiers & VSC.TypeSystem.Modifiers.STATIC) != 0;
        //    m.IsVirtual = (modifiers & VSC.TypeSystem.Modifiers.VIRTUAL) != 0;
        //}
        //public Accessibility? GetAccessibility(VSC.TypeSystem.Modifiers modifiers)
        //{
        //    switch (modifiers & VSC.TypeSystem.Modifiers.AccessibilityMask)
        //    {
        //        case VSC.TypeSystem.Modifiers.PRIVATE:
        //            return Accessibility.Private;
        //        case VSC.TypeSystem.Modifiers.INTERNAL:
        //            return Accessibility.Internal;
        //        case VSC.TypeSystem.Modifiers.PROTECTED | VSC.TypeSystem.Modifiers.INTERNAL:
        //            return Accessibility.ProtectedOrInternal;
        //        case VSC.TypeSystem.Modifiers.PROTECTED:
        //            return Accessibility.Protected;
        //        case VSC.TypeSystem.Modifiers.PUBLIC:
        //            return Accessibility.Public;
        //        default:
        //            return null;
        //    }
        //}
        //#endregion


        ////TODO:Addition Constant folding support
        //#region Constant Values
        //public IConstantValue ConvertConstantValue(ITypeReference targetType, Expression expression)
        //{
        //    return ConvertConstantValue(targetType, expression, currentTypeDefinition, currentMethod, usingScope, interningProvider);
        //}

        //internal static IConstantValue ConvertConstantValue(
        //    ITypeReference targetType, Expression expression,
        //    IUnresolvedTypeDefinition parentTypeDefinition, IUnresolvedMethod parentMethodDefinition, UsingScope parentUsingScope,
        //    InterningProvider interningProvider)
        //{
        //    LiteralExpression l = expression as LiteralExpression;
        //    if (l == null)
        //        return new ErrorConstantValue(targetType);

        //    ConstantExpression c = l.ConstantExpr;
        //    if (c == null)
        //        return new ErrorConstantValue(targetType);
        //    PrimitiveConstantExpression pc = c as PrimitiveConstantExpression;
        //    if (pc != null && pc.Type == targetType)
        //        // Save memory by directly using a SimpleConstantValue.
        //        return interningProvider.Intern(new SimpleConstantValue(targetType, pc.Value));

        //    // cast to the desired type
        //    return interningProvider.Intern(new ConstantCast(targetType, c, true));
        //}
        //IConstantValue ConvertAttributeArgument(Expression expression)
        //{
        //    if (expression is ConstantExpression)
        //        return expression as ConstantExpression;

        //    return null;
        //}
        //#endregion

        //#region Attributes

        //public void ConvertAttributes(IList<IUnresolvedAttribute> outputList, List<VSC.AST.Attribute> attributeSection)
        //{
        //    foreach (VSC.AST.Attribute attr in attributeSection)
        //        outputList.Addition(ConvertAttribute(attr));

        //}



        //VSharpAttribute ConvertAttribute(AST.Attribute attr)
        //{
        //    DomRegion region = MakeRegion(attr);

        //    ITypeReference type = ConvertTypeReference(attr._package_or_type_expr, NameLookupMode.Type, interningProvider);
        //    List<IConstantValue> positionalArguments = null;
        //    List<KeyValuePair<string, IConstantValue>> namedCtorArguments = null;
        //    List<KeyValuePair<string, IConstantValue>> namedArguments = null;
        //    if (attr._opt_argument_list != null)
        //    {
        //        foreach (var arg in attr._opt_argument_list._argument_list)
        //        {
        //            if (arg._named_argument_expression != null)
        //            {
        //                string name = interningProvider.Intern(arg._named_argument_expression._identifier._Identifier);
        //                if (namedArguments == null)
        //                    namedArguments = new List<KeyValuePair<string, IConstantValue>>();
        //                namedArguments.Addition(new KeyValuePair<string, IConstantValue>(name, ConvertAttributeArgument(arg._named_argument_expression._expression)));
        //            }
        //            else if (arg._non_simple_argument != null)
        //            {
        //                if (positionalArguments == null)
        //                    positionalArguments = new List<IConstantValue>();
        //                positionalArguments.Addition(ConvertAttributeArgument(arg._non_simple_argument._expression));
        //            }
        //            else
        //            {
        //                if (positionalArguments == null)
        //                    positionalArguments = new List<IConstantValue>();
        //                positionalArguments.Addition(ConvertAttributeArgument(arg._expression));
        //            }

        //        }
        //    }
        //    return new VSharpAttribute(type, region, interningProvider.InternList(positionalArguments), namedCtorArguments, namedArguments);
        //}
        //#endregion

        //#region Parameters
        //Parameter ConvertParameter(FixedParameter pd)
        //{
        //    Parameter p = new Parameter(ConvertTypeReference(pd._type, NameLookupMode.Type), interningProvider.Intern(pd._identifier._Identifier));
        //    p.Region = MakeRegion(pd);
        //    ConvertAttributes(p.Attributes, pd._opt_attributes._Attributes);

        //    if (pd._opt_parameter_modifier._parameter_modifier != null)
        //    {
        //        switch (pd._opt_parameter_modifier._parameter_modifier._Modifier)
        //        {
        //            case VSC.TypeSystem.ParameterModifier.Ref:
        //                p.IsRef = true;
        //                p.Type = interningProvider.Intern(new ByReferenceTypeReference(p.Type));
        //                break;
        //            case VSC.TypeSystem.ParameterModifier.Out:
        //                p.IsOut = true;
        //                p.Type = interningProvider.Intern(new ByReferenceTypeReference(p.Type));
        //                break;
        //        }
        //    }
        //    if (pd._expression != null)
        //        p.DefaultValue = ConvertConstantValue(p.Type, pd._expression);

        //    return p;

        //}
        //Parameter ConvertParameter(ParameterArray pd)
        //{
        //    Parameter p = new Parameter(ConvertTypeReference(pd._type, NameLookupMode.Type), interningProvider.Intern(pd._identifier._Identifier));
        //    p.Region = MakeRegion(pd);

        //    ConvertAttributes(p.Attributes, pd._opt_attributes._Attributes);

        //    p.IsParams = true;
        //    if (pd._expression != null)
        //        p.DefaultValue = ConvertConstantValue(p.Type, pd._expression);

        //    return p;

        //}
        //public void ConvertParameters(IList<IUnresolvedParameter> outputList, IEnumerable<FixedParameter> parameters, ParameterArray par = null)
        //{
        //    if (parameters != null)
        //        foreach (FixedParameter pd in parameters)
        //            outputList.Addition(interningProvider.Intern(ConvertParameter(pd)));

        //    if (par != null)
        //        outputList.Addition(interningProvider.Intern(ConvertParameter(par)));
        //}

        //public IList<ITypeReference> GetParameterTypes(IEnumerable<FixedParameter> parameters, InterningProvider interningProvider)
        //{
        //    List<ITypeReference> result = new List<ITypeReference>();
        //    foreach (FixedParameter pd in parameters)
        //    {
        //        ITypeReference type = ConvertTypeReference(pd._type, NameLookupMode.Type, interningProvider);


        //        if (pd._opt_parameter_modifier._parameter_modifier != null)
        //            if (pd._opt_parameter_modifier._parameter_modifier._Modifier == VSC.TypeSystem.ParameterModifier.Ref || pd._opt_parameter_modifier._parameter_modifier._Modifier == VSC.TypeSystem.ParameterModifier.Out)
        //                type = interningProvider.Intern(new ByReferenceTypeReference(type));


        //        result.Addition(type);
        //    }
        //    return result;
        //}
        //#endregion

        //#region Documentation
        //public void AddDocumentation(IUnresolvedEntity entity, OptDocumentation doc)
        //{

        //    if (doc._documentation != null)
        //    {
        //        StringBuilder documentation = new StringBuilder();
        //        foreach (var d in doc._documentation)
        //            documentation.AppendLine(d.Documentation);

        //        unresolvedFile.AddDocumentation(entity, documentation.ToString());
        //    }
        //}

        //void PrepareMultilineDocumentation(string content, StringBuilder b)
        //{
        //    using (var reader = new StringReader(content))
        //    {
        //        string firstLine = reader.ReadLine();
        //        // Addition first line only if it's not empty:
        //        if (!string.IsNullOrWhiteSpace(firstLine))
        //        {
        //            if (firstLine[0] == ' ')
        //                b.Append(firstLine, 1, firstLine.Length - 1);
        //            else
        //                b.Append(firstLine);
        //        }
        //        // Read lines into list:
        //        List<string> lines = new List<string>();
        //        string line;
        //        while ((line = reader.ReadLine()) != null)
        //            lines.Addition(line);
        //        // If the last line (the line with '*/' delimiter) is white space only, ignore it.
        //        if (lines.Count > 0 && string.IsNullOrWhiteSpace(lines[lines.Count - 1]))
        //            lines.RemoveAt(lines.Count - 1);
        //        if (lines.Count > 0)
        //        {
        //            // Extract pattern from lines[0]: whitespace, asterisk, whitespace
        //            int patternLength = 0;
        //            string secondLine = lines[0];
        //            while (patternLength < secondLine.Length && char.IsWhiteSpace(secondLine[patternLength]))
        //                patternLength++;
        //            if (patternLength < secondLine.Length && secondLine[patternLength] == '*')
        //            {
        //                patternLength++;
        //                while (patternLength < secondLine.Length && char.IsWhiteSpace(secondLine[patternLength]))
        //                    patternLength++;
        //            }
        //            else
        //            {
        //                // no asterisk
        //                patternLength = 0;
        //            }
        //            // Now reduce pattern length to the common pattern:
        //            for (int i = 1; i < lines.Count; i++)
        //            {
        //                line = lines[i];
        //                if (line.Length < patternLength)
        //                    patternLength = line.Length;
        //                for (int j = 0; j < patternLength; j++)
        //                {
        //                    if (secondLine[j] != line[j])
        //                        patternLength = j;
        //                }
        //            }
        //            // Append the lines to the string builder:
        //            for (int i = 0; i < lines.Count; i++)
        //            {
        //                if (b.Length > 0 || i > 0)
        //                    b.Append(Environment.NewLine);
        //                b.Append(lines[i], patternLength, lines[i].Length - patternLength);
        //            }
        //        }
        //    }
        //}
        //#endregion

        //#region Type Parameters


        //public void ConvertTypeParameters(IList<IUnresolvedTypeParameter> output, TypeParameters typeParameters,
        //                           TypeParameterConstraintsClauses constraints, SymbolKind ownerType)
        //{
        //    // output might be non-empty when type parameters were copied from an outer class
        //    int index = output.Count;
        //    List<UnresolvedTypeParameterSpec> list = new List<UnresolvedTypeParameterSpec>();
        //    if (typeParameters != null)
        //    {
        //        foreach (Identifier tpDecl in typeParameters)
        //        {
        //            UnresolvedTypeParameterSpec tp = new UnresolvedTypeParameterSpec(ownerType, index++, tpDecl._Identifier);
        //            tp.Region = MakeRegion(tpDecl);
        //            list.Addition(tp);
        //            output.Addition(tp); // tp must be added to list here so that it can be referenced by constraints
        //        }
        //        if (constraints != null)
        //            foreach (var c in constraints)
        //            {
        //                bool parfound = false;
        //                foreach (var tp in list)
        //                {
        //                    if (tp.Name == c._identifier._Identifier)
        //                    {
        //                        foreach (TypeParameterConstraints type in c._type_parameter_constraints)
        //                        {


        //                            if (type._kind == TypeParameterConstraintKind.Ctor)
        //                            {
        //                                tp.HasDefaultConstructorConstraint = true;
        //                                continue;
        //                            }
        //                            else if (type._kind == TypeParameterConstraintKind.Class)
        //                            {
        //                                tp.HasReferenceTypeConstraint = true;
        //                                continue;
        //                            }
        //                            else if (type._kind == TypeParameterConstraintKind.Struct)
        //                            {
        //                                tp.HasValueTypeConstraint = true;
        //                                continue;
        //                            }

        //                            var lookupMode = (ownerType == SymbolKind.TypeDefinition) ? NameLookupMode.BaseTypeReference : NameLookupMode.Type;
        //                            tp.Constraints.Addition(ConvertTypeReference(type._type, lookupMode));
        //                        }
        //                        parfound = true;
        //                        break;
        //                    }
        //                }
        //                if (!parfound)
        //                    Compiler.Report.Error(2, constraints.Location, "Type parameters constraints cannot be defined unless there is a type parameter declared, no type parameter with this `{0}` has been found", c._identifier._Identifier);

        //            }
        //        foreach (UnresolvedTypeParameterSpec tp in list)
        //            tp.ApplyInterningProvider(interningProvider);
        //    }
        //    else if (constraints != null)
        //        Compiler.Report.Error(2, constraints.Location, "Type parameters constraints cannot be defined unless there is a type parameter declared");

        //}
        //#endregion


        //#region Accessor
        //public MethodOrOperator ConvertAccessor(RaiseAccessorDeclaration accessor, IUnresolvedMember p, bool memberIsExtern)
        //{
        //    if (accessor == null)
        //        return null;
        //    var a = new MethodOrOperator(currentTypeDefinition, "raise_" + p.Name);
        //    a.SymbolKind = SymbolKind.Accessor;
        //    a.AccessorOwner = p;
        //    a.Accessibility = GetAccessibility(accessor._opt_modifiers._Modifiers) ?? p.Accessibility;
        //    a.IsAbstract = p.IsAbstract;
        //    a.IsOverride = p.IsOverride;
        //    a.IsSealed = p.IsSealed;
        //    a.IsStatic = p.IsStatic;
        //    a.IsSynthetic = p.IsSynthetic;
        //    a.IsVirtual = p.IsVirtual;

        //    a.Region = MakeRegion(accessor);
        //    a.BodyRegion = MakeRegion(accessor._block_or_semicolon);
        //    // An accessor has no body if both are true:
        //    //  a) there's no body in the code
        //    //  b) the member is either abstract or extern
        //    a.HasBody = !(accessor._block_or_semicolon._block_statement == null && (p.IsAbstract || memberIsExtern));
        //    if (p.SymbolKind == SymbolKind.Indexer)
        //    {
        //        foreach (var indexerParam in ((IUnresolvedProperty)p).Parameters)
        //            a.Parameters.Addition(indexerParam);
        //    }
        //    Parameter param = null;

        //    param = new Parameter(p.ReturnType, "value");
        //    a.Parameters.Addition(param);
        //    a.ReturnType = KnownTypeReference.Void;


        //    ConvertAttributes(a.ReturnTypeAttributes, accessor._opt_attributes._ReturnAttributes);
        //    ConvertAttributes(a.Attributes, accessor._opt_attributes._Attributes);

        //    if (p.IsExplicitInterfaceImplementation)
        //    {
        //        a.IsExplicitInterfaceImplementation = true;
        //        Debug.Assert(p.ExplicitInterfaceImplementations.Count == 1);
        //        a.ExplicitInterfaceImplementations.Addition(interningProvider.Intern(new MemberReferenceSpec(
        //            SymbolKind.Accessor,
        //            p.ExplicitInterfaceImplementations[0].DeclaringTypeReference,
        //            a.Name, 0, GetParameterTypes(a.Parameters)
        //        )));
        //    }
        //    a.ApplyInterningProvider(interningProvider);
        //    return a;
        //}
        //public MethodOrOperator ConvertAccessor(AddAccessorDeclaration accessor, IUnresolvedMember p, bool memberIsExtern)
        //{
        //    if (accessor == null)
        //        return null;
        //    var a = new MethodOrOperator(currentTypeDefinition, "add_" + p.Name);
        //    a.SymbolKind = SymbolKind.Accessor;
        //    a.AccessorOwner = p;
        //    a.Accessibility = GetAccessibility(accessor._opt_modifiers._Modifiers) ?? p.Accessibility;
        //    a.IsAbstract = p.IsAbstract;
        //    a.IsOverride = p.IsOverride;
        //    a.IsSealed = p.IsSealed;
        //    a.IsStatic = p.IsStatic;
        //    a.IsSynthetic = p.IsSynthetic;
        //    a.IsVirtual = p.IsVirtual;

        //    a.Region = MakeRegion(accessor);
        //    a.BodyRegion = MakeRegion(accessor._block_or_semicolon);
        //    // An accessor has no body if both are true:
        //    //  a) there's no body in the code
        //    //  b) the member is either abstract or extern
        //    a.HasBody = !(accessor._block_or_semicolon._block_statement == null && (p.IsAbstract || memberIsExtern));
        //    if (p.SymbolKind == SymbolKind.Indexer)
        //    {
        //        foreach (var indexerParam in ((IUnresolvedProperty)p).Parameters)
        //            a.Parameters.Addition(indexerParam);
        //    }
        //    Parameter param = null;

        //    param = new Parameter(p.ReturnType, "value");
        //    a.Parameters.Addition(param);
        //    a.ReturnType = KnownTypeReference.Void;

        //    ConvertAttributes(a.ReturnTypeAttributes, accessor._opt_attributes._ReturnAttributes);
        //    ConvertAttributes(a.Attributes, accessor._opt_attributes._Attributes);

        //    if (p.IsExplicitInterfaceImplementation)
        //    {
        //        a.IsExplicitInterfaceImplementation = true;
        //        Debug.Assert(p.ExplicitInterfaceImplementations.Count == 1);
        //        a.ExplicitInterfaceImplementations.Addition(interningProvider.Intern(new MemberReferenceSpec(
        //            SymbolKind.Accessor,
        //            p.ExplicitInterfaceImplementations[0].DeclaringTypeReference,
        //            a.Name, 0, GetParameterTypes(a.Parameters)
        //        )));
        //    }
        //    a.ApplyInterningProvider(interningProvider);
        //    return a;
        //}
        //public MethodOrOperator ConvertAccessor(RemoveAccessorDeclaration accessor, IUnresolvedMember p, bool memberIsExtern)
        //{
        //    if (accessor == null)
        //        return null;
        //    var a = new MethodOrOperator(currentTypeDefinition, "remove_" + p.Name);
        //    a.SymbolKind = SymbolKind.Accessor;
        //    a.AccessorOwner = p;
        //    a.Accessibility = GetAccessibility(accessor._opt_modifiers._Modifiers) ?? p.Accessibility;
        //    a.IsAbstract = p.IsAbstract;
        //    a.IsOverride = p.IsOverride;
        //    a.IsSealed = p.IsSealed;
        //    a.IsStatic = p.IsStatic;
        //    a.IsSynthetic = p.IsSynthetic;
        //    a.IsVirtual = p.IsVirtual;

        //    a.Region = MakeRegion(accessor);
        //    a.BodyRegion = MakeRegion(accessor._block_or_semicolon);
        //    // An accessor has no body if both are true:
        //    //  a) there's no body in the code
        //    //  b) the member is either abstract or extern
        //    a.HasBody = !(accessor._block_or_semicolon._block_statement == null && (p.IsAbstract || memberIsExtern));
        //    if (p.SymbolKind == SymbolKind.Indexer)
        //    {
        //        foreach (var indexerParam in ((IUnresolvedProperty)p).Parameters)
        //            a.Parameters.Addition(indexerParam);
        //    }
        //    Parameter param = null;

        //    param = new Parameter(p.ReturnType, "value");
        //    a.Parameters.Addition(param);
        //    a.ReturnType = KnownTypeReference.Void;


        //    ConvertAttributes(a.ReturnTypeAttributes, accessor._opt_attributes._ReturnAttributes);
        //    ConvertAttributes(a.Attributes, accessor._opt_attributes._Attributes);

        //    if (p.IsExplicitInterfaceImplementation)
        //    {
        //        a.IsExplicitInterfaceImplementation = true;
        //        Debug.Assert(p.ExplicitInterfaceImplementations.Count == 1);
        //        a.ExplicitInterfaceImplementations.Addition(interningProvider.Intern(new MemberReferenceSpec(
        //            SymbolKind.Accessor,
        //            p.ExplicitInterfaceImplementations[0].DeclaringTypeReference,
        //            a.Name, 0, GetParameterTypes(a.Parameters)
        //        )));
        //    }
        //    a.ApplyInterningProvider(interningProvider);
        //    return a;
        //}
        //public MethodOrOperator ConvertAccessor(GetAccessorDeclaration accessor, IUnresolvedMember p, bool memberIsExtern)
        //{
        //    if (accessor == null)
        //        return null;
        //    var a = new MethodOrOperator(currentTypeDefinition, "get_" + p.Name);
        //    a.SymbolKind = SymbolKind.Accessor;
        //    a.AccessorOwner = p;
        //    a.Accessibility = GetAccessibility(accessor._opt_modifiers._Modifiers) ?? p.Accessibility;
        //    a.IsAbstract = p.IsAbstract;
        //    a.IsOverride = p.IsOverride;
        //    a.IsSealed = p.IsSealed;
        //    a.IsStatic = p.IsStatic;
        //    a.IsSynthetic = p.IsSynthetic;
        //    a.IsVirtual = p.IsVirtual;

        //    a.Region = MakeRegion(accessor);
        //    a.BodyRegion = MakeRegion(accessor._block_or_semicolon);
        //    // An accessor has no body if both are true:
        //    //  a) there's no body in the code
        //    //  b) the member is either abstract or extern
        //    a.HasBody = !(accessor._block_or_semicolon._block_statement == null && (p.IsAbstract || memberIsExtern));
        //    if (p.SymbolKind == SymbolKind.Indexer)
        //    {
        //        foreach (var indexerParam in ((IUnresolvedProperty)p).Parameters)
        //            a.Parameters.Addition(indexerParam);
        //    }

        //    a.ReturnType = p.ReturnType;



        //    ConvertAttributes(a.ReturnTypeAttributes, accessor._opt_attributes._ReturnAttributes);
        //    ConvertAttributes(a.Attributes, accessor._opt_attributes._Attributes);

        //    if (p.IsExplicitInterfaceImplementation)
        //    {
        //        a.IsExplicitInterfaceImplementation = true;
        //        Debug.Assert(p.ExplicitInterfaceImplementations.Count == 1);
        //        a.ExplicitInterfaceImplementations.Addition(interningProvider.Intern(new MemberReferenceSpec(
        //            SymbolKind.Accessor,
        //            p.ExplicitInterfaceImplementations[0].DeclaringTypeReference,
        //            a.Name, 0, GetParameterTypes(a.Parameters)
        //        )));
        //    }
        //    a.ApplyInterningProvider(interningProvider);
        //    return a;
        //}
        //public MethodOrOperator ConvertAccessor(SetAccessorDeclaration accessor, IUnresolvedMember p, bool memberIsExtern)
        //{
        //    if (accessor == null)
        //        return null;
        //    var a = new MethodOrOperator(currentTypeDefinition, "set_" + p.Name);
        //    a.SymbolKind = SymbolKind.Accessor;
        //    a.AccessorOwner = p;
        //    a.Accessibility = GetAccessibility(accessor._opt_modifiers._Modifiers) ?? p.Accessibility;
        //    a.IsAbstract = p.IsAbstract;
        //    a.IsOverride = p.IsOverride;
        //    a.IsSealed = p.IsSealed;
        //    a.IsStatic = p.IsStatic;
        //    a.IsSynthetic = p.IsSynthetic;
        //    a.IsVirtual = p.IsVirtual;

        //    a.Region = MakeRegion(accessor);
        //    a.BodyRegion = MakeRegion(accessor._block_or_semicolon);
        //    // An accessor has no body if both are true:
        //    //  a) there's no body in the code
        //    //  b) the member is either abstract or extern
        //    a.HasBody = !(accessor._block_or_semicolon._block_statement == null && (p.IsAbstract || memberIsExtern));
        //    if (p.SymbolKind == SymbolKind.Indexer)
        //    {
        //        foreach (var indexerParam in ((IUnresolvedProperty)p).Parameters)
        //            a.Parameters.Addition(indexerParam);
        //    }
        //    Parameter param = null;

        //    param = new Parameter(p.ReturnType, "value");
        //    a.Parameters.Addition(param);
        //    a.ReturnType = KnownTypeReference.Void;

        //    ConvertAttributes(a.ReturnTypeAttributes, accessor._opt_attributes._ReturnAttributes);
        //    ConvertAttributes(a.Attributes, accessor._opt_attributes._Attributes);

        //    if (p.IsExplicitInterfaceImplementation)
        //    {
        //        a.IsExplicitInterfaceImplementation = true;
        //        Debug.Assert(p.ExplicitInterfaceImplementations.Count == 1);
        //        a.ExplicitInterfaceImplementations.Addition(interningProvider.Intern(new MemberReferenceSpec(
        //            SymbolKind.Accessor,
        //            p.ExplicitInterfaceImplementations[0].DeclaringTypeReference,
        //            a.Name, 0, GetParameterTypes(a.Parameters)
        //        )));
        //    }
        //    a.ApplyInterningProvider(interningProvider);
        //    return a;
        //}

        //public MethodOrOperator CreateDefaultEventAccessor(IUnresolvedEvent ev, string name, IUnresolvedParameter valueParameter)
        //{
        //    var a = new MethodOrOperator(currentTypeDefinition, name);
        //    a.SymbolKind = SymbolKind.Accessor;
        //    a.AccessorOwner = ev;
        //    a.Region = ev.BodyRegion;
        //    a.BodyRegion = DomRegion.Empty;
        //    a.Accessibility = ev.Accessibility;
        //    a.IsAbstract = ev.IsAbstract;
        //    a.IsOverride = ev.IsOverride;
        //    a.IsSealed = ev.IsSealed;
        //    a.IsStatic = ev.IsStatic;
        //    a.IsSynthetic = ev.IsSynthetic;
        //    a.IsVirtual = ev.IsVirtual;
        //    a.HasBody = true; // even if it's compiler-generated; the body still exists
        //    a.ReturnType = KnownTypeReference.Void;
        //    a.Parameters.Addition(valueParameter);
        //    return a;
        //}
        //#endregion

        //static IUnresolvedParameter MakeParameter(ITypeReference type, string name)
        //{
        //    Parameter p = new Parameter(type, name);
        //    p.Freeze();
        //    return p;
        //}
        //static readonly IUnresolvedParameter delegateObjectParameter = MakeParameter(KnownTypeReference.Object, "object");
        //static readonly IUnresolvedParameter delegateIntPtrMethodParameter = MakeParameter(KnownTypeReference.IntPtr, "method");

        ///// <summary>
        ///// Adds the 'Invoke' method, and a constructor, to the <paramref name="delegateType"/>.
        ///// </summary>
        //public void AddDefaultMethodsToDelegate(UnresolvedTypeDefinitionSpec delegateType, ITypeReference returnType, IEnumerable<IUnresolvedParameter> parameters)
        //{
        //    if (delegateType == null)
        //        throw new ArgumentNullException("delegateType");
        //    if (returnType == null)
        //        throw new ArgumentNullException("returnType");
        //    if (parameters == null)
        //        throw new ArgumentNullException("parameters");

        //    DomRegion region = delegateType.Region;
        //    region = new DomRegion(region.FileName, region.BeginLine, region.BeginColumn); // remove end position

        //    MethodOrOperator invoke = new MethodOrOperator(delegateType, "Invoke");
        //    invoke.Accessibility = Accessibility.Public;
        //    invoke.IsSynthetic = true;
        //    foreach (var p in parameters)
        //        invoke.Parameters.Addition(p);
        //    invoke.ReturnType = returnType;
        //    invoke.Region = region;
        //    delegateType.Members.Addition(invoke);

        //    MethodOrOperator ctor = new MethodOrOperator(delegateType, ".ctor");
        //    ctor.SymbolKind = SymbolKind.Constructor;
        //    ctor.Accessibility = Accessibility.Public;
        //    ctor.IsSynthetic = true;
        //    ctor.Parameters.Addition(delegateObjectParameter);
        //    ctor.Parameters.Addition(delegateIntPtrMethodParameter);
        //    ctor.ReturnType = delegateType;
        //    ctor.Region = region;
        //    delegateType.Members.Addition(ctor);
        //}

        //public IList<ITypeReference> GetParameterTypes(IList<IUnresolvedParameter> parameters)
        //{
        //    if (parameters.Count == 0)
        //        return EmptyList<ITypeReference>.Instance;
        //    ITypeReference[] types = new ITypeReference[parameters.Count];
        //    for (int i = 0; i < types.Length; i++)
        //        types[i] = parameters[i].Type;

        //    return interningProvider.InternList(types);
        //}
        //public bool InheritsConstraints(MethodOrOperator methodDeclaration)
        //{
        //    // overrides and explicit interface implementations inherit constraints
        //    if ((methodDeclaration._Modifiers & VSC.TypeSystem.Modifiers.OVERRIDE) == VSC.TypeSystem.Modifiers.OVERRIDE)
        //        return true;


        //    return methodDeclaration._method_header._method_declaration_name._explicit_interface != null;
        //}

        //public DomRegion MakeRegion(Declaration node)
        //{
        //    return MakeRegion(node as Semantic);
        //}
        //public DomRegion MakeRegion(MethodBodyExpressionBlock node)
        //{
        //    if (node._block_or_semicolon != null)
        //        return MakeRegion(node._block_or_semicolon);
        //    else return MakeRegion(node._expression_block.start, node._expression_block.end);
        //}
        //public DomRegion MakeRegion(BlockOrSemicolon node)
        //{
        //    if (node._block_statement != null)
        //        return MakeRegion(node._block_statement);
        //    else return MakeRegion(node as Semantic);
        //}
        //public DomRegion MakeRegion(BlockStatement node)
        //{
        //    return MakeRegion(node.start, node.end);
        //}
        //public DomRegion MakeRegion(IndexerBody node)
        //{
        //    return MakeRegion(node.start, node.end);
        //}
        //public DomRegion MakeRegion(Semantic node)
        //{
        //    if (node == null)
        //        return DomRegion.Empty;
        //    else
        //        return MakeRegion(node.Location, Location.Null);
        //}
        //public DomRegion MakeRegion(Semantic node, Semantic end)
        //{
        //    if (node == null)
        //        return DomRegion.Empty;
        //    else
        //        return MakeRegion(node.Location, end.Location);
        //}
        //public DomRegion MakeRegion(Semantic node, MethodBodyExpressionBlock end)
        //{
        //    DomRegion reg = MakeRegion(end);
        //    if (node == null)
        //        return DomRegion.Empty;
        //    else
        //        return MakeRegion(node.Location, reg.End);
        //}
        //public DomRegion MakeRegion(Semantic node, BlockOrSemicolon end)
        //{
        //    DomRegion reg = MakeRegion(end);
        //    if (node == null)
        //        return DomRegion.Empty;
        //    else
        //        return MakeRegion(node.Location, reg.End);
        //}
  

    }
}
