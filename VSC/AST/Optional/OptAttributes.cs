using System;
using System.Collections.Generic;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OptAttributes : Semantic {
 			public AttributeSections _attribute_sections;

			[Rule("<opt attributes> ::= ")]
			public OptAttributes()
				{
                    _ReturnAttributes = new List<Attribute>();
                    _Attributes = new List<Attribute>();
				}
			[Rule("<opt attributes> ::= <attribute sections>")]
			public OptAttributes(AttributeSections _AttributeSections)
				{
				_attribute_sections = _AttributeSections;
                _ReturnAttributes = new List<Attribute>();
                _Attributes = new List<Attribute>();

                foreach(var ase in _AttributeSections)
                    if (ase._opt_attribute_target._IsReturn)
                    {
                        foreach (var at in ase._attribute_list)
                            _ReturnAttributes.Add(at);
                    }
                    else
                    {
                        foreach (var at in ase._attribute_list)
                            _Attributes.Add(at);
                    }

				}
            public List<Attribute> _ReturnAttributes;
            public List<Attribute> _Attributes;
}
}
