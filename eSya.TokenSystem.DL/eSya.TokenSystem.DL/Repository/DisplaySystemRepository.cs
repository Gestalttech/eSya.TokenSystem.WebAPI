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
    public class DisplaySystemRepository: IDisplaySystemRepository
    {
        private readonly IStringLocalizer<DisplaySystemRepository> _localizer;
        public DisplaySystemRepository(IStringLocalizer<DisplaySystemRepository> localizer)
        {
            _localizer = localizer;
        }
        public async Task<List<DO_Token>> GetTokenForCounterDisplay(int businessKey, List<string> counterList)
        {
            using (var db = new eSyaEnterprise())
            {
                try
                {
                    var ds = db.GtTokm04s
                        .Where(w => w.BusinessKey == businessKey
                            && w.TokenDate.Date == System.DateTime.Now.Date
                            && counterList.Contains(w.CallingCounter)
                            && w.TokenStatus == "RG"
                            && w.TokenCalling
                            && w.ActiveStatus)
                         .AsNoTracking()
                        .Select(r => new DO_Token
                        {
                            TokenKey = r.TokenKey,
                            TokenDate = r.TokenDate,
                            SequeueNumber = r.SequeueNumber,
                            TokenHold = r.TokenHold,
                            TokenCalling = r.TokenCalling,
                            CallingCounter = r.CallingCounter,
                            TokenCallingTime = r.TokenCallingTime,
                        }).OrderBy(o => o.TokenCallingTime).ToListAsync();

                    return await ds;

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        #region Display_IP_Config
        public async Task<DO_ReturnParameter> InsertUpdateDisplayConfig(DO_DisplaySystemConfig obj)
        {
            using (var db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (obj.ActiveStatus)
                        {
                            var ipexist = db.GtQsdssies.Where(w => w.BusinessKey == obj.BusinessKey && w.DisplayIpaddress == obj.DisplayIPAddress).Count();
                            if (ipexist > 0)
                            {
                                var updatestatus = db.GtQsdssies.Where(w => w.BusinessKey == obj.BusinessKey && w.DisplayIpaddress == obj.DisplayIPAddress).ToList();
                                foreach (GtQsdssy r in updatestatus)
                                {
                                    r.ActiveStatus = false;
                                }
                            }
                        }

                        if (obj.DisplayId == 0)
                        {
                            var newDisplayId = db.GtQsdssies.Select(a => a.DisplayId).DefaultIfEmpty(0).Max() + 1;
                            //var newSerial = _context.GtQsdssy.Where(w => w.BusinessKey == obj.BusinessKey && w.DisplayIpaddress == obj.DisplayIPAddress).Select(a => a.SerialNumber).DefaultIfEmpty(0).Max() + 1;

                            var displayIP = new GtQsdssy
                            {
                                DisplayId = newDisplayId,
                                BusinessKey = obj.BusinessKey,
                                DisplayIpaddress = obj.DisplayIPAddress,
                                DisplayScreenType = obj.DisplayScreenType,
                                DisplayUrl = obj.DisplayURL,
                                QueryString = obj.QueryString,
                                ActiveStatus = obj.ActiveStatus,
                                FormId = obj.FormID,
                                CreatedTerminal = obj.TerminalID,
                                CreatedBy = obj.UserID,
                                CreatedOn = obj.CreatedOn
                            };
                            db.GtQsdssies.Add(displayIP);
                        }
                        else
                        {

                            var updateddisplayIP = db.GtQsdssies.Where(w => w.DisplayId == obj.DisplayId).FirstOrDefault();

                            updateddisplayIP.DisplayIpaddress = obj.DisplayIPAddress;
                            updateddisplayIP.DisplayScreenType = obj.DisplayScreenType;
                            updateddisplayIP.DisplayUrl = obj.DisplayURL;
                            updateddisplayIP.QueryString = obj.QueryString;
                            updateddisplayIP.ActiveStatus = obj.ActiveStatus;
                            updateddisplayIP.ModifiedTerminal = obj.TerminalID;
                            updateddisplayIP.ModifiedBy = obj.UserID;
                            updateddisplayIP.ModifiedOn = obj.CreatedOn;

                        }
                        await db.SaveChangesAsync();
                        dbContext.Commit();

                        return new DO_ReturnParameter() { Status = true, StatusCode = "S0001", Message = string.Format(_localizer[name: "S0001"]) };
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
        public async Task<DO_DisplaySystemConfig> GetDisplayConfigByID(int DisplayId)
        {
            using (var db = new eSyaEnterprise())
            {
                try
                {
                    {
                        var ds = db.GtQsdssies.Where(w => w.DisplayId == DisplayId)
                            .Select(r => new DO_DisplaySystemConfig
                            {
                                DisplayIPAddress = r.DisplayIpaddress,
                                DisplayScreenType = r.DisplayScreenType,
                                DisplayURL = r.DisplayUrl,
                                QueryString = r.QueryString,
                                ActiveStatus = r.ActiveStatus
                            }).FirstOrDefaultAsync();

                        return await ds;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public async Task<DO_DisplaySystemConfig> GetDisplayConfigByIPAdddress(string ipAddress)
        {
            using (var db = new eSyaEnterprise())
            {
                try
                {
                    var ds = db.GtQsdssies.Where(w => w.DisplayIpaddress == ipAddress && w.ActiveStatus)
                        .Select(r => new DO_DisplaySystemConfig
                        {
                            DisplayIPAddress = r.DisplayIpaddress,
                            DisplayScreenType = r.DisplayScreenType,
                            DisplayURL = r.DisplayUrl,
                            QueryString = r.QueryString,
                            ActiveStatus = r.ActiveStatus
                        }).FirstOrDefaultAsync();

                    return await ds;

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public async Task<List<DO_DisplaySystemConfig>> GetDisplayIPList()
        {
            using (var db = new eSyaEnterprise())
            {
                try
                {
                    var ds = db.GtQsdssies
                        .Select(r => new DO_DisplaySystemConfig
                        {
                            DisplayId = r.DisplayId,
                            DisplayIPAddress = r.DisplayIpaddress,
                            DisplayScreenType = r.DisplayScreenType,
                            DisplayURL = r.DisplayUrl,
                            QueryString = r.QueryString,
                            ActiveStatus = r.ActiveStatus
                        }).ToListAsync();

                    return await ds;

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public async Task<DO_ReturnParameter> DeleteDisplayIPByID(DO_DisplaySystemConfig obj)
        {
            using (var db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        GtQsdssy displayIP = db.GtQsdssies.Where(w => w.DisplayId == obj.DisplayId).FirstOrDefault();
                        if (displayIP == null)
                        {
                            return new DO_ReturnParameter() { Status = false, StatusCode = "W0189", Message = string.Format(_localizer[name: "W0189"]) };
                        }

                        db.GtQsdssies.Remove(displayIP);

                        await db.SaveChangesAsync();
                        dbContext.Commit();

                        return new DO_ReturnParameter() { Status = true, StatusCode = "S0005", Message = string.Format(_localizer[name: "S0005"]) };
                    }
                    catch (Exception ex)
                    {
                        dbContext.Rollback();
                        throw ex;
                    }
                }
            }
        }
        
        #endregion
    }
}
