using System.Linq;
using AppCommon;
using MyMessages;
using NLog;
using NServiceBus;

namespace AppForSubmittingRequests
{
    public class RequestExpiredEventHandler : IHandleMessages<IRequestExpiredEvent>
    {

        private static Logger logger = LogManager.GetCurrentClassLogger();

        
        public Context<ItemViewModel> Context { get; set; }

        public void Handle(IRequestExpiredEvent message)
        {
            Context.MarshalToUiThread(() => HandleOnUiThread(message));
        }

        private void HandleOnUiThread(IRequestExpiredEvent message)
        {
            var item = Context.Items.Single(x => x.RequestId == message.RequestId);
            item.Status = "Expired";
            logger.Info("--------AppForSubmittingRequests Handle-------" + item.Status);

        }
    }
}
