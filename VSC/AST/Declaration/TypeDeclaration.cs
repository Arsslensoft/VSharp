using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class TypeDeclaration : Declaration {
 			public ClassDeclaration _class_declaration;
			public StructDeclaration _struct_declaration;
			public UnionDeclaration _union_declaration;
			public InterfaceDeclaration _interface_declaration;
			public EnumDeclaration _enum_declaration;
			public DelegateDeclaration _delegate_declaration;

     
			[Rule("<type declaration> ::= <class declaration>")]
			public TypeDeclaration(ClassDeclaration _ClassDeclaration)
				{
				_class_declaration = _ClassDeclaration;
				}
			[Rule("<type declaration> ::= <struct declaration>")]
			public TypeDeclaration(StructDeclaration _StructDeclaration)
				{
				_struct_declaration = _StructDeclaration;
				}
			[Rule("<type declaration> ::= <union declaration>")]
			public TypeDeclaration(UnionDeclaration _UnionDeclaration)
				{
				_union_declaration = _UnionDeclaration;
				}
			[Rule("<type declaration> ::= <interface declaration>")]
			public TypeDeclaration(InterfaceDeclaration _InterfaceDeclaration)
				{
				_interface_declaration = _InterfaceDeclaration;
				}
			[Rule("<type declaration> ::= <enum declaration>")]
			public TypeDeclaration(EnumDeclaration _EnumDeclaration)
				{
				_enum_declaration = _EnumDeclaration;
				}
			[Rule("<type declaration> ::= <delegate declaration>")]
			public TypeDeclaration(DelegateDeclaration _DelegateDeclaration)
				{
				_delegate_declaration = _DelegateDeclaration;
				}

            public override bool Resolve(Context.SymbolResolveContext rc)
            {
                return rc.ResolveOne(_class_declaration, _struct_declaration, _union_declaration, _interface_declaration, _enum_declaration, _delegate_declaration);
            }
}
}
