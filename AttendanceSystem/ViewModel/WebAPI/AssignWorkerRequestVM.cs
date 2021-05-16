using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceSystem.ViewModel.WebAPI
{
    public class AssignWorkerRequestVM
    {
        public long SiteId { get; set; }
        public DateTime Date { get; set; }

        public List<long> WorkerList { get; set; }
    }

    public class WorkerListRequestVM
    {
        public long SiteId { get; set; }
        public DateTime Date { get; set; }
    }

    public class CloseWorkerRequestVM
    {
        public long SiteId { get; set; }
        public DateTime Date { get; set; }
        public long EmployeeId { get; set; }
    }
}