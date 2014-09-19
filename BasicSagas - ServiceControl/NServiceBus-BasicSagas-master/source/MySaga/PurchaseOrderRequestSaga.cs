using System;
using MyMessages;
using NLog;
using NServiceBus;
using NServiceBus.Saga;

namespace MySaga
{
    public class PurchaseOrderRequestSaga : Saga<PurchaseOrderRequestData>,
        IAmStartedByMessages<SubmitRequestCommand>,
        IHandleMessages<ApproveRequestCommand>,
        IHandleMessages<DenyRequestCommand>,
        IHandleMessages<RecordEncumbranceReplyMessage>,
        IHandleTimeouts<TimeoutMessage>
    {

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public override void ConfigureHowToFindSaga()
        {
            ConfigureMapping<ApproveRequestCommand>(x => x.RequestId).ToSaga(x => x.RequestId);
            ConfigureMapping<DenyRequestCommand>(x => x.RequestId).ToSaga(x => x.RequestId);
        }

        public void Handle(SubmitRequestCommand message)
        {

            logger.Info("--------MySaga Handle-------" + message);
           
            
            RequestTimeout<TimeoutMessage>(TimeSpan.FromSeconds(60));

            Data.RequestId = message.RequestId;
            Data.Description = message.Description;
            Data.Cost = message.Cost;
            Data.RequiresApprovalByLevel1 = message.Cost > 100.00m;
            Data.RequiresApprovalByLevel2 = message.Cost > 1000.00m;
            Data.ApprovedByLevel1 = false;
            Data.ApprovedByLevel2 = false;

            ProcessApproval();
        }

        public void Handle(ApproveRequestCommand message)
        {
            logger.Info("--------MySaga Handle-------" + message);
            if (message.Approver == Approver.Level1)
            {
                Data.ApprovedByLevel1 = true;
            }

            if (message.Approver == Approver.Level2)
            {
                Data.ApprovedByLevel2 = true;
            }

            ProcessApproval();
        }

        public void Handle(DenyRequestCommand message)
        {
            logger.Info("--------MySaga Handle-------" + message);
            var reply = new SubmitRequestReplyMessage
            {
                RequestId = Data.RequestId,
                Approved = false
            };

            ReplyToOriginator(reply);
            MarkAsComplete();
        }

        public void Handle(RecordEncumbranceReplyMessage message)
        {
            logger.Info("--------MySaga Handle-------" + message);
            var reply = new SubmitRequestReplyMessage
            {
                RequestId = Data.RequestId,
                Approved = true,
                PurchaseOrderNumber = message.PurchaseOrderNumber
            };

            ReplyToOriginator(reply);
            MarkAsComplete();
        }

        public void Timeout(TimeoutMessage state)
        {
            logger.Info("--------MySaga Timeout-------");
            Bus.Publish<IRequestExpiredEvent>(x => x.RequestId = Data.RequestId);
            MarkAsComplete();
        }

        private void ProcessApproval()
        {
            logger.Info("--------MySaga Process Approval-------");
            if (Data.RequiresApprovalByLevel1 && !Data.ApprovedByLevel1)
            {
                logger.Info("--------SolicitApprovalFromLevel1Command-------");
                var command = new SolicitApprovalFromLevel1Command
                {
                    RequestId = Data.RequestId,
                    Description = Data.Description,
                    Cost = Data.Cost
                };

                Bus.Send(command);
                return;
            }

            if (Data.RequiresApprovalByLevel2 && !Data.ApprovedByLevel2)
            {
                logger.Info("--------SolicitApprovalFromLevel2Command-------");
                var command = new SolicitApprovalFromLevel2Command
                {
                    RequestId = Data.RequestId,
                    Description = Data.Description,
                    Cost = Data.Cost
                };

                Bus.Send(command);
                return;
            }

            var commandToRecordEncumbrance = new RecordEncumbranceCommand
            {
                Description = Data.Description,
                Cost = Data.Cost
            };
            logger.Info("--------commandToRecordEncumbrance-------");
            Bus.Send(commandToRecordEncumbrance);
        }
    }
}
