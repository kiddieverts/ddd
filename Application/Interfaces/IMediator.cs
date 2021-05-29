using System.Threading.Tasks;

namespace MyRental
{
    public interface ICommand { }
    public interface IQuery<T> { }
    public interface ICommandHandler<ICommand>
    {
        Task<Result<Unit>> Handle(ICommand command);
    }

    public interface IMediator
    {
        Task<Result<Unit>> Command(ICommand command);
        Task<Result<T>> Query<T>(IQuery<T> query);
    }
}