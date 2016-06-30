using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSC.AST
{
    public class Sequence<T> : Semantic, IEnumerable<T> where T : Semantic
    {
        private readonly T item;
        private readonly Sequence<T> next;





        public Sequence(T item)
            : this(item, null)
        {
        }



        public Sequence(T item, Sequence<T> next)
        {
            this.item = item;
            this.next = next;
        }

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            for (Sequence<T> sequence = this; sequence != null; sequence = sequence.next)
            {
                if (sequence.item != null)
                {
                    yield return sequence.item;
                }
            }
        }

         IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion


        public static implicit operator List<T>(Sequence<T> seq)
        {
            List<T> l = new List<T>();
            foreach (T el in seq)
                l.Add(el);

            return l;
        }
    }
}
