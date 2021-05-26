using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MyRental.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly CreateRecording.Handler _createRecordinghandler;

        public TestController(CreateRecording.Handler handler)
        {
            _createRecordinghandler = handler;
        }

        [HttpGet]
        public async Task<ActionResult<string>> CreateRecording()
        {
            var command = new CreateRecording.Command(Guid.NewGuid(), "Ok Computer", "Radiohead", 1997);

            await _createRecordinghandler.Handle(command);

            return "ok";
        }
    }
}
