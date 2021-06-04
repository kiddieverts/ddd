using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DDDExperiment
{
    [ApiController]
    public class ApiControllerBase : ControllerBase
    {
        protected readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;

        public ApiControllerBase(IMediator mediator, IUnitOfWork unitOfWork)
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }

        protected async Task<Result<Unit>> Commit(Unit u) => await _unitOfWork.Commit();
    }
}