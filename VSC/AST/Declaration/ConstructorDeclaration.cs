using System;
using VSC.Base.GoldParser.Semantic;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
namespace VSC.AST {
    public class ConstructorDeclaration : Declaration
    {
        public UnresolvedMethodSpec UnresolvedMethod;
 			public ConstructorDeclarator _constructor_declarator;
			public BlockOrSemicolon _block_or_semicolon;
            public OptDocumentation _opt_documentation;
            [Rule("<constructor declaration> ::= <Opt Documentation> <constructor declarator> <block or semicolon>")]
            public ConstructorDeclaration(OptDocumentation _OptDocumentation, ConstructorDeclarator _ConstructorDeclarator, BlockOrSemicolon _BlockOrSemicolon)
            {
                _opt_documentation = _OptDocumentation;
				_constructor_declarator = _ConstructorDeclarator;
                _block_or_semicolon = _BlockOrSemicolon;
				}
          
        
        public override bool Resolve(Context.SymbolResolveContext rc)
            {
                VSC.TypeSystem.Modifiers modifiers = _constructor_declarator._opt_modifiers._Modifiers;
                bool isStatic = (modifiers & VSC.TypeSystem.Modifiers.STATIC) != 0;
                UnresolvedMethodSpec ctor = new UnresolvedMethodSpec(rc.currentTypeDefinition, isStatic ? ".cctor" : ".ctor");
                ctor.SymbolKind = SymbolKind.Constructor;
                ctor.Region = rc.MakeRegion(this, _block_or_semicolon);
                ctor.BodyRegion = rc.MakeRegion(_block_or_semicolon); 
            // documentation
                rc.AddDocumentation(ctor, _opt_documentation);
                // return
                ctor.ReturnType = KnownTypeReference.Void;

                // attributes
                rc.ConvertAttributes(ctor.Attributes, _constructor_declarator._opt_attributes._Attributes);
               
                // parameters
              if(_constructor_declarator._opt_formal_parameter_list._formal_parameter_list != null)
                rc.ConvertParameters(ctor.Parameters, _constructor_declarator._opt_formal_parameter_list._formal_parameter_list._fixed_parameters, _constructor_declarator._opt_formal_parameter_list._formal_parameter_list._parameter_array);
             
                // body
              ctor.HasBody = _block_or_semicolon._block_statement != null;
              

                // modifiers
                if (isStatic)
                    ctor.IsStatic = true;
                else
                    rc.ApplyModifiers(ctor, modifiers);

                // add to resolver
                rc.currentTypeDefinition.Members.Add(ctor);
                ctor.ApplyInterningProvider(rc.interningProvider);
                UnresolvedMethod = ctor;
                return true;
            }
}
}
