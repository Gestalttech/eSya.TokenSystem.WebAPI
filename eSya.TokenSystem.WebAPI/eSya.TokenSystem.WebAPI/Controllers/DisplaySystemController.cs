using eSya.TokenSystem.DO;
using eSya.TokenSystem.IF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eSya.TokenSystem.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DisplaySystemController : ControllerBase
    {
        private readonly IDisplaySystemRepository _iDisplaySystemRepository;

        public DisplaySystemController(IDisplaySystemRepository iDisplaySystemRepository)
        {
            _iDisplaySystemRepository = iDisplaySystemRepository;

        }


        /// <summary>
        /// Get Token For Counter Display
        /// UI Reffered -Token Display System
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetTokenForCounterDisplay(int businessKey, string arrayOfCounterList)
        {
            List<string> counterList = arrayOfCounterList.Split(new char[] { ',' }, StringSplitOptions.None).ToList();
            var msg = await _iDisplaySystemRepository.GetTokenForCounterDisplay(businessKey, counterList);
            return Ok(msg);
        }

        #region Display_IP_Config
        [HttpPost]
        public async Task<IActionResult> InsertUpdateDisplayConfig(DO_DisplaySystemConfig obj)
        {
            var rs = await _iDisplaySystemRepository.InsertUpdateDisplayConfig(obj);
            return Ok(rs);
        }
        [HttpGet]
        public async Task<IActionResult> GetDisplayConfigByID(int DisplayId)
        {
            var rs = await _iDisplaySystemRepository.GetDisplayConfigByID(DisplayId);
            return Ok(rs);
        }
        [HttpGet]
        public async Task<IActionResult> GetDisplayConfigByIPAdddress(string ipAddress)
        {
            var rs = await _iDisplaySystemRepository.GetDisplayConfigByIPAdddress(ipAddress);
            return Ok(rs);
        }
        [HttpGet]
        public async Task<IActionResult> GetDisplayIPList()
        {
            var rs = await _iDisplaySystemRepository.GetDisplayIPList();
            return Ok(rs);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteDisplayIPByID(DO_DisplaySystemConfig obj)
        {
            var rs = await _iDisplaySystemRepository.DeleteDisplayIPByID(obj);
            return Ok(rs);
        }
        #endregion
    }
}
