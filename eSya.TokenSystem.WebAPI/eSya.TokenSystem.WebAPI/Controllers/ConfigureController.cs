using eSya.TokenSystem.DO;
using eSya.TokenSystem.IF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eSya.TokenSystem.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ConfigureController : ControllerBase
    {
        private readonly ITokenConfigurationRepository _TokenConfigurationRepository;

        public ConfigureController(ITokenConfigurationRepository TokenConfigurationRepository)
        {
            _TokenConfigurationRepository = TokenConfigurationRepository;

        }
        #region Token Configuration

        /// <summary>
        /// Getting Active Tokens for dropdown.
        /// UI Reffered - Token Generation
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetActiveTokens()
        {
            var t_cnfg = await _TokenConfigurationRepository.GetActiveTokens();
            return Ok(t_cnfg);
        }

        /// <summary>
        /// Getting All Token Configuration.
        /// UI Reffered - Token Configure
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllConfigureTokens()
        {
            var t_cnfg = await _TokenConfigurationRepository.GetAllConfigureTokens();
            return Ok(t_cnfg);
        }

        /// <summary>
        /// Insert Token Configuration.
        /// UI Reffered -Token Configure
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> InsertIntoToken(DO_TokenConfiguration obj)
        {
            var msg = await _TokenConfigurationRepository.InsertIntoToken(obj);
            return Ok(msg);

        }

        /// <summary>
        /// Update Token Configuration.
        /// UI Reffered -Token Configure
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UpdateToken(DO_TokenConfiguration obj)
        {
            var msg = await _TokenConfigurationRepository.UpdateToken(obj);
            return Ok(msg);

        }

        /// <summary>
        /// Active DeActive Token Configuration.
        /// UI Reffered -Token Configure
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ActiveOrDeActiveToken(bool status, string tokentype)
        {
            var msg = await _TokenConfigurationRepository.ActiveOrDeActiveToken(status, tokentype);
            return Ok(msg);

        }
        #endregion Questionaire Group
    }
}
