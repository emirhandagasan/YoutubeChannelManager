using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using YoutubeChannelManager.BLL.Features.Files.Commands.ImportCsv;

namespace YoutubeChannelManager.BLL.Validators
{
    public class ImportCsvCommandValidator : AbstractValidator<ImportCsvCommand>
    {
        public ImportCsvCommandValidator()
        {
            RuleFor(x => x.FileStream)
                .NotNull().WithMessage("CSV file stream must be provided.");
        }
    }
}