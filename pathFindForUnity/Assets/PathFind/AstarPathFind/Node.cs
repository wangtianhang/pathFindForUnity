using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathFind
{
    class Node
    {
        public Vector2i m_pos = new Vector2i();
        public Node m_next = null;
        public Node m_prev = null;
        public float m_heuristic = 0;
        public void clear()
        {
            m_pos = new Vector2i();
            m_next = null;
            m_prev = null;
            m_heuristic = 0;
        }
    }

    class NodePool
    {
        int m_pathSearchNodeMax = 0;
        public NodePool(int pathSearchNodeMax)
        {
            m_pathSearchNodeMax = pathSearchNodeMax;

            for (int i = 0; i < m_pathSearchNodeMax; ++i)
            {
                Node node = new Node();
                m_node_list.Add(node);
            }
        }

        public void clear()
        {
            for (int i = 0; i < m_node_list.Count; ++i)
            {
                m_node_list[i].clear();
            }
            m_index = 0;
        }

        public Node getNewNode()
        {
            if (m_index < m_node_list.Count)
            {
                Node node = m_node_list[m_index];
                m_index++;
                return node;
            }
            else
            {
                return null;
            }
        }

        int m_index = 0;
        List<Node> m_node_list = new List<Node>();
    }
}
