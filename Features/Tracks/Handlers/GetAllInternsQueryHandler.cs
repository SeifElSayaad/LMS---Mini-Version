using MediatR;
using LMS___Mini_Version.Features.Tracks.Queries;
using LMS___Mini_Version.DTOs;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Domain.Entities;

namespace LMS___Mini_Version.Features.Tracks.Handlers
{
    public class GetAllInternsQueryHandler : IRequestHandler<GetAllInternsQuery, IEnumerable<InternDto>>
    {

        private readonly IGeneralRepository<Intern> _internRepository;

        public GetAllInternsQueryHandler(IGeneralRepository<Intern> internRepository)
        {
            _internRepository = internRepository;
        }

        public Task<IEnumerable<InternDto>> Handle(GetAllInternsQuery request, CancellationToken cancellationToken)
        {
            var interns = _internRepository.GetAll().Select(i => new InternDto
            {
                Id = i.Id,
                FullName = i.FullName,
                Email = i.Email,
                BirthYear = i.BirthYear,
                Status = i.Status,
                TrackId = i.TrackId,
                TrackName = i.Track.Name
            }).ToList();
            return Task.FromResult<IEnumerable<InternDto>>(interns);
        }
    }
}
