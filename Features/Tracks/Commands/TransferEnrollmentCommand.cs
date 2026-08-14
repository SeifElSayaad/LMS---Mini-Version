
using MediatR;

namespace LMS___Mini_Version.Features.Tracks.Commands
{
    public record TransferEnrollmentCommand(int EnrollmentId, int NewTrackId) : IRequest;
}
