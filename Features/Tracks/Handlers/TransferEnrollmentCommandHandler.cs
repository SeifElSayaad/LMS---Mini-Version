using MediatR;
using LMS___Mini_Version.Features.Tracks.Commands;
using LMS___Mini_Version.Services.Interfaces;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Domain.Enums;

namespace LMS___Mini_Version.Features.Tracks.Handlers
{
    public class TransferEnrollmentCommandHandler : IRequestHandler<TransferEnrollmentCommand, Unit>
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly ITrackService _trackService;
        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;

        public TransferEnrollmentCommandHandler(
            IEnrollmentService enrollmentService,
            ITrackService trackService,
            IPaymentService paymentService,
            IUnitOfWork unitOfWork)
        {
            _enrollmentService = enrollmentService;
            _trackService = trackService;
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(TransferEnrollmentCommand request, CancellationToken cancellationToken)
        {
            var enrollment = await _enrollmentService.GetByIdAsync(request.EnrollmentId);
            if (enrollment == null || enrollment.Status == EnrollmentStatus.Cancelled || enrollment.TrackId == request.NewTrackId)
            {
                return Unit.Value;
            }

            var newTrack = await _trackService.GetByIdAsync(request.NewTrackId);
            if (newTrack == null || !newTrack.IsActive)
            {
                return Unit.Value;
            }

            var hasCapacity = await _trackService.CheckCapacityAsync(request.NewTrackId);
            if (!hasCapacity)
            {
                return Unit.Value;
            }

            await _enrollmentService.UpdateTrackAsync(request.EnrollmentId, request.NewTrackId);

            if (newTrack.Fees > 0)
            {
                await _paymentService.UpdatePaymentAmountAsync(request.EnrollmentId, newTrack.Fees);
            }
            await _unitOfWork.CompleteAsync();

            return Unit.Value;
        }
    }
}
