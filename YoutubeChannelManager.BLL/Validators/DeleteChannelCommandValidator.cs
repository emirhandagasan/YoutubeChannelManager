using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using YoutubeChannelManager.BLL.Features.Channels.Commands.DeleteChannel;

namespace YoutubeChannelManager.BLL.Validators
{
    public class DeleteChannelCommandValidator : AbstractValidator<DeleteChannelCommand>
    {
        public DeleteChannelCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id must be provided.");
        }
    }
}