using System;


namespace StrattonStudios.Networking
{

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public sealed class QueryParameterAttribute : Attribute
    {

        public readonly string ParameterName;

        public QueryParameterAttribute(string parameterName)
        {
            this.ParameterName = parameterName;
        }

    }

}