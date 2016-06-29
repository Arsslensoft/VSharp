using System;
using VSC.Base.GoldParser.Semantic;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
namespace VSC.AST {
    public class EventDeclaration : Declaration
    {
        public UnresolvedEventSpec UnresolvedEvent;
 			public OptAttributes _opt_attributes;
			public OptModifiers _opt_modifiers;
			public Type _type;
            public MethodDeclarationName _member_declaration_name;
			public OptEventInitializer _opt_event_initializer;
			public OptEventDeclarators _opt_event_declarators;
			public EventAccessorDeclarations _event_accessor_declarations;
            public OptDocumentation _opt_documentation;
            [Rule("<event declaration> ::= <Opt Documentation> <opt attributes> <opt modifiers> event <type> <method declaration name> <opt event initializer> <opt event declarators> ';'")]
            public EventDeclaration(OptDocumentation _OptDocumentation, OptAttributes _OptAttributes, OptModifiers _OptModifiers, Semantic _symbol93, Type _Type, MethodDeclarationName _MemberDeclarationName, OptEventInitializer _OptEventInitializer, OptEventDeclarators _OptEventDeclarators, Semantic _symbol31)
            {
                _opt_documentation = _OptDocumentation;
				_opt_attributes = _OptAttributes;
				_opt_modifiers = _OptModifiers;
				_type = _Type;
				_member_declaration_name = _MemberDeclarationName;
				_opt_event_initializer = _OptEventInitializer;
				_opt_event_declarators = _OptEventDeclarators;
				}
            [Rule("<event declaration> ::= <Opt Documentation> <opt attributes> <opt modifiers> event <type> <method declaration name> '{' <event accessor declarations> '}'")]
            public EventDeclaration(OptDocumentation _OptDocumentation, OptAttributes _OptAttributes, OptModifiers _OptModifiers, Semantic _symbol93, Type _Type, MethodDeclarationName _MemberDeclarationName, Semantic _symbol43, EventAccessorDeclarations _EventAccessorDeclarations, Semantic _symbol47)
            {
                _opt_documentation = _OptDocumentation;
				_opt_attributes = _OptAttributes;
				_opt_modifiers = _OptModifiers;
				_type = _Type;
				_member_declaration_name = _MemberDeclarationName;
				_event_accessor_declarations = _EventAccessorDeclarations;
				}

            public override bool Resolve(Context.SymbolResolveContext rc)
            {
                string name = "";
                if (_member_declaration_name._type_declaration_name != null)
                    name = _member_declaration_name._type_declaration_name._identifier._Identifier;
                else
                    name = _member_declaration_name._identifier._Identifier;


                UnresolvedEventSpec e = new UnresolvedEventSpec(rc.currentTypeDefinition, name);
                e.Region = rc.MakeRegion(this);
               // e.BodyRegion = MakeBraceRegion(eventDeclaration);
                rc.ApplyModifiers(e, _opt_modifiers._Modifiers);
                e.ReturnType = rc.ConvertTypeReference(_type, TypeSystem.Resolver.NameLookupMode.Type);
                rc.ConvertAttributes(e.Attributes,_opt_attributes._Attributes);
                // documentation
                rc.AddDocumentation(e, _opt_documentation);

                if (_member_declaration_name._explicit_interface != null)
                {
                    e.Accessibility = Accessibility.None;
                    e.IsExplicitInterfaceImplementation = true;
                    e.ExplicitInterfaceImplementations.Add(rc.interningProvider.Intern(new MemberReferenceSpec(
                        e.SymbolKind, rc.ConvertTypeReference(_member_declaration_name._explicit_interface ,  TypeSystem.Resolver.NameLookupMode.Type), e.Name)));
                }

                // custom events can't be extern; the non-custom event syntax must be used for extern events
                if(_event_accessor_declarations._add_accessor_declaration != null)
                    e.AddAccessor = rc.ConvertAccessor(_event_accessor_declarations._add_accessor_declaration, e, false);

                if(_event_accessor_declarations._remove_accessor_declaration != null)
                    e.RemoveAccessor = rc.ConvertAccessor(_event_accessor_declarations._remove_accessor_declaration, e, false);

                if (_event_accessor_declarations._raise_accessor_declaration_opt._raise_accessor_declaration != null)
                    e.InvokeAccessor = rc.ConvertAccessor(_event_accessor_declarations._raise_accessor_declaration_opt._raise_accessor_declaration, e, false);


                rc.currentTypeDefinition.Members.Add(e);
                e.ApplyInterningProvider(rc.interningProvider);
                UnresolvedEvent = e;
                return true;
            }
}
}
