using eSya.TokenSystem.DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSya.TokenSystem.IF
{
    public interface ICounterMappingRepository
    {
        #region Counter Creation
        Task<List<DO_Floor>> GetFloorsbyFloorId(int codetype);

        Task<List<DO_CounterCreation>> GetTokenCountersbyBusinessKey(int businesskey);

        Task<DO_ReturnParameter> InsertIntoTokenCounter(DO_CounterCreation obj);

        Task<DO_ReturnParameter> UpdateTokenCounter(DO_CounterCreation obj);

        Task<DO_ReturnParameter> ActiveOrDeActiveTokenCounter(bool status, int businesskey, string counternumber);

        #endregion

        #region Counter Mapping

        Task<List<DO_CounterCreation>> GetCounterNumbersbyFloorId(int floorId);

        Task<List<DO_CounterMapping>> GetCounterMappingbyBusinessKey(int businesskey);

        Task<DO_ReturnParameter> InsertIntoCounterMapping(DO_CounterMapping obj);

        Task<DO_ReturnParameter> UpdateCounterMapping(DO_CounterMapping obj);

        Task<DO_ReturnParameter> ActiveOrDeActiveCounterMapping(bool status, int businesskey, string tokentype, string counternumber);
        #endregion
    }
}
