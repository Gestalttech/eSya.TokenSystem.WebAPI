﻿using System;
using System.Collections.Generic;
using System.Text;

namespace eSya.TokenSystem.DO
{
    public class DO_OTP
    {
        public int Id { get; set; }
        public string Otptype { get; set; }
        public string MobileNumber { get; set; }
        public decimal Otp { get; set; }
        public DateTime GeneratedOn { get; set; }
        public DateTime? ConfirmedOn { get; set; }
    }
}
