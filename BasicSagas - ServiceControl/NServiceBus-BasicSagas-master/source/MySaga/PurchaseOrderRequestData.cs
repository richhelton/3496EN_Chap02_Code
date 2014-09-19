using System;
using NServiceBus.Saga;

namespace MySaga
{
    public class PurchaseOrderRequestData : IContainSagaData
    {
        /*** 
         * Gets/sets the Id of the process. Do NOT generate this value in your code.
           The value of the Id will be generated automatically to provide the
           best performance for saving in a database.
         * ***/
        public Guid Id { get; set; }  // Required
        /***
         * Contains the return address of the endpoint that caused the process to run.
         * ***/
        public string Originator { get; set; }  //Required
        /***
         * Contains the Id of the message which caused the saga to start.
           This is needed so that when we reply to the Originator, any
           registered callbacks will be fired correctly.
         * ***/
        public string OriginalMessageId { get; set; }  //Required

        [Unique]
        public Guid RequestId { get; set; }  // Unique ID to lookup Request message
        public string Description { get; set; }
        public decimal Cost { get; set; }
        public bool RequiresApprovalByLevel1 { get; set; }
        public bool RequiresApprovalByLevel2 { get; set; }
        public bool ApprovedByLevel1 { get; set; }
        public bool ApprovedByLevel2 { get; set; }
    }
}
