using System;
using VSC.Base.GoldParser.Semantic;
using VSC.TypeSystem.Implementation;
namespace VSC.AST {
    public class FieldDeclaration : Declaration
    {
 			public OptAttributes _opt_attributes;
			public OptModifiers _opt_modifiers;
			public MemberType _member_type;
			public Identifier _identifier;
			public OptFieldInitializer _opt_field_initializer;
			public OptFieldDeclarators _opt_field_declarators;

			[Rule("<field declaration> ::= <opt attributes> <opt modifiers> <member type> <Identifier> <opt field initializer> <opt field declarators> ';'")]
			public FieldDeclaration(OptAttributes _OptAttributes,OptModifiers _OptModifiers,MemberType _MemberType,Identifier _Identifier,OptFieldInitializer _OptFieldInitializer,OptFieldDeclarators _OptFieldDeclarators, Semantic _symbol31)
				{
				_opt_attributes = _OptAttributes;
				_opt_modifiers = _OptModifiers;
				_member_type = _MemberType;
				_identifier = _Identifier;
				_opt_field_initializer = _OptFieldInitializer;
				_opt_field_declarators = _OptFieldDeclarators;
				}

            public override bool Resolve(Context.SymbolResolveContext rc)
            {
                bool isSingleField = _opt_field_declarators._field_declarators == null;
                VSC.TypeSystem.Modifiers modifiers = _opt_modifiers._Modifiers;
                UnresolvedFieldSpec field = null;
                // first one
                field = new UnresolvedFieldSpec(rc.currentTypeDefinition, _identifier._Identifier);

                field.Region = rc.MakeRegion(_identifier);
                field.BodyRegion = rc.MakeRegion(_opt_field_initializer);
                // attributes
                rc.ConvertAttributes(field.Attributes,_opt_attributes._Attributes);
                // modifiers
                rc.ApplyModifiers(field, modifiers);

                field.IsReadOnly = (modifiers & VSC.TypeSystem.Modifiers.READONLY) != 0;

                field.ReturnType = rc.ConvertTypeReference(_member_type, TypeSystem.Resolver.NameLookupMode.Type);
                if (_opt_field_initializer._variable_initializer != null && _opt_field_initializer._variable_initializer._expression != null)
                    field.ConstantValue = rc.ConvertConstantValue(field.ReturnType, _opt_field_initializer._variable_initializer._expression);
     
                rc.currentTypeDefinition.Members.Add(field);
                field.ApplyInterningProvider(rc.interningProvider);

                // other fields
             if(!isSingleField)
                foreach (FieldDeclarator vi in _opt_field_declarators._field_declarators)
                {
                    field = new UnresolvedFieldSpec(rc.currentTypeDefinition, vi.Name);

                    field.Region =  rc.MakeRegion(vi);
                    field.BodyRegion = rc.MakeRegion(vi._variable_initializer);
                    // attributes
                    rc.ConvertAttributes(field.Attributes, _opt_attributes._Attributes);
                    // modifiers
                    rc.ApplyModifiers(field, modifiers);

           
                    field.IsReadOnly = (modifiers & VSC.TypeSystem.Modifiers.READONLY) != 0;
                    if (vi._variable_initializer._expression != null)
                        field.ConstantValue = rc.ConvertConstantValue(field.ReturnType,vi._variable_initializer._expression);
                
                    field.ReturnType = rc.ConvertTypeReference(_member_type, TypeSystem.Resolver.NameLookupMode.Type);


                    rc.currentTypeDefinition.Members.Add(field);
                    field.ApplyInterningProvider(rc.interningProvider);
                }
                return true;
            }
}
}
