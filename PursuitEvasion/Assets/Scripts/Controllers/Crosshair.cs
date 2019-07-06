using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AISandbox
{
    public class Crosshair : MonoBehaviour
    {
        #region Variables

        #endregion

        #region Getters_Setters
        public Vector2 Future_Position { get; set; }
        #endregion

        #region Unity
        private void Start()
        {

        }

        private void Update()
        {
            transform.position = Future_Position;
        }
        #endregion

        #region Custom

        #endregion
    }
}