using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequisitionPortal.BL.Utils
{
    public class SystemEnums
    {
        public enum Status
        {
            AwaitingMgrApproval = 1,
            ManagerApproved = 2,
            AwaitingPickup = 3,
            AwaitingAcknowledgement = 4,
            Completed = 5,
            UserCancelled = 6,
            ManagerCancelled = 7,
            OutofStock = 8
        }

        public enum AuditAction
        {
            MadeRequisition = 1,
            ApprovedRequisition = 2,
            ClaimedAvailability = 3, //Store Officer claimed he was available
            GivenOutItems = 4,
            AcknowledgedReceipt = 5,
            RejectedRequisition = 6,
            CancelledRequisition = 7,
            Completed = 8
        }
    }
}
