using MediatR;

using LMS___Mini_Version.DTOs;

namespace LMS___Mini_Version.Features.Tracks.Queries
{
    public record GetInternByIdQuery(int Id) : IRequest<InternDto?>;
}