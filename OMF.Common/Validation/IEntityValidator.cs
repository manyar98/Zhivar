using FluentValidation;
using System.Collections;
using System.Collections.Generic;

namespace OMF.Common.Validation
{
    public interface IEntityValidator : IValidator, IEnumerable<IValidationRule>, IEnumerable
    {
    }
}
