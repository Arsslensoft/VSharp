using System;
using VSC.Base.GoldParser.Semantic;
using VSC.TypeSystem.Implementation;
namespace VSC.AST { 
	public class ConstantDeclaration : Declaration {
        public OptAttributes _opt_attributes;
        public OptModifiers _opt_modifiers;
        public Type _type;
        public Identifier _identifier;
        public VariableInitializer _variable_initializer;
        public OptConstantDeclarators _opt_constant_declarators;

        [Rule("<constant declaration> ::= <opt attributes> <opt modifiers> const <type> <Identifier> '=' <variable initializer> <opt constant declarators> ';'")]
        public ConstantDeclaration(OptAttributes _OptAttributes, OptModifiers _OptModifiers, Semantic _symbol83, Type _Type, Identifier _Identifier, Semantic _symbol60, VariableInitializer _VariableInitializer, OptConstantDeclarators _OptConstantDeclarators, Semantic _symbol31)
        {
            _opt_attributes = _OptAttributes;
            _opt_modifiers = _OptModifiers;
            _type = _Type;
            _identifier = _Identifier;
            _variable_initializer = _VariableInitializer;
            _opt_constant_declarators = _OptConstantDeclarators;
        }
            public override bool Resolve(Context.SymbolResolveContext rc)
            {

                bool isSingleField = _opt_constant_declarators._constant_declarators == null;
                VSC.TypeSystem.Modifiers modifiers = _opt_modifiers._Modifiers;
                UnresolvedFieldSpec field = null;
                // first one
                field = new UnresolvedFieldSpec(rc.currentTypeDefinition, _identifier._Identifier);

                field.Region = rc.MakeRegion(_identifier);
                field.BodyRegion = rc.MakeRegion(_variable_initializer);
                // attributes
                rc.ConvertAttributes(field.Attributes, _opt_attributes._Attributes);
                // modifiers
                rc.ApplyModifiers(field, modifiers);

             

                field.ReturnType = rc.ConvertTypeReference(_type, TypeSystem.Resolver.NameLookupMode.Type);

                if (_variable_initializer._expression != null)
                {
                    field.ConstantValue = rc.ConvertConstantValue(field.ReturnType, _variable_initializer._expression);
                    field.IsStatic = true;
                }
          
                rc.currentTypeDefinition.Members.Add(field);
                field.ApplyInterningProvider(rc.interningProvider);

                // other fields
                if (!isSingleField)
                    foreach (var vi in  _opt_constant_declarators._constant_declarators )
                    {
                        field = new UnresolvedFieldSpec(rc.currentTypeDefinition, vi.Name);

                        field.Region = rc.MakeRegion(vi);
                        field.BodyRegion = rc.MakeRegion(vi._variable_initializer);
                        // attributes
                        rc.ConvertAttributes(field.Attributes, _opt_attributes._Attributes);
                        // modifiers
                        rc.ApplyModifiers(field, modifiers);


                        field.ReturnType = rc.ConvertTypeReference(_type, TypeSystem.Resolver.NameLookupMode.Type);


                        if (vi._variable_initializer._expression != null)
                        {
                            field.ConstantValue = rc.ConvertConstantValue(field.ReturnType, vi._variable_initializer._expression);
                            field.IsStatic = true;
                        }
         

                        rc.currentTypeDefinition.Members.Add(field);
                        field.ApplyInterningProvider(rc.interningProvider);
                    }
                return true;
            }
}
}
