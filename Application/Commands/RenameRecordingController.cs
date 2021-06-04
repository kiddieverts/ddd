using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MyRental
{
    public class RenameRecordingController : ApiControllerBase
    {
        public RenameRecordingController(IMediator mediator, IUnitOfWork unitOfWork) : base(mediator, unitOfWork) { }

        [HttpPost("recordings/rename")]
        public async Task<ActionResult<Unit>> RenameRecording([FromBody] RenameRecording.Command command)
        {
            var result = await _mediator.Command(command).SelectMany(Commit);
            return result.ToActionResult(this);
        }
    }
}