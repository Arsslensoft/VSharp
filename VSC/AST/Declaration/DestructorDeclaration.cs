using System;
using VSC.Base.GoldParser.Semantic;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
namespace VSC.AST { 
	public class DestructorDeclaration : Declaration {
 			public OptAttributes _opt_attributes;
			public OptModifiers _opt_modifiers;
			public BlockOrSemicolon _block_or_semicolon;

			[Rule("<destructor declaration> ::= <opt attributes> <opt modifiers> '~' self '(' ')' <block or semicolon>")]
			public DestructorDeclaration(OptAttributes _OptAttributes,OptModifiers _OptModifiers, Semantic _symbol48, Semantic _symbol138, Semantic _symbol20, Semantic _symbol21,BlockOrSemicolon _BlockOrSemicolon)
				{
				_opt_attributes = _OptAttributes;
				_opt_modifiers = _OptModifiers;
				_block_or_semicolon = _BlockOrSemicolon;
				}
            public override bool Resolve(Context.SymbolResolveContext rc)
            {
                UnresolvedMethodSpec dtor = new UnresolvedMethodSpec(rc.currentTypeDefinition, "Finalize");
                dtor.SymbolKind = SymbolKind.Destructor;
                dtor.Region = rc.MakeRegion(this);
                dtor.BodyRegion = rc.MakeRegion(_block_or_semicolon);
                dtor.Accessibility = Accessibility.Protected;
                dtor.IsOverride = true;
                dtor.ReturnType = KnownTypeReference.Void;
                dtor.HasBody = _block_or_semicolon._block_statement != null;

                rc.ConvertAttributes(dtor.Attributes, _opt_attributes._Attributes);
             
                rc.currentTypeDefinition.Members.Add(dtor);
                dtor.ApplyInterningProvider(rc.interningProvider);
                return true;
            }
}
}
