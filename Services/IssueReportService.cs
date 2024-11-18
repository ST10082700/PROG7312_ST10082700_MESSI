using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PROG_ST10082700_MESSI.Trees;
using PROG_ST10082700_MESSI.DataStructures;
using PROG_ST10082700_MESSI.Models;


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
            try
            {
                // Ensure priority is assigned
                if (string.IsNullOrEmpty(issue.Priority))
                {
                    issue.AssignPriority();
                }

                // Convert IssueReport to ServiceRequest
                var serviceRequest = ConvertToServiceRequest(issue);

                // Add to all data structures
                _avlTree.Insert(serviceRequest);
                _priorityHeap.Insert(serviceRequest);
                _rbTree.Insert(serviceRequest);
                _graph.AddRequest(serviceRequest);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding issue: {ex.Message}");
            }
        }

        public IssueReport GetIssueById(string id)
        {
            try
            {
                // Create a ServiceRequest for search
                var searchRequest = new ServiceRequest { Id = id };

                // Use Red-Black tree for efficient ID lookup
                var found = _rbTree.Search(searchRequest);

                // Convert back to IssueReport if found
                return found != null ? ConvertToIssueReport(found) : null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving issue: {ex.Message}");
            }
        }

        public IssueReport[] GetAllIssues()
        {
            try
            {
                var issues = new IssueReport[100];
                int count = 0;

                _avlTree.InorderTraversal(serviceRequest => {
                    if (count < issues.Length)
                    {
                        issues[count++] = ConvertToIssueReport(serviceRequest);
                    }
                });

                // Create properly sized array
                var result = new IssueReport[count];
                Array.Copy(issues, result, count);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving issues: {ex.Message}");
            }
        }

        public IssueReport[] GetIssuesByStatus(string status)
        {
            try
            {
                var issues = new IssueReport[100];
                int count = 0;

                _avlTree.InorderTraversal(serviceRequest => {
                    if (count < issues.Length && serviceRequest.Status == status)
                    {
                        issues[count++] = ConvertToIssueReport(serviceRequest);
                    }
                });

                var result = new IssueReport[count];
                Array.Copy(issues, result, count);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving issues by status: {ex.Message}");
            }
        }

        public void UpdateIssueStatus(string id, string newStatus)
        {
            try
            {
                var issue = GetIssueById(id);
                if (issue != null)
                {
                    issue.UpdateStatus(newStatus);

                    // Convert and re-add to maintain proper ordering
                    var serviceRequest = ConvertToServiceRequest(issue);

                    _avlTree.Insert(serviceRequest);
                    _priorityHeap.Insert(serviceRequest);
                    _rbTree.Insert(serviceRequest);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating issue status: {ex.Message}");
            }
        }

        public void AssignIssue(string id, string assignedTo)
        {
            try
            {
                var issue = GetIssueById(id);
                if (issue != null)
                {
                    issue.AssignedTo = assignedTo;
                    issue.LastUpdated = DateTime.Now;

                    // Update in data structures
                    var serviceRequest = ConvertToServiceRequest(issue);
                    _avlTree.Insert(serviceRequest);
                    _priorityHeap.Insert(serviceRequest);
                    _rbTree.Insert(serviceRequest);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error assigning issue: {ex.Message}");
            }
        }

        // Helper method to convert IssueReport to ServiceRequest
        private ServiceRequest ConvertToServiceRequest(IssueReport issue)
        {
            return new ServiceRequest
            {
                Id = issue.Id,
                Description = issue.Title,
                Status = issue.Status,
                SubmissionDate = issue.ReportDate,
                Priority = issue.Priority,
                Location = issue.Location,
                Category = issue.Category
            };
        }

        // Helper method to convert ServiceRequest to IssueReport
        private IssueReport ConvertToIssueReport(ServiceRequest request)
        {
            return new IssueReport
            {
                Id = request.Id,
                Title = request.Description,
                Status = request.Status,
                ReportDate = request.SubmissionDate,
                Priority = request.Priority,
                Location = request.Location,
                Category = request.Category,
                LastUpdated = DateTime.Now
            };
        }

        // Additional methods for getting related issues
        public IssueReport[] GetRelatedIssues(string issueId)
        {
            try
            {
                var relatedRequests = _graph.GetRelatedRequests(issueId);
                var relatedIssues = new IssueReport[relatedRequests.Length];

                for (int i = 0; i < relatedRequests.Length; i++)
                {
                    relatedIssues[i] = ConvertToIssueReport(relatedRequests[i]);
                }

                return relatedIssues;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving related issues: {ex.Message}");
            }
        }

        // Method to get high-priority issues
        public IssueReport[] GetHighPriorityIssues()
        {
            try
            {
                var prioritizedRequests = _priorityHeap.GetSorted();
                var prioritizedIssues = new IssueReport[prioritizedRequests.Length];

                for (int i = 0; i < prioritizedRequests.Length; i++)
                {
                    prioritizedIssues[i] = ConvertToIssueReport(prioritizedRequests[i]);
                }

                return prioritizedIssues;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving high-priority issues: {ex.Message}");
            }
        }
    }
}