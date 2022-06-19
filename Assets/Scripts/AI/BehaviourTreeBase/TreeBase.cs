using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
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
            // TODO change this so the entire tree traversal doesn't happen every frame -> cache running state, only update that one
            Root.Update();
        }

        /// <summary>
        /// Initiate every tree in this method - called from Start.
        /// </summary>
        protected abstract void Init();


    }
}
