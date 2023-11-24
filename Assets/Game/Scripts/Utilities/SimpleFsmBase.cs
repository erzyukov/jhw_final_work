namespace Game.Utilities
{
	using System;
	using System.Collections.Generic;

	public abstract class SimpleFsmBase<T> where T : struct, IComparable
	{
		private readonly Dictionary<T, Action> _onEnterActions = new Dictionary<T, Action>();
		private readonly Dictionary<T, Action> _onExitActions = new Dictionary<T, Action>();

		protected SimpleFsmBase()
		{
			State = DefaultState;
		}

		public T State { get; private set; }

		protected virtual T DefaultState { get; } = default;

		public void Tick()
		{
			StateTransitions();
			StateTick();
		}

		public void FixedTick()
		{
			StateFixedTick();
		}

		public virtual void Transition(T state)
		{
			if (State.Equals(state))
				throw new Exception($"FSM is already in state: {State}");

			if (_onExitActions.TryGetValue(State, out Action onExitAction))
				onExitAction();

			State = state;

			if (_onEnterActions.TryGetValue(State, out Action onEnterAction))
				onEnterAction();
		}

		protected void AddOnEnterAction(T state, Action action) => _onEnterActions.Add(state, action);
		protected void AddOnExitAction(T state, Action action) => _onExitActions.Add(state, action);
		protected virtual void StateTransitions() { }
		protected virtual void StateTick() { }
		protected virtual void StateFixedTick() { }
	}
}