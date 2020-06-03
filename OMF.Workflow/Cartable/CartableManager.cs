using OMF.Workflow.Cartable.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OMF.Workflow.Cartable
{
    public static class CartableManager
    {
        private static ICartableHandlerAsync ctbHandler = (ICartableHandlerAsync)new OMFCartableHandler();

        public static void InitiateCartableHandler(ICartableHandlerAsync cartableHandler)
        {
            CartableManager.ctbHandler = cartableHandler;
        }

        public static List<CtbMenuItem> GetMenuItems(MenuItemsRequest menuRequest)
        {
            return CartableManager.ctbHandler.GetMenuItems(menuRequest);
        }

        public static async Task<List<CtbMenuItem>> GetMenuItemsAsync(
          MenuItemsRequest menuRequest)
        {
            List<CtbMenuItem> menuItemsAsync = await CartableManager.ctbHandler.GetMenuItemsAsync(menuRequest);
            return menuItemsAsync;
        }

        public static List<CtbSubMenuItem> GetSubMenuItems(
          SubMenuItemsRequest subMenuRequest)
        {
            return CartableManager.ctbHandler.GetSubMenuItems(subMenuRequest);
        }

        public static async Task<List<CtbSubMenuItem>> GetSubMenuItemsAsync(
          SubMenuItemsRequest subMenuRequest)
        {
            List<CtbSubMenuItem> subMenuItemsAsync = await CartableManager.ctbHandler.GetSubMenuItemsAsync(subMenuRequest);
            return subMenuItemsAsync;
        }

        public static List<MessageInfo> GetMessages(MessagesInfoesRequest messagesRequest)
        {
            return CartableManager.ctbHandler.GetMessages(messagesRequest);
        }

        public static async Task<List<MessageInfo>> GetMessagesAsync(
          MessagesInfoesRequest messagesRequest)
        {
            List<MessageInfo> messagesAsync = await CartableManager.ctbHandler.GetMessagesAsync(messagesRequest);
            return messagesAsync;
        }

        public static List<CtbMessageAction> GetMessageStepActions(
          MessageActionsRequest messageActionRequest)
        {
            return CartableManager.ctbHandler.GetMessageStepActions(messageActionRequest);
        }

        public static async Task<List<CtbMessageAction>> GetMessageStepActionsAsync(
          MessageActionsRequest messageActionRequest)
        {
            List<CtbMessageAction> stepActionsAsync = await CartableManager.ctbHandler.GetMessageStepActionsAsync(messageActionRequest);
            return stepActionsAsync;
        }

        public static List<MessageInfoHistory> GetMessageInfoHistories(
          MessageInfoHistoryRequest messageInfoHistryRequest)
        {
            return CartableManager.ctbHandler.GetMessageInfoHistories(messageInfoHistryRequest);
        }

        public static async Task<List<MessageInfoHistory>> GetMessageInfoHistoriesAsync(
          MessageInfoHistoryRequest messageInfoHistryRequest)
        {
            List<MessageInfoHistory> infoHistoriesAsync = await CartableManager.ctbHandler.GetMessageInfoHistoriesAsync(messageInfoHistryRequest);
            return infoHistoriesAsync;
        }

        public static List<NextStepInfo> GetNextSteps(NextStepsRequest nextStepsRequest)
        {
            return CartableManager.ctbHandler.GetNextSteps(nextStepsRequest);
        }

        public static async Task<List<NextStepInfo>> GetNextStepsAsync(
          NextStepsRequest nextStepsRequest)
        {
            List<NextStepInfo> nextStepsAsync = await CartableManager.ctbHandler.GetNextStepsAsync(nextStepsRequest);
            return nextStepsAsync;
        }

        public static bool HasUserAccessTo(HasUserAccessToRequest hasUserAccessToRequest)
        {
            return CartableManager.ctbHandler.HasUserAccessTo(hasUserAccessToRequest);
        }

        public static async Task<bool> HasUserAccessToAsync(
          HasUserAccessToRequest hasUserAccessToRequest)
        {
            bool async = await CartableManager.ctbHandler.HasUserAccessToAsync(hasUserAccessToRequest);
            return async;
        }
    }
}
