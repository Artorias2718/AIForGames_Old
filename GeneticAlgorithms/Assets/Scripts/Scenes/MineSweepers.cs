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
        public Transform m_mine_prefab;
        public int m_tank_count = 15;
        public int m_mine_count = 30;

        private const float m_SPAWN_RANGE = 15.0f;
        private const float m_SPAWN_OFFSET = 4.0f;
        private List<TankBoid> m_tank_list;
        private List<Transform> m_mine_list;

        #endregion

        #region Getters_Setters

        #endregion

        #region Unity
        private void Start()
        {
            m_tank_prefab = transform.Find("Tank").GetComponentInChildren<TankBoid>(true);
            m_mine_prefab = transform.Find("Mine").GetComponentInChildren<Transform>(true);

            m_tank_list = new List<TankBoid>();
            m_mine_list = new List<Transform>();

            for (int i = 0; i < m_tank_count; ++i)
            {
                TankBoid tank = Instantiate(m_tank_prefab);
                tank.name = string.Format("Tank {0}", (i + 1));
                tank.gameObject.SetActive(true);
                m_tank_list.Add(tank);
            }

            for (int i = 0; i < m_mine_count; ++i)
            {
                Transform mine = Instantiate(m_mine_prefab);
                mine.name = string.Format("Mine {0}", (i + 1));
                mine.position = new Vector2(Random.Range(-m_SPAWN_RANGE, m_SPAWN_RANGE), Random.Range(-m_SPAWN_RANGE, m_SPAWN_RANGE)) + new Vector2(m_SPAWN_OFFSET, m_SPAWN_OFFSET);
                mine.gameObject.SetActive(true);
                m_mine_list.Add(mine);
            }
        }

        #endregion

        #region Custom

        #endregion
    }
}