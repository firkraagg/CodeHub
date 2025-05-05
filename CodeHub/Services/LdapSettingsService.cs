using CodeHub.Configuration;

namespace CodeHub.Services
{
    public class LdapSettingsService
    {
        private readonly IConfiguration _configuration;

        public LdapSettingsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public LdapSettings GetSettings()
        {
            var settings = new LdapSettings();
            _configuration.GetSection("LdapSettings").Bind(settings);
            return settings;
        }
    }
}