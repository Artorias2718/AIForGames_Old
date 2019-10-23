using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AISandbox
{
    public class MineSweepers : MonoBehaviour
    {
        #region Variables
        public TankBoid m_tank_prefab;
        public int m_tank_count = 15;

        private List<TankBoid> m_tank_list;

        #endregion

        #region Getters_Setters

        #endregion

        #region Unity
        private void Start()
        {
            m_tank_prefab = transform.Find("Tank").GetComponentInChildren<TankBoid>(true);

            m_tank_list = new List<TankBoid>();

            for (int i = 0; i < m_tank_count; ++i)
            {
                TankBoid tank = Instantiate(m_tank_prefab);
                tank.name = string.Format("Tank {0}", (i + 1));
                tank.gameObject.SetActive(true);
                m_tank_list.Add(tank);
            }
        }

        #endregion

        #region Custom

        #endregion
    }
}