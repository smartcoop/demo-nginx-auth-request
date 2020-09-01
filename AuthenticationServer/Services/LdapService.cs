using System;
using System.DirectoryServices;
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
        public string Login(string trigram, string password)
        {
            string userName = null;
            var entry = new DirectoryEntry(ActiveDirectoryPath, trigram, password);
            var searcher = new DirectorySearcher(entry);
            searcher.SearchScope = SearchScope.OneLevel;

            try
            {
                // search for user
                var result = searcher.FindOne();
                if (result != null)
                {
                    // get user info
                    entry = new DirectoryEntry(ActiveDirectoryPath, trigram, password);
                    searcher = new DirectorySearcher(entry);
                    searcher.Filter = (Convert.ToString("(SAMAccountName=") + trigram) + ")";
                    searcher.PropertiesToLoad.Add("givenName");
                    result = searcher.FindOne();
                    if (result != null)
                    {
                        userName = result.GetDirectoryEntry().Properties["mailNickname"].Value?.ToString();
                    }
                }
            } catch
            {
            }

            return userName;
        }
    }
}
