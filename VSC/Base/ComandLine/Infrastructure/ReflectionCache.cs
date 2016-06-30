using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;


namespace VSC.Base.CommandLine.Infrastructure
{
    internal sealed class ReflectionCache
    {
        private static readonly ReflectionCache Singleton;
        private readonly IDictionary<Pair<Type, object>, WeakReference> _cache;

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Singleton, by design")]
        static ReflectionCache()
        {
            Singleton = new ReflectionCache();
        }

        private ReflectionCache()
        {
            _cache = new Dictionary<Pair<Type, object>, WeakReference>();
        }

        public static ReflectionCache Instance
        {
            get { return Singleton; }
        }

        public object this[Pair<Type, object> key]
        {
            get
            {
                if (key == null)
                {
                    throw new ArgumentNullException("key");
                }

                return _cache.ContainsKey(key) ? _cache[key].Target : null;
            }

            set
            {
                if (key == null)
                {
                    throw new ArgumentNullException("key");
                }

                _cache[key] = new WeakReference(value);
            }
        }
    }
}
