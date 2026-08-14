using MediatR;
using LMS___Mini_Version.Features.Tracks.Commands;
using LMS___Mini_Version.Services.Interfaces;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Domain.Enums;

namespace LMS___Mini_Version.Features.Tracks.Handlers
{
    public class CancelEnrollmentCommandHandler : IRequestHandler<CancelEnrollmentCommand, Unit>
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;

        public CancelEnrollmentCommandHandler(
            IEnrollmentService enrollmentService,
            IPaymentService paymentService,
            IUnitOfWork unitOfWork)
        {
            _enrollmentService = enrollmentService;
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(CancelEnrollmentCommand request, CancellationToken cancellationToken)
        {
            var enrollment = await _enrollmentService.GetByIdAsync(request.EnrollmentId);

            if (enrollment != null && enrollment.Status != EnrollmentStatus.Cancelled)
            {
                await _enrollmentService.UpdateStatusAsync(request.EnrollmentId, EnrollmentStatus.Cancelled);
                await _paymentService.RefundPaymentAsync(request.EnrollmentId);
                await _unitOfWork.CompleteAsync();
            }
            return Unit.Value;
        }
    }
}
