using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AISandbox
{
    public class StateMachines : MonoBehaviour
    {
        #region Variables
        public Grid m_grid;
        public Transform m_key_container;
        public Transform m_door_container;
        public Transform m_treasure;

        private List<Transform> m_key_list;
        private List<Transform> m_door_list;
        private PathFollower m_path_follower;
        #endregion

        #region Getters_Setters

        #endregion

        #region Unity

        private void Start()
        {
            m_path_follower = GetComponentInChildren<PathFollower>(true);
            m_path_follower.gameObject.SetActive(true);

            m_key_list = m_key_container.GetComponentsInChildren<Transform>(true).ToList();
            m_door_list = m_door_container.GetComponentsInChildren<Transform>(true).Where(child => child.name != "Lock").ToList();

            foreach (Transform key in m_key_list)
            {
                GridNode random_node = m_grid.GetNode(Random.Range(0, m_grid.Nodes.GetLength(0)), Random.Range(0, m_grid.Nodes.GetLength(1)));
                random_node.Blocked = false;
                key.transform.position = random_node.Position;
                key.gameObject.SetActive(true);
            }
            
            foreach (Transform door in m_door_list)
            {
                GridNode random_node = m_grid.GetNode(Random.Range(0, m_grid.Nodes.GetLength(0)), Random.Range(0, m_grid.Nodes.GetLength(1)));
                random_node.Blocked = false;
                door.transform.position = random_node.Position;
                door.gameObject.SetActive(true);
            }

            {
                GridNode random_node = m_grid.GetNode(Random.Range(0, m_grid.Nodes.GetLength(0)), Random.Range(0, m_grid.Nodes.GetLength(1)));
                random_node.Blocked = true;
                m_treasure.transform.position = random_node.Position;
                m_treasure.gameObject.SetActive(true);
            }
        }

        private void Update()
        {

        }
        #endregion

        #region Custom

        #endregion
    }
}