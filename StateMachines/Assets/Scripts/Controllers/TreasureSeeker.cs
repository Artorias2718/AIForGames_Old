using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AISandbox
{
    public class TreasureSeeker : MonoBehaviour
    {
        #region Variables
        private PathFollower m_path_follower;
        private KeyState m_key_state;
        private DoorState m_door_state;
        private TreasureState m_treasure_state;
        #endregion

        #region Getters_Setters
        public StateMachine StateMachine{ get; set; }
        #endregion

        #region Unity
        private void Start()
        {
            m_path_follower = GetComponentInChildren<PathFollower>();
            StateMachine = new StateMachine();
            m_key_state = new KeyState();
            m_door_state = new DoorState();
            m_treasure_state = new TreasureState();

            StateMachine.AddState(m_key_state);
            StateMachine.AddState(m_door_state);
            StateMachine.AddState(m_treasure_state);

            StateMachine.SetActiveState(m_key_state);
        }

        private void Update()
        {
            StateMachine.Update();
        }
        #endregion

        #region Custom

        #endregion
    }
}