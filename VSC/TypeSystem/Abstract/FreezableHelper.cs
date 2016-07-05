using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VSC.Base;

namespace VSC.TypeSystem.Implementation
{
    public static class FreezableHelper
    {
        public static void ThrowIfFrozen(IFreezable freezable)
        {
            if (freezable.IsFrozen)
                throw new InvalidOperationException("Cannot mutate frozen " + freezable.GetType().Name);
        }
		
        public static IList<T> FreezeListAndElements<T>(IList<T> list)
        {
            if (list != null) {
                foreach (T item in list)
                    Freeze(item);
            }
            return FreezeList(list);
        }
		
        public static IList<T> FreezeList<T>(IList<T> list)
        {
            if (list == null || list.Count == 0)
                return EmptyList<T>.Instance;
            if (list.IsReadOnly) {
                // If the list is already read-only, return it directly.
                // This is important, otherwise we might undo the effects of interning.
                return list;
            } else {
                return new ReadOnlyCollection<T>(list.ToArray());
            }
        }
		
        public static void Freeze(object item)
        {
            IFreezable f = item as IFreezable;
            if (f != null)
                f.Freeze();
        }
		
        public static T FreezeAndReturn<T>(T item) where T : IFreezable
        {
            item.Freeze();
            return item;
        }
		
        /// <summary>
        /// If the item is not frozen, this method creates and returns a frozen clone.
        /// If the item is already frozen, it is returned without creating a clone.
        /// </summary>
        public static T GetFrozenClone<T>(T item) where T : IFreezable, ICloneable
        {
            if (!item.IsFrozen) {
                item = (T)item.Clone();
                item.Freeze();
            }
            return item;
        }
    }
}