using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using YoutubeChannelManager.BLL.Features.Channels.Commands.CreateChannel;
using YoutubeChannelManager.BLL.Interfaces;

namespace YoutubeChannelManager.BLL.Validators
{
    public class CreateChannelCommandValidator : AbstractValidator<CreateChannelCommand>
    {
        public CreateChannelCommandValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.ChannelName)
                .NotEmpty().WithMessage("Channel name is required.")
                .MinimumLength(2).WithMessage("Channel name must be at least 2 characters long.")
                .MaximumLength(100).WithMessage("Channel name must be at most 100 characters.")
                .Matches(@"^[\p{L}\p{Nd}\s\-_&]+$").WithMessage("Channel name contains invalid characters.")
                .Must(name => !HasConsecutiveSpaces(name)).WithMessage("Channel name cannot contain consecutive spaces.");

            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("Category is required.")
                .NotEmpty().WithMessage("Category is required.")
                .MinimumLength(3).WithMessage("Category must be at least 3 characters long.")
                .MaximumLength(50).WithMessage("Category must be at most 50 characters.");

            RuleFor(x => x.Subscribers)
                .GreaterThanOrEqualTo(0).WithMessage("Subscribers cannot be negative.")
                .LessThanOrEqualTo(1_000_000_000).WithMessage("Subscribers exceeds maximum allowed value.");

            RuleFor(x => x.IsActive)
                .NotNull().WithMessage("IsActive field must be provided.");
        }

        private static bool HasConsecutiveSpaces(string? s) => s?.Contains("  ") == true;
       
    }
}