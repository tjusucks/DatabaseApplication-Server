using AutoMapper;
using DbApp.Domain.Interfaces.UserSystem;
using DbApp.Domain.Specifications.Common;
using DbApp.Domain.Specifications.UserSystem;
using MediatR;

namespace DbApp.Application.UserSystem.Visitors;

/// <summary>
/// Centralized handler for all visitor queries.
/// </summary>
public class VisitorQueryHandlers(IVisitorRepository visitorRepo, IMapper mapper) :
    IRequestHandler<GetVisitorByIdQuery, VisitorDto?>,
    IRequestHandler<GetAllVisitorsQuery, List<VisitorDto>>,
    IRequestHandler<SearchVisitorsQuery, SearchVisitorsResult>,
    IRequestHandler<GetVisitorStatsQuery, VisitorStatsDto>,
    IRequestHandler<GetGroupedVisitorStatsQuery, List<GroupedVisitorStatsDto>>
{
    private readonly IVisitorRepository _visitorRepo = visitorRepo;
    private readonly IMapper _mapper = mapper;

    public async Task<VisitorDto?> Handle(GetVisitorByIdQuery request, CancellationToken cancellationToken)
    {
        var visitor = await _visitorRepo.GetByIdAsync(request.VisitorId);
        return visitor == null ? null : _mapper.Map<VisitorDto>(visitor);
    }

    public async Task<List<VisitorDto>> Handle(GetAllVisitorsQuery request, CancellationToken cancellationToken)
    {
        var visitors = await _visitorRepo.GetAllAsync();
        return _mapper.Map<List<VisitorDto>>(visitors);
    }

    public async Task<SearchVisitorsResult> Handle(SearchVisitorsQuery request, CancellationToken cancellationToken)
    {
        var searchSpec = _mapper.Map<PaginatedSpec<VisitorSpec>>(request);
        var countSpec = searchSpec.InnerSpec;

        var visitors = await _visitorRepo.SearchAsync(searchSpec);
        var totalCount = await _visitorRepo.CountAsync(countSpec);

        var resultDtos = _mapper.Map<List<VisitorDto>>(visitors);

        return new SearchVisitorsResult
        {
            Items = resultDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
        };
    }

    public async Task<VisitorStatsDto> Handle(GetVisitorStatsQuery request, CancellationToken cancellationToken)
    {
        var statsSpec = _mapper.Map<VisitorSpec>(request);
        var stats = await _visitorRepo.GetStatsAsync(statsSpec);

        return _mapper.Map<VisitorStatsDto>(stats);
    }

    public async Task<List<GroupedVisitorStatsDto>> Handle(GetGroupedVisitorStatsQuery request, CancellationToken cancellationToken)
    {
        var groupedStatsSpec = _mapper.Map<GroupedSpec<VisitorSpec>>(request);
        var groupedStats = await _visitorRepo.GetGroupedStatsAsync(groupedStatsSpec);

        return _mapper.Map<List<GroupedVisitorStatsDto>>(groupedStats);
    }
}
