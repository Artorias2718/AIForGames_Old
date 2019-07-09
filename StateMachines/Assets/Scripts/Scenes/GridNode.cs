using UnityEngine;
using System.Collections.Generic;

namespace AISandbox
{
    public class GridNode : MonoBehaviour
    {
        #region Variables
        public Grid m_grid;
        public Sprite[] m_tiles;
        public int m_column;
        public int m_row;
        private SpriteRenderer m_sprite_renderer;
        private int m_cost;

        private static Sprite m_path_sprite;
        private static Sprite m_blocked_sprite;
        private Sprite m_original_sprite;
        private Color m_original_color;
        private TextMesh m_priority_label;

        #endregion

        #region Getters_Setters
        [SerializeField]
        private bool m_blocked = false;
        public bool Blocked
        {
            get
            {
                return m_blocked;
            }
            set
            {
                m_blocked = value;
                m_sprite_renderer.sprite = m_blocked ? m_blocked_sprite : m_original_sprite;
                m_sprite_renderer.color = m_original_color;
            }
        }

        public SpriteRenderer Sprite_Renderer
        {
            get { return m_sprite_renderer; }
        }

        public static Sprite Path_Sprite
        {
            get { return m_path_sprite; }
        }

        public static Sprite Blocked_Sprite
        {
            get { return m_blocked_sprite; }
        }

        public Sprite Original_Sprite
        {
            get { return m_original_sprite; }
        }

        public Vector2 Position
        {
            get { return transform.position; }
        }

        public int Cost
        {
            get { return m_cost; }
        }

        #endregion

        #region Unity
        private void Awake()
        {
            m_sprite_renderer = GetComponent<SpriteRenderer>();
            m_priority_label = GetComponentInChildren<TextMesh>();
            
            m_original_color = m_sprite_renderer.color;
            m_path_sprite = m_tiles[0];
            m_blocked_sprite = m_tiles[1];
            m_original_sprite = m_tiles[Random.Range(2, m_tiles.Length)];
            m_cost = System.Array.IndexOf(m_tiles, m_original_sprite) - 1;
            m_sprite_renderer.sprite = m_original_sprite;
        }
        #endregion

        #region Custom
        public IList<GridNode> GetNeighbors(bool include_diagonal = false)
        {
            return m_grid.GetNodeNeighbors(m_row, m_column, include_diagonal);
        }

        public void SetPriority(int priority)
        {
            if (priority >= 0)
            {
                m_priority_label.text = priority.ToString();
            }
            else
            {
                m_priority_label.text = "";
            }
        }
        #endregion
    }
}