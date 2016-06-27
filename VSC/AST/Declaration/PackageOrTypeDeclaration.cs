using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class PackageOrTypeDeclaration : Declaration {
 			public PackageDeclaration _package_declaration;
			public TypeDeclaration _type_declaration;
			public GlobalMemberDeclaration _global_member_declaration;

			[Rule("<package or type declaration> ::= <package declaration>")]
			public PackageOrTypeDeclaration(PackageDeclaration _PackageDeclaration)
				{
				_package_declaration = _PackageDeclaration;
				}
			[Rule("<package or type declaration> ::= <type declaration>")]
			public PackageOrTypeDeclaration(TypeDeclaration _TypeDeclaration)
				{
				_type_declaration = _TypeDeclaration;
				}
			[Rule("<package or type declaration> ::= <global member declaration>")]
			public PackageOrTypeDeclaration(GlobalMemberDeclaration _GlobalMemberDeclaration)
				{
				_global_member_declaration = _GlobalMemberDeclaration;
				}

            public override bool Resolve(Context.SymbolResolveContext rc)
            {
                return rc.ResolveOne(_global_member_declaration, _type_declaration, _package_declaration);
            }
}
}
