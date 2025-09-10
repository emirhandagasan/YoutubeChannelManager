using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using YoutubeChannelManager.BLL.Features.Channels.Queries.GetByIdChannel;

namespace YoutubeChannelManager.BLL.Validators
{
    public class GetChannelByIdQueryValidator : AbstractValidator<GetChannelByIdQuery>
    {
        public GetChannelByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id must be provided.");
        }
    }
}