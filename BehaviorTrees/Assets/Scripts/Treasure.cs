using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AISandbox
{
    public class Treasure : MonoBehaviour
    {
        #region Variables
        public Sprite m_unlocked_chest;
        private SpriteRenderer m_sprite_renderer;
        #endregion

        #region Getters_Setters
        public Vector2 Position { get { return transform.position; } set { transform.position = value; } }
        public bool Collided { get; set; }
        #endregion

        #region Unity
        private void Awake()
        {
            m_sprite_renderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            PathFollower path_follower = collision.GetComponentInChildren<PathFollower>();
            Collided = (path_follower != null);
            m_sprite_renderer.sprite = m_unlocked_chest;
        }
        #endregion

        #region Custom

        #endregion
    }
}