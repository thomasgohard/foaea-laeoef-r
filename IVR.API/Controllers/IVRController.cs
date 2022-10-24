using FOAEA3.Business.Areas.IVR;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces.Repository;
using FOAEA3.Model.IVR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace IVR.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class IVRController : ControllerBase
    {
        private readonly CustomConfig config;

        public IVRController(IOptions<CustomConfig> config)
        {
            this.config = config.Value;
        }

        [HttpGet("fpIVR_Check_Creditor_Id")]
        [AllowAnonymous]
        public async Task<ActionResult<CheckCreditorIdReturnData>> IVRCheckCreditorId(
                                               [FromBody] CheckCreditorIdGetData checkCreditorIdGetData,
                                               [FromServices] IIVRRepository ivrDB)
        {
            var manager = new IVRManager(ivrDB);

            var result = await manager.CheckCreditorId(checkCreditorIdGetData);

            if (result is not null)
                return Ok(result);
            else
                return NotFound();
        }

        [HttpGet("fpIVR_Check_Ctrl_Cd")]
        [AllowAnonymous]
        public async Task<ActionResult<CheckControlCodeReturnData>> IVRCheckControlCode(
                                               [FromBody] CheckControlCodeGetData checkControlCodeGetData,
                                               [FromServices] IIVRRepository ivrDB)
        {
            var manager = new IVRManager(ivrDB);

            var result = await manager.CheckControlCode(checkControlCodeGetData);

            if (result is not null)
                return Ok(result);
            else
                return NotFound();
        }

        [HttpGet("fpIVR_Check_Debtor_Id")]
        [AllowAnonymous]
        public async Task<ActionResult<CheckDebtorIdReturnData>> IVRCheckDebtorID(
                                               [FromBody] CheckDebtorIdGetData checkDebtorIdGetData,
                                               [FromServices] IIVRRepository ivrDB)
        {
            var manager = new IVRManager(ivrDB);

            var result = await manager.CheckDebtorId(checkDebtorIdGetData);

            if (result is not null)
                return Ok(result);
            else
                return NotFound();
        }

        [HttpGet("fpIVR_Check_Debtor_Letter")]
        [AllowAnonymous]
        public async Task<ActionResult<CheckDebtorIdReturnData>> IVRCheckDebtorLetter(
                                               [FromBody] CheckDebtorLetterGetData checkDebtorLetterGetData,
                                               [FromServices] IIVRRepository ivrDB)
        {
            var manager = new IVRManager(ivrDB);

            var result = await manager.CheckDebtorLetter(checkDebtorLetterGetData);

            if (result is not null)
                return Ok(result);
            else
                return NotFound();
        }

        [HttpGet("fpIVR_Check_Sin")]
        [AllowAnonymous]
        public async Task<ActionResult<CheckSinReturnData>> IVRCheckSin(
                                                [FromBody] CheckSinGetData sinCountGetData,
                                                [FromServices] IIVRRepository ivrDB)
        {
            var manager = new IVRManager(ivrDB);

            var result = await manager.GetSinCount(sinCountGetData);

            if (result is not null)
                return Ok(result);
            else
                return NotFound();
        }

        [HttpGet("fpIVR_Get_Agency")]
        [AllowAnonymous]
        public async Task<ActionResult<CheckSinReturnData>> IVRGetAgency(
                                                [FromBody] GetAgencyGetData getAgencyGetData,
                                                [FromServices] IIVRRepository ivrDB)
        {
            var manager = new IVRManager(ivrDB);

            var result = await manager.GetAgency(getAgencyGetData);

            if (result is not null)
                return Ok(result);
            else
                return NotFound();
        }

        [HttpGet("fpIVR_Get_Agency_Deb")]
        [AllowAnonymous]
        public async Task<ActionResult<CheckSinReturnData>> IVRGetAgencyDeb(
                                                [FromBody] GetAgencyDebGetData getAgencyDebGetData,
                                                [FromServices] IIVRRepository ivrDB)
        {
            var manager = new IVRManager(ivrDB);

            var result = await manager.GetAgencyDeb(getAgencyDebGetData);

            if (result is not null)
                return Ok(result);
            else
                return NotFound();
        }

        [HttpGet("fpIVR_Get_Appl_Ctrlcd")]
        [AllowAnonymous]
        public async Task<ActionResult<GetApplControlCodeReturnData>> IVRGetApplControlCode(
                                                [FromBody] GetApplControlCodeGetData getApplControlCodeGetData,
                                                [FromServices] IIVRRepository ivrDB)
        {
            var manager = new IVRManager(ivrDB);

            var result = await manager.GetApplControlCode(getApplControlCodeGetData);

            if (result is not null)
                return Ok(result);
            else
                return NotFound();
        }
        
        [HttpGet("fpIVR_Get_Appl_Enfsrv_Cd")]
        [AllowAnonymous]
        public async Task<ActionResult<GetApplEnforcementCodeReturnData>> IVRGetApplEnforcementCode(
                                                [FromBody] GetApplEnforcementCodeGetData getApplEnforcementCodeGetData,
                                                [FromServices] IIVRRepository ivrDB)
        {
            var manager = new IVRManager(ivrDB);

            var result = await manager.GetApplEnforcementCode(getApplEnforcementCodeGetData);

            if (result is not null)
                return Ok(result);
            else
                return NotFound();
        }

        [HttpGet("fpIVR_Get_HldbCnd")]
        [AllowAnonymous]
        public async Task<ActionResult<GetHoldbackConditionReturnData>> IVRGetHoldbackCondition(
                                        [FromBody] GetHoldbackConditionGetData getHoldbackConditionGetData,
                                        [FromServices] IIVRRepository ivrDB)
        {
            var manager = new IVRManager(ivrDB);

            var result = await manager.GetHoldbackCondition(getHoldbackConditionGetData);

            if (result is not null)
                return Ok(result);
            else
                return NotFound();
        }

        [HttpGet("fpIVR_Get_L01Agency")]
        [AllowAnonymous]
        public async Task<ActionResult<GetHoldbackConditionReturnData>> IVRGetL01Agency(
                                        [FromBody] GetL01AgencyGetData getL01AgencyGetData,
                                        [FromServices] IIVRRepository ivrDB)
        {
            var manager = new IVRManager(ivrDB);

            var result = await manager.GetL01Agency(getL01AgencyGetData);

            if (result is not null)
                return Ok(result);
            else
                return NotFound();
        }

        [HttpGet("fpIVR_Get_Payments")]
        [AllowAnonymous]
        public async Task<ActionResult<GetHoldbackConditionReturnData>> IVRGetPayments(
                                        [FromBody] GetPaymentsGetData getPaymentsGetData,
                                        [FromServices] IIVRRepository ivrDB)
        {
            var manager = new IVRManager(ivrDB);

            var result = await manager.GetPayments(getPaymentsGetData);

            if (result is not null)
                return Ok(result);
            else
                return NotFound();
        }

        [HttpGet("fpIVR_Get_Summons")]
        [AllowAnonymous]
        public async Task<ActionResult<GetHoldbackConditionReturnData>> IVRGetSummons(
                                        [FromBody] GetSummonsGetData getSummonsGetData,
                                        [FromServices] IIVRRepository ivrDB)
        {
            var manager = new IVRManager(ivrDB);

            var result = await manager.GetSummons(getSummonsGetData);

            if (result is not null)
                return Ok(result);
            else
                return NotFound();
        }
    }
}
