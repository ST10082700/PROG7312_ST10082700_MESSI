using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROG_ST10082700_MESSI.DataStructures
{
    /// <summary>
    /// Implements a weighted graph for Service Requests to manage relationships and dependencies.
    /// Used for finding related requests and optimizing request handling.
    /// </summary>
    public class ServiceRequestGraph
    {
        private readonly ServiceRequest[] vertices;
        private readonly double[,] adjacencyMatrix;
        private readonly int maxVertices;
        private int vertexCount;

        public ServiceRequestGraph(int maxSize = 100)
        {
            maxVertices = maxSize;
            vertices = new ServiceRequest[maxSize];
            adjacencyMatrix = new double[maxSize, maxSize];
            vertexCount = 0;
        }

        /// <summary>
        /// Adds a new service request to the graph
        /// </summary>
        public void AddRequest(ServiceRequest request)
        {
            if (vertexCount >= maxVertices)
                throw new InvalidOperationException("Graph is full");

            vertices[vertexCount] = request;

            // Calculate and add relationships with existing requests
            for (int i = 0; i < vertexCount; i++)
            {
                double weight = CalculateRelationshipWeight(vertices[i], request);
                adjacencyMatrix[i, vertexCount] = weight;
                adjacencyMatrix[vertexCount, i] = weight;
            }

            vertexCount++;
        }

        /// <summary>
        /// Calculates relationship weight between two requests based on various factors
        /// </summary>
        private double CalculateRelationshipWeight(ServiceRequest r1, ServiceRequest r2)
        {
            double weight = 0;

            // Location-based relationship (40%)
            if (r1.Location?.Equals(r2.Location, StringComparison.OrdinalIgnoreCase) == true)
                weight += 0.4;

            // Category-based relationship (30%)
            if (r1.Category?.Equals(r2.Category, StringComparison.OrdinalIgnoreCase) == true)
                weight += 0.3;

            // Time-based relationship (20%) - requests within 24 hours
            if (Math.Abs((r1.SubmissionDate - r2.SubmissionDate).TotalHours) <= 24)
                weight += 0.2;

            // Status-based relationship (10%)
            if (r1.Status?.Equals(r2.Status, StringComparison.OrdinalIgnoreCase) == true)
                weight += 0.1;

            return weight;
        }

        /// <summary>
        /// Finds all related requests using depth-first search
        /// </summary>
        public ServiceRequest[] GetRelatedRequests(string requestId, double minRelationWeight = 0.3)
        {
            int startIndex = FindRequestIndex(requestId);
            if (startIndex == -1)
                return Array.Empty<ServiceRequest>();

            bool[] visited = new bool[vertexCount];
            ServiceRequest[] related = new ServiceRequest[vertexCount];
            int relatedCount = 0;

            DFSRelated(startIndex, visited, related, ref relatedCount, minRelationWeight);

            // Create result array of exact size
            ServiceRequest[] result = new ServiceRequest[relatedCount];
            Array.Copy(related, result, relatedCount);
            return result;
        }

        private void DFSRelated(int vertex, bool[] visited, ServiceRequest[] related,
                              ref int relatedCount, double minRelationWeight)
        {
            visited[vertex] = true;

            // Don't add the starting vertex to related requests
            if (relatedCount > 0)
            {
                related[relatedCount++] = vertices[vertex];
            }

            for (int i = 0; i < vertexCount; i++)
            {
                if (!visited[i] && adjacencyMatrix[vertex, i] >= minRelationWeight)
                {
                    DFSRelated(i, visited, related, ref relatedCount, minRelationWeight);
                }
            }
        }

        private int FindRequestIndex(string requestId)
        {
            for (int i = 0; i < vertexCount; i++)
            {
                if (vertices[i].Id == requestId)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Generates a minimum spanning tree using Prim's algorithm
        /// for optimizing request handling routes
        /// </summary>
        public (ServiceRequest[] requests, double[,] connections) GetOptimizedStructure()
        {
            bool[] included = new bool[vertexCount];
            double[] key = new double[vertexCount];
            int[] parent = new int[vertexCount];

            for (int i = 0; i < vertexCount; i++)
            {
                key[i] = double.MaxValue;
                included[i] = false;
            }

            key[0] = 0;
            parent[0] = -1;

            for (int count = 0; count < vertexCount - 1; count++)
            {
                double min = double.MaxValue;
                int minIndex = 0;

                for (int v = 0; v < vertexCount; v++)
                {
                    if (!included[v] && key[v] < min)
                    {
                        min = key[v];
                        minIndex = v;
                    }
                }

                included[minIndex] = true;

                for (int v = 0; v < vertexCount; v++)
                {
                    if (adjacencyMatrix[minIndex, v] != 0 &&
                        !included[v] &&
                        adjacencyMatrix[minIndex, v] < key[v])
                    {
                        parent[v] = minIndex;
                        key[v] = adjacencyMatrix[minIndex, v];
                    }
                }
            }

            double[,] optimizedConnections = new double[vertexCount, vertexCount];
            for (int i = 1; i < vertexCount; i++)
            {
                optimizedConnections[parent[i], i] = adjacencyMatrix[parent[i], i];
                optimizedConnections[i, parent[i]] = adjacencyMatrix[i, parent[i]];
            }

            return (vertices, optimizedConnections);
        }
    }
}

