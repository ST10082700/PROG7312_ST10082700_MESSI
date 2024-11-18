using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROG_ST10082700_MESSI.Models
{
    public class ServiceRequest : IComparable<ServiceRequest>
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string Priority { get; set; }
        public string Location { get; set; }
        public string Category { get; set; }

        public ServiceRequest()
        {
            Id = $"SR{DateTime.Now:yyyyMMddHHmmss}";
            SubmissionDate = DateTime.Now;
            Status = "Pending";
        }

        public int CompareTo(ServiceRequest? other)
        {
            if (other == null) return 1;
            return string.Compare(this.Id, other.Id, StringComparison.Ordinal);
        }
    }
}
