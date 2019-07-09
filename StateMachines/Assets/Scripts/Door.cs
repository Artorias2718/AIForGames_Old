using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AISandbox
{
    public class Door : MonoBehaviour
    {
        #region Variables
        public Sprite m_unlocked_sprite;
        private SpriteRenderer m_sprite_renderer;
        #endregion

        #region Getters_Setters
        public Vector2 Position { get { return transform.position; } set { transform.position = value; } }
        public bool Collided { get; set; }
        #endregion

        #region Unity
        private void Start()
        {
            m_sprite_renderer = GetComponent<SpriteRenderer>();
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            PathFollower path_follower = collision.GetComponentInChildren<PathFollower>();
            Collided = (path_follower != null);
            transform.Find("Lock").gameObject.SetActive(false);
            m_sprite_renderer.sprite = m_unlocked_sprite;
        }
        #endregion

        #region Custom

        #endregion
    }
}