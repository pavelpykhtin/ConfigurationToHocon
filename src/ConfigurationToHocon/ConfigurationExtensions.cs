using Akka.Configuration;
using Akka.Configuration.Hocon;
using Microsoft.Extensions.Configuration;

namespace ConfigurationToHocon
{
    /// <summary>
    /// Helper methods to convert IConfiguration to HOCON object.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Convert <see cref="IConfiguration" /> to <see cref="Config" />.
        /// </summary>
        /// <param name="configuration">Configuration section.</param>
        /// <returns>HOCON configuration object.</returns>
        public static Config ToHoconf(this IConfiguration configuration)
        {
            var root = new HoconValue();

            ParseObject(root, "", configuration);

            return new Config(new HoconRoot(root));
        }

        private static void ParseObject(
            HoconValue owner,
            string currentPath,
            IConfiguration conf)
        {
            if (!owner.IsObject())
                owner.NewValue(new HoconObject());

            HoconObject hoconObject = owner.GetObject();

            foreach (var section in conf.GetChildren())
                if (int.TryParse(section.Key, out _))
                {
                    if (!owner.Values[0].IsArray())
                    {
                        owner.Clear();
                        owner.NewValue(new HoconArray());
                    }

                    var array = (HoconArray) owner.Values[0];
                    var value = new HoconValue();
                    ParseSection(currentPath, section, value);
                    array.Add(value);
                }
                else
                {
                    ParseSection(currentPath, section, hoconObject.GetOrCreateKey(section.Key));
                }
        }

        private static void ParseSection(
            string currentPath, 
            IConfigurationSection section, 
            HoconValue parseInto)
        {
            if (section.Value == null)
                ParseObject(
                    parseInto,
                    currentPath == "" ? section.Key : $"{currentPath}.{section.Key}",
                    section);
            else
                ParseValue(
                    section.Value,
                    parseInto);
        }

        private static void ParseValue(
            string value,
            HoconValue owner)
        {
            if (owner.IsObject())
                owner.Clear();
            HoconLiteral hoconLiteral = new HoconLiteral()
            {
                Value = value
            };
            owner.AppendValue(hoconLiteral);
        }
    }
}