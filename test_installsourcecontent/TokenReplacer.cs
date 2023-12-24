using System.Collections.ObjectModel;


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
        public ReadOnlyDictionary<string, string> Variables { get; set; } = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());

        public string Replace(string str)
        {
            if (null == Variables || Variables.Count == 0)
                return str;

            if (str.Contains(Prefix) || str.Contains(Suffix))
            {
                string strTemp = str;
                foreach (var variable in Variables)
                    strTemp = strTemp.Replace(Prefix + variable.Key + Suffix, variable.Value);
                return strTemp;
            }

            return str;
        }
    }
}