using MediatR;

namespace DbApp.Application.UserSystem.Users;

public record CreateUserCommand(string Username) : IRequest<int>;

public record UpdateUserCommand(int UserId, string Username) : IRequest<Unit>;

public record DeleteUserCommand(int UserId) : IRequest<Unit>;
