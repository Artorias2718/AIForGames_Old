using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AISandbox
{
    public class StateData
    {
        public int keys_collected { get; set; }
        public int doors_unlocked { get; set; }
        public Grid grid { get; set; }
        public PathFollower actor { get; set; }
        public List<Key> keys { get; set; }
        public List<Door> doors { get; set; }
        public Treasure treasure { get; set; }
        public GridNode start { get; set; }
        public GridNode target { get; set; }

        public StateData()
        {
            actor = Object.FindObjectOfType<PathFollower>();
            keys = Object.FindObjectsOfType<Key>().ToList();
            doors = Object.FindObjectsOfType<Door>().ToList();
            treasure = Object.FindObjectOfType<Treasure>();
            grid = Object.FindObjectOfType<Grid>();
        }
    }

    public abstract class State
    {
        #region Variables
        #endregion

        #region Getters_Setters
        public abstract string Name { get; }
        public StateMachine StateMachine { get; set; }
        public StateData StateData { get; set; }
        #endregion

        #region Unity
        #endregion

        #region Custom
        public virtual void Enter() { }
        public virtual void Update() { }
        public virtual void Exit() { }
        #endregion
    }

    public class KeyState : State
    {
        #region Variables
        #endregion

        #region Getters_Setters
        public override string Name { get { return "Key"; } }

        #endregion

        #region Unity
        #endregion

        #region Custom
        public KeyState() { }

        public override void Enter() { }
        public override void Update()
        {
            if (StateData.keys_collected < StateData.keys.Count)
            {
                if (StateData.keys[StateData.keys_collected].Collided)
                {
                    StateData.grid.GetNodeFromPosition(StateData.doors[StateData.doors_unlocked].Position).Blocked = false;
                    StateData.keys_collected++;
                }
            }

            if (StateData.keys_collected < 3)
            {
                for (int i = 0; i < 3; ++i)
                {
                    if (StateData.keys[i].isActiveAndEnabled)
                    {
                        StateData.start = StateData.grid.GetNodeFromPosition(StateData.actor.Boid.Position);
                        StateData.target = StateData.grid.GetNodeFromPosition(StateData.keys[StateData.keys_collected].Position);
                        StateData.actor.Path = StateData.actor.PathFinder.FindNewPath(StateData.start, StateData.target);
                        break;
                    }
                }
            }
            else
            {
                StateMachine.SetActiveState(StateMachine.States[1]);
            }

        }
        public override void Exit() { }
        #endregion
    }

    public class DoorState : State
    {
        #region Variables
        #endregion

        #region Getters_Setters
        public override string Name { get { return "Door"; } }

        #endregion

        #region Unity
        #endregion

        #region Custom
        public DoorState() { }

        public override void Enter() { }
        public override void Update()
        {
            if (StateData.doors_unlocked < StateData.doors.Count)
            {
                if (StateData.doors[StateData.doors_unlocked].Collided)
                {
                    StateData.doors_unlocked++;
                }
            }

            if (StateData.doors_unlocked < 3)
            {
                for (int i = 0; i < 3; ++i)
                {
                    if (StateData.doors[i].isActiveAndEnabled)
                    {
                        StateData.start = StateData.grid.GetNodeFromPosition(StateData.actor.Boid.Position);
                        StateData.target = StateData.grid.GetNodeFromPosition(StateData.doors[StateData.doors_unlocked].Position);
                        StateData.actor.Path = StateData.actor.PathFinder.FindNewPath(StateData.start, StateData.target);
                        break;
                    }
                }
            }
            else
            {
                StateData.grid.GetNodeFromPosition(StateData.treasure.Position).Blocked = false;
                StateMachine.SetActiveState(StateMachine.States[2]);
            }

        }
        public override void Exit() { }
        #endregion
    }

    public class TreasureState : State
    {
        #region Variables
        #endregion

        #region Getters_Setters
        public override string Name { get { return "Treasure"; } }
        #endregion

        #region Unity
        #endregion

        #region Custom
        public TreasureState() { }
        public override void Enter()
        {
            StateData.start = StateData.grid.GetNodeFromPosition(StateData.actor.Boid.Position);
            StateData.target = StateData.grid.GetNodeFromPosition(StateData.treasure.Position);
            StateData.actor.Path = StateData.actor.PathFinder.FindNewPath(StateData.start, StateData.target);

            StateData.target.Blocked = false;
            StateData.actor.Arriving = false;
        }
        public override void Update()
        {
            if (StateData.treasure.Collided)
            {
                StateData.actor.Boid.Position = StateData.target.Position;
            }

        }
        public override void Exit() { }
        #endregion
    }
}