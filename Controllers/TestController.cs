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
        private readonly UnitOfWork _unitOfWork;

        public TestController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<string>> CreateRecording()
        {
            Console.WriteLine("!!!!!!!!!!!!");
            var id = Guid.NewGuid();
            var name = "Ok Computer";
            var artist = "Radiohead";
            var year = 1997;
            var agg = RecordingAggregate.Create(id, name, artist, year);
            await _unitOfWork.Save(agg);
            var isSuccess = false;
            try
            {
                await _unitOfWork.Commit();
                isSuccess = true;
            }
            catch
            {
                isSuccess = false;
            }

            Console.WriteLine(isSuccess ? "Saving success" : "Saving failed");

            if (isSuccess)
            {
                var agg2 = _unitOfWork.RecordingRepo.GetById(id);
                Console.WriteLine("FROM DB ... " + agg2.Name);
            }

            Console.WriteLine("Events uncommitted ... " + agg.GetUncommittedEvents().Count());

            return "ok";
        }
    }
}
