using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace AISandbox
{
    public class PathFinding : MonoBehaviour
    {
        #region Variables
        public Grid m_grid;

        private PriorityQueue<int, GridNode> m_frontier;        // Open Set
        private Dictionary<GridNode, GridNode> m_visited;       // Visited Nodes
        private List<GridNode> m_interior;                      // Closed Set
        private GridNode m_start_node;
        private GridNode m_goal_node;
        private List<GridNode> m_path;

        private List<int> m_visited_priorities;
        private List<int> m_path_priorities;

        #endregion

        #region Getters_Setters

        public Grid Grid
        {
            get
            {
                return m_grid;
            }
        }
        public GridNode StartNode
        {
            get
            {
                return m_start_node;
            }
            set
            {
                m_start_node = value;
            }
        }

        public GridNode GoalNode
        {
            get
            {
                return m_goal_node;
            }
            set
            {
                m_goal_node = value;
            }
        }

        public List<GridNode> Path
        {
            get
            {
                return m_path;
            }
        }

        #endregion

        #region Unity

        #endregion

        #region Custom
        public GridNode[] FindNewPath(GridNode start = null, GridNode goal = null)
        {
            ClearLists();
            SetEndPoints(start, goal);
            FindPath();

            //Debug.Log("Path Count: " + ((m_path != null) ? m_path.Count : 0));
            return m_path.ToArray();
        }

        private void ClearLists()
        {
            if (m_visited != null)
            {
                foreach (GridNode visited_node in m_visited.Keys)
                {
                    visited_node.Sprite_Renderer.sprite = visited_node.Original_Sprite;
                    visited_node.SetPriority(-1);
                }
            }

            if (m_path != null)
            {
                foreach (GridNode path_node in m_path)
                {
                    path_node.Sprite_Renderer.sprite = path_node.Original_Sprite;
                    path_node.SetPriority(-1);
                }
            }
        }

        public void SetEndPoints(GridNode start = null)
        {
            GridNode[] unblocked_nodes = (
                from GridNode node in m_grid.Nodes
                where !node.Blocked
                select node
                ).ToArray();

            if (start)
            {
                m_start_node = start;
                int goal_index = 0;
                do
                {
                    goal_index = UnityEngine.Random.Range(0, unblocked_nodes.Length);
                }
                while (start == unblocked_nodes[goal_index]);

                m_goal_node = unblocked_nodes[goal_index];
            }
            else
            {
                int start_index = UnityEngine.Random.Range(0, unblocked_nodes.Length);
                int goal_index = 0;

                m_start_node = unblocked_nodes[start_index];
                do
                {
                    goal_index = UnityEngine.Random.Range(0, unblocked_nodes.Length);
                } while (start_index == goal_index);

                m_goal_node = unblocked_nodes[goal_index];
            }
        }

        public void SetEndPoints(GridNode start, GridNode goal)
        {
            m_start_node = start;
            m_goal_node = goal;
        }

        public void FindPath()
        {
            m_frontier = new PriorityQueue<int, GridNode>();
            m_visited = new Dictionary<GridNode, GridNode>();
            m_path = new List<GridNode>();
            m_visited_priorities = new List<int>();
            m_path_priorities = new List<int>();

            m_frontier.Enqueue(0, m_start_node);
            m_visited.Add(m_start_node, null);

            // *   Remove the first node from the frontier queue

            // *   If it's the goal node, we're done constructing the path

            // *   Otherwise, mark it as visited and assign priority values
            //     based on the Heuristic result between each of the current
            //     node's neighbors and the goal node
            //     (The lower the priority value, the more preference a node is given)
            // *   Note that each pair of neighboring nodes links back to the corresponding current node

            while (!m_frontier.IsEmpty)
            {
                GridNode current = m_frontier.Dequeue().Value;
                if (current == m_goal_node)
                {
                    break;
                }
                foreach (GridNode neighbor in m_grid.GetNodeNeighbors(current.m_row, current.m_column, true))
                {
                    if (m_visited.ContainsKey(neighbor) || neighbor.Blocked)
                    {
                        continue;
                    }

                    int priority = Heuristic(neighbor, m_goal_node);
                    m_frontier.Enqueue(priority, neighbor);
                    m_visited.Add(neighbor, current);
                    m_visited_priorities.Add(priority);
                }
            }

            //  *   To reconstruct the path, take each value node in the key/value pairs and add it to the path
            //      as long as it's not null

            GridNode path_constructor = m_goal_node;
            m_path_priorities.Add(1);

            while (path_constructor != m_start_node)
            {
                m_path.Add(path_constructor);

                if (m_visited[path_constructor] != null)
                {
                    path_constructor = m_visited[path_constructor];
                    int priority = Heuristic(path_constructor, m_goal_node);
                    m_path_priorities.Add(priority);
                }
            }

            //  Remove the start node from the visited nodes; it does not have a valid link
            m_visited = m_visited.Where(node_pair => node_pair.Value != null).ToDictionary(new_node_pair => new_node_pair.Key, new_node_pair => new_node_pair.Value);

            //  Add the start node back into the path and reverse the order of the path nodes (the path is currently backwards)
            int start_priority = Heuristic(m_start_node, m_goal_node);

            m_path.Add(m_start_node);
            m_path.Reverse();
            m_path_priorities.Reverse();
        }

        private int Heuristic(GridNode node_a, GridNode node_b)
        {
            Vector3 dist = node_b.transform.position - node_a.transform.position;
            return (int)(Mathf.Abs(dist.x) + Mathf.Abs(dist.y) + Mathf.Abs(dist.z)) + node_a.Cost;
        }

        public void MarkPath()
        {
            int i = 0;

            if (m_visited != null)
            {
                foreach (GridNode visited_node in m_visited.Keys)
                {
                    visited_node.Sprite_Renderer.sprite = GridNode.Path_Sprite;
                    visited_node.Sprite_Renderer.color = new Color(1.0f / 255, 133.0f / 255, 189.0f / 255);
                    visited_node.SetPriority(m_visited_priorities[i]);
                    if (i < m_visited_priorities.Count - 1)
                    {
                        i++;
                    }
                }
            }

            i = 0;

            if (m_path != null)
            {
                foreach (GridNode path_node in m_path)
                {
                    path_node.Sprite_Renderer.sprite = GridNode.Path_Sprite;
                    path_node.Sprite_Renderer.color = new Color(247.0f / 255, 91.0f / 255, 0);
                    path_node.SetPriority(m_path_priorities[i]);
                    if (i < m_path_priorities.Count - 1)
                    {
                        i++;
                    }
                }
            }

            if (m_start_node != null)
            {
                m_start_node.Sprite_Renderer.sprite = GridNode.Path_Sprite;
                m_start_node.Sprite_Renderer.color = Color.green;
            }

            if (m_goal_node != null)
            {
                m_goal_node.Sprite_Renderer.sprite = GridNode.Path_Sprite;
                m_goal_node.Sprite_Renderer.color = Color.red;
            }
        }
        #endregion
    }
}