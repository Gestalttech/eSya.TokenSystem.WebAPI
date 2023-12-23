using eSya.TokenSystem.DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSya.TokenSystem.IF
{
    public interface ITokenManagementRepository
    {
        Task<List<DO_Token>> GetTokenDetailByTokenType(int businessKey, string tokenType);
        Task<List<DO_CounterMapping>> GetTokenTypeByCounter(int businessKey, string counterNumber);
        Task<List<DO_Token>> GetTokenDetailByMobile(int businessKey, int isdCode, string mobileNumber);
        Task<DO_ReturnParameter> UpdateCallingToken(DO_Token obj);
        Task<DO_ReturnParameter> UpdateTokenToHold(DO_Token obj);
        Task<DO_ReturnParameter> UpdateTokenToRelease(DO_Token obj);
        Task<DO_ReturnParameter> UpdateTokenStatusToCompleted(DO_Token obj);
        Task<DO_ReturnParameter> UpdateToCallingNextToken(DO_Token obj);
        Task<DO_ReturnParameter> UpdateCallingConfirmation(DO_Token obj);
    }
}
