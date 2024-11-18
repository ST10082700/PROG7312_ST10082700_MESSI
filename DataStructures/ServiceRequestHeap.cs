using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PROG_ST10082700_MESSI.Models;

namespace PROG_ST10082700_MESSI.DataStructures
{
    /// <summary>
    /// Implements a Max Heap for Service Requests prioritization based on various factors.
    /// Used for organizing requests by priority and response time requirements.
    /// </summary>
    public class ServiceRequestHeap
    {
        private ServiceRequest[] heap;
        private int size;
        private readonly int maxSize;

        public ServiceRequestHeap(int maxSize = 100)
        {
            this.maxSize = maxSize;
            heap = new ServiceRequest[maxSize];
            size = 0;
        }

        // Helper methods for navigating the heap
        private int Parent(int pos) => (pos - 1) / 2;
        private int LeftChild(int pos) => (2 * pos) + 1;
        private int RightChild(int pos) => (2 * pos) + 2;

        /// <summary>
        /// Calculates priority score based on multiple factors
        /// </summary>
        private int CalculatePriorityScore(ServiceRequest request)
        {
            // Base priority score
            int priorityScore = request.Priority?.ToLower() switch
            {
                "critical" => 100,
                "high" => 75,
                "medium" => 50,
                "low" => 25,
                _ => 0
            };

            // Add age factor (older requests get higher priority)
            int daysOld = (DateTime.Now - request.SubmissionDate).Days;
            priorityScore += Math.Min(daysOld * 2, 50); // Cap age bonus at 50

            // Category weight
            priorityScore += request.Category?.ToLower() switch
            {
                "utilities" => 30, // Critical infrastructure
                "roads" => 25,     // Safety concern
                "sanitation" => 20,// Health concern
                _ => 15           // Other categories
            };

            return priorityScore;
        }

        private void Swap(int pos1, int pos2)
        {
            var temp = heap[pos1];
            heap[pos1] = heap[pos2];
            heap[pos2] = temp;
        }

        /// <summary>
        /// Inserts a new request into the heap maintaining max-heap property
        /// </summary>
        public void Insert(ServiceRequest request)
        {
            if (size >= maxSize)
                throw new InvalidOperationException("Heap is full");

            // Add new request at the end
            heap[size] = request;
            int current = size;
            size++;

            // Heapify up - move the new request up if it has higher priority
            while (current > 0 &&
                   CalculatePriorityScore(heap[current]) >
                   CalculatePriorityScore(heap[Parent(current)]))
            {
                Swap(current, Parent(current));
                current = Parent(current);
            }
        }

        /// <summary>
        /// Removes and returns the highest priority request
        /// </summary>
        public ServiceRequest ExtractMax()
        {
            if (size <= 0)
                throw new InvalidOperationException("Heap is empty");

            ServiceRequest max = heap[0];
            heap[0] = heap[size - 1];
            size--;
            MaxHeapify(0);

            return max;
        }

        /// <summary>
        /// Maintains max-heap property starting from given position
        /// </summary>
        private void MaxHeapify(int pos)
        {
            int left = LeftChild(pos);
            int right = RightChild(pos);
            int largest = pos;

            // Find the largest among parent and children
            if (left < size &&
                CalculatePriorityScore(heap[left]) > CalculatePriorityScore(heap[largest]))
                largest = left;

            if (right < size &&
                CalculatePriorityScore(heap[right]) > CalculatePriorityScore(heap[largest]))
                largest = right;

            // If largest is not parent, swap and continue heapifying
            if (largest != pos)
            {
                Swap(pos, largest);
                MaxHeapify(largest);
            }
        }

        /// <summary>
        /// Returns all requests sorted by priority
        /// </summary>
        public ServiceRequest[] GetSorted()
        {
            ServiceRequest[] sorted = new ServiceRequest[size];
            int originalSize = size;

            for (int i = 0; i < originalSize; i++)
            {
                sorted[i] = ExtractMax();
            }

            // Restore heap
            foreach (var request in sorted)
            {
                Insert(request);
            }

            return sorted;
        }
    }
}