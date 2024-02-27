using eSya.TokenSystem.DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSya.TokenSystem.IF
{
    public interface ITokenConfigurationRepository
    {
        #region Token Configuration
        Task<List<DO_TokenConfiguration>> GetActiveTokens();

        Task<List<DO_TokenConfiguration>> GetAllConfigureTokens();

        Task<DO_ReturnParameter> InsertIntoToken(DO_TokenConfiguration obj);

        Task<DO_ReturnParameter> UpdateToken(DO_TokenConfiguration obj);

        Task<DO_ReturnParameter> ActiveOrDeActiveToken(bool status, string tokentype, string tokenPrefix);

        #endregion
    }
}
