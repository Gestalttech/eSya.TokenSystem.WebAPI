using eSya.TokenSystem.DL.Entities;
using eSya.TokenSystem.DO;
using eSya.TokenSystem.IF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSya.TokenSystem.DL.Repository
{
    public class TokenManagementRepository: ITokenManagementRepository
    {
        private readonly IStringLocalizer<TokenManagementRepository> _localizer;
        public TokenManagementRepository(IStringLocalizer<TokenManagementRepository> localizer)
        {
            _localizer = localizer;
        }
        public async Task<List<DO_Token>> GetTokenDetailByTokenType(int businessKey, string tokenType)
        {
            using (var db = new eSyaEnterprise())
            {
                try
                {

                    var ds = db.GtTokm04s
                    .Join(db.GtTokm01s,
                    d => d.TokenType,
                    m => m.TokenType,
                    (d, m) => new { d, m })
                        .Where(w => w.d.BusinessKey == businessKey
                                    && w.d.TokenDate.Date == System.DateTime.Now.Date
                                    && (w.d.TokenType == tokenType || tokenType == "A")
                                    && w.d.TokenStatus == "RG"
                                    && w.d.ActiveStatus
                                    && !w.d.CallingConfirmation)
                        .Select(r => new DO_Token
                        {
                            TokenKey = r.d.TokenKey,
                            TokenType = r.d.TokenType,
                            TokenDate = r.d.TokenDate,
                            SequeueNumber = r.d.SequeueNumber,
                            TokenHold = r.d.TokenHold,
                            CreatedOn = r.d.CreatedOn,
                            TokenCalling = r.d.TokenCalling,
                            CallingConfirmation = r.d.CallingConfirmation,
                            ConfirmationUrl = r.m.ConfirmationUrl

                        }).OrderBy(o => o.CreatedOn).ToListAsync();

                    return await ds;

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public async Task<List<DO_CounterMapping>> GetTokenTypeByCounter(int businessKey, string counterNumber)
        {
            using (var db = new eSyaEnterprise())
            {
                try
                {

                    var ds = db.GtTokm02s
                        .Join(db.GtTokm01s,
                        m => m.TokenType,
                        t => t.TokenType,
                        (m, t) => new { m, t })
                        .Where(w => w.m.BusinessKey == businessKey
                                    && w.m.CounterNumber == counterNumber
                                    && w.m.ActiveStatus)
                        .Select(r => new DO_CounterMapping
                        {
                            CounterNumber = r.m.CounterNumber,
                            TokenType = r.m.TokenType,
                            TokenDesc = r.t.TokenDesc,
                            DisplaySequence = r.t.DisplaySequence,

                        }).OrderBy(o => o.DisplaySequence).ToListAsync();

                    return await ds;

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public async Task<List<DO_Token>> GetTokenDetailByMobile(int businessKey, int isdCode, string mobileNumber)
        {
            using (var db = new eSyaEnterprise())
            {
                try
                {

                    var ds = db.GtTokm04s
                        .Join(db.GtTokm01s,
                        m => m.TokenType,
                        t => t.TokenType,
                        (m, t) => new { m, t })
                        .Where(w => w.m.BusinessKey == businessKey
                                    && w.m.TokenDate.Date == System.DateTime.Now.Date
                                    && w.m.Isdcode == isdCode
                                    && w.m.MobileNumber == mobileNumber
                                    && w.m.TokenStatus == "RG"
                                    && w.m.ActiveStatus)
                        .Select(r => new DO_Token
                        {
                            TokenKey = r.m.TokenKey,
                            TokenType = r.t.TokenDesc,
                            TokenStatus = r.m.TokenStatus == "CM" ? "Completed" : "Inprogress",
                            CreatedOn = r.m.CreatedOn
                        }).OrderBy(o => o.CreatedOn).ToListAsync();

                    return await ds;

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public async Task<DO_ReturnParameter> UpdateCallingToken(DO_Token obj)
        {

            using (var db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        var dc = db.GtTokm04s.Where(w => w.BusinessKey == obj.BusinessKey
                            && w.TokenDate.Date == System.DateTime.Now.Date
                            && w.TokenType == obj.TokenType
                            && w.TokenKey == obj.TokenKey).FirstOrDefault();

                        if (dc != null)
                        {
                            if (dc.TokenCalling && dc.CallingCounter != obj.CallingCounter)
                            {
                                dbContext.Rollback();
                                return new DO_ReturnParameter { Status = false, Message = "Token No. " + dc.TokenKey + " is Serving by " + dc.CallingCounter };
                            }
                            dc.TokenHold = false;
                            dc.TokenCalling = true;
                            dc.CallingCounter = obj.CallingCounter;
                            dc.CallingOccurence = dc.CallingOccurence + 1;
                            dc.TokenCallingTime = System.DateTime.Now;
                            if (dc.FirstCallingTime == null)
                                dc.FirstCallingTime = System.DateTime.Now;
                            dc.ModifiedBy = obj.UserID;
                            dc.ModifiedOn = System.DateTime.Now;
                            dc.ModifiedTerminal = obj.TerminalID;
                        }

                        var remainingToken = db.GtTokm04s
                           .Where(w => w.BusinessKey == obj.BusinessKey
                               && w.TokenType == obj.TokenType
                               && w.TokenKey != obj.TokenKey
                               && w.TokenDate.Date == System.DateTime.Now.Date
                               && w.CallingCounter == obj.CallingCounter
                               && w.TokenCalling == true);

                        foreach (var t in remainingToken)
                        {
                            t.TokenCalling = false;
                        }




                        await db.SaveChangesAsync();
                        dbContext.Commit();

                        return new DO_ReturnParameter { Status = true, Key = obj.TokenKey };
                    }
                    catch (DbUpdateException ex)
                    {
                        dbContext.Rollback();
                        throw ex;
                    }
                    catch (Exception ex)
                    {
                        dbContext.Rollback();
                        throw ex;
                    }
                }
            }

        }
        public async Task<DO_ReturnParameter> UpdateTokenToHold(DO_Token obj)
        {

            using (var db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        var dc = db.GtTokm04s.Where(w => w.BusinessKey == obj.BusinessKey
                            && w.TokenDate.Date == System.DateTime.Now.Date
                            && w.TokenType == obj.TokenType
                            && w.TokenKey == obj.TokenKey
                            ).FirstOrDefault();

                        dc.TokenCalling = false;
                        dc.TokenHold = true;
                        dc.TokenHoldOccurence = dc.TokenHoldOccurence + 1;
                        dc.TokenHoldingTime = System.DateTime.Now;
                        dc.ModifiedBy = obj.UserID;
                        dc.ModifiedOn = System.DateTime.Now;
                        dc.ModifiedTerminal = obj.TerminalID;

                        await db.SaveChangesAsync();
                        dbContext.Commit();

                        return new DO_ReturnParameter { Status = true, Key = obj.TokenKey };
                    }
                    catch (DbUpdateException ex)
                    {
                        dbContext.Rollback();
                        throw ex;
                    }
                    catch (Exception ex)
                    {
                        dbContext.Rollback();
                        throw ex;
                    }
                }
            }

        }
        public async Task<DO_ReturnParameter> UpdateTokenToRelease(DO_Token obj)
        {
            using (var db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        var dc = db.GtTokm04s.Where(w => w.BusinessKey == obj.BusinessKey
                            && w.TokenDate.Date == System.DateTime.Now.Date
                            && w.TokenType == obj.TokenType
                            && w.TokenKey == obj.TokenKey).FirstOrDefault();

                        dc.TokenHold = false;
                        dc.ModifiedBy = obj.UserID;
                        dc.ModifiedOn = System.DateTime.Now;
                        dc.ModifiedTerminal = obj.TerminalID;

                        await db.SaveChangesAsync();
                        dbContext.Commit();

                        return new DO_ReturnParameter { Status = true, Key = obj.TokenKey };
                    }
                    catch (DbUpdateException ex)
                    {
                        dbContext.Rollback();
                        throw ex;
                    }
                    catch (Exception ex)
                    {
                        dbContext.Rollback();
                        throw ex;
                    }
                }
            }

        }
        public async Task<DO_ReturnParameter> UpdateTokenStatusToCompleted(DO_Token obj)
        {
            using (var db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        var dc = db.GtTokm04s.Where(w => w.BusinessKey == obj.BusinessKey
                            && w.TokenDate.Date == System.DateTime.Now.Date
                            && w.TokenType == obj.TokenType
                            && w.TokenKey == obj.TokenKey).FirstOrDefault();

                        dc.TokenCalling = false;
                        dc.TokenStatus = "CM";
                        dc.CompletedTime = System.DateTime.Now;
                        dc.ModifiedBy = obj.UserID;
                        dc.ModifiedOn = System.DateTime.Now;
                        dc.ModifiedTerminal = obj.TerminalID;



                        await db.SaveChangesAsync();
                        dbContext.Commit();

                        return new DO_ReturnParameter { Status = true, Key = obj.TokenKey };
                    }
                    catch (DbUpdateException ex)
                    {
                        dbContext.Rollback();
                        throw ex;
                    }
                    catch (Exception ex)
                    {
                        dbContext.Rollback();
                        throw ex;
                    }
                }
            }

        }
        public async Task<DO_ReturnParameter> UpdateToCallingNextToken(DO_Token obj)
        {
            using (var db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        var dc = db.GtTokm04s.Where(w => w.BusinessKey == obj.BusinessKey
                            && w.TokenDate.Date == System.DateTime.Now.Date
                            && w.TokenType == obj.TokenType
                            && w.TokenKey == obj.TokenKey).FirstOrDefault();

                        int sq_No = -1;
                        if (dc != null)
                        {
                            dc.TokenCalling = false;
                            dc.ModifiedBy = obj.UserID;
                            dc.ModifiedOn = System.DateTime.Now;
                            dc.ModifiedTerminal = obj.TerminalID;

                            sq_No = dc.SequeueNumber;
                        }

                        var nextToken = db.GtTokm04s
                            .Where(w => w.BusinessKey == obj.BusinessKey
                                && w.TokenType == obj.TokenType
                                && w.TokenDate.Date == System.DateTime.Now.Date
                                && w.TokenStatus == "RG"
                                && w.TokenHold == false
                                && w.SequeueNumber > sq_No)
                            .OrderBy(o => o.SequeueNumber).FirstOrDefault();

                        string strTokenKey = "";
                        if (nextToken != null)
                        {
                            nextToken.TokenCalling = true;
                            nextToken.CallingCounter = obj.CallingCounter;
                            nextToken.CallingOccurence = nextToken.CallingOccurence + 1;
                            nextToken.TokenCallingTime = System.DateTime.Now;
                            if (nextToken.FirstCallingTime == null)
                                nextToken.FirstCallingTime = System.DateTime.Now;
                            nextToken.ModifiedBy = obj.UserID;
                            nextToken.ModifiedOn = System.DateTime.Now;
                            nextToken.ModifiedTerminal = obj.TerminalID;

                            strTokenKey = nextToken.TokenKey;
                        }
                        else
                        {
                            nextToken = db.GtTokm04s
                            .Where(w => w.BusinessKey == obj.BusinessKey
                                && w.TokenType == obj.TokenType
                                && w.TokenDate.Date == System.DateTime.Now.Date
                                && w.TokenStatus == "RG"
                                && w.TokenHold == false
                                )
                            .OrderBy(o => o.SequeueNumber).FirstOrDefault();

                            if (nextToken != null)
                            {
                                nextToken.TokenCalling = true;
                                nextToken.CallingOccurence = obj.CallingOccurence;
                                nextToken.CallingOccurence = nextToken.CallingOccurence + 1;
                                nextToken.TokenCallingTime = System.DateTime.Now;
                                if (nextToken.FirstCallingTime == null)
                                    nextToken.FirstCallingTime = System.DateTime.Now;
                                nextToken.ModifiedBy = obj.UserID;
                                nextToken.ModifiedOn = System.DateTime.Now;
                                nextToken.ModifiedTerminal = obj.TerminalID;

                                strTokenKey = nextToken.TokenKey;
                            }
                        }

                        await db.SaveChangesAsync();
                        dbContext.Commit();

                        return new DO_ReturnParameter { Status = true, Key = strTokenKey };
                    }
                    catch (DbUpdateException ex)
                    {
                        dbContext.Rollback();
                        throw ex;
                    }
                    catch (Exception ex)
                    {
                        dbContext.Rollback();
                        throw ex;
                    }
                }
            }

        }

        public async Task<DO_ReturnParameter> UpdateCallingConfirmation(DO_Token obj)
        {

            using (var db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        var dc = db.GtTokm04s.Where(w => w.BusinessKey == obj.BusinessKey
                            && w.TokenDate.Date == System.DateTime.Now.Date
                            && w.TokenType == obj.TokenType
                            && w.TokenKey == obj.TokenKey).FirstOrDefault();

                        if (dc != null)
                        {
                            if (dc.CallingConfirmation)
                            {
                                dbContext.Rollback();
                                return new DO_ReturnParameter { Status = false, Message = "Token No. " + dc.TokenKey + " is already confirmed" };
                            }
                            dc.CallingConfirmation = true;
                            dc.CallingConfirmationTime = System.DateTime.Now;

                            dc.ModifiedBy = obj.UserID;
                            dc.ModifiedOn = System.DateTime.Now;
                            dc.ModifiedTerminal = obj.TerminalID;
                        }

                        await db.SaveChangesAsync();
                        dbContext.Commit();

                        return new DO_ReturnParameter { Status = true, Key = obj.TokenKey };
                    }
                    catch (DbUpdateException ex)
                    {
                        dbContext.Rollback();
                        throw ex;
                    }
                    catch (Exception ex)
                    {
                        dbContext.Rollback();
                        throw ex;
                    }
                }
            }

        }
    }
}
