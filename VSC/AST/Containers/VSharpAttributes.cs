using System.Collections.Generic;
using System.Linq;
using VSC.TypeSystem;

namespace VSC.AST
{
    public class VSharpAttributes : IList<IUnresolvedAttribute>
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

        public int IndexOf(IUnresolvedAttribute item)
        {
            return Attrs.IndexOf((VSharpAttribute)item);
        }

        public void Insert(int index, IUnresolvedAttribute item)
        {
            Attrs.Insert(index,(VSharpAttribute)item);
        }

        public void RemoveAt(int index)
        {
           Attrs.RemoveAt(index);
        }

        public IUnresolvedAttribute this[int index]
        {
            get { return Attrs[index]; }
            set { Attrs[index] = (VSharpAttribute)value; }
        }

        public void Add(IUnresolvedAttribute item)
        {
            Attrs.Add((VSharpAttribute)item);
        }

        public void Clear()
        {
            Attrs.Clear();
        }

        public bool Contains(IUnresolvedAttribute item)
        {
            return Attrs.Contains((VSharpAttribute)item);
        }

        public void CopyTo(IUnresolvedAttribute[] array, int arrayIndex)
        {
          Attrs.Cast<IUnresolvedAttribute>().ToList().CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return Attrs.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(IUnresolvedAttribute item)
        {
            return Attrs.Remove((VSharpAttribute) item);
        }

        public IEnumerator<IUnresolvedAttribute> GetEnumerator()
        {
            return Attrs.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Attrs.GetEnumerator();
        }
    }
}