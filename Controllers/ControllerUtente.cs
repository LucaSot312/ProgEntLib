using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProgEntLib.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[utente]")]
    public class ControllerUtente
    {

    }
}
