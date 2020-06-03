using OMF.Business;
using OMF.Common;
using OMF.Common.Configuration;
using OMF.Common.Security;
using OMF.EntityFramework.Ef6;
using OMF.EntityFramework.Query;
using OMF.EntityFramework.UnitOfWork;
using OMF.Security.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Behsho.Common;
using System.Data.Entity;
using static OMF.Common.Enums;
using OMF.Common.Extensions;

namespace Zhivar.Business.Security
{
    public class OperationRule : BusinessRuleBase<Operation>
    {
        public OperationRule() : base()
        {
        }

        public OperationRule(IUnitOfWorkAsync uow) : base(uow)
        {
        }

        public async Task<List<Operation>> GetOperationAndMenusAsync(QueryInfo searchRequestInfo)
        {
            SecurityManager.ThrowIfUserContextNull();

            using (var uow = new UnitOfWork())
            {
                var operationQuery = uow.RepositoryAsync<Operation>()
                                              .Queryable()
                                              .Where(op => !op.IsDeleted && op.ApplicationId == ConfigurationController.ApplicationID);

                if (searchRequestInfo.Filter != null && searchRequestInfo.Filter.Filters != null && searchRequestInfo.Filter.Filters.Any())
                {
                    FilterInfo nameFilterInfo = new FilterInfo();
                    FilterInfo codeFilterInfo = new FilterInfo();

                    string namPropName = Entity.GetPropertyName<Operation>(op => op.Name);
                    if (searchRequestInfo.Filter.Filters.Any(filter => filter.Field == namPropName))
                    {
                        FilterData nameFilterData = searchRequestInfo.Filter.Filters.FirstOrDefault(filter => filter.Field == namPropName);
                        nameFilterInfo.Filters.Add(new FilterData() { Field = namPropName, Operator = nameFilterData.Operator, Value = nameFilterData.Value });

                        var operationExpression = nameFilterInfo.TranslateFilter<Operation>();
                        operationQuery = operationQuery.Where(operationExpression);
                    }

                    string codePropName = Entity.GetPropertyName<Operation>(op => op.Code);
                    if (searchRequestInfo.Filter.Filters.Any(filter => filter.Field == codePropName))
                    {
                        FilterData codeFilterData = searchRequestInfo.Filter.Filters.FirstOrDefault(filter => filter.Field == codePropName);
                        codeFilterInfo.Filters.Add(new FilterData() { Field = codePropName, Operator = codeFilterData.Operator, Value = codeFilterData.Value });

                        var operationExpression = codeFilterInfo.TranslateFilter<Operation>();
                        operationQuery = operationQuery.Where(operationExpression);
                    }
                }

                //if (!SecurityManager.CurrentUserContext.IsDeveloperUser())
                //    operationQuery = operationQuery.Where(op => !op.IsSystem);

                operationQuery = operationQuery.Where(op => op.OperationType != OperationType.Other);
                return await operationQuery.ToListAsync2();
            }

        }
    }
}
