﻿using Autofac.Core;
using System.Reflection;

namespace MyAutoFac
{
    public class CustomPropertySelector : IPropertySelector
    {
        public bool InjectProperty(PropertyInfo propertyInfo, object instance)
        {
            //需要给控制器告知哪些属性是可以注入的  (需要一个判断的维度)
            return propertyInfo.CustomAttributes.Any(x => x.AttributeType == typeof(CustomPropertyAttribute));
        }
    }
}
