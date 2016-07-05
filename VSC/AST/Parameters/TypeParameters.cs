using System.Collections.Generic;
using System.Linq;
using System.Text;
using VSC.TypeSystem.Implementation;

namespace VSC.AST
{
    public class TypeParameters
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



        public UnresolvedTypeParameterSpec this[int index]
        {
            get
            {
                return names[index];
            }
            set
            {
                names[index] = value;
            }
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


    }
}