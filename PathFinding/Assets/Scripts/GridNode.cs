using UnityEngine;
using System.Collections.Generic;

namespace AISandbox
{
    public class GridNode : MonoBehaviour
    {
        #region Variables
        public Grid m_grid;
        public int m_column;
        public int m_row;

        private SpriteRenderer m_sprite_renderer;
        private Color m_original_color;
        private Color m_blocked_color;
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
                m_sprite_renderer.color = m_blocked ? m_blocked_color : m_original_color;
            }
        }

        public SpriteRenderer Sprite_Renderer
        {
            get { return m_sprite_renderer; }
        }

        public Color Original_Color
        {
            get
            {
                return m_original_color;
            }
        }
        #endregion

        #region Unity
        private void Awake()
        {
            m_sprite_renderer = GetComponent<SpriteRenderer>();
            m_priority_label = GetComponentInChildren<TextMesh>();
            m_original_color = m_sprite_renderer.color;
            m_blocked_color = new Color(0.5f * m_original_color.r, 0.5f * m_original_color.g, 0.5f * m_original_color.b);
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