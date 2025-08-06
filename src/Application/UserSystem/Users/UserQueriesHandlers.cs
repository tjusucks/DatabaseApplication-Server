using DbApp.Domain.Entities;
using DbApp.Domain.Interfaces;
using MediatR;

namespace DbApp.Application.UserSystem.Users;

public class GetAllUsersQueryHandler(IUserRepository userRepository) : IRequestHandler<GetAllUsersQuery, List<User>>
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<List<User>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        return await _userRepository.GetAllAsync();
    }
}

public class GetUserByIdQueryHandler(IUserRepository userRepository) : IRequestHandler<GetUserByIdQuery, User?>
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<User?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        return await _userRepository.GetByIdAsync(request.UserId);
    }
}
