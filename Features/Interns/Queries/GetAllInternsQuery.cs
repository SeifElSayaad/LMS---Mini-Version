using MediatR;
using LMS___Mini_Version.DTOs;

namespace LMS___Mini_Version.Features.Interns.Queries
{
    public record GetAllInternsQuery : IRequest<IEnumerable<InternDto>>;
}
