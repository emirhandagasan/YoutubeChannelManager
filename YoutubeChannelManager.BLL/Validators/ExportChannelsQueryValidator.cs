using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using YoutubeChannelManager.BLL.Features.Files.Queries.ExportChannels;

namespace YoutubeChannelManager.BLL.Validators
{
    public class ExportChannelsQueryValidator : AbstractValidator<ExportChannelsQuery>
    {
        public ExportChannelsQueryValidator()
        {
            RuleFor(x => x.Format)
                .NotEmpty().WithMessage("Format is required.")
                .Must(f => f.ToLower() == "csv" || f.ToLower() == "xlsx")
                .WithMessage("Format must be either csv or xlsx.");
        }
    }
}