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
                    var ds = await db.GtTokm03s.Where(r => r.BusinessKey == businesskey).Join(db.GtEcapcds,
                            x => x.FloorId,
                            y => y.ApplicationCode,
                            (x, y) => new DO_CounterCreation
                            {
                                BusinessKey = x.BusinessKey,
                                CounterNumber = x.CounterNumber,
                                ActiveStatus = x.ActiveStatus,
                                FloorId = x.FloorId,
                                FloorName = y.CodeDesc
                            }).ToListAsync();

                    return ds;
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
                        var _counter = db.GtTokm03s.Where(x => x.BusinessKey == obj.BusinessKey && x.CounterNumber.ToUpper().Replace(" ", "") == obj.CounterNumber.ToUpper().Replace(" ", "")).FirstOrDefault();
                        if (_counter != null)
                        {
                            return new DO_ReturnParameter() { Status = false, StatusCode = "W0185", Message = string.Format(_localizer[name: "W0185"]) };
                        }

                        GtTokm03 counter = new GtTokm03();
                        counter.BusinessKey = obj.BusinessKey;
                        counter.CounterNumber = obj.CounterNumber;
                        counter.ActiveStatus = obj.ActiveStatus;
                        counter.FloorId = obj.FloorId;
                        counter.FormId = obj.FormId;
                        counter.CreatedBy = obj.UserID;
                        counter.CreatedOn = System.DateTime.Now;
                        counter.CreatedTerminal = obj.TerminalID;
                        db.GtTokm03s.Add(counter);
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
                        var _counter = db.GtTokm03s.Where(x => x.BusinessKey == obj.BusinessKey && x.CounterNumber.ToUpper().Replace(" ", "") == obj.CounterNumber.ToUpper().Replace(" ", "")).FirstOrDefault();
                        if (_counter == null)
                        {
                            return new DO_ReturnParameter() { Status = false, StatusCode = "W0186", Message = string.Format(_localizer[name: "W0186"]) };
                        }
                        _counter.FloorId = obj.FloorId;
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

        public async Task<DO_ReturnParameter> ActiveOrDeActiveTokenCounter(bool status, int businesskey, string counternumber)
        {
            using (var db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        var _counter = db.GtTokm03s.Where(x => x.BusinessKey == businesskey && x.CounterNumber.ToUpper().Replace(" ", "") == counternumber.ToUpper().Replace(" ", "")).FirstOrDefault();
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
        public async Task<List<DO_CounterCreation>> GetCounterNumbersbyFloorId(int floorId)
        {
            using (var db = new eSyaEnterprise())
            {
                try
                {
                    var ds = await db.GtTokm03s.Where(x => x.FloorId == floorId && x.ActiveStatus == true).Select(
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
                    var ds = await db.GtTokm02s.Where(k => k.BusinessKey == businesskey).Join(db.GtTokm01s,
                               x => x.TokenType.ToUpper().Replace(" ", ""),
                               y => y.TokenType.ToUpper().Replace(" ", ""),
                               (x, y) => new { x, y }).Join(db.GtTokm03s,
                               a => a.x.CounterNumber.ToUpper().Replace(" ", ""),
                               p => p.CounterNumber.ToUpper().Replace(" ", ""), (a, p) => new { a, p }).Join(db.GtEcapcds,
                                //b => b.a.x.FloorId,
                                b => b.p.FloorId,
                               c => c.ApplicationCode, (b, c) => new { b, c }).Select(r => new DO_CounterMapping
                               {
                                   BusinessKey = r.b.a.x.BusinessKey,
                                   CounterNumber = r.b.a.x.CounterNumber,
                                   TokenType = r.b.a.x.TokenType,
                                   //FloorId = r.b.a.x.FloorId,
                                   FloorId = r.b.p.FloorId,
                                   ActiveStatus = r.b.a.x.ActiveStatus,
                                   TokenDesc = r.b.a.y.TokenDesc,
                                   CounterNumberdesc = r.b.p.CounterNumber,
                                   FloorName = r.c.CodeDesc
                               }).ToListAsync();

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
                        var _map = db.GtTokm02s.Where(x => x.BusinessKey == obj.BusinessKey && x.TokenType.ToUpper().Replace(" ", "") == obj.TokenType.ToUpper().Replace(" ", "")
                        && x.CounterNumber.ToUpper().Replace(" ", "") == obj.CounterNumber.ToUpper().Replace(" ", "")).FirstOrDefault();
                        if (_map != null)
                        {
                            return new DO_ReturnParameter() { Status = false, StatusCode = "W0187", Message = string.Format(_localizer[name: "W0187"]) };
                        }

                        GtTokm02 t_map = new GtTokm02();
                        t_map.BusinessKey = obj.BusinessKey;
                        t_map.TokenType = obj.TokenType;
                        t_map.CounterNumber = obj.CounterNumber;
                        //t_map.FloorId = obj.FloorId;
                        t_map.ActiveStatus = obj.ActiveStatus;
                        t_map.FormId = obj.FormId;
                        t_map.CreatedBy = obj.UserID;
                        t_map.CreatedOn = System.DateTime.Now;
                        t_map.CreatedTerminal = obj.TerminalID;
                        db.GtTokm02s.Add(t_map);
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
                        var t_map = db.GtTokm02s.Where(x => x.BusinessKey == obj.BusinessKey && x.TokenType.ToUpper().Replace(" ", "") == obj.TokenType.ToUpper().Replace(" ", "")
                       && x.CounterNumber.ToUpper().Replace(" ", "") == obj.CounterNumber.ToUpper().Replace(" ", "")).FirstOrDefault();
                        if (t_map == null)
                        {
                            return new DO_ReturnParameter() { Status = false, StatusCode = "W0188", Message = string.Format(_localizer[name: "W0188"]) };
                        }
                        //t_map.FloorId = obj.FloorId;
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

        public async Task<DO_ReturnParameter> ActiveOrDeActiveCounterMapping(bool status, int businesskey, string tokentype, string counternumber)
        {
            using (var db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        var t_map = db.GtTokm02s.Where(x => x.BusinessKey == businesskey && x.TokenType.ToUpper().Replace(" ", "") == tokentype.ToUpper().Replace(" ", "")
                           && x.CounterNumber.ToUpper().Replace(" ", "") == counternumber.ToUpper().Replace(" ", "")).FirstOrDefault();
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
