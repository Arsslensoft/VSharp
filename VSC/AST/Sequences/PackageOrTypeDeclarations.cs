using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class PackageOrTypeDeclarations : Sequence<PackageOrTypeDeclaration> {

			[Rule("<package or type declarations> ::= <package or type declarations> <package or type declaration>")]
			public PackageOrTypeDeclarations(PackageOrTypeDeclarations _PackageOrTypeDeclarations,PackageOrTypeDeclaration _PackageOrTypeDeclaration) : base(_PackageOrTypeDeclaration,_PackageOrTypeDeclarations)
				{
				}
			[Rule("<package or type declarations> ::= ")]
			public PackageOrTypeDeclarations() : base(null)
				{
				}
}
}
