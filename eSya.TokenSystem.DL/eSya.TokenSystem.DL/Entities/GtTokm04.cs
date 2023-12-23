using System;
using System.Collections.Generic;

namespace eSya.TokenSystem.DL.Entities
{
    public partial class GtTokm04
    {
        public int BusinessKey { get; set; }
        public DateTime TokenDate { get; set; }
        public string TokenKey { get; set; } = null!;
        public string TokenType { get; set; } = null!;
        public int SequeueNumber { get; set; }
        public int? Isdcode { get; set; }
        public string? MobileNumber { get; set; }
        public bool TokenCalling { get; set; }
        public string? CallingCounter { get; set; }
        public int CallingOccurence { get; set; }
        public DateTime? FirstCallingTime { get; set; }
        public DateTime? TokenCallingTime { get; set; }
        public bool TokenHold { get; set; }
        public int TokenHoldOccurence { get; set; }
        public DateTime? TokenHoldingTime { get; set; }
        public string TokenStatus { get; set; } = null!;
        public string? ConfirmedTokenType { get; set; }
        public DateTime? CompletedTime { get; set; }
        public bool ActiveStatus { get; set; }
        public string FormId { get; set; } = null!;
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedTerminal { get; set; } = null!;
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedTerminal { get; set; }
        public bool CallingConfirmation { get; set; }
        public DateTime? CallingConfirmationTime { get; set; }
    }
}
