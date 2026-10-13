using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    /// <summary>
    /// Base class for all behaviour trees.
    /// </summary>
    public abstract class TreeBase : MonoBehaviour
    {
        private Node root;
        private Node currentNode;

        public Node Root { get => root; protected set => root = value; }

        private void Start()
        {
            Init();
        }

        private void Update()
        {
            Root.Update();
        }

        /// <summary>
        /// Initiate every tree in this method - called from Start.
        /// </summary>
        protected abstract void Init();


    }
}
