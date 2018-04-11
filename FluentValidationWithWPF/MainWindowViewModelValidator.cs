using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FluentValidationWithWPF
{
    public class MainWindowViewModelValidator : AbstractValidator<MainWindowViewModel>
    {
        public MainWindowViewModelValidator()
        {
            RuleFor(u => u.Email)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .EmailAddress();
                
        }
    }
}
