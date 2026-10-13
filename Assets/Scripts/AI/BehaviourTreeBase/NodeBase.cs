using System.Collections.Generic;

namespace BehaviourTree
{

    /// <summary>
    /// Enum which describes the node status. Success and Failure are results, while a Running status means
    /// the node has not yet determined whether it will return a Success or a Failure.
    /// </summary>
    public enum NodeStatus
    {
        Running,
        Success,
        Failure        
    }

    /// <summary>
    /// Base class for all behavior tree nodes.
    /// </summary>
    public abstract class Node
    {
        private Node parent;
        protected NodeStatus status;

        public Node Parent { get => parent; set => parent = value; }

        protected Node()
        {
            Parent = null;
        }

        /// <summary>
        /// Updates the node according to the node's specific logic.
        /// </summary>
        /// <returns>The node status</returns>
        public abstract NodeStatus Update();

    }

    /// <summary>
    /// Base class for all behavior tree composite nodes. Composite nodes have one or more children
    /// which are processed either in a first to last sequence or in random order.
    /// </summary>
    public abstract class Composite : Node
    {
        protected List<Node> children;
        protected int lastProcessedChild = 0;
        protected bool sequentialProcessing = true; // if true process children sequentially, else randomly

        protected Composite(List<Node> children) : base()
        {            
            this.children = new List<Node>();
            foreach (var child in children)
            {
                child.Parent = this;
                this.children.Add(child);
            }
        }
    }

    /// <summary>
    /// A composite node which will process all its children in order - it proceeds to the next child
    /// when the previous one reports success. Equivalent to an AND operation - returns success only
    /// if all of its children returned success. When entering the sequence proceeds from the child
    /// processed in the previous tick - useful for a sequence of tasks that need to be completed one after another.
    /// </summary>
    public class SequenceWithCachedLastChild : Composite
    {
        public SequenceWithCachedLastChild(List<Node> children) : base(children) { }

        public override NodeStatus Update()
        {
            NodeStatus childStatus = children[lastProcessedChild].Update();
            
            if (childStatus == NodeStatus.Success)
            {
                ++lastProcessedChild;

                // if all children processed and returned success, return success and reset
                if (lastProcessedChild == children.Count)
                {
                    lastProcessedChild = 0;
                    status = NodeStatus.Success;
                }
                // else keep running the sequence
                else
                {
                    status = NodeStatus.Running;
                }
                return status;
            }
            // if child failed or is running, report it upwards
            else
            {
                // if a child failed, reset the sequence for subsequent calls
                if (childStatus == NodeStatus.Failure) lastProcessedChild = 0;

                status = childStatus;
                return status;
            }
        }
    }
    
     /// <summary>
     /// A composite node which will process all its children in order - it proceeds to the next child
     /// when the previous one reports success. Equivalent to an AND operation - returns success only
     /// if all of its children returned success.
     /// </summary>
    public class Sequence : Composite
    {
        public Sequence(List<Node> children) : base(children) { }

        public override NodeStatus Update()
        {
            foreach (var child in children)
            {
                NodeStatus childStatus = child.Update();

                // a child failed or is running, report
                if (childStatus != NodeStatus.Success)
                {
                    status = childStatus;
                    return status;
                }
            }

            // all children succeeded
            status = NodeStatus.Success;
            return status;
        }
    }

    /// <summary>
    /// A composite node which will process all its children in order - when a child reports success it reports success
    /// and does not process any further children. Equivalent to an OR operation - returns success if any of its
    /// children returned success.
    /// </summary>
    public class Selector : Composite
    {
        public Selector(List<Node> children) : base(children) { }

        public override NodeStatus Update()
        {
            foreach (var child in children)
            {
                NodeStatus childStatus = child.Update();

                // a child succeeded or is running, report
                if (childStatus != NodeStatus.Failure)
                {
                    status = childStatus;
                    return status;
                }
            }

            // all children failed
            status = NodeStatus.Failure;
            return status;
        }
    }


    /// <summary>
    /// Base class for all behavior tree decorator nodes. Decorator nodes have exactly one child.
    /// </summary>
    public abstract class Decorator : Node
    {
        protected Node child;

        public Decorator(Node child) : base()
        {
            child.Parent = this;
            this.child = child;
        }
    }

    /// <summary>
    /// A decorator node which inverts the result of the child.
    /// </summary>
    public class Inverter : Decorator
    {
        public Inverter(Node child) : base(child) { }

        public override NodeStatus Update()
        {
            // invert child result
            switch (child.Update())
            {
                case NodeStatus.Success:
                    status = NodeStatus.Failure;
                    return status;
                case NodeStatus.Failure:
                    status = NodeStatus.Success;
                    return status;
            }

            // if child still running, report running upwards
            status = NodeStatus.Running;
            return status;
        }
    }

    /// <summary>
    /// A decorator node which always returns success.
    /// </summary>
    public class Succeeder : Decorator
    {
        public Succeeder(Node child) : base(child)
        {
            status = NodeStatus.Success;
        }

        public override NodeStatus Update()
        {
            // run child, report success upwards
            child.Update();
            return status;
        }

    }

    /// <summary>
    /// A decorator node which repeatedly processes its child after it returns a result (meaning either a success or 
    /// a failure). Example usage: at the top of the tree to make the tree run continuously.
    /// </summary>
    public class Repeater : Decorator
    {
        public Repeater(Node child) : base(child)
        {
            status = NodeStatus.Running;
        }

        public override NodeStatus Update()
        {
            // run child, report running upwards
            child.Update();
            return status;
        }

    }

    /// <summary>
    /// Leaf node base class. Leaves have no children and their function is to perform character
    /// specific actions or checks.
    /// </summary>
    public abstract class Leaf : Node
    {
        private string debugName;

        protected Leaf(string debugName = "") : base()
        {
            this.debugName = debugName;
        }

        public override string ToString()
        {
            return debugName == "" ? base.ToString() : debugName;
        }
    }

}