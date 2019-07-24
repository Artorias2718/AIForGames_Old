using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AISandbox
{
    public class Key : MonoBehaviour
    {
        #region Variables
    
        #endregion

        #region Getters_Setters
        public Vector2 Position { get { return transform.position; } set { transform.position = value; } }
        public bool Collided { get; set; }
        #endregion

        #region Unity
        private void OnTriggerEnter2D(Collider2D collision)
        {
            PathFollower path_follower = collision.GetComponentInChildren<PathFollower>();
            Collided = (path_follower != null);
            gameObject.SetActive(false);
        }
        #endregion

        #region Custom

        #endregion
    }
}