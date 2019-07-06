using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AISandbox
{
    public class PursuitEvasion : MonoBehaviour
    {
        #region Variables
        private const float SPAWN_RANGE = 10.0f;

        public float reset = 5.0f;

        public Boid m_target;
        public Boid m_pursuer;
        public Boid m_evader;

        public Crosshair m_crosshair;

        #endregion

        #region Getters_Setters

        #endregion

        #region Unity

        private void Awake()
        {
            // Choose a random position for the target actor
            Vector3 position = new Vector3(Random.Range(-SPAWN_RANGE, SPAWN_RANGE), Random.Range(-SPAWN_RANGE, SPAWN_RANGE), 0.0f);
            m_target.transform.position = position;

            // The pursuing and evading actor start at the same position
            position = new Vector3(Random.Range(-SPAWN_RANGE, SPAWN_RANGE), Random.Range(-SPAWN_RANGE, SPAWN_RANGE), 0.0f);
            m_pursuer.transform.position = position;
            m_evader.transform.position = position;

            // Reset the level every 15 seconds
            Invoke("ResetLevel", reset);
        }


        #endregion

        #region Custom

        private void ResetLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        #endregion
    }
}