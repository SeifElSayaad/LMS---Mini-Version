using MediatR;
using LMS___Mini_Version.Features.Enrollments.Commands;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.Features.Enrollments.Handlers
{
    public class CancelEnrollmentCommandHandler : IRequestHandler<CancelEnrollmentCommand, Unit>
    {
        private readonly IGeneralRepository<Enrollment> _enrollmentRepo;
        private readonly IUnitOfWork _unitOfWork;

        public CancelEnrollmentCommandHandler(
            IGeneralRepository<Enrollment> enrollmentRepo,
            IUnitOfWork unitOfWork)
        {
            _enrollmentRepo = enrollmentRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(CancelEnrollmentCommand request, CancellationToken cancellationToken)
        {
            var enrollment = await _enrollmentRepo.GetTable()
                .Include(e => e.Payment)
                .FirstOrDefaultAsync(e => e.Id == request.EnrollmentId, cancellationToken);
            if (enrollment == null || enrollment.Status == EnrollmentStatus.Cancelled)
            {
                return Unit.Value;
            }

            enrollment.Status = EnrollmentStatus.Cancelled;
            if (enrollment.Payment != null)
            {
                enrollment.Payment.Status = PaymentStatus.Refunded;
            }

            await _unitOfWork.CompleteAsync();
            return Unit.Value;
        }
    }
}
