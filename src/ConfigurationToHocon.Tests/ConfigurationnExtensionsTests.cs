using Akka.Configuration;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace ConfigurationToHocon.Tests
{
    public class ConfigurationnExtensionsTests
    {
        [Fact]
        public void FromConfiguration_CanMapFirstLevelScalar()
        {
            var conf = new ConfigurationBuilder()
                .AddJsonFile("conf.json")
                .Build();

            Config hoconf = conf.ToHoconf();

            Assert.Equal("other string", hoconf.GetString("firstLevel-string"));
            Assert.Equal(123, hoconf.GetInt("firstLevel-int"));
        }
        
        [Fact]
        public void FromConfiguration_CanMapDeep()
        {
            var conf = new ConfigurationBuilder()
                .AddJsonFile("conf.json")
                .Build();

            Config hoconf = conf.ToHoconf();

            Assert.Equal("some string", hoconf.GetString("firstLevel-object.secondLevel-string"));
            Assert.Equal(456, hoconf.GetInt("firstLevel-object.secondLevel-int"));
        }

        [Fact]
        public void FromConfiguration_CanMapArrayOfScalars()
        {
            var conf = new ConfigurationBuilder()
                .AddJsonFile("conf.json")
                .Build();

            Config hoconf = conf.ToHoconf();

            var s = hoconf.ToString();

            var values = hoconf.GetStringList("firstLevel-object.secondLevel-scalarArray");
            Assert.Equal("value1", values[0]);
            Assert.Equal("value2", values[1]);
            Assert.Equal("value3", values[2]);
        }
    }
}