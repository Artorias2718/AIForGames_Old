using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AISandbox
{
    public class BehaviorTree
    {
        #region Variables
        #region Variables
        private BTNodeData m_btnode_data;
        #endregion

        #region Getters_Setters
        public BTNode RootNode { get; set; }

        #endregion
        #endregion

        #region Getters_Setters

        #endregion

        #region Unity
        #endregion

        #region Custom
        public BehaviorTree(BTNode root_node)
        {
            m_btnode_data = new BTNodeData();
            RootNode = root_node;
            Init();
        }

        public void Init()
        {
            RootNode.Init(m_btnode_data);
        }

        public BTResult Process()
        {
            return RootNode.Process();
        }
        #endregion
    }
}