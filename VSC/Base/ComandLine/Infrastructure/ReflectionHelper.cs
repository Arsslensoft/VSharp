using System;
using System.Collections.Generic;
using System.Reflection;


namespace VSC.Base.CommandLine.Infrastructure
{
    internal static class ReflectionHelper
    {
        static ReflectionHelper()
        {
            AssemblyFromWhichToPullInformation = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        }

        /// <summary>
        /// Gets or sets the assembly from which to pull information. Setter provided for testing purpose.
        /// </summary>
        internal static Assembly AssemblyFromWhichToPullInformation
        {
            get; set;
        }

        public static IList<Pair<PropertyInfo, TAttribute>> RetrievePropertyList<TAttribute>(object target)
                where TAttribute : Attribute
        {
            var key = new Pair<Type, object>(typeof(Pair<PropertyInfo, TAttribute>), target);
            var cached = ReflectionCache.Instance[key];
            if (cached == null)
            {
                IList<Pair<PropertyInfo, TAttribute>> list = new List<Pair<PropertyInfo, TAttribute>>();
                if (target != null)
                {
                    var propertiesInfo = target.GetType().GetProperties();

                    foreach (var property in propertiesInfo)
                    {
                        if (property == null || (!property.CanRead || !property.CanWrite))
                        {
                            continue;
                        }

                        var setMethod = property.GetSetMethod();
                        if (setMethod == null || setMethod.IsStatic)
                        {
                            continue;
                        }

                        var attribute = Attribute.GetCustomAttribute(property, typeof(TAttribute), false);
                        if (attribute != null)
                        {
                            list.Add(new Pair<PropertyInfo, TAttribute>(property, (TAttribute)attribute));
                        }
                    }
                }

                ReflectionCache.Instance[key] = list;
                return list;
            }

            return (IList<Pair<PropertyInfo, TAttribute>>)cached;
        }

        public static Pair<MethodInfo, TAttribute> RetrieveMethod<TAttribute>(object target)
                where TAttribute : Attribute
        {
            var key = new Pair<Type, object>(typeof(Pair<MethodInfo, TAttribute>), target);
            var cached = ReflectionCache.Instance[key];
            if (cached == null)
            {
                var info = target.GetType().GetMethods();
                foreach (var method in info)
                {
                    if (method.IsStatic)
                    {
                        continue;
                    }

                    var attribute = Attribute.GetCustomAttribute(method, typeof(TAttribute), false);
                    if (attribute == null)
                    {
                        continue;
                    }

                    var data = new Pair<MethodInfo, TAttribute>(method, (TAttribute)attribute);
                    ReflectionCache.Instance[key] = data;
                    return data;
                }

                return null;
            }

            return (Pair<MethodInfo, TAttribute>)cached;
        }

        public static TAttribute RetrieveMethodAttributeOnly<TAttribute>(object target)
                where TAttribute : Attribute
        {
            var key = new Pair<Type, object>(typeof(TAttribute), target);
            var cached = ReflectionCache.Instance[key];
            if (cached == null)
            {
                var info = target.GetType().GetMethods();
                foreach (var method in info)
                {
                    if (method.IsStatic)
                    {
                        continue;
                    }

                    var attribute = Attribute.GetCustomAttribute(method, typeof(TAttribute), false);
                    if (attribute == null)
                    {
                        continue;
                    }

                    var data = (TAttribute)attribute;
                    ReflectionCache.Instance[key] = data;
                    return data;
                }

                return null;
            }

            return (TAttribute)cached;
        }

        public static IList<TAttribute> RetrievePropertyAttributeList<TAttribute>(object target)
                where TAttribute : Attribute
        {
            var key = new Pair<Type, object>(typeof(IList<TAttribute>), target);
            var cached = ReflectionCache.Instance[key];
            if (cached == null)
            {
                IList<TAttribute> list = new List<TAttribute>();
                var info = target.GetType().GetProperties();

                foreach (var property in info)
                {
                    if (property == null || (!property.CanRead || !property.CanWrite))
                    {
                        continue;
                    }

                    var setMethod = property.GetSetMethod();
                    if (setMethod == null || setMethod.IsStatic)
                    {
                        continue;
                    }

                    var attribute = Attribute.GetCustomAttribute(property, typeof(TAttribute), false);
                    if (attribute != null)
                    {
                        list.Add((TAttribute)attribute);
                    }
                }

                ReflectionCache.Instance[key] = list;
                return list;
            }

            return (IList<TAttribute>)cached;
        }

        public static TAttribute GetAttribute<TAttribute>()
            where TAttribute : Attribute
        {
            var a = AssemblyFromWhichToPullInformation.GetCustomAttributes(typeof(TAttribute), false);
            if (a.Length <= 0)
            {
                return null;
            }

            return (TAttribute)a[0];
        }

        public static Pair<PropertyInfo, TAttribute> RetrieveOptionProperty<TAttribute>(object target, string uniqueName)
                where TAttribute : BaseOptionAttribute
        {
            var key = new Pair<Type, object>(typeof(Pair<PropertyInfo, BaseOptionAttribute>), target);
            var cached = ReflectionCache.Instance[key];
            if (cached == null)
            {
                if (target == null)
                {
                    return null;
                }

                var propertiesInfo = target.GetType().GetProperties();

                foreach (var property in propertiesInfo)
                {
                    if (property == null || (!property.CanRead || !property.CanWrite))
                    {
                        continue;
                    }

                    var setMethod = property.GetSetMethod();
                    if (setMethod == null || setMethod.IsStatic)
                    {
                        continue;
                    }

                    var attribute = Attribute.GetCustomAttribute(property, typeof(TAttribute), false);
                    var optionAttr = (TAttribute)attribute;
                    if (optionAttr == null || string.CompareOrdinal(uniqueName, optionAttr.UniqueName) != 0)
                    {
                        continue;
                    }

                    var found = new Pair<PropertyInfo, TAttribute>(property, (TAttribute)attribute);
                    ReflectionCache.Instance[key] = found;
                    return found;
                }
            }

            return (Pair<PropertyInfo, TAttribute>)cached;
        }

        public static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}