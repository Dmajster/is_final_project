using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MusicShare.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName;
        public string LastName;
        public string City;
    }
}
