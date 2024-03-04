namespace HalloDoc
{
    public class EnumStatus
    {
        public enum Status
        {
            Unassigned = 1 /*New*/, Accepted = 2 /*Pending*/,  MDEnRoute = 4/*Active*/, MDONSite = 5/*Active*/, Conclude = 6/*Conclude*/, Cancelled = 3/*To-close*/, CancelledByPatient = 7/*To-close*/, Closed = 8/*To-close*/, Unpaid = 9/*Unpaid*/, Clear = 10/*Will not show in dashboard*/
        }
    }
}