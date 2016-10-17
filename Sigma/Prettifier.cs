using System.Collections.Generic;
using System.Linq;

namespace Sigma
{
    /// <summary>
    /// Prettifies internal names (such as database field names) to user-friendly ones, and uglifies back as well.
    /// </summary>
    public class Prettifier
    {
        public Prettifier()
        {
            prettyNames = new Dictionary<string, string>() { { "", "" } };
            uglyNames = prettyNames.ToDictionary(entry => entry.Value, entry => entry.Key);
        }

        public void Add(string uglyName, string prettyName)
        {
            prettyNames.Add(uglyName, prettyName);
            uglyNames.Add(prettyName, uglyName);
        }

        public void Remove(string uglyName)
        {
            string prettyName;
            if (prettyNames.TryGetValue(uglyName, out prettyName))
            {
                prettyNames.Remove(uglyName);
                prettyNames.Remove(prettyName);
            }
        }

        public string Prettify(string uglyName)
        {
            string prettyName;
            if (prettyNames.TryGetValue(uglyName, out prettyName)) return prettyName;
            return uglyName;
        }

        public string Uglify(string prettyName)
        {
            string uglyName;
            if (uglyNames.TryGetValue(prettyName, out uglyName)) return uglyName;
            return prettyName;
        }

        private readonly Dictionary<string, string> prettyNames;
        private readonly Dictionary<string, string> uglyNames;
    }
}
