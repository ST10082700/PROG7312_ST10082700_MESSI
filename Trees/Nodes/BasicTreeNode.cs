using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROG_ST10082700_MESSI.Trees.Nodes
{
    public class BasicTreeNode<T>
    {
        public T Data { get; set; }
        public BasicTreeNode<T>[] Children { get; set; }

        public BasicTreeNode(T data, int maxChildren = 10)
        {
            Data = data;
            Children = new BasicTreeNode<T>[maxChildren];
        }
    }
}
