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
        private static Dictionary<EReferenceContent, GameEnumAttribute> _GameReferenceCacheDic = new Dictionary<EReferenceContent, GameEnumAttribute>();

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

        public static GameEnumAttribute GetGameCacheEnumAttribute(EReferenceContent content)
        {
            GameEnumAttribute attri;
            if(_GameReferenceCacheDic.TryGetValue(content,out attri))
            {
                return attri;
            }
            var t = content.GetType();
            FieldInfo field = t.GetField(Enum.GetName(t, content));
            attri = Attribute.GetCustomAttribute(field, typeof(GameEnumAttribute)) as GameEnumAttribute;
            if(attri == null)
            {
                DebugHelper.LogError(content + field.Name + " 没有设置Type");
                return null;
            }
            _GameReferenceCacheDic.Add(content, attri);
            return attri;
        }
    }
}

