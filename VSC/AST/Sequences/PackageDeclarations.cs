using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class PackageDeclarations : Sequence<PackageDeclaration> {
 			[Rule("<package declarations> ::= ")]
			public PackageDeclarations() : base(null)
				{
				}
			[Rule("<package declarations> ::= <package declarations> <package declaration>")]
			public PackageDeclarations(PackageDeclarations _PackageDeclarations,PackageDeclaration _PackageDeclaration) : base(_PackageDeclaration,_PackageDeclarations)
				{
			
				}
}
}
