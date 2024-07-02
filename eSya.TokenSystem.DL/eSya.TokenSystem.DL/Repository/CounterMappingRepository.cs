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
using static eSya.TokenSystem.DO.DO_CounterMapping;

namespace eSya.TokenSystem.DL.Repository
{
    public class CounterMappingRepository: ICounterMappingRepository
    {
        private readonly IStringLocalizer<CounterMappingRepository> _localizer;
        public CounterMappingRepository(IStringLocalizer<CounterMappingRepository> localizer)
        {
            _localizer = localizer;
        }

        #region Token Counter Creation
        public async Task<List<DO_Floor>> GetFloorsbyFloorId(int codetype)
        {
            using (var db = new eSyaEnterprise())
            {
                try
                {
                    var ds = await db.GtEcapcds.Where(x => x.CodeType == codetype && x.ActiveStatus == true).Select(
                          f => new DO_Floor
                          {
                              FloorId = f.ApplicationCode,
                              FloorName = f.CodeDesc
                          }).ToListAsync();

                    return ds;

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public async Task<List<DO_CounterCreation>> GetTokenCountersbyBusinessKey(int businesskey)
        {
            using (var db = new eSyaEnterprise())
            {
                try
                {
                    var ds = await db.GtTokm02s.Where(r => r.BusinessKey == businesskey).Join(db.GtEcapcds,
                            x => x.FloorId,
                            y => y.ApplicationCode,
                            (x, y) => new DO_CounterCreation
                            {
                                BusinessKey = x.BusinessKey,
                                FloorId = x.FloorId,
                                CounterNumber = x.CounterNumber,
                                ActiveStatus = x.ActiveStatus,
                                FloorName = y.CodeDesc
                            }).ToListAsync();
                   
                    return ds.OrderBy(x => x.FloorName).ThenBy(x => x.CounterNumber).ToList();
                }

                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public async Task<DO_ReturnParameter> InsertIntoTokenCounter(DO_CounterCreation obj)
        {
            using (var db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        var _counter = db.GtTokm02s.Where(x => x.BusinessKey == obj.BusinessKey && x.FloorId == obj.FloorId && x.CounterNumber.ToUpper().Replace(" ", "") == obj.CounterNumber.ToUpper().Replace(" ", "")).FirstOrDefault();
                        if (_counter != null)
                        {
                            return new DO_ReturnParameter() { Status = false, StatusCode = "W0185", Message = string.Format(_localizer[name: "W0185"]) };
                        }

                        GtTokm02 counter = new GtTokm02();
                        counter.BusinessKey = obj.BusinessKey;
                        counter.FloorId = obj.FloorId;
                        counter.CounterNumber = obj.CounterNumber;
                        counter.ActiveStatus = obj.ActiveStatus;
                        counter.FormId = obj.FormId;
                        counter.CreatedBy = obj.UserID;
                        counter.CreatedOn = System.DateTime.Now;
                        counter.CreatedTerminal = obj.TerminalID;
                        db.GtTokm02s.Add(counter);
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

        public async Task<DO_ReturnParameter> UpdateTokenCounter(DO_CounterCreation obj)
        {
            using (var db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        var _counter = db.GtTokm02s.Where(x => x.BusinessKey == obj.BusinessKey && x.FloorId == obj.FloorId && x.CounterNumber.ToUpper().Replace(" ", "") == obj.CounterNumber.ToUpper().Replace(" ", "")).FirstOrDefault();
                        if (_counter == null)
                        {
                            return new DO_ReturnParameter() { Status = false, StatusCode = "W0186", Message = string.Format(_localizer[name: "W0186"]) };
                        }
                        _counter.ActiveStatus = obj.ActiveStatus;
                        _counter.ModifiedBy = obj.UserID;
                        _counter.ModifiedOn = System.DateTime.Now;
                        _counter.ModifiedTerminal = obj.TerminalID;
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

        public async Task<DO_ReturnParameter> ActiveOrDeActiveTokenCounter(bool status, int businesskey, string counternumber,int floorId)
        {
            using (var db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        var _counter = db.GtTokm02s.Where(x => x.BusinessKey == businesskey && x.FloorId == floorId && x.CounterNumber.ToUpper().Replace(" ", "") == counternumber.ToUpper().Replace(" ", "")).FirstOrDefault();
                        if (_counter == null)
                        {
                            return new DO_ReturnParameter() { Status = false, StatusCode = "W0186", Message = string.Format(_localizer[name: "W0186"]) };
                        }

                        _counter.ActiveStatus = status;
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

        #region Counter Mapping 

        public async Task<List<DO_TokenConfiguration>> GetActiveTokensPrefix()
        {
            using (var db = new eSyaEnterprise())
            {
                try
                {
                    var tokens =  db.GtTokm01s.Where(x => x.ActiveStatus == true)

                                        .Select(t => new DO_TokenConfiguration
                                        {
                                            TokenPrefix = t.TokenPrefix,
                                            TokenDesc = t.TokenDesc
                                        }).OrderBy(x=>x.DisplaySequence).ToList();
                    return tokens;
                }

                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public async Task<List<DO_Floor>> GetActiveFloorsbyBusinessKey(int businesskey)
        {
            using (var db = new eSyaEnterprise())
            {
                try
                {
                    var floors = await db.GtTokm02s.Where(x => x.ActiveStatus == true && x.BusinessKey==businesskey).Join(db.GtEcapcds,
                        x=>new {x.FloorId},
                        y => new {FloorId=y.ApplicationCode}, (x, y) => new { x, y })
                                        .Select(t => new DO_Floor
                                        {
                                            FloorId = t.x.FloorId,
                                            FloorName = t.y.CodeDesc
                                        }).
                    GroupBy(y => y.FloorId, (key, grp) => grp.FirstOrDefault())
                  .ToListAsync();
                    return floors;
                }

                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public async Task<List<DO_CounterCreation>> GetCounterNumbersbyFloorId(int floorId)
        {
            using (var db = new eSyaEnterprise())
            {
                try
                {
                    var ds = await db.GtTokm02s.Where(x => x.FloorId == floorId && x.ActiveStatus == true).Select(
                          f => new DO_CounterCreation
                          {
                              CounterNumber = f.CounterNumber
                          }).ToListAsync();

                    return ds;
                }

                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public async Task<List<DO_CounterMapping>> GetCounterMappingbyBusinessKey(int businesskey)
        {
            using (var db = new eSyaEnterprise())
            {
                try
                {
                   
                    var ds = await db.GtTokm03s.Where(k => k.BusinessKey == businesskey).Join(db.GtTokm01s,
                               x => x.TokenPrefix.ToUpper().Replace(" ", ""),
                               y => y.TokenPrefix.ToUpper().Replace(" ", ""),
                               (x, y) => new { x, y }).Join(db.GtTokm02s,
                               a => new { a.x.CounterNumber, a.x.FloorId },
                               p => new { p.CounterNumber, p.FloorId },
                               (a, p) => new { a, p }).
                               Join(db.GtEcapcds,
                                b => new { b.p.FloorId },
                               c => new { FloorId = c.ApplicationCode }, (b, c) => new { b, c }).
                               Select(r => new DO_CounterMapping
                               {
                                   BusinessKey = r.b.a.x.BusinessKey,
                                   CounterNumber = r.b.a.x.CounterNumber,
                                   TokenPrefix = r.b.a.x.TokenPrefix,
                                   CounterKey= r.b.a.x.CounterKey,
                                   //FloorId = r.b.p.FloorId,
                                   ActiveStatus = r.b.a.x.ActiveStatus,
                                   TokenType = r.b.a.y.TokenType,
                                   TokenDesc = r.b.a.y.TokenDesc,
                                   //CounterNumberdesc = r.b.p.CounterNumber,
                                   FloorId = r.b.a.x.FloorId,
                                   CounterNumberdesc = r.b.a.x.CounterNumber,
                                   FloorName = r.c.CodeDesc,
                                   
                               }).OrderBy(x => x.FloorName).ThenBy(x => x.CounterNumber).ToListAsync();
                    return ds;


                }

                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public async Task<DO_ReturnParameter> InsertIntoCounterMapping(DO_CounterMapping obj)
        {
            using (var db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        var _map = db.GtTokm03s.Where(x => x.BusinessKey == obj.BusinessKey && x.TokenPrefix.ToUpper().Replace(" ", "") == obj.TokenPrefix.ToUpper().Replace(" ", "")
                       &&x.FloorId==obj.FloorId && x.CounterNumber.ToUpper().Replace(" ", "") == obj.CounterNumber.ToUpper().Replace(" ", "")).FirstOrDefault();
                        if (_map != null)
                        {
                            return new DO_ReturnParameter() { Status = false, StatusCode = "W0187", Message = string.Format(_localizer[name: "W0187"]) };
                        }

                        GtTokm03 t_map = new GtTokm03();
                        t_map.BusinessKey = obj.BusinessKey;
                        t_map.TokenPrefix = obj.TokenPrefix;
                        t_map.FloorId = obj.FloorId;
                        t_map.CounterNumber = obj.CounterNumber;
                        t_map.CounterKey = (obj.TokenPrefix+"-"+obj.FloorId+"-"+obj.CounterNumber).ToString();
                        t_map.ActiveStatus = obj.ActiveStatus;
                        t_map.FormId = obj.FormId;
                        t_map.CreatedBy = obj.UserID;
                        t_map.CreatedOn = System.DateTime.Now;
                        t_map.CreatedTerminal = obj.TerminalID;
                        db.GtTokm03s.Add(t_map);
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

        public async Task<DO_ReturnParameter> UpdateCounterMapping(DO_CounterMapping obj)
        {
            using (var db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        var t_map = db.GtTokm03s.Where(x => x.BusinessKey == obj.BusinessKey && x.TokenPrefix.ToUpper().Replace(" ", "") == obj.TokenPrefix.ToUpper().Replace(" ", "")
                        &&x.FloorId==obj.FloorId  && x.CounterNumber.ToUpper().Replace(" ", "") == obj.CounterNumber.ToUpper().Replace(" ", "")).FirstOrDefault();
                        if (t_map == null)
                        {
                            return new DO_ReturnParameter() { Status = false, StatusCode = "W0188", Message = string.Format(_localizer[name: "W0188"]) };
                        }
                        //t_map.CounterKey = (obj.TokenPrefix+"-"+obj.FloorId+"-"+obj.CounterNumber).ToString();
                        t_map.ActiveStatus = obj.ActiveStatus;
                        t_map.ModifiedBy = obj.UserID;
                        t_map.ModifiedOn = System.DateTime.Now;
                        t_map.ModifiedTerminal = obj.TerminalID;
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

        public async Task<DO_ReturnParameter> ActiveOrDeActiveCounterMapping(bool status, int businesskey, string tokenprefix, string counternumber,int floorId)
        {
            using (var db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        var t_map = db.GtTokm03s.Where(x => x.BusinessKey == businesskey && x.TokenPrefix.ToUpper().Replace(" ", "") == tokenprefix.ToUpper().Replace(" ", "")
                         && x.FloorId== floorId && x.CounterNumber.ToUpper().Replace(" ", "") == counternumber.ToUpper().Replace(" ", "")).FirstOrDefault();
                        if (t_map == null)
                        {
                            return new DO_ReturnParameter() { Status = false, StatusCode = "W0188", Message = string.Format(_localizer[name: "W0188"]) };
                        }
                        t_map.ActiveStatus = status;
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
