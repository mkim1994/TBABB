using System;


namespace BehaviorTree
{
    public abstract class Node<T>
    {
        public abstract bool Update(T context);
    }

    public class Tree<T> : Node<T>
    {
        private readonly Node<T> _root;

        public Tree(Node<T> root)
        {
            _root = root;
        }

        public override bool Update(T context)
        {
            return _root.Update(context);
        }
    }

    //NODE TYPES

    //OUTER, LEAF OR 'ACTION' NODES

    //DO -- execute an in-line action

    public class Do<T> : Node<T>
    {
        public delegate bool NodeAction(T context);

        private readonly NodeAction _action;

        public Do(NodeAction action)
        {
            _action = action;
        }

        public override bool Update(T context)
        {
            return _action(context);
        }
    }

    //CONDITION

    public class Condition<T> : Node<T>
    {
        private readonly Predicate<T> _condition;

        public Condition(Predicate<T> condition)
        {
            _condition = condition;
        }

        public override bool Update(T context)
        {
            return _condition(context);
        }
    }

    //DECISION NODES

    public abstract class BranchNode<T> : Node<T>
    {
        protected Node<T>[] Children { get; private set; }

        protected BranchNode(params Node<T>[] children)
        {
            Children = children;
        }
    }

    //SELECTOR
    //Succeeds when ONE of its children succeeded
    //Fails when ALL fail
    //Used to select from a series of ranked options.

    public class Selector<T> : BranchNode<T>
    {
        public Selector(params Node<T>[] children) : base(children)
        {
        }

        public override bool Update(T context)
        {
            foreach (var child in Children)
            {
                if (child.Update(context)) return true;
            }

            return false;
        }
    }

    //SEQUENCE
    //Succeeds when all children succeed
    //Fails when ONE fails.

    public class Sequence<T> : BranchNode<T>
    {
        public Sequence(params Node<T>[] children) : base(children)
        {
        }

        public override bool Update(T context)
        {
            foreach (var child in Children)
            {
                if (!child.Update(context)) return false;
            }

            return true;
        }
    }


    //DECORATOR
    //Decorators are nodes that act as a 'modifier' for another node
    //This is a base class that just holds a reference to the 'modified' or 'decorated' node.

    public abstract class Decorator<T> : Node<T>
    {
        protected Node<T> Child { get; private set; }

        protected Decorator(Node<T> child)
        {
            Child = child;
        }
    }

    //a common example of a Decorator node is a Not or Negate node
    // that inverts the result of anotehr node. 

    public class Not<T> : Decorator<T>
    {
        public Not(Node<T> child) : base(child)
        {
        }

        public override bool Update(T context)
        {
            return !Child.Update(context);
        }
    }
}
