using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DDDExperiment
{
    public class CreateRecordingController : ApiControllerBase
    {
        public CreateRecordingController(IMediator mediator, IUnitOfWork unitOfWork) : base(mediator, unitOfWork) { }

        [HttpPost]
        [Route("recordings")]
        public async Task<ActionResult<Unit>> CreateRecording([FromBody] CreateRecording.Command command)
        {
            var result = await _mediator.Command(command).SelectMany(Commit);
            return result.ToActionResult(this);
        }
    }
}