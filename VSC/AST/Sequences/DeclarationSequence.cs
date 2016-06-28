using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class DeclaratonSequence : Sequence<Declaration> {

            [Rule("<enum member declarations> ::= <enum member declarations> ~',' <enum member declaration>")]
            [Rule("<class member declarations> ::= <class member declarations> <class member declaration>")]
        [Rule("<interface member declarations> ::= <interface member declarations> <interface member declaration>")]
			[Rule("<package or type declarations> ::= <package or type declarations> <package or type declaration>")]
        public DeclaratonSequence(DeclaratonSequence _PackageOrTypeDeclarations, Declaration _PackageOrTypeDeclaration)
            : base(_PackageOrTypeDeclaration, _PackageOrTypeDeclarations)
				{
				}
            [Rule("<enum member declarations> ::= <enum member declaration>")]
                [Rule("<class member declarations> ::= <class member declaration>")]
        [Rule("<interface member declarations> ::= <interface member declaration>")]
            public DeclaratonSequence(Declaration _PackageOrTypeDeclaration)
                : base(_PackageOrTypeDeclaration)
            {
            }
			[Rule("<package or type declarations> ::= ")]
			public DeclaratonSequence() : base(null)
				{
				}
}
}
