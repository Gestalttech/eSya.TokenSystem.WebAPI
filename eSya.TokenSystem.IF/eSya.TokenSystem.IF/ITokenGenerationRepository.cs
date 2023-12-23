using eSya.TokenSystem.DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSya.TokenSystem.IF
{
    public interface ITokenGenerationRepository
    {
        Task<DO_ReturnParameter> GenerateToken(DO_Token obj);
        Task<DO_ReturnParameter> GenerateOTP(DO_OTP obj);
        Task<DO_ReturnParameter> ConfirmOTP(DO_OTP obj);
    }
}
