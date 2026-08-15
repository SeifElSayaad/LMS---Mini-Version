using MediatR;
using Microsoft.EntityFrameworkCore;
using LMS___Mini_Version.Features.Enrollments.Queries;
using LMS___Mini_Version.DTOs;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Domain.Entities;

namespace LMS___Mini_Version.Features.Enrollments.Handlers
{
    public class GetEnrollmentsByInternQueryHandler : IRequestHandler<GetEnrollmentsByInternQuery, IEnumerable<EnrollmentDto>>
    {
        private readonly IGeneralRepository<Enrollment> _enrollmentRepository;

        public GetEnrollmentsByInternQueryHandler(IGeneralRepository<Enrollment> enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<IEnumerable<EnrollmentDto>> Handle(GetEnrollmentsByInternQuery request, CancellationToken cancellationToken)
        {
            var enrollments = await _enrollmentRepository.GetTable()
                .AsNoTracking()
                .Where(e => e.InternId == request.InternId)
                .Select(e => new EnrollmentDto
                {
                    Id = e.Id,
                    InternId = e.InternId,
                    TrackId = e.TrackId,
                    EnrollmentDate = e.EnrollmentDate,
                    Status = e.Status,
                    InternName = e.Intern.FullName,
                    TrackName = e.Track.Name,
                })
                .ToListAsync(cancellationToken);

            return enrollments;
        }
    }
}
