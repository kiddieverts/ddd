using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MyRental
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;

        public BaseController(IMediator mediator, IUnitOfWork unitOfWork)
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }

        protected async Task<Result<Unit>> Commit(Unit u) => await _unitOfWork.Commit();
    }
}