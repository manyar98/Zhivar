using OMF.Workflow.Cartable.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OMF.Workflow.Cartable
{
    public interface ICartableHandlerAsync : ICartableHandler
    {
        Task<List<CtbMenuItem>> GetMenuItemsAsync(MenuItemsRequest menuRequest);

        Task<List<CtbSubMenuItem>> GetSubMenuItemsAsync(
          SubMenuItemsRequest subMenuRequest);

        Task<List<MessageInfo>> GetMessagesAsync(
          MessagesInfoesRequest messagesRequest);

        Task<List<CtbMessageAction>> GetMessageStepActionsAsync(
          MessageActionsRequest messageActionRequest);

        Task<List<MessageInfoHistory>> GetMessageInfoHistoriesAsync(
          MessageInfoHistoryRequest messageInfoHistryRequest);

        Task<List<NextStepInfo>> GetNextStepsAsync(NextStepsRequest nextStepsRequest);

        Task<bool> HasUserAccessToAsync(HasUserAccessToRequest hasUserAccessToRequest);
    }
}
