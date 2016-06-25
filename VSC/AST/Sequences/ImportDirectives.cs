using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class ImportDirectives : Sequence<ImportDirective> {

			[Rule("<import directives> ::= <import directives> <import directive>")]
			public ImportDirectives(ImportDirectives _ImportDirectives,ImportDirective _ImportDirective) : base(_ImportDirective,_ImportDirectives)
				{
				}
			[Rule("<import directives> ::= ")]
			public ImportDirectives() : base(null)
				{
				}
}
}
