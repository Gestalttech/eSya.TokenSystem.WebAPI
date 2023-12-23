using eSya.TokenSystem.DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSya.TokenSystem.IF
{
    public interface IDisplaySystemRepository
    {
        Task<List<DO_Token>> GetTokenForCounterDisplay(int businessKey, List<string> counterList);

        #region Display_IP_Config
        Task<DO_ReturnParameter> InsertUpdateDisplayConfig(DO_DisplaySystemConfig obj);
        Task<DO_DisplaySystemConfig> GetDisplayConfigByID(int DisplayId);
        Task<DO_DisplaySystemConfig> GetDisplayConfigByIPAdddress(string ipAddress);
        Task<List<DO_DisplaySystemConfig>> GetDisplayIPList();
        Task<DO_ReturnParameter> DeleteDisplayIPByID(DO_DisplaySystemConfig obj);
        #endregion
    }
}
