using LdapForNet;
using LdapForNet.Native;
using static LdapForNet.Native.Native;

namespace CodeHub.Services
{
    public class LdapService
    {
        private readonly string _ldapHost;
        private readonly int _ldapPort;
        private readonly string _ldapBaseDn;

        public LdapService(string ldapHost, int ldapPort)
        {
            _ldapHost = ldapHost;
            _ldapPort = ldapPort;
        }

        public async Task<LdapEntry?> AuthenticateUserAsync(string username, string password)
        {
            return await SearchLdapAsync(username, password);
        }

        public async Task<LdapEntry?> FindUser(string username)
        {
            return await SearchLdapAsync(username);
        }

        public async Task<LdapEntry?> SearchLdapAsync(string username, string? password = null)
        {
            try
            {
                using var connection = password != null
                    ? await CreateLdapConnectionAsync(username, password)
                    : await CreateLdapConnectionAsync("zuzcak4", "Skunizavyskask8");

                if (connection == null) return null;
                
                var entries = await connection.SearchAsync("dc=fri,dc=uniza,dc=sk", $"(uid={username})");
                return entries.FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        public async Task<LdapConnection?> CreateLdapConnectionAsync(string username, string password)
        {
            try
            {
                var ldapConnection = new LdapConnection();
                ldapConnection.Connect(_ldapHost, _ldapPort, LdapSchema.LDAP);
                ldapConnection.SetOption(LdapOption.LDAP_OPT_REFERRALS, nint.Zero);

                await ldapConnection.BindAsync(LdapAuthType.Simple, new LdapCredential
                {
                    UserName = $"{username}@fri.uniza.sk",
                    Password = password
                });

                return ldapConnection;
            }
            catch
            {
                return null;
            }
        }
    }
}