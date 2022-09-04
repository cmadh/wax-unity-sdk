using System;
using System.Linq;

using StrattonStudios.EosioUnity.Models;

namespace StrattonStudios.EosioUnity.Utilities
{

    public static class AbiTypeUtility
    {

        /// <summary>
        /// Unwraps the ABI type definition.
        /// </summary>
        /// <param name="abi">The ABI schema</param>
        /// <param name="type">The ABI type</param>
        /// <returns>Returns the unwrapped type definition</returns>
        public static string UnwrapTypeDefinition(Abi abi, string type)
        {
            var wtype = abi.Types.FirstOrDefault(t => t.new_type_name == type);
            if (wtype != null && wtype.type != type)
            {
                return UnwrapTypeDefinition(abi, wtype.type);
            }

            return type;
        }

        /// <summary>
        /// Looks through the dictionary to find the given field name using different casing styles such as pascal case and snake case.
        /// </summary>
        /// <param name="name">The name of the field</param>
        /// <param name="dictionary">The dictionary</param>
        /// <returns>Returns the object field name if found, otherwise null</returns>
        public static string FindObjectFieldName(string name, System.Collections.IDictionary dictionary)
        {
            if (dictionary.Contains(name))
            {
                return name;
            }

            name = StringUtility.SnakeCaseToPascalCase(name);

            if (dictionary.Contains(name))
            {
                return name;
            }

            name = StringUtility.PascalCaseToSnakeCase(name);

            if (dictionary.Contains(name))
            {
                return name;
            }

            return null;
        }

        /// <summary>
        /// Looks through the object fields to find the given field name using different casing styles such as pascal case and snake case.
        /// </summary>
        /// <param name="name">The name of the field</param>
        /// <param name="objectType">The object type</param>
        /// <returns>Returns the object field name if found, otherwise null</returns>
        public static string FindObjectFieldName(string name, Type objectType)
        {
            if (objectType.GetFields().Any(p => p.Name == name))
            {
                return name;
            }

            name = StringUtility.SnakeCaseToPascalCase(name);

            if (objectType.GetFields().Any(p => p.Name == name))
            {
                return name;
            }

            name = StringUtility.PascalCaseToSnakeCase(name);

            if (objectType.GetFields().Any(p => p.Name == name))
            {
                return name;
            }

            return null;
        }

        /// <summary>
        /// Looks through the object properties to find the given field name using different casing styles such as pascal case and snake case.
        /// </summary>
        /// <param name="name">The name of the field</param>
        /// <param name="objectType">The object type</param>
        /// <returns>Returns the object property name if found, otherwise null</returns>
        public static string FindObjectPropertyName(string name, Type objectType)
        {
            if (objectType.GetProperties().Any(p => p.Name == name))
            {
                return name;
            }

            name = StringUtility.SnakeCaseToPascalCase(name);

            if (objectType.GetProperties().Any(p => p.Name == name))
            {
                return name;
            }

            name = StringUtility.PascalCaseToSnakeCase(name);

            if (objectType.GetProperties().Any(p => p.Name == name))
            {
                return name;
            }

            return null;
        }

    }

}