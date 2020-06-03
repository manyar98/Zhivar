using OMF.Common.Validation;
using FluentValidation;

namespace OMF.Security.Model.Validators
{
    public class RoleValidator : EntityValidator<Role>
    {
        public RoleValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            foreach (IValidationRule rule in (AbstractValidator<RoleBase>)new RoleBaseValidator())
                this.AddRule(rule);
        }
    }
}
