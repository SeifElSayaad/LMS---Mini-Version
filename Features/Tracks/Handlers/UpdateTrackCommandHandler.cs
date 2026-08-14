using MediatR;
using LMS___Mini_Version.Features.Tracks.Commands;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Domain.Entities;

namespace LMS___Mini_Version.Features.Tracks.Handlers
{
    public class UpdateTrackCommandHandler : IRequestHandler<UpdateTrackCommand, Unit>
    {
        private readonly IGeneralRepository<Track> _trackRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTrackCommandHandler(IGeneralRepository<Track> trackRepository, IUnitOfWork unitOfWork)
        {
            _trackRepository = trackRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateTrackCommand request, CancellationToken cancellationToken)
        {
            var track = await _trackRepository.GetByIdAsync(request.Id);
            if (track != null)
            {
                track.Name = request.Name;
                track.Fees = request.Fees;
                track.IsActive = request.IsActive;
                track.MaxCapacity = request.MaxCapacity;


                _trackRepository.Update(track);
                await _unitOfWork.CompleteAsync();
            }
            return Unit.Value;
        }
    }
}
