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
    public class TokenGenerationRepository: ITokenGenerationRepository
    {
        private readonly IStringLocalizer<TokenGenerationRepository> _localizer;
        public TokenGenerationRepository(IStringLocalizer<TokenGenerationRepository> localizer)
        {
            _localizer = localizer;
        }

        public async Task<DO_ReturnParameter> GenerateToken(DO_Token obj)
        {
            using (var db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {

                    try
                    {
                        if (!string.IsNullOrEmpty(obj.TokenKey))
                        {

                            var RecordExist = db.GtTokm04s.Where(w => w.TokenKey == obj.TokenKey && obj.TokenKey != "0" && w.BusinessKey == obj.BusinessKey && w.TokenDate == System.DateTime.Now.Date).Count();
                            if (RecordExist > 0)
                            {
                                return new DO_ReturnParameter() { Status = false, StatusCode = "W0192", Message = string.Format(_localizer[name: "W0192"]) };
                            }
                        }

                        var token_type = db.GtTokm01s.Where(w => w.TokenType == obj.TokenType).FirstOrDefault();
                        if (token_type == null)
                        {
                            return new DO_ReturnParameter() { Status = false, StatusCode = "W0193", Message = string.Format(_localizer[name: "W0193"]) };
                        }
                        var tSqNo = db.GtTokm04s.Where(w => w.BusinessKey == obj.BusinessKey
                              && w.TokenType == obj.TokenType
                              && w.TokenDate.Date == System.DateTime.Now.Date)
                          .Select(s => s.SequeueNumber)
                          .DefaultIfEmpty().Max();
                        tSqNo = tSqNo + 1;

                        var sr_token = token_type.TokenPrefix + (Convert.ToInt32(tSqNo)).ToString().PadLeft(token_type.TokenNumberLength, '0');



                        var token = new GtTokm04
                        {
                            BusinessKey = obj.BusinessKey,
                            TokenDate = System.DateTime.Now,
                            TokenKey = sr_token,
                            TokenType = obj.TokenType,

                            Isdcode = obj.Isdcode,
                            MobileNumber = obj.MobileNumber,

                            SequeueNumber = tSqNo,
                            TokenCalling = false,
                            CallingOccurence = 0,
                            TokenHold = false,
                            TokenHoldOccurence = 0,
                            TokenStatus = "RG",

                            ConfirmedTokenType = "",

                            ActiveStatus = true,
                            FormId = obj.FormID,
                            CreatedBy = obj.UserID,
                            CreatedOn = System.DateTime.Now,
                            CreatedTerminal = obj.TerminalID
                        };


                        db.GtTokm04s.Add(token);

                        await db.SaveChangesAsync();
                        dbContext.Commit();
                        //return new DO_ReturnParameter() { Status = true, StatusCode = "S0001", Message = string.Format(_localizer[name: "S0001"]),Key = sr_token };

                        return new DO_ReturnParameter { Status = true, Key = sr_token };
                    }


                    catch (DbUpdateException ex)
                    {
                        dbContext.Rollback();
                        throw new Exception(CommonMethod.GetValidationMessageFromException(ex));
                    }
                }

            }
        }

        public async Task<DO_ReturnParameter> GenerateOTP(DO_OTP obj)
        {
            using (var db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {

                    try
                    {

                        Random rnd = new Random();
                        var OTP = rnd.Next(1000, 9999).ToString();
                        var _id = db.GtEcmotps.Select(s => s.Id).DefaultIfEmpty().Max();
                        _id = _id + 1;

                        var otp = new GtEcmotp
                        {
                            Id = _id,
                            MobileNumber = obj.MobileNumber,
                            Otptype = obj.Otptype,
                            Otp = Convert.ToDecimal(OTP),
                            GeneratedOn = System.DateTime.Now
                        };


                        db.GtEcmotps.Add(otp);

                        await db.SaveChangesAsync();
                        dbContext.Commit();

                        return new DO_ReturnParameter { Status = true, Key = OTP };
                    }


                    catch (DbUpdateException ex)
                    {
                        dbContext.Rollback();
                        throw new Exception(CommonMethod.GetValidationMessageFromException(ex));
                    }
                }

            }


        }
        public async Task<DO_ReturnParameter> ConfirmOTP(DO_OTP obj)
        {
            using (var db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {

                    try
                    {

                        var _exist = db.GtEcmotps
                            .Where(w => w.Otptype == obj.Otptype && w.MobileNumber == obj.MobileNumber && w.ConfirmedOn == null)
                            .OrderByDescending(o => o.GeneratedOn).FirstOrDefault();
                        if (_exist == null)
                        {
                            return new DO_ReturnParameter() { Status = false, StatusCode = "W0200", Message = string.Format(_localizer[name: "W0200"]) };

                        }
                        else
                        {
                            if (_exist.Otp != obj.Otp)
                            {
                                return new DO_ReturnParameter() { Status = false, StatusCode = "W0200", Message = string.Format(_localizer[name: "W0200"]) };
                            }
                            else
                            {
                                _exist.ConfirmedOn = System.DateTime.Now;
                                await db.SaveChangesAsync();
                                dbContext.Commit();
                                return new DO_ReturnParameter() { Status = true, StatusCode = "S0006", Message = string.Format(_localizer[name: "S0006"]) };
                            }

                        }

                    }


                    catch (DbUpdateException ex)
                    {
                        dbContext.Rollback();
                        throw new Exception(CommonMethod.GetValidationMessageFromException(ex));
                    }
                }
            }
        }
    }
}
