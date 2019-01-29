﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AuthorizationServer.ViewModels
{
    public class LoginModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string PassWrod { get; set; }
        public string ReturnUrl { get; set; }
    }
}
