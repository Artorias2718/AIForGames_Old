using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AISandbox
{
    public class StateMachine
    {
        #region Variables
        #endregion

        #region Getters_Setters
        public State m_active_state { get; set; }
        public List<State> States { get; }
        public StateData StateData { get; set; }
        #endregion

        #region Unity
        #endregion

        #region Custom
        public StateMachine()
        {
            m_active_state = null;
            States = new List<State>();
            StateData = new StateData();
        }

        public void AddState(State state)
        {
            if(!States.Contains(state))
            {
                state.StateMachine = this;
                state.StateData = StateData;
                States.Add(state);
            }
        }

        public void RemoveState(State state)
        {
            if(States.Contains(state))
            {
                state.StateMachine = null;
                States.Remove(state);
            }
        }

        public void SetActiveState(State state)
        {
            if (m_active_state != null)
            {
                m_active_state.Exit();
            }
            m_active_state = state;
            m_active_state.Enter();
        }

        public void Update()
        {
            if(m_active_state != null)
            {
                m_active_state.Update();
            }
        }

        #endregion
    }
}