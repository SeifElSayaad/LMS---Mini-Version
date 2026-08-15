using MediatR;
using LMS___Mini_Version.Features.Enrollments.Commands;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.Features.Enrollments.Handlers
{
    public class TransferEnrollmentCommandHandler : IRequestHandler<TransferEnrollmentCommand, Unit>
    {
        private readonly IGeneralRepository<Enrollment> _enrollmentRepo;
        private readonly IGeneralRepository<Track> _trackRepo;
        private readonly IUnitOfWork _unitOfWork;

        public TransferEnrollmentCommandHandler(
            IGeneralRepository<Enrollment> enrollmentRepo,
            IGeneralRepository<Track> trackRepo,
            IUnitOfWork unitOfWork)
        {
            _enrollmentRepo = enrollmentRepo;
            _trackRepo = trackRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(TransferEnrollmentCommand request, CancellationToken cancellationToken)
        {
            var enrollment = await _enrollmentRepo.GetTable()
.Include(e => e.Payment)
.FirstOrDefaultAsync(e => e.Id == request.EnrollmentId, cancellationToken);
            if (enrollment == null ||
                enrollment.Status == EnrollmentStatus.Cancelled ||
                enrollment.TrackId == request.NewTrackId)
            {
                return Unit.Value;
            }
            var newTrack = await _trackRepo.GetTable()
                .FirstOrDefaultAsync(t => t.Id == request.NewTrackId && t.IsActive, cancellationToken);
            if (newTrack == null) return Unit.Value;
            var currentEnrollments = await _enrollmentRepo.GetTable()
                .CountAsync(e => e.TrackId == request.NewTrackId &&
                                   e.Status == EnrollmentStatus.Active, cancellationToken);
            if (currentEnrollments >= newTrack.MaxCapacity)
            {
                return Unit.Value;
            }
            enrollment.TrackId = request.NewTrackId;
            if (newTrack.Fees > 0 && enrollment.Payment != null)
            {
                enrollment.Payment.Amount = newTrack.Fees;
            }
            await _unitOfWork.CompleteAsync();
            return Unit.Value;
        }
    }
}
