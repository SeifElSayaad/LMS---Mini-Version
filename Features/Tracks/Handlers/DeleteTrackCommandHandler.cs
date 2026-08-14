
using MediatR;
using LMS___Mini_Version.Features.Tracks.Commands;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Domain.Entities;

namespace LMS___Mini_Version.Features.Tracks.Handlers
{
    public class DeleteTrackCommandHandler : IRequestHandler<DeleteTrackCommand, Unit>
    {
        private readonly IGeneralRepository<Track> _trackRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteTrackCommandHandler(IGeneralRepository<Track> trackRepository, IUnitOfWork unitOfWork)
        {
            _trackRepository = trackRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteTrackCommand request, CancellationToken cancellationToken)
        {
            var track = await _trackRepository.GetByIdAsync(request.Id);
            if (track != null)
            {
                _trackRepository.Delete(track);
                await _unitOfWork.CompleteAsync();
            }
            return Unit.Value;
        }
    }
}
