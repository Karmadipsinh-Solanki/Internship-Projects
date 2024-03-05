namespace HalloDoc
{
    public class EnumStatus
    {
        public enum Status
        {
            Unassigned = 1 /*New*/, Accepted = 2 /*Pending*/,  MDEnRoute = 3/*Active*/, MDONSite = 4/*Active*/, Conclude = 5/*Conclude*/, Cancelled = 6/*To-close*/, CancelledByPatient = 7/*To-close*/, Closed = 8/*To-close*/, Unpaid = 9/*Unpaid*/, Clear = 10/*Will not show in dashboard*/, Blocked = 11
        }
    }
}