using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using UnityEngine;

namespace StrattonStudios.Networking.Utilities
{

    /// <summary>
    /// URL query utilities.
    /// </summary>
    public static class UrlQueryUtility
    {

        /// <summary>
        /// Converts the object to a URL encoded query string.
        /// </summary>
        /// <param name="request">The object to encode</param>
        /// <param name="separator">The separator for collections</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string ToQueryString(this object request, string separator = ",")
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            var type = request.GetType();

            // Get all properties on the object
            var properties = request.GetType().GetProperties()
                .Where(x => x.CanRead)
                .Where(x => x.GetValue(request, null) != null)
                .ToDictionary(x =>
                {
                    var attributes = Attribute.GetCustomAttributes(x, typeof(QueryParameterAttribute));
                    if (attributes != null && attributes.Length > 0)
                    {
                        return (attributes[0] as QueryParameterAttribute).ParameterName;
                    }
                    return x.Name;
                }, x =>
                {
                    var value = x.GetValue(request, null);
                    if (type.IsEnum)
                    {
                        try
                        {
                            var enumType = typeof(QueryParameterAttribute);
                            var memberInfos = enumType.GetMember(value.ToString());
                            var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);
                            var valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(QueryParameterAttribute), false);
                            return ((QueryParameterAttribute)valueAttributes[0]).ParameterName;
                        }
                        catch
                        {
                            return value;
                        }
                    }
                    return x.GetValue(request, null);
                });

            // Get names for all IEnumerable properties (excl. string)
            var propertyNames = properties
                .Where(x => !(x.Value is string) && x.Value is IEnumerable)
                .Select(x => x.Key)
                .ToList();

            // Concat all IEnumerable properties into a comma separated string
            foreach (var key in propertyNames)
            {
                var valueType = properties[key].GetType();
                var valueElemType = valueType.IsGenericType
                                        ? valueType.GetGenericArguments()[0]
                                        : valueType.GetElementType();
                if (valueElemType.IsPrimitive || valueElemType == typeof(string))
                {
                    var enumerable = properties[key] as IEnumerable;
                    properties[key] = string.Join(separator, enumerable.Cast<object>());
                }
            }

            // Concat all key/value pairs into a string separated by ampersand
            return string.Join("&", properties
                .Select(x => string.Concat(
                    Uri.EscapeDataString(x.Key), "=",
                    Uri.EscapeDataString(x.Value.ToString()))));
        }

    }

}