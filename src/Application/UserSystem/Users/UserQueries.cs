using DbApp.Domain.Entities.UserSystem;
using MediatR;

namespace DbApp.Application.UserSystem.Users;

public record GetAllUsersQuery : IRequest<List<User>>;

public record GetUserByIdQuery(int UserId) : IRequest<User?>;
