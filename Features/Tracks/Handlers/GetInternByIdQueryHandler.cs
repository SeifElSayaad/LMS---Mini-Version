using MediatR;
using LMS___Mini_Version.Features.Tracks.Queries;
using LMS___Mini_Version.DTOs;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Domain.Entities;


namespace LMS___Mini_Version.Features.Tracks.Handlers
{
    public class GetInternByIdQueryHandler : IRequestHandler<GetInternByIdQuery, InternDto?>
    {
        private readonly IGeneralRepository<Intern> _internRepository;

        public GetInternByIdQueryHandler(IGeneralRepository<Intern> internRepository)
        {
            _internRepository = internRepository;
        }

        public async Task<InternDto?> Handle(GetInternByIdQuery request, CancellationToken cancellationToken)
        {
            var intern = await _internRepository.GetByIdAsync(request.Id);
            if (intern == null)
            {
                return null;
            }
            return new InternDto
            {
                Id = intern.Id,
                FullName = intern.FullName,
                Email = intern.Email,
                BirthYear = intern.BirthYear,
                Status = intern.Status,
                TrackId = intern.TrackId,
                TrackName = intern.Track?.Name ?? string.Empty
            };
        }
    }
}
