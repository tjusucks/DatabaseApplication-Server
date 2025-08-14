using DbApp.Domain.Entities;
using DbApp.Domain.Interfaces;
using MediatR;

namespace DbApp.Application.UserSystem.Users;

public class CreateUserCommandHandler(IUserRepository userRepository) : IRequestHandler<CreateUserCommand, int>
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<int> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Username = request.Username,
            CreatedAt = DateTime.UtcNow
        };

        return await _userRepository.CreateAsync(user);
    }
}

public class UpdateUserCommandHandler(IUserRepository userRepository) : IRequestHandler<UpdateUserCommand, Unit>
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId)
            ?? throw new InvalidOperationException("User not found");

        user.Username = request.Username;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
        return Unit.Value;
    }
}

public class DeleteUserCommandHandler(IUserRepository userRepository) : IRequestHandler<DeleteUserCommand, Unit>
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId)
            ?? throw new InvalidOperationException("User not found");
        await _userRepository.DeleteAsync(user);
        return Unit.Value;
    }
}
