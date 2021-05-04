using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AiLing
{
    public static class ReflectionHelper
    {
        public static IEnumerable<Type> GetSubtypes(Assembly assembly, Type baseType, bool includeBase)
        {
            return assembly.GetTypes()
                           .Where(type =>
                           {
                               if (includeBase)
                                   return baseType.IsAssignableFrom(type);
                               else
                                   return baseType.IsAssignableFrom(type) && type != baseType;
                           });
        }

        public static GameEventInfoAttribute GetGameEventAttribute(GameEvent eve)
        {
            Type t = eve.GetType();
            GameEventInfoAttribute attri = t.GetCustomAttribute<GameEventInfoAttribute>();
            return attri;
        }
    }
}

