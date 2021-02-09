using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationServer.Services {
    public class LdapService {
        public string ActiveDirectoryPath;

        public bool Login(string trigram, string password) {

            string domainName = "ubik.be";

            string userDn = $"{trigram}@{domainName}";
            try {
                using (var connection = new LdapConnection { SecureSocketLayer = false }) {
                    connection.Connect(domainName, LdapConnection.DefaultPort);
                    connection.Bind(userDn, password);

                    if (connection.Bound) {
                        return true;
                    }
                }
            } catch (LdapException ex) {
                //Console.WriteLine(ex.Message);
            }
            return false;

        }

    }
}
