﻿using System;

namespace Ifp.Validation.TestProxy.Tests.Documentation
{
    public class RegisterNewUserModel
    {
        public string EMail { get; set; }
        public string Password { get; set; }
        public string PasswordRepeated { get; set; }
        public string GivenName { get; set; }
        public string SurName { get; set; }
        public DateTime? BithDate { get; set; }
    }
}
