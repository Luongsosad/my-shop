using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace my_shop.Application.DTOs
{
    public class AdminLoginResponseDto
    {
        public string Token { get; set; } = default!;
        public string Email { get; set; } = default!;
    }
}
