using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FileBroker.API.MEP.Interception.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize(Roles = "MEPinterception,System")]
    public class ESDfilesController : ControllerBase
    {

    }
}
