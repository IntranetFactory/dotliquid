using System;

namespace DotLiquid.NamingConventions
{
    public class CSharpNamingConvention : INamingConvention
    {
        public StringComparer StringComparer
        {
            get { return StringComparer.Ordinal; }
        }

        public string GetMemberName(string name)
        {

            //if (!string.IsNullOrEmpty(name))
            //{
            //    name = name[0].ToString().ToUpper() + name.Substring(1); //first letter uppercase other lowercase
            //}
            if (name == "default")
            {
                name = "Default";
            }
            return name;

        }

        public bool OperatorEquals(string testedOperator, string referenceOperator)
        {

            return UpperFirstLetter(testedOperator).Equals(referenceOperator)
            || LowerFirstLetter(testedOperator).Equals(referenceOperator);

            //return GetMemberName(testedOperator).Equals(referenceOperator);

            //return testedOperator.ToLower().Equals(referenceOperator.ToLower());

        }

        private static string UpperFirstLetter(string word)
        {
            return char.ToUpperInvariant(word[0]) + word.Substring(1);
        }

        private static string LowerFirstLetter(string word)
        {
            return char.ToUpperInvariant(word[0]) + word.Substring(1);
        }
    }
}
