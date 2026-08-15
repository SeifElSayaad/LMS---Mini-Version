using MediatR;
using LMS___Mini_Version.DTOs;

namespace LMS___Mini_Version.Features.Enrollments.Queries
{
    public record GetEnrollmentsByInternQuery(int InternId) : IRequest<IEnumerable<EnrollmentDto>>;
}
