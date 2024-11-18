using PROG_ST10082700_MESSI.Trees.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROG_ST10082700_MESSI.Trees
{
    /// <summary>
    /// Red-Black Tree implementation for Service Requests.
    /// Provides self-balancing binary search tree with O(log n) operations and better insertion performance.
    /// </summary>
    public class ServiceRequestRedBlackTree
    {
        private RedBlackNode<ServiceRequest> root;
        private readonly RedBlackNode<ServiceRequest> TNULL;

        public ServiceRequestRedBlackTree()
        {
            TNULL = new RedBlackNode<ServiceRequest>(null) { Color = NodeColor.Black };
            root = TNULL;
        }

        /// <summary>
        /// Fixes the Red-Black tree properties after insertion
        /// </summary>
        private void FixInsert(RedBlackNode<ServiceRequest> k)
        {
            RedBlackNode<ServiceRequest> u;
            while (k.Parent.Color == NodeColor.Red)
            {
                if (k.Parent == k.Parent.Parent.Right)
                {
                    u = k.Parent.Parent.Left;
                    if (u.Color == NodeColor.Red)
                    {
                        u.Color = NodeColor.Black;
                        k.Parent.Color = NodeColor.Black;
                        k.Parent.Parent.Color = NodeColor.Red;
                        k = k.Parent.Parent;
                    }
                    else
                    {
                        if (k == k.Parent.Left)
                        {
                            k = k.Parent;
                            RightRotate(k);
                        }
                        k.Parent.Color = NodeColor.Black;
                        k.Parent.Parent.Color = NodeColor.Red;
                        LeftRotate(k.Parent.Parent);
                    }
                }
                else
                {
                    u = k.Parent.Parent.Right;
                    if (u.Color == NodeColor.Red)
                    {
                        u.Color = NodeColor.Black;
                        k.Parent.Color = NodeColor.Black;
                        k.Parent.Parent.Color = NodeColor.Red;
                        k = k.Parent.Parent;
                    }
                    else
                    {
                        if (k == k.Parent.Right)
                        {
                            k = k.Parent;
                            LeftRotate(k);
                        }
                        k.Parent.Color = NodeColor.Black;
                        k.Parent.Parent.Color = NodeColor.Red;
                        RightRotate(k.Parent.Parent);
                    }
                }
                if (k == root)
                    break;
            }
            root.Color = NodeColor.Black;
        }

        /// <summary>
        /// Performs a left rotation to maintain Red-Black tree properties
        /// </summary>
        private void LeftRotate(RedBlackNode<ServiceRequest> x)
        {
            RedBlackNode<ServiceRequest> y = x.Right;
            x.Right = y.Left;
            if (y.Left != TNULL)
                y.Left.Parent = x;
            y.Parent = x.Parent;
            if (x.Parent == null)
                root = y;
            else if (x == x.Parent.Left)
                x.Parent.Left = y;
            else
                x.Parent.Right = y;
            y.Left = x;
            x.Parent = y;
        }

        /// <summary>
        /// Performs a right rotation to maintain Red-Black tree properties
        /// </summary>
        private void RightRotate(RedBlackNode<ServiceRequest> x)
        {
            RedBlackNode<ServiceRequest> y = x.Left;
            x.Left = y.Right;
            if (y.Right != TNULL)
                y.Right.Parent = x;
            y.Parent = x.Parent;
            if (x.Parent == null)
                root = y;
            else if (x == x.Parent.Right)
                x.Parent.Right = y;
            else
                x.Parent.Left = y;
            y.Right = x;
            x.Parent = y;
        }

        /// <summary>
        /// Inserts a new service request into the Red-Black tree
        /// </summary>
        public void Insert(ServiceRequest request)
        {
            RedBlackNode<ServiceRequest> node = new RedBlackNode<ServiceRequest>(request);
            RedBlackNode<ServiceRequest> y = null;
            RedBlackNode<ServiceRequest> x = root;

            while (x != TNULL)
            {
                y = x;
                if (node.Data.CompareTo(x.Data) < 0)
                    x = x.Left;
                else
                    x = x.Right;
            }

            node.Parent = y;
            if (y == null)
                root = node;
            else if (node.Data.CompareTo(y.Data) < 0)
                y.Left = node;
            else
                y.Right = node;

            node.Left = TNULL;
            node.Right = TNULL;
            node.Color = NodeColor.Red;

            FixInsert(node);
        }

        /// <summary>
        /// Searches for a service request in the Red-Black tree
        /// </summary>
        /// <returns>The found service request or null if not found</returns>
        public ServiceRequest Search(ServiceRequest request)
        {
            return SearchRec(root, request);
        }

        private ServiceRequest SearchRec(RedBlackNode<ServiceRequest> node, ServiceRequest request)
        {
            if (node == TNULL || request.CompareTo(node.Data) == 0)
                return node?.Data;

            if (request.CompareTo(node.Data) < 0)
                return SearchRec(node.Left, request);

            return SearchRec(node.Right, request);
        }

        /// <summary>
        /// Performs an inorder traversal of the Red-Black tree
        /// </summary>
        public void InorderTraversal(Action<ServiceRequest> action)
        {
            InorderTraversalRec(root, action);
        }

        private void InorderTraversalRec(RedBlackNode<ServiceRequest> node, Action<ServiceRequest> action)
        {
            if (node != TNULL)
            {
                InorderTraversalRec(node.Left, action);
                action(node.Data);
                InorderTraversalRec(node.Right, action);
            }
        }
    }
}

