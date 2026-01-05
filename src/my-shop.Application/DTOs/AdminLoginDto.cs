using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace my_shop.Application.DTOs
{
    public class AdminLoginDto
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}


