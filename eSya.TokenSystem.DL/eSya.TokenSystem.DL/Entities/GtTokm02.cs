﻿using System;
using System.Collections.Generic;

namespace eSya.TokenSystem.DL.Entities
{
    public partial class GtTokm02
    {
        public int BusinessKey { get; set; }
        public string TokenType { get; set; } = null!;
        public string CounterNumber { get; set; } = null!;
        public bool ActiveStatus { get; set; }
        public string FormId { get; set; } = null!;
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedTerminal { get; set; } = null!;
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedTerminal { get; set; }
    }
}
