using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AISandbox
{
    public class BTNodeData
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

        public BTNodeData()
        {
            actor = Object.FindObjectOfType<PathFollower>();
            keys = Object.FindObjectsOfType<Key>().ToList();
            doors = Object.FindObjectsOfType<Door>().ToList();
            treasure = Object.FindObjectOfType<Treasure>();
            grid = Object.FindObjectOfType<Grid>();
        }
    }

    public abstract class BTNode
    {
        #region Variables
        #endregion

        #region Getters_Setters
        public BTNodeData BTNodeData { get; set; }
        #endregion

        #region Unity

        #endregion

        #region Custom

        public BTNode()
        {
            BTNodeData = new BTNodeData();
        }

        public virtual void Init(BTNodeData node_data)
        {
            BTNodeData = node_data;
        }

        public abstract BTResult Process();
        #endregion
    }

    public abstract class Composite : BTNode
    {
        protected int m_child_index = 0;
        public List<BTNode> Children { get; set; }
        public Composite(bool randomize = false)
        {
            BTNodeData = new BTNodeData();
            Children = new List<BTNode>();

            if (randomize)
            {
                int count = Children.Count;
                int end = count - 1;

                for (int i = 0; i < end; ++i)
                {
                    int random = Random.Range(i, count);
                    BTNode temp = Children[i];
                    Children[i] = Children[random];
                    Children[random] = temp;
                }
            }

            foreach(BTNode child in Children)
            {
                if (child != null)
                {
                    child.Init(BTNodeData);
                }
            }
        }
    }

    public class Sequence : Composite
    {
        public Sequence(bool randomize = false) : base(randomize) {}

        public override BTResult Process()
        {
            BTResult result = BTResult.RUNNING;

            if(m_child_index < Children.Count)
            {
                result = Children[m_child_index].Process();
            }

            if (result == BTResult.SUCCESS)
            {
                if (m_child_index == Children.Count - 1)
                {
                    return BTResult.SUCCESS;
                }
                else
                {
                    m_child_index++;
                    return BTResult.RUNNING;
                }
            }
            return BTResult.FAILURE;
        }
    }

    public class Selector : Composite
    {
        public Selector(bool randomize = false) : base(randomize) { }

        public override BTResult Process()
        {
            BTResult result = Children[m_child_index].Process();

            if (m_child_index < Children.Count)
            {
                result = Children[m_child_index].Process();
            }

            if (result == BTResult.FAILURE)
            {
                if (m_child_index == Children.Count - 1)
                {
                    return BTResult.FAILURE;
                }
                else
                {
                    m_child_index++;
                    return BTResult.RUNNING;
                }
            }
            return BTResult.SUCCESS;
        }
    }

    public abstract class Decorator : BTNode
    {
        public BTNode Child { get; set; }
        public Decorator()
        {
            BTNodeData = new BTNodeData();
            if(Child != null)
            {
                Child.Init(BTNodeData);
            }
        }
    }

    public class Inverter : Decorator
    {
        public Inverter() : base() { }

        public override BTResult Process()
        {
            BTResult result = Child.Process();
            if(result == BTResult.SUCCESS)
            {
                return BTResult.FAILURE;
            }
            else if(result == BTResult.FAILURE)
            {
                return BTResult.SUCCESS;
            }
            return BTResult.RUNNING;
        }
    }

    public class Succeeder : Decorator
    {
        public Succeeder() : base() { }

        public override BTResult Process()
        {
            Child.Process();
            return BTResult.SUCCESS;
        }
    }

    public class Repeater : Decorator
    {
        private bool m_repeat_until_fail;
        public Repeater(bool repeat_until_fail = false) : base()
        {
            m_repeat_until_fail = repeat_until_fail;
        }

        public override BTResult Process()
        {
            if (m_repeat_until_fail)
            {
                BTResult result = Child.Process();
                if (result == BTResult.FAILURE)
                {
                    return BTResult.SUCCESS;
                }
                else
                {
                    return Child.Process();
                }
            }
            return Child.Process();
        }
    }

    public abstract class Leaf : BTNode { }

    public class KeyTracker : Leaf
    {
        private int m_key_index;
        public override BTResult Process()
        {
            if (m_key_index < 3)
            {
                m_key_index = BTNodeData.keys_collected++;
                return BTResult.SUCCESS;
            }
            return BTResult.FAILURE;
        }
    }

    public class KeyLeaf : Leaf
    {
        private Key[] m_keys;
        private Door[] m_doors;
        private PathFollower m_actor;
        private GridNode m_current;
        private GridNode m_target;
        public override void Init(BTNodeData node_data)
        {
            base.Init(node_data);
            m_actor = node_data.actor;
            m_keys = node_data.keys.ToArray();
            m_doors = node_data.doors.ToArray();
            System.Array.Reverse(m_keys);
        }

        public override BTResult Process()
        {
            m_current = m_actor.PathFinder.Grid.GetNodeFromPosition(m_actor.Boid.Position);
            foreach (Key key in m_keys)
            {
                if (key.isActiveAndEnabled)
                {
                    m_target = m_actor.PathFinder.Grid.GetNodeFromPosition(key.transform.position);
                    m_actor.Path = m_actor.PathFinder.FindNewPath(m_current, m_target);
                    BTNodeData.grid.GetNodeFromPosition(m_doors.Where(door => key.name == door.name).First().Position).Blocked = false;
                    return BTResult.RUNNING;
                }
            }

            if (m_current == m_target)
            {
                if (m_target.Position == m_keys[m_keys.Length - 1].Position)
                {
                    return BTResult.SUCCESS;
                }
            }

            return BTResult.RUNNING;
        }
    }

    public class DoorTracker : Leaf
    {
        private int m_door_index;
        public override BTResult Process()
        {
            if (m_door_index < 3)
            {
                m_door_index = BTNodeData.doors_unlocked++;
                return BTResult.SUCCESS;
            }
            return BTResult.FAILURE;
        }
    }

    public class DoorLeaf : Leaf
    {
        private Door[] m_doors;
        private Treasure m_treasure;
        private PathFollower m_actor;
        private GridNode m_current;
        private GridNode m_target;
        public override void Init(BTNodeData node_data)
        {
            base.Init(node_data);
            m_actor = node_data.actor;
            m_doors = node_data.doors.ToArray();
            m_treasure = node_data.treasure;
            System.Array.Reverse(m_doors);
        }

        public override BTResult Process()
        {
            m_current = m_actor.PathFinder.Grid.GetNodeFromPosition(m_actor.Boid.Position);
            foreach (Door door in m_doors)
            {
                if (door.GetComponentInChildren<SpriteRenderer>().sprite != door.m_unlocked_sprite)
                {
                    m_target = m_actor.PathFinder.Grid.GetNodeFromPosition(door.Position);
                    m_actor.Path = m_actor.PathFinder.FindNewPath(m_current, m_target);
                    return BTResult.RUNNING;
                }
            }

            if (m_current == m_target)
            {
                if (m_target.Position == m_doors[m_doors.Length - 1].Position)
                {
                    BTNodeData.grid.GetNodeFromPosition(m_treasure.Position).Blocked = false;
                    return BTResult.SUCCESS;
                }
            }

            return BTResult.RUNNING;
        }
    }

    public class TreasureLeaf : Leaf
    {
        private PathFollower m_actor;
        private Treasure m_treasure_object;

        private GridNode m_current;
        private GridNode m_target;

        public override void Init(BTNodeData node_data)
        {
            base.Init(node_data);
            m_actor = node_data.actor;
            m_treasure_object = node_data.treasure;
        }

        public override BTResult Process()
        {
            m_current = m_actor.PathFinder.Grid.GetNodeFromPosition(m_actor.Boid.Position);
            if (m_target == null)
            {
                m_target = m_actor.PathFinder.Grid.GetNodeFromPosition(m_treasure_object.transform.position);
                m_actor.Path = m_actor.PathFinder.FindNewPath(m_current, m_target);
            }
            if (m_current == m_target)
            {
                m_actor.Boid.Position = m_target.Position;
                m_actor.Boid.Steering = Vector2.zero;
                m_actor.Boid.Velocity = Vector2.zero;
                return BTResult.SUCCESS;
            }
            else
            {
                return BTResult.RUNNING;
            }
        }
    }

}