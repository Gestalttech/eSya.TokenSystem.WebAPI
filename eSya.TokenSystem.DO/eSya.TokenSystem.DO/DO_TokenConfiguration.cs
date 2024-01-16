﻿using System;
using System.Collections.Generic;
using System.Text;

namespace eSya.TokenSystem.DO
{
   public class DO_TokenConfiguration
    {
        public string TokenType { get; set; }
        public string TokenDesc { get; set; }
        public string? ConfirmationUrl { get; set; }
        public int DisplaySequence { get; set; }
        public string TokenPrefix { get; set; }
        public int TokenNumberLength { get; set; }
        public bool ActiveStatus { get; set; }
        public string FormId { get; set; }
        public int UserID { get; set; }
        public string TerminalID { get; set; }
    }
    public class DO_CounterCreation
    {
        public int BusinessKey { get; set; }
        public string CounterNumber { get; set; }
        public int FloorId { get; set; }
        public bool ActiveStatus { get; set; }
        public string FormId { get; set; }
        public int UserID { get; set; }
        public string TerminalID { get; set; }
        public string? FloorName { get; set; }
    }
    public class DO_CounterMapping
    {
        public int BusinessKey { get; set; }
        public string TokenType { get; set; }
        public string CounterNumber { get; set; }
        public int FloorId { get; set; }
        public bool ActiveStatus { get; set; }
        public string FormId { get; set; }
        public int UserID { get; set; }
        public string TerminalID { get; set; }
        public string? TokenDesc { get; set; }
        public string? FloorName { get; set; }
        public string? CounterNumberdesc { get; set; }
        public int DisplaySequence { get; set; }
    }

    public class DO_ReturnParameter
    {
        public bool Status { get; set; }
        public string StatusCode { get; set; }
        public string Message { get; set; }
        public string ErrorCode { get; set; }
        public decimal ID { get; set; }
        public string Key { get; set; }
    }
    public class DO_Floor
    {
        public int FloorId { get; set; }
        public string FloorName { get; set; }
    }
  
}
