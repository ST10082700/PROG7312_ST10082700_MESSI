using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROG_ST10082700_MESSI.Trees.Nodes
{
    public enum NodeColor { Red, Black }

    public class RedBlackNode<T> where T : IComparable<T>
    {
        public T Data { get; set; }
        public RedBlackNode<T> Left { get; set; }
        public RedBlackNode<T> Right { get; set; }
        public RedBlackNode<T> Parent { get; set; }
        public NodeColor Color { get; set; }

        public RedBlackNode(T data)
        {
            Data = data;
            Color = NodeColor.Red;
        }
    }
}
