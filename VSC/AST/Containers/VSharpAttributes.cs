using System.Collections.Generic;

namespace VSC.AST
{
    public class VSharpAttributes
    {
        public readonly List<VSharpAttribute> Attrs;

        public VSharpAttributes(VSharpAttribute a)
        {
            Attrs = new List<VSharpAttribute>();
            Attrs.Add(a);
        }
       
        public VSharpAttributes(List<VSharpAttribute> attrs)
        {
            Attrs = attrs ?? new List<VSharpAttribute>();
        }

        public void AddAttribute(VSharpAttribute attr)
        {
            Attrs.Add(attr);
        }

        public void AddAttributes(List<VSharpAttribute> attrs)
        {
            Attrs.AddRange(attrs);
        }
    }
}