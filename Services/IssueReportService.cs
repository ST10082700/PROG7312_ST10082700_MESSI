using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROG_ST10082700_MESSI.Services
{
    public class IssueReportService : IIssueReportService
    {
        private readonly ServiceRequestAVLTree _avlTree;
        private readonly ServiceRequestHeap _priorityHeap;
        private readonly ServiceRequestRedBlackTree _rbTree;
        private readonly ServiceRequestGraph _graph;

        public IssueReportService()
        {
            _avlTree = new ServiceRequestAVLTree();
            _priorityHeap = new ServiceRequestHeap();
            _rbTree = new ServiceRequestRedBlackTree();
            _graph = new ServiceRequestGraph();
        }

        public void AddIssue(IssueReport issue)
        {
            // Ensure priority is assigned
            if (string.IsNullOrEmpty(issue.Priority))
            {
                issue.AssignPriority();
            }

            // Add to all data structures
            _avlTree.Insert(issue);
            _priorityHeap.Insert(issue);
            _rbTree.Insert(issue);
            _graph.AddRequest(issue);
        }

        public IssueReport GetIssueById(string id)
        {
            // Use Red-Black tree for efficient ID lookup
            return _rbTree.Search(new IssueReport { Id = id });
        }

        public IssueReport[] GetAllIssues()
        {
            // Use AVL tree for sorted retrieval
            var issues = new IssueReport[100];
            int count = 0;

            _avlTree.InorderTraversal(issue => {
                if (count < issues.Length)
                {
                    issues[count++] = issue;
                }
            });

            // Create properly sized array
            var result = new IssueReport[count];
            Array.Copy(issues, result, count);
            return result;
        }

        public IssueReport[] GetIssuesByStatus(string status)
        {
            var issues = new IssueReport[100];
            int count = 0;

            _avlTree.InorderTraversal(issue => {
                if (count < issues.Length && issue.Status == status)
                {
                    issues[count++] = issue;
                }
            });

            var result = new IssueReport[count];
            Array.Copy(issues, result, count);
            return result;
        }

        public void UpdateIssueStatus(string id, string newStatus)
        {
            var issue = GetIssueById(id);
            if (issue != null)
            {
                issue.UpdateStatus(newStatus);
                // Re-add to maintain proper ordering
                _avlTree.Insert(issue);
                _priorityHeap.Insert(issue);
            }
        }

        public void AssignIssue(string id, string assignedTo)
        {
            var issue = GetIssueById(id);
            if (issue != null)
            {
                issue.AssignedTo = assignedTo;
                issue.LastUpdated = DateTime.Now;
            }
        }
    }
}
