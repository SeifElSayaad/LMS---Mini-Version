using MediatR;

namespace LMS___Mini_Version.Features.Tracks.Commands
{
    public record CancelEnrollmentCommand(int EnrollmentId) : IRequest;
}
