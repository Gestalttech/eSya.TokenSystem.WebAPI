using eSya.TokenSystem.DO;
using eSya.TokenSystem.IF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eSya.TokenSystem.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CounterMappingController : ControllerBase
    {
        private readonly ICounterMappingRepository _iCounterMappingRepository;

        public CounterMappingController(ICounterMappingRepository iCounterMappingRepository)
        {
            _iCounterMappingRepository = iCounterMappingRepository;

        }

        #region Token Counter

        /// <summary>
        /// Getting  Floors by code Type.
        /// UI Reffered - Token Counter
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetFloorsbyFloorId(int codetype)
        {
            var floors = await _iCounterMappingRepository.GetFloorsbyFloorId(codetype);
            return Ok(floors);
        }


        /// <summary>
        /// Getting  Token Counter by Business key.
        /// UI Reffered - Token Counter
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetTokenCountersbyBusinessKey(int businesskey)
        {
            var _tgens = await _iCounterMappingRepository.GetTokenCountersbyBusinessKey(businesskey);
            return Ok(_tgens);
        }

        /// <summary>
        /// Insert Token Counter
        /// UI Reffered -Token Counter
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> InsertIntoTokenCounter(DO_CounterCreation obj)
        {
            var msg = await _iCounterMappingRepository.InsertIntoTokenCounter(obj);
            return Ok(msg);

        }

        /// <summary>
        /// Update Token Counter
        /// UI Reffered -Token Counter
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UpdateTokenCounter(DO_CounterCreation obj)
        {
            var msg = await _iCounterMappingRepository.UpdateTokenCounter(obj);
            return Ok(msg);

        }

        /// <summary>
        /// Active DeActive Token Counter
        /// UI Reffered -Token Counter
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ActiveOrDeActiveTokenCounter(bool status, int businesskey, string counternumber, int floorId)
        {
            var msg = await _iCounterMappingRepository.ActiveOrDeActiveTokenCounter(status, businesskey, counternumber, floorId);
            return Ok(msg);

        }
        #endregion


        #region Token Mapping

        /// <summary>
        /// Getting  Active Token Prefix for dropdown.
        /// UI Reffered - Counter Mapping
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetActiveTokensPrefix()
        {
            var _cnos = await _iCounterMappingRepository.GetActiveTokensPrefix();
            return Ok(_cnos);
        }

        /// <summary>
        /// Getting  Active Floors by Business Key for dropdown.
        /// UI Reffered - Counter Mapping
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetActiveFloorsbyBusinessKey(int businesskey)
        {
            var floors = await _iCounterMappingRepository.GetActiveFloorsbyBusinessKey(businesskey);
            return Ok(floors);
        }
        /// <summary>
        /// Getting  Counter Numbers by Floor Id.
        /// UI Reffered - Counter Mapping
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCounterNumbersbyFloorId(int floorId)
        {
            var _cnos = await _iCounterMappingRepository.GetCounterNumbersbyFloorId(floorId);
            return Ok(_cnos);
        }

        /// <summary>
        /// Getting  Counter Mapping by Business key.
        /// UI Reffered - Counter Mapping
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCounterMappingbyBusinessKey(int businesskey)
        {
            var _tgens = await _iCounterMappingRepository.GetCounterMappingbyBusinessKey(businesskey);
            return Ok(_tgens);
        }

        /// <summary>
        /// Insert Counter Mapping.
        /// UI Reffered -Counter Mapping
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> InsertIntoCounterMapping(DO_CounterMapping obj)
        {
            var msg = await _iCounterMappingRepository.InsertIntoCounterMapping(obj);
            return Ok(msg);

        }

        /// <summary>
        /// Update Counter Mapping.
        /// UI Reffered -Counter Mapping
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UpdateCounterMapping(DO_CounterMapping obj)
        {
            var msg = await _iCounterMappingRepository.UpdateCounterMapping(obj);
            return Ok(msg);

        }

        /// <summary>
        /// Active DeActive Counter Mapping.
        /// UI Reffered -Counter Mapping
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ActiveOrDeActiveCounterMapping(bool status, int businesskey, string tokenprefix, string counternumber, int floorId)
        {
            var msg = await _iCounterMappingRepository.ActiveOrDeActiveCounterMapping(status, businesskey, tokenprefix, counternumber, floorId);
            return Ok(msg);

        }
        #endregion 
    }
}
