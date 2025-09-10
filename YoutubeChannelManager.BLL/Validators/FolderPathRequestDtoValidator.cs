using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using YoutubeChannelManager.BLL.DTOs;

namespace YoutubeChannelManager.BLL.Validators
{
    public class FolderPathRequestDtoValidator : AbstractValidator<FolderPathRequestDto>
    {
        public FolderPathRequestDtoValidator()
        {
            RuleFor(x => x.FolderPath)
                .NotEmpty().WithMessage("FolderPath is required.")
                .Must(path => !string.IsNullOrWhiteSpace(path))
                    .WithMessage("FolderPath cannot be whitespace.")
                .MinimumLength(3).WithMessage("FolderPath is too short.")
                .MaximumLength(260).WithMessage("FolderPath is too long.")
                .Must(ValidPath).WithMessage("FolderPath contains invalid characters.")
                .Must(Directory.Exists).WithMessage("The specified folder does not exist.");
        }

        private bool ValidPath(string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            char[] invalidChars = Path.GetInvalidPathChars();
                return !path.Any(c => invalidChars.Contains(c));
        }
    }
}
