using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AISandbox
{
    public class TreasureSeeker : MonoBehaviour
    {
        #region Variables
        private PathFollower m_path_follower;
        #endregion

        #region Getters_Setters
        public BehaviorTree BehaviorTree { get; set; }
        #endregion

        #region Unity
        private void Start()
        {
            m_path_follower = GetComponentInChildren<PathFollower>();

            Sequence root = new Sequence();
                Sequence key_sequence = new Sequence();

            KeyTracker key_tracker = new KeyTracker();
                    Repeater key_repeater = new Repeater();
                        KeyLeaf key_leaf = new KeyLeaf();
                        
                        key_sequence.Children.Add(key_tracker);
                        key_sequence.Children.Add(key_repeater);

            Sequence door_sequence = new Sequence();
                    DoorTracker door_tracker = new DoorTracker();
                    Repeater door_repeater = new Repeater();
                        DoorLeaf door_leaf = new DoorLeaf();

                        door_sequence.Children.Add(door_tracker);
                        door_sequence.Children.Add(door_repeater);

                TreasureLeaf treasure_leaf = new TreasureLeaf();

            root.BTNodeData.actor = FindObjectOfType<PathFollower>();
            root.BTNodeData.keys = FindObjectsOfType<Key>().ToList();
            root.BTNodeData.doors = FindObjectsOfType<Door>().ToList();
            root.BTNodeData.treasure = FindObjectOfType<Treasure>();

            root.Init(root.BTNodeData);

            key_sequence.Init(root.BTNodeData);
            key_leaf.Init(root.BTNodeData);

            key_repeater.Init(root.BTNodeData);
            key_repeater.Child = key_leaf;

            door_sequence.Init(root.BTNodeData);
            door_leaf.Init(root.BTNodeData);
            door_repeater.Init(root.BTNodeData);
            door_repeater.Child = door_leaf;

            foreach (BTNode child in key_sequence.Children)
            {
                child.Init(root.BTNodeData);
            }
            foreach(BTNode child in door_sequence.Children)
            {
                child.Init(root.BTNodeData);
            }

            root.Children.Add(key_repeater);
            root.Children.Add(door_repeater);

            root.Children.Add(treasure_leaf);
            root.Children[2].Init(root.BTNodeData);

            BehaviorTree = new BehaviorTree(root);
        }

        private void Update()
        {
            BehaviorTree.Process();
        }
        #endregion

        #region Custom

        #endregion
    }
}