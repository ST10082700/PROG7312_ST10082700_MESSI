using PROG_ST10082700_MESSI.Trees.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROG_ST10082700_MESSI.Trees
{
    /// <summary>
    /// Basic Tree implementation for Service Requests.
    /// Provides hierarchical organization of requests based on categories and statuses.
    /// </summary>
    public class ServiceRequestBasicTree
    {
        private BasicTreeNode<ServiceRequest> root;
        private readonly int maxChildren;

        // Constants for indexing different categories and statuses
        private static class TreeIndices
        {
            // Category indices
            public const int UTILITIES = 0;
            public const int ROADS = 1;
            public const int SANITATION = 2;
            public const int OTHER = 3;

            // Status indices
            public const int PENDING = 0;
            public const int IN_PROGRESS = 1;
            public const int UNDER_REVIEW = 2;
            public const int RESOLVED = 3;
            public const int CLOSED = 4;

            // Get category index
            public static int GetCategoryIndex(string category) => category?.ToLower() switch
            {
                "utilities" => UTILITIES,
                "roads" => ROADS,
                "sanitation" => SANITATION,
                _ => OTHER
            };

            // Get status index
            public static int GetStatusIndex(string status) => status?.ToLower() switch
            {
                "pending" => PENDING,
                "in progress" => IN_PROGRESS,
                "under review" => UNDER_REVIEW,
                "resolved" => RESOLVED,
                "closed" => CLOSED,
                _ => PENDING
            };
        }

        public ServiceRequestBasicTree(int maxChildrenPerNode = 10)
        {
            maxChildren = maxChildrenPerNode;
            InitializeTree();
        }

        /// <summary>
        /// Initializes the basic tree structure with category and status nodes
        /// </summary>
        private void InitializeTree()
        {
            // Create root node
            root = new BasicTreeNode<ServiceRequest>(null, maxChildren);

            // Create category nodes
            root.Children[TreeIndices.UTILITIES] = new BasicTreeNode<ServiceRequest>(
                new ServiceRequest { Category = "Utilities" }, maxChildren);
            root.Children[TreeIndices.ROADS] = new BasicTreeNode<ServiceRequest>(
                new ServiceRequest { Category = "Roads" }, maxChildren);
            root.Children[TreeIndices.SANITATION] = new BasicTreeNode<ServiceRequest>(
                new ServiceRequest { Category = "Sanitation" }, maxChildren);
            root.Children[TreeIndices.OTHER] = new BasicTreeNode<ServiceRequest>(
                new ServiceRequest { Category = "Other" }, maxChildren);

            // Initialize status nodes for each category
            for (int i = 0; i < 4; i++)
            {
                var categoryNode = root.Children[i];
                if (categoryNode != null)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        categoryNode.Children[j] = new BasicTreeNode<ServiceRequest>(null, maxChildren);
                    }
                }
            }
        }

        /// <summary>
        /// Inserts a service request into the tree based on its category and status
        /// </summary>
        public void Insert(ServiceRequest request)
        {
            int categoryIndex = TreeIndices.GetCategoryIndex(request.Category);
            int statusIndex = TreeIndices.GetStatusIndex(request.Status);

            var categoryNode = root.Children[categoryIndex];
            var statusNode = categoryNode.Children[statusIndex];

            // Find empty slot in status node
            for (int i = 0; i < maxChildren; i++)
            {
                if (statusNode.Children[i] == null)
                {
                    statusNode.Children[i] = new BasicTreeNode<ServiceRequest>(request);
                    break;
                }
            }
        }

        /// <summary>
        /// Gets all requests for a specific category
        /// </summary>
        public ServiceRequest[] GetRequestsByCategory(string category)
        {
            int categoryIndex = TreeIndices.GetCategoryIndex(category);
            var requests = new ServiceRequest[maxChildren * 5]; // 5 statuses
            int count = 0;

            var categoryNode = root.Children[categoryIndex];
            if (categoryNode != null)
            {
                // Traverse all status nodes for this category
                for (int statusIdx = 0; statusIdx < 5; statusIdx++)
                {
                    var statusNode = categoryNode.Children[statusIdx];
                    if (statusNode != null)
                    {
                        for (int i = 0; i < maxChildren; i++)
                        {
                            if (statusNode.Children[i]?.Data != null)
                            {
                                requests[count++] = statusNode.Children[i].Data;
                            }
                        }
                    }
                }
            }

            // Create properly sized array
            var result = new ServiceRequest[count];
            Array.Copy(requests, result, count);
            return result;
        }

        /// <summary>
        /// Gets all requests for a specific status
        /// </summary>
        public ServiceRequest[] GetRequestsByStatus(string status)
        {
            int statusIndex = TreeIndices.GetStatusIndex(status);
            var requests = new ServiceRequest[maxChildren * 4]; // 4 categories
            int count = 0;

            // Check each category's status node
            for (int categoryIdx = 0; categoryIdx < 4; categoryIdx++)
            {
                var categoryNode = root.Children[categoryIdx];
                if (categoryNode != null)
                {
                    var statusNode = categoryNode.Children[statusIndex];
                    if (statusNode != null)
                    {
                        for (int i = 0; i < maxChildren; i++)
                        {
                            if (statusNode.Children[i]?.Data != null)
                            {
                                requests[count++] = statusNode.Children[i].Data;
                            }
                        }
                    }
                }
            }

            // Create properly sized array
            var result = new ServiceRequest[count];
            Array.Copy(requests, result, count);
            return result;
        }

        /// <summary>
        /// Gets all requests in the tree
        /// </summary>
        public ServiceRequest[] GetAllRequests()
        {
            var requests = new ServiceRequest[maxChildren * 4 * 5]; // All possible slots
            int count = 0;

            // Traverse all category and status nodes
            for (int categoryIdx = 0; categoryIdx < 4; categoryIdx++)
            {
                var categoryNode = root.Children[categoryIdx];
                if (categoryNode != null)
                {
                    for (int statusIdx = 0; statusIdx < 5; statusIdx++)
                    {
                        var statusNode = categoryNode.Children[statusIdx];
                        if (statusNode != null)
                        {
                            for (int i = 0; i < maxChildren; i++)
                            {
                                if (statusNode.Children[i]?.Data != null)
                                {
                                    requests[count++] = statusNode.Children[i].Data;
                                }
                            }
                        }
                    }
                }
            }

            // Create properly sized array
            var result = new ServiceRequest[count];
            Array.Copy(requests, result, count);
            return result;
        }

        /// <summary>
        /// Updates the status of a service request
        /// </summary>
        public void UpdateRequestStatus(string requestId, string newStatus)
        {
            // Find and remove the request
            ServiceRequest requestToUpdate = null;
            BasicTreeNode<ServiceRequest> oldStatusNode = null;
            int oldSlot = -1;

            // Search through all nodes
            for (int categoryIdx = 0; categoryIdx < 4; categoryIdx++)
            {
                var categoryNode = root.Children[categoryIdx];
                if (categoryNode != null)
                {
                    for (int statusIdx = 0; statusIdx < 5; statusIdx++)
                    {
                        var statusNode = categoryNode.Children[statusIdx];
                        if (statusNode != null)
                        {
                            for (int i = 0; i < maxChildren; i++)
                            {
                                if (statusNode.Children[i]?.Data?.Id == requestId)
                                {
                                    requestToUpdate = statusNode.Children[i].Data;
                                    oldStatusNode = statusNode;
                                    oldSlot = i;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            // If found, update and reinsert
            if (requestToUpdate != null)
            {
                // Remove from old location
                oldStatusNode.Children[oldSlot] = null;

                // Update status and reinsert
                requestToUpdate.Status = newStatus;
                Insert(requestToUpdate);
            }
        }
    }
}
