using FluentValidation;
using System.Collections;
using System.Collections.Generic;

namespace OMF.Common.Validation
{
    public class EntityValidator<TEntity> : AbstractValidator<TEntity>, IEntityValidator, IValidator, IEnumerable<IValidationRule>, IEnumerable
    {
    }
}
