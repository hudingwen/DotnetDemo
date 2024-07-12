﻿namespace MyAutoFac
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CustomPropertyAttribute : Attribute
    {
        // 标识特性
        //1.AttributeTargets.Property (限制只能属性 使用)

        //2.为CustomPropertySelector 作标识 过滤使用的
    }
}
