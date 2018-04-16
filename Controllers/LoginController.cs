using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LibraryMSAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace LibraryMSAPI.Controllers
{
    public class ManagerInfo
    {
        [Required]
        public string name { get; set; }
        [Required]
        public string password { get; set; }
    }
    [Route("api/[controller]")]
    public class LoginController:Controller
    {
        public LibraryDbContext DbContext { get; }
        public LoginController(LibraryDbContext libraryDbContext)
        {
            DbContext = libraryDbContext;
        }
        // login  name&password
        [HttpPost]
        public async Task<string> Post([FromBody]ManagerInfo managerInfo)
        {
            var name = managerInfo.name;
            var password = managerInfo.password;
            Console.WriteLine(name);
            Console.WriteLine("------------");
            var _passwords = await (from manager in DbContext.Managers  select manager.password).ToArrayAsync();
            if (_passwords.Length == 0) return "no this manager";
            var _password = _passwords[0].Trim();
            if (password.Equals(_password))
            {
                var result = "cc98" + name + "CC98" + password;
                MD5 md5 = new MD5CryptoServiceProvider();
                var output = BitConverter.ToString((md5.ComputeHash(Encoding.UTF8.GetBytes(result)))).Replace("-", "");
                return output;
            }
            else
            {
                return "password error";
            }
          
        }
        [HttpPost("{name}")]
        public async Task<string> Get(string name,[FromBody] string password)
        {

            return name+password;
        }
    }
}
