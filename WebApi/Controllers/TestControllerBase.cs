using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("{tenant}/[controller]/[action]")]
    public class TestControllerBase : ControllerBase
    {
    }
}