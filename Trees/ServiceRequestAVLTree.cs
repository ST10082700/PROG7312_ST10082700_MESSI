using PROG_ST10082700_MESSI.Trees.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROG_ST10082700_MESSI.Trees
{
    /// <summary>
    /// AVL Tree implementation for Service Requests.
    /// Provides self-balancing binary search tree functionality with O(log n) operations.
    /// </summary>
    public class ServiceRequestAVLTree
    {
        private AVLNode<ServiceRequest> root;

        /// <summary>
        /// Gets the height of a node. Returns 0 for null nodes.
        /// </summary>
        private int GetHeight(AVLNode<ServiceRequest> node)
        {
            return node == null ? 0 : node.Height;
        }

        /// <summary>
        /// Calculates the balance factor of a node.
        /// Balance factor = height of left subtree - height of right subtree
        /// </summary>
        private int GetBalance(AVLNode<ServiceRequest> node)
        {
            return node == null ? 0 : GetHeight(node.Left) - GetHeight(node.Right);
        }

        /// <summary>
        /// Performs a right rotation to maintain AVL tree balance
        /// </summary>
        /// <param name="y">The node to rotate around</param>
        /// <returns>The new root node after rotation</returns>
        private AVLNode<ServiceRequest> RightRotate(AVLNode<ServiceRequest> y)
        {
            AVLNode<ServiceRequest> x = y.Left;
            AVLNode<ServiceRequest> T2 = x.Right;

            // Perform rotation
            x.Right = y;
            y.Left = T2;

            // Update heights
            y.Height = Math.Max(GetHeight(y.Left), GetHeight(y.Right)) + 1;
            x.Height = Math.Max(GetHeight(x.Left), GetHeight(x.Right)) + 1;

            return x;
        }

        /// <summary>
        /// Performs a left rotation to maintain AVL tree balance
        /// </summary>
        /// <param name="x">The node to rotate around</param>
        /// <returns>The new root node after rotation</returns>
        private AVLNode<ServiceRequest> LeftRotate(AVLNode<ServiceRequest> x)
        {
            AVLNode<ServiceRequest> y = x.Right;
            AVLNode<ServiceRequest> T2 = y.Left;

            // Perform rotation
            y.Left = x;
            x.Right = T2;

            // Update heights
            x.Height = Math.Max(GetHeight(x.Left), GetHeight(x.Right)) + 1;
            y.Height = Math.Max(GetHeight(y.Left), GetHeight(y.Right)) + 1;

            return y;
        }

        /// <summary>
        /// Inserts a new service request into the AVL tree
        /// </summary>
        public void Insert(ServiceRequest request)
        {
            root = InsertRec(root, request);
        }

        /// <summary>
        /// Recursive helper method for insertion
        /// Maintains AVL tree balance after insertion
        /// </summary>
        private AVLNode<ServiceRequest> InsertRec(AVLNode<ServiceRequest> node, ServiceRequest request)
        {
            // 1. Perform standard BST insertion
            if (node == null)
                return new AVLNode<ServiceRequest>(request);

            if (request.CompareTo(node.Data) < 0)
                node.Left = InsertRec(node.Left, request);
            else if (request.CompareTo(node.Data) > 0)
                node.Right = InsertRec(node.Right, request);
            else
                return node; // Duplicate requests not allowed

            // 2. Update height of current node
            node.Height = 1 + Math.Max(GetHeight(node.Left), GetHeight(node.Right));

            // 3. Get the balance factor and balance if needed
            int balance = GetBalance(node);

            // Left Left Case
            if (balance > 1 && request.CompareTo(node.Left.Data) < 0)
                return RightRotate(node);

            // Right Right Case
            if (balance < -1 && request.CompareTo(node.Right.Data) > 0)
                return LeftRotate(node);

            // Left Right Case
            if (balance > 1 && request.CompareTo(node.Left.Data) > 0)
            {
                node.Left = LeftRotate(node.Left);
                return RightRotate(node);
            }

            // Right Left Case
            if (balance < -1 && request.CompareTo(node.Right.Data) < 0)
            {
                node.Right = RightRotate(node.Right);
                return LeftRotate(node);
            }

            return node;
        }

        /// <summary>
        /// Performs an inorder traversal of the tree
        /// </summary>
        /// <param name="action">Action to perform on each node's data</param>
        public void InorderTraversal(Action<ServiceRequest> action)
        {
            InorderTraversalRec(root, action);
        }

        private void InorderTraversalRec(AVLNode<ServiceRequest> node, Action<ServiceRequest> action)
        {
            if (node != null)
            {
                InorderTraversalRec(node.Left, action);
                action(node.Data);
                InorderTraversalRec(node.Right, action);
            }
        }

        /// <summary>
        /// Searches for a service request in the tree
        /// </summary>
        /// <returns>The found service request or null if not found</returns>
        public ServiceRequest Search(ServiceRequest request)
        {
            return SearchRec(root, request)?.Data;
        }

        private AVLNode<ServiceRequest> SearchRec(AVLNode<ServiceRequest> node, ServiceRequest request)
        {
            if (node == null || request.CompareTo(node.Data) == 0)
                return node;

            if (request.CompareTo(node.Data) < 0)
                return SearchRec(node.Left, request);

            return SearchRec(node.Right, request);
        }
    }
}