using SII2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SII2.Extensions
{
    public static class MemoryTypeExtensions
    {
        private static string GetCustomDescription(object objEnum)
        {
            var fi = objEnum.GetType().GetField(objEnum.ToString());
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : objEnum.ToString();
        }

        public static string Description(this MemoryType value)
        {
            return GetCustomDescription(value);
        }
    }
}
