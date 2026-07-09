using FluentValidation;

namespace RealtimeChat.Application.Messages.Queries;

public class GetMessageHistoryQueryValidator : AbstractValidator<GetMessageHistoryQuery>
{
    public GetMessageHistoryQueryValidator()
    {
        RuleFor(x => x.RoomId)
            .NotEmpty().WithMessage("RoomId is required.");

        RuleFor(x => x.Take)
            .InclusiveBetween(1, 200).WithMessage("Take must be between 1 and 200.");
    }
}
