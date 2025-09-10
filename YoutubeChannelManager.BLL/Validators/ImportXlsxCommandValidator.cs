using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using YoutubeChannelManager.BLL.Features.Files.Commands.ImportXlsx;

namespace YoutubeChannelManager.BLL.Validators
{
    public class ImportXlsxCommandValidator : AbstractValidator<ImportXlsxCommand>
    {
        public ImportXlsxCommandValidator()
        {
            RuleFor(x => x.FileStream)
                .NotNull().WithMessage("Excel file stream must be provided.");
        }
    }
}