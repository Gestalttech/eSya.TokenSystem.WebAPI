using eSya.TokenSystem.DO;
using eSya.TokenSystem.IF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eSya.TokenSystem.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TokenGenerationController : ControllerBase
    {
        private readonly ITokenGenerationRepository _iTokenGenerationRepository;

        public TokenGenerationController(ITokenGenerationRepository iTokenGenerationRepository)
        {
            _iTokenGenerationRepository = iTokenGenerationRepository;

        }


        /// <summary>
        /// Generate Token
        /// UI Reffered -Token Generation
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> GenerateToken(DO_Token obj)
        {
            var msg = await _iTokenGenerationRepository.GenerateToken(obj);
            return Ok(msg);

        }

        [HttpPost]
        public async Task<IActionResult> GenerateOTP(DO_OTP obj)
        {
            var msg = await _iTokenGenerationRepository.GenerateOTP(obj);
            return Ok(msg);

        }
        [HttpPost]
        public async Task<IActionResult> ConfirmOTP(DO_OTP obj)
        {
            var msg = await _iTokenGenerationRepository.ConfirmOTP(obj);
            return Ok(msg);

        }
    }
}
