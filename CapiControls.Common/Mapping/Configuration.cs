using AutoMapper;
using CapiControls.Common.Mapping.Profiles;
using System;

namespace CapiControls.Common.Mapping
{
    public class Configuration
    {
        private static Lazy<IConfigurationProvider> _defaultConfiguration = new Lazy<IConfigurationProvider>(() =>
            new MapperConfiguration(config => {
                    config.AddProfile(new BLLProfile());
            })
        );

        public static IConfigurationProvider DefaultConfiguration => _defaultConfiguration.Value;

        public static IMapper CreateDefaultMapper() => new Mapper(DefaultConfiguration);
    }
}
