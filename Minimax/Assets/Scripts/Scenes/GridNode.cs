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
        public Sprite[] m_value_sprites;

        private Color m_original_color;
        private SpriteRenderer m_sprite_renderer;
        private int m_value;
        #endregion

        #region Getters_Setters

        public Vector2 Position
        {
            get { return transform.position; }
        }

        public int Value
        {
            get { return m_value; }
            set
            {
                m_value = value;
                m_sprite_renderer.sprite = m_value_sprites[m_value];
                switch (m_value)
                {
                    case 0:
                        m_sprite_renderer.color = Color.white;
                        break;
                    case 1:
                        m_sprite_renderer.color = Color.blue;
                        break;
                    case 2:
                        m_sprite_renderer.color = Color.red;
                        break;
                }
            }
        }

        #endregion

        #region Unity
        private void Start()
        {
            m_sprite_renderer = GetComponent<SpriteRenderer>();
        }
        //private void Update()
        //{
        //    if (m_value == 0)
        //    {
        //        m_sprite_renderer.sprite = m_value_sprites[0];
        //        m_sprite_renderer.color = Color.white;
        //    }

        //    if (m_value == 1)
        //    {
        //        m_sprite_renderer.sprite = m_value_sprites[1];
        //        m_sprite_renderer.color = Color.blue;
        //    }

        //    if (m_value == 2)
        //    {
        //        m_sprite_renderer.sprite = m_value_sprites[2];
        //        m_sprite_renderer.color = Color.red;
        //    }
        //}

        #endregion

        #region Custom

        #endregion
    }
}