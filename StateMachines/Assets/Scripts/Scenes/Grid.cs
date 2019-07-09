using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AISandbox
{
    public class Grid : MonoBehaviour
    {
        #region Variables
        public GridNode m_grid_node_prefab;

        public int m_rows = 30;
        public int m_columns = 30;

        private GridNode[,] m_nodes;
        private float m_node_width;
        private float m_node_height;
        private bool m_draw_blocked;
        #endregion

        #region Getters_Setters

        public Vector2 Size
        {
            get
            {
                return new Vector2(m_node_width * m_nodes.GetLength(1), m_node_height * m_nodes.GetLength(0));
            }
        }

        public GridNode[,] Nodes
        {
            get
            {
                return m_nodes;
            }
        }

        #endregion

        #region Unity

        private void Awake()
        {
            m_nodes = new GridNode[m_rows, m_columns];

            // Create and center the grid
            Create(m_rows, m_columns);
            Vector2 grid_size = Size;
            Vector2 grid_position = 0.5f * new Vector2(-grid_size.x, grid_size.y);
            transform.position = grid_position;
            BlockNodes();
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 world_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 local_pos = transform.InverseTransformPoint(world_pos);

                // This trick makes a lot of assumptions that the nodes haven't been modified since initialization.
                int column = Mathf.FloorToInt(local_pos.x / m_node_width);
                int row = Mathf.FloorToInt(-local_pos.y / m_node_height);
                if (row >= 0 && row < m_nodes.GetLength(0)
                 && column >= 0 && column < m_nodes.GetLength(1))
                {
                    GridNode node = m_nodes[row, column];
                    if (Input.GetMouseButtonDown(0))
                    {
                        m_draw_blocked = !node.Blocked;
                    }
                    if (node.Blocked != m_draw_blocked)
                    {
                        node.Blocked = m_draw_blocked;
                    }
                }
            }
        }
        #endregion

        #region Custom        
        private GridNode CreateNode(int row, int col)
        {
            GridNode node = Instantiate<GridNode>(m_grid_node_prefab);
            node.name = string.Format("Node {0}{1}", (char)('A' + row), col);
            node.m_grid = this;
            node.m_row = row;
            node.m_column = col;
            node.transform.parent = transform;
            node.gameObject.SetActive(true);
            return node;
        }

        public void Create(int rows, int columns)
        {
            m_node_width = m_grid_node_prefab.GetComponent<Renderer>().bounds.size.x;
            m_node_height = m_grid_node_prefab.GetComponent<Renderer>().bounds.size.y;
            Vector2 node_position = new Vector2(m_node_width * 0.5f, m_node_height * -0.5f);
            for (int row = 0; row < rows; ++row)
            {
                for (int col = 0; col < columns; ++col)
                {
                    GridNode node = CreateNode(row, col);
                    node.transform.localPosition = node_position;
                    m_nodes[row, col] = node;

                    node_position.x += m_node_width;
                }
                node_position.x = m_node_width * 0.5f;
                node_position.y -= m_node_height;
            }
        }

        public GridNode GetNode(int row, int col)
        {
            return m_nodes[row, col];
        }

        public IList<GridNode> GetNodeNeighbors(int row, int col, bool include_diagonal = false)
        {
            IList<GridNode> neighbors = new List<GridNode>();

            int start_row = Mathf.Max(row - 1, 0);
            int start_col = Mathf.Max(col - 1, 0);
            int end_row = Mathf.Min(row + 1, m_nodes.GetLength(0) - 1);
            int end_col = Mathf.Min(col + 1, m_nodes.GetLength(1) - 1);

            for (int row_index = start_row; row_index <= end_row; ++row_index)
            {
                for (int col_index = start_col; col_index <= end_col; ++col_index)
                {
                    if (include_diagonal || row_index == row || col_index == col)
                    {
                        neighbors.Add(m_nodes[row_index, col_index]);
                    }
                }
            }
            return neighbors;
        }

        public GridNode GetNodeFromPosition(Vector2 position)
        {
            Vector2 world_pos = position;
            Vector2 local_pos = transform.InverseTransformPoint(world_pos);

            int column = Mathf.FloorToInt(local_pos.x / m_node_width);
            int row = Mathf.FloorToInt(-local_pos.y / m_node_height);

            if (row >= 0 && row < m_nodes.GetLength(0) && column >= 0 && column < m_nodes.GetLength(1))
            {
                return m_nodes[row, column];
            }
            else
            {
                return null;
            }
        }

        private void BlockNodes()
        {
            foreach (GridNode node in Nodes)
            {
                int block = UnityEngine.Random.Range(0, 1000);
                node.Blocked = block < 100 ? true : false;
            }
        }
        #endregion
    }
}