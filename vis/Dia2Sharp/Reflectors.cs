﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dia2Sharp
{
    public static class Reflectors
    {
        public static MinSym Match(this IList<MinSym> SymList, ulong ContainedBy)
        {
            return (from sym in SymList
                   where sym.Address >= ContainedBy && sym.Address + sym.Length <= ContainedBy
                    select sym).FirstOrDefault();
        }
        public static MinSym MatchNearest(this IList<MinSym> SymList, ulong Closest)
        {
            return (from sym in SymList
                    where sym.Address >= Closest 
                    select sym).FirstOrDefault();
        }

        public static IEnumerable<MethodInfo> LinqPublicFunctions(this Type type)
        {
            if (!type.IsInterface)
                return type.GetMethods();

            return (new Type[] { type })
                .Concat(type.GetInterfaces())
                   .SelectMany(i => i.GetMethods());
        }

        public static IEnumerable<PropertyInfo> LinqPublicProperties(this Type type)
        {
            if (!type.IsInterface)
                return type.GetProperties();

            return (new Type[] { type })
                   .Concat(type.GetInterfaces())
                   .SelectMany(i => i.GetProperties());
        }

        public static PropertyInfo[] GetPublicProperties(this Type type)
        {
            if (type.IsInterface)
            {
                var propertyInfos = new List<PropertyInfo>();

                var considered = new List<Type>();
                var queue = new Queue<Type>();
                considered.Add(type);
                queue.Enqueue(type);
                while (queue.Count > 0)
                {
                    var subType = queue.Dequeue();
                    foreach (var subInterface in subType.GetInterfaces())
                    {
                        if (considered.Contains(subInterface)) continue;

                        considered.Add(subInterface);
                        queue.Enqueue(subInterface);
                    }

                    var typeProperties = subType.GetProperties(
                        BindingFlags.FlattenHierarchy
                        | BindingFlags.Public
                        | BindingFlags.Instance);

                    var newPropertyInfos = typeProperties
                        .Where(x => !propertyInfos.Contains(x));

                    propertyInfos.InsertRange(0, newPropertyInfos);
                }

                return propertyInfos.ToArray();
            }

            return type.GetProperties(BindingFlags.FlattenHierarchy
                | BindingFlags.Public | BindingFlags.Instance);
        }
    }
}
