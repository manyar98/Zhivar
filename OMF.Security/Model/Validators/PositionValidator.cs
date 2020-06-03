using OMF.Common.Validation;
using FluentValidation;

namespace OMF.Security.Model.Validators
{
    internal class PositionValidator : EntityValidator<Position>
    {
        public PositionValidator()
        {
            foreach (IValidationRule rule in (AbstractValidator<RoleBase>)new RoleBaseValidator())
                this.AddRule(rule);
        }
    }
}
