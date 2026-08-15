using MediatR;
using LMS___Mini_Version.Features.Interns.Queries;
using LMS___Mini_Version.DTOs;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.Features.Interns.Handlers
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
            return await _internRepository.GetTable()
               .Where(i => i.Id == request.Id)
               .Select(i => new InternDto
               {
                   Id = i.Id,
                   FullName = i.FullName,
                   Email = i.Email,
                   BirthYear = i.BirthYear,
                   Status = i.Status,
                   TrackId = i.TrackId,
                   TrackName = i.Track.Name
               })
               .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
