using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationServer.Models;
using Microsoft.Extensions.Configuration;

namespace AuthenticationServer.Services
{
    public class UserService
    {
        private Dictionary<string, string> _inMemoryUsers;
        private LdapService _ldapService;
        public UserService(IConfiguration config)
        {
            _ldapService = new LdapService(config);
            _inMemoryUsers = new Dictionary<string, string>();
            _inMemoryUsers.Add("bob", "bob");
        }
        public string Login(LoginViewModel loginViewModel)
        {
            var isLogin = _ldapService.Login(loginViewModel.Username, loginViewModel.Password);
            if (!isLogin)
            {
                isLogin = _inMemoryUsers.Any(x => x.Key == loginViewModel.Username && x.Value == loginViewModel.Password);
            }

            return isLogin? loginViewModel.Username : null;
        }
    }
}
