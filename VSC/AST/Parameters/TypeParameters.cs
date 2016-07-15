using System.Collections.Generic;
using System.Linq;
using System.Text;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;

namespace VSC.AST
{
    public class TypeParameters : IList<IUnresolvedTypeParameter>
    {
        public List<UnresolvedTypeParameterSpec> names;
        TypeParameterSpec[] types;

        public TypeParameters()
        {
            names = new List<UnresolvedTypeParameterSpec>();
        }

        public TypeParameters(int count)
        {
            names = new List<UnresolvedTypeParameterSpec>(count);
        }

        #region Properties

        public int Count
        {
            get
            {
                return names.Count;
            }
        }

        public TypeParameterSpec[] Types
        {
            get
            {
                return types;
            }
        }

        #endregion

        public void Add(UnresolvedTypeParameterSpec tparam)
        {
            names.Add(tparam);
        }

        public void Add(TypeParameters tparams)
        {
            names.AddRange(tparams.names);
        }



      

        public UnresolvedTypeParameterSpec Find(string name)
        {
            foreach (var tp in names)
            {
                if (tp.Name == name)
                    return tp;
            }

            return null;
        }

        public string[] GetAllNames()
        {
            return names.Select(l => l.Name).ToArray();
        }

        public string GetSignatureForError()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Count; ++i)
            {
                if (i > 0)
                    sb.Append(',');

                var name = names[i];
                if (name != null)
                    sb.Append(name.Name);
            }

            return sb.ToString();
        }
        public int IndexOf(IUnresolvedTypeParameter item)
        {
            return names.IndexOf((UnresolvedTypeParameterSpec)item);
        }

        public void Insert(int index, IUnresolvedTypeParameter item)
        {
            names.Insert(index, (UnresolvedTypeParameterSpec)item);
        }

        public void RemoveAt(int index)
        {
            names.RemoveAt(index);
        }

        public IUnresolvedTypeParameter this[int index]
        {
            get { return names[index]; }
            set { names[index] = (UnresolvedTypeParameterSpec)value; }
        }

        public void Add(IUnresolvedTypeParameter item)
        {
            names.Add((UnresolvedTypeParameterSpec)item);
        }

        public void Clear()
        {
            names.Clear();
        }

        public bool Contains(IUnresolvedTypeParameter item)
        {
            return names.Contains((UnresolvedTypeParameterSpec)item);
        }

        public void CopyTo(IUnresolvedTypeParameter[] array, int arrayIndex)
        {
            names.Cast<IUnresolvedTypeParameter>().ToList().CopyTo(array, arrayIndex);
        }
        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(IUnresolvedTypeParameter item)
        {
            return names.Remove((UnresolvedTypeParameterSpec)item);
        }

        public IEnumerator<IUnresolvedTypeParameter> GetEnumerator()
        {
            return names.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return names.GetEnumerator();
        }

    }
}