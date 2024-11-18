using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROG_ST10082700_MESSI.Trees.Nodes
{
    public class AVLNode<T> where T : IComparable<T>
    {
        public T Data { get; set; }
        public AVLNode<T> Left { get; set; }
        public AVLNode<T> Right { get; set; }
        public int Height { get; set; }

        public AVLNode(T data)
        {
            Data = data;
            Height = 1;
        }
    }
}
