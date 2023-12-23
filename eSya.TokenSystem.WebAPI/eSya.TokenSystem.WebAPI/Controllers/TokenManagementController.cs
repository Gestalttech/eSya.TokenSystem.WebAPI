using eSya.TokenSystem.DO;
using eSya.TokenSystem.IF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eSya.TokenSystem.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TokenManagementController : ControllerBase
    {
        private readonly ITokenManagementRepository _iTokenManagementRepository;

        public TokenManagementController(ITokenManagementRepository iTokenManagementRepository)
        {
            _iTokenManagementRepository = iTokenManagementRepository;

        }


        /// <summary>
        /// Get Token Details By Type
        /// UI Reffered -Token Management Counter
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetTokenDetailByTokenType(int businessKey, string tokenType)
        {
            var msg = await _iTokenManagementRepository.GetTokenDetailByTokenType(businessKey, tokenType);
            return Ok(msg);
        }

        /// <summary>
        /// Get Token Types By Counter
        /// UI Reffered -Token Management Counter
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetTokenTypeByCounter(int businessKey, string counterNumber)
        {
            var msg = await _iTokenManagementRepository.GetTokenTypeByCounter(businessKey, counterNumber);
            return Ok(msg);
        }

        /// <summary>
        /// Get Token Details By Mobile Number
        /// UI Reffered - Token Generation By Mobile
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetTokenDetailByMobile(int businessKey, int isdCode, string mobileNumber)
        {
            var msg = await _iTokenManagementRepository.GetTokenDetailByMobile(businessKey, isdCode, mobileNumber);
            return Ok(msg);
        }

        /// <summary>
        /// Token Calling
        /// UI Reffered -Token Management Counter 
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UpdateCallingToken(DO_Token obj)
        {
            var msg = await _iTokenManagementRepository.UpdateCallingToken(obj);
            return Ok(msg);
        }

        /// <summary>
        /// Token Holding
        /// UI Reffered -Token Management Counter 
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UpdateTokenToHold(DO_Token obj)
        {
            var msg = await _iTokenManagementRepository.UpdateTokenToHold(obj);
            return Ok(msg);
        }

        /// <summary>
        /// Token Releasing
        /// UI Reffered -Token Management Counter 
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UpdateTokenToRelease(DO_Token obj)
        {
            var msg = await _iTokenManagementRepository.UpdateTokenToRelease(obj);
            return Ok(msg);
        }

        /// <summary>
        /// Token Completing
        /// UI Reffered -Token Management Counter 
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UpdateTokenStatusToCompleted(DO_Token obj)
        {
            var msg = await _iTokenManagementRepository.UpdateTokenStatusToCompleted(obj);
            return Ok(msg);
        }

        /// <summary>
        /// Next Token Calling
        /// UI Reffered -Token Management Counter 
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UpdateToCallingNextToken(DO_Token obj)
        {
            var msg = await _iTokenManagementRepository.UpdateToCallingNextToken(obj);
            return Ok(msg);
        }

        /// <summary>
        /// Calling Cconfirmation
        /// UI Reffered -Token Management Counter 
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UpdateCallingConfirmation(DO_Token obj)
        {
            var msg = await _iTokenManagementRepository.UpdateCallingConfirmation(obj);
            return Ok(msg);
        }
    }
}
