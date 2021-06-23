using Api.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(LogUserActivityActionFilter))]
    public class BaseApiController : ControllerBase
    {

    }
}
