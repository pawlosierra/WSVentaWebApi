﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WSVenta.Models.Request
{
    public class AuthRequest
    {
        //datanotation
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
