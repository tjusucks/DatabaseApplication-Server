using AutoMapper;

namespace DbApp.Application;

/// <summary>
/// Marker interface for automatic MediatR registration.
/// </summary>
public interface IMediatorModule
{
}

/// <summary>
/// Marker class for automatic AutoMapper profiles registration.
/// </summary>
public class MappingProfile : Profile
{
}
