using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PROG_ST10082700_MESSI.Models;

namespace PROG_ST10082700_MESSI.Services
{
    public interface IIssueReportService
    {
        void AddIssue(IssueReport issue);
        IssueReport GetIssueById(string id);
        IssueReport[] GetAllIssues();
        IssueReport[] GetIssuesByStatus(string status);
        void UpdateIssueStatus(string id, string newStatus);
        void AssignIssue(string id, string assignedTo);
    }
}
