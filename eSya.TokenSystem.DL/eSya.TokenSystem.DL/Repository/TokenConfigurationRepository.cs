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
    public class TokenConfigurationRepository: ITokenConfigurationRepository
    {
        private readonly IStringLocalizer<TokenConfigurationRepository> _localizer;
        public TokenConfigurationRepository(IStringLocalizer<TokenConfigurationRepository> localizer)
        {
            _localizer = localizer;
        }
        #region Token Configuration

        public async Task<List<DO_TokenConfiguration>> GetActiveTokens()
        {
            using (var db = new eSyaEnterprise())
            {
                try
                {
                    var tokens = await db.GtTokm01s.Where(x => x.ActiveStatus == true)

                                        .Select(t => new DO_TokenConfiguration
                                        {
                                            TokenType = t.TokenType,
                                            TokenDesc = t.TokenDesc
                                        }).ToListAsync();
                    return tokens;
                }

                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public async Task<List<DO_TokenConfiguration>> GetAllConfigureTokens()
        {
            using (var db = new eSyaEnterprise())
            {
                try
                {
                    var tokens = await db.GtTokm01s
                                    .Select(t => new DO_TokenConfiguration
                                    {
                                        TokenType = t.TokenType,
                                        TokenDesc = t.TokenDesc,
                                        ConfirmationUrl = t.ConfirmationUrl,
                                        TokenPrefix = t.TokenPrefix,
                                        DisplaySequence = t.DisplaySequence,
                                        TokenNumberLength = t.TokenNumberLength,
                                        ActiveStatus = t.ActiveStatus
                                    }).OrderBy(o => o.DisplaySequence).ToListAsync();
                    return tokens;
                }

                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public async Task<DO_ReturnParameter> InsertIntoToken(DO_TokenConfiguration obj)
        {
            using (var db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        var t_type = db.GtTokm01s.Where(x => x.TokenType.ToUpper().Replace(" ", "") == obj.TokenType.ToUpper().Replace(" ", "")).FirstOrDefault();
                        if (t_type != null)
                        {
                            return new DO_ReturnParameter() { Status = false, StatusCode = "W0190", Message = string.Format(_localizer[name: "W0190"]) };
                        }

                        //Display Sequence Adjustment
                        var seq_exist = db.GtTokm01s.Where(s => s.DisplaySequence == obj.DisplaySequence).FirstOrDefault();
                        if (seq_exist != null)
                        {
                            var seq_adj = db.GtTokm01s.Where(s => s.DisplaySequence >= obj.DisplaySequence).ToList();
                            foreach (GtTokm01 seq in seq_adj)
                            {
                                seq.DisplaySequence += 1;
                            }
                            db.SaveChanges();
                        }


                        GtTokm01 t_cnfg = new GtTokm01();
                        t_cnfg.TokenType = obj.TokenType;
                        t_cnfg.TokenDesc = obj.TokenDesc;
                        t_cnfg.ConfirmationUrl = obj.ConfirmationUrl;
                        t_cnfg.DisplaySequence = obj.DisplaySequence;
                        t_cnfg.TokenPrefix = obj.TokenPrefix;
                        t_cnfg.TokenNumberLength = obj.TokenNumberLength;
                        t_cnfg.ActiveStatus = obj.ActiveStatus;
                        t_cnfg.FormId = obj.FormId;
                        t_cnfg.CreatedBy = obj.UserID;
                        t_cnfg.CreatedOn = System.DateTime.Now;
                        t_cnfg.CreatedTerminal = obj.TerminalID;
                        db.GtTokm01s.Add(t_cnfg);
                        await db.SaveChangesAsync();
                        dbContext.Commit();
                        return new DO_ReturnParameter() { Status = true, StatusCode = "S0001", Message = string.Format(_localizer[name: "S0001"]) };

                    }


                    catch (DbUpdateException ex)
                    {
                        dbContext.Rollback();
                        throw new Exception(CommonMethod.GetValidationMessageFromException(ex));
                    }
                }

            }
        }

        public async Task<DO_ReturnParameter> UpdateToken(DO_TokenConfiguration obj)
        {
            using (var db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        var t_cnfg = db.GtTokm01s.Where(x => x.TokenType.ToUpper().Replace(" ", "") == obj.TokenType.ToUpper().Replace(" ", "")).FirstOrDefault();
                        if (t_cnfg == null)
                        {
                            return new DO_ReturnParameter() { Status = false, StatusCode = "W0191", Message = string.Format(_localizer[name: "W0191"]) };
                        }

                        //Display Sequence Adjustment
                        var seq_exist = db.GtTokm01s.Where(s => s.TokenType == obj.TokenType).FirstOrDefault();
                        if (seq_exist != null)
                        {
                            if (seq_exist.DisplaySequence > obj.DisplaySequence)
                            {
                                var seq_adj = db.GtTokm01s.Where(s => s.DisplaySequence >= obj.DisplaySequence && s.DisplaySequence < seq_exist.DisplaySequence).ToList();
                                foreach (GtTokm01 seq in seq_adj)
                                {
                                    seq.DisplaySequence += 1;
                                }
                            }
                            if (seq_exist.DisplaySequence < obj.DisplaySequence)
                            {
                                var seq_adj = db.GtTokm01s.Where(s => s.DisplaySequence > seq_exist.DisplaySequence && s.DisplaySequence <= obj.DisplaySequence).ToList();
                                foreach (GtTokm01 seq in seq_adj)
                                {
                                    seq.DisplaySequence -= 1;
                                }
                            }

                            db.SaveChanges();
                        }


                        t_cnfg.TokenDesc = obj.TokenDesc;
                        t_cnfg.ConfirmationUrl = obj.ConfirmationUrl;
                        t_cnfg.DisplaySequence = obj.DisplaySequence;
                        t_cnfg.TokenPrefix = obj.TokenPrefix;
                        t_cnfg.TokenNumberLength = obj.TokenNumberLength;
                        t_cnfg.ActiveStatus = obj.ActiveStatus;
                        t_cnfg.ModifiedBy = obj.UserID;
                        t_cnfg.ModifiedOn = System.DateTime.Now;
                        t_cnfg.ModifiedTerminal = obj.TerminalID;
                        await db.SaveChangesAsync();
                        dbContext.Commit();
                        return new DO_ReturnParameter() { Status = true, StatusCode = "S0002", Message = string.Format(_localizer[name: "S0002"]) };
                    }

                    catch (DbUpdateException ex)
                    {
                        dbContext.Rollback();
                        throw new Exception(CommonMethod.GetValidationMessageFromException(ex));
                    }
                }
            }
        }

        public async Task<DO_ReturnParameter> ActiveOrDeActiveToken(bool status, string tokentype)
        {
            using (var db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        GtTokm01 t_cnfg = await db.GtTokm01s.Where(x => x.TokenType.ToUpper().Replace(" ", "") == tokentype.ToUpper().Replace(" ", "")).FirstOrDefaultAsync();

                        if (t_cnfg == null)
                        {
                            return new DO_ReturnParameter() { Status = false, StatusCode = "W0191", Message = string.Format(_localizer[name: "W0191"]) };
                        }

                        t_cnfg.ActiveStatus = status;
                        await db.SaveChangesAsync();
                        dbContext.Commit();
                        if (status == true)
                            return new DO_ReturnParameter() { Status = true, StatusCode = "S0003", Message = string.Format(_localizer[name: "S0003"]) };
                        else
                            return new DO_ReturnParameter() { Status = true, StatusCode = "S0004", Message = string.Format(_localizer[name: "S0004"]) };
                    }
                    catch (DbUpdateException ex)
                    {
                        dbContext.Rollback();
                        throw new Exception(CommonMethod.GetValidationMessageFromException(ex));

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }


        #endregion
    }
}
