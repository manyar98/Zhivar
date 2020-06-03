using OMF.Workflow.Cartable.Model;
using System.Collections.Generic;

namespace OMF.Workflow.Cartable
{
    public interface ICartableHandler
    {
        List<CtbMenuItem> GetMenuItems(MenuItemsRequest menuRequest);

        List<CtbSubMenuItem> GetSubMenuItems(SubMenuItemsRequest subMenuRequest);

        List<MessageInfo> GetMessages(MessagesInfoesRequest messagesRequest);

        List<CtbMessageAction> GetMessageStepActions(
          MessageActionsRequest messageActionRequest);

        List<MessageInfoHistory> GetMessageInfoHistories(
          MessageInfoHistoryRequest messageInfoHistryRequest);

        List<NextStepInfo> GetNextSteps(NextStepsRequest nextStepsRequest);

        bool HasUserAccessTo(HasUserAccessToRequest hasUserAccessToRequest);
    }
}
