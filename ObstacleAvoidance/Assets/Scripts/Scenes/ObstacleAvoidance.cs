using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AISandbox
{
    public class ObstacleAvoidance : MonoBehaviour
    {
        #region Variables
        public int m_boid_count;
        public Avoidance m_avoidance_prefab;

        private List<Avoidance> m_avoidance_list;
        #endregion

        #region Getters_Setters

        #endregion

        #region Unity
        private void Awake()
        {
            m_avoidance_list = new List<Avoidance>();
            for (int i = 0; i < m_boid_count; ++i)
            {
                Avoidance avoidance = Instantiate(m_avoidance_prefab);
                avoidance.name = "Avoidance";
                avoidance.gameObject.SetActive(true);
                m_avoidance_list.Add(avoidance);
            }
        }
        #endregion

        #region Custom

        #endregion
    }
}