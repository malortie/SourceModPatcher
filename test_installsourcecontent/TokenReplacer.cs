using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace test_installsourcecontent
{
    public interface ITokenReplacer
    {
        public string Prefix { get; set; }
        public string Suffix { get; set; }
        ReadOnlyDictionary<string, string> Variables { get; set; }
        string Replace(string str);
    }

    public class TokenReplacer : ITokenReplacer
    {
        public string Prefix { get; set; } = "$(";
        public string Suffix { get; set; } = ")";
        public ReadOnlyDictionary<string, string> Variables { get; set; }
        
        public string Replace(string str)
        {
            if (null == Variables || Variables.Count == 0)
                return str;

            string strTemp = str;

            string regexPrefix = "";
            foreach (char c in Prefix)
                regexPrefix += $"\\{c}";
            string regexSuffix = "";
            foreach (char c in Suffix)
                regexSuffix = $"\\{c}";

            string pattern = $"{regexPrefix}([^{Suffix}]*){regexSuffix}";

            Regex tokenSubstitution = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

            MatchCollection matches = tokenSubstitution.Matches(str);
            for (int i = 0; i < matches.Count; ++i)
            {
                if (matches[i].Groups.Count > 1)
                {
                    string variable = matches[i].Groups[1].Value;
                    strTemp = strTemp.Replace($"$({variable})", Variables[variable]);
                }
            }

            return strTemp;
        }
    }
}