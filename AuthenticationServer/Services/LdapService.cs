using System;
using LdapForNet;
using LdapForNet.Native;
using Microsoft.Extensions.Configuration;

namespace AuthenticationServer.Services
{
    public class LdapService
    {
        public string ActiveDirectoryPath;
        public LdapService(IConfiguration configRoot)
        {
            ActiveDirectoryPath = configRoot["AppSettings:LdapUrl"];
        }
        public bool Login(string trigram, string password)
        {
            try
            {
                using (var cn = new LdapConnection())
                {
                    cn.Connect(ActiveDirectoryPath, 389, Native.LdapSchema.LDAP, Native.LdapVersion.LDAP_VERSION3);
                    cn.Bind(userDn:trigram, password:password);
                    return true;
                }
            } catch (LdapException ex)
            {
                Console.WriteLine("LdapException : " + ex.Message);
                Console.WriteLine("LdapException : " + ex.ResultCode);
                Console.WriteLine("LdapException : " + ex.StackTrace);
                Console.WriteLine("LdapException : " + ex.InnerException);
                // Log exception
            }
            return false;

        }
    }
}
