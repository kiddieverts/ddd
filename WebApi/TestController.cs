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
        private readonly RenameRecording.Handler _renameRecordinghandler;

        public TestController(CreateRecording.Handler handler, RenameRecording.Handler renameRecordinghandler)
        {
            _createRecordinghandler = handler;
            _renameRecordinghandler = renameRecordinghandler;
        }

        [HttpGet]
        public async Task<ActionResult<string>> CreateRecording()
        {
            var command = new CreateRecording.Command(Guid.NewGuid(), "Ok Computer", "Radiohead", 1997);

            await _createRecordinghandler.Handle(command);

            return "ok";
        }

        [HttpGet("rename")]
        public async Task<ActionResult<string>> RenameRecording([FromQuery] Guid id)
        {
            var command = new RenameRecording.Command(id, "Test");

            await _renameRecordinghandler.Handle(command);

            return "ok";
        }
    }
}
