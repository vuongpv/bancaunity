using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GFramework
{
	/// <summary>
	/// Event handler response value
	/// </summary>
	public enum EventResponse
	{
		Block,
		PassThrough,
	}

	/// <summary>
	/// Base class for event data
	/// </summary>
	public class EventBase
	{
		public UnityEngine.Object source;
		public UnityEngine.Object target;
	}

	/// <summary>
	/// Base class key event handler
	/// </summary>
	public class EventHandler<TEvent> where TEvent : EventBase
	{
		public delegate EventResponse EventDelegate();
		public delegate EventResponse EventDelegateWithParam(TEvent e);

		private EventDelegate _delegate;
		private EventDelegateWithParam _delegateParam;

		protected EventHandler(EventDelegate @delegate)
		{
			_delegate = @delegate;
			_delegateParam = null;
		}

		public EventHandler(EventDelegateWithParam @delegate)
		{
			_delegate = null;
			_delegateParam = @delegate;
		}

		public EventResponse Execute(TEvent e)
		{
			if (_delegateParam != null)
			{
				return _delegateParam(e);
			}

			if (_delegate != null)
				return OnExecute(e, _delegate);

			return EventResponse.PassThrough;
		}

		protected virtual EventResponse OnExecute(TEvent e, EventDelegate @delegate)
		{
			_delegate();
			return EventResponse.PassThrough;
		}
	}

	/// <summary>
	/// Event dispatcher base class
	/// </summary>
	/// <typeparam name="TID"></typeparam>
	/// <typeparam name="TEvent"></typeparam>
	public class EventDispatcher<TID, TEvent> where TEvent : EventBase
	{
		private MultiPriorityMap<TID, EventHandler<TEvent>> eventsMap;

		public EventDispatcher()
		{
			eventsMap = new MultiPriorityMap<TID, EventHandler<TEvent>>();
		}

		public EventDispatcher(IEqualityComparer<TID> comparer)
		{
			eventsMap = new MultiPriorityMap<TID, EventHandler<TEvent>>(comparer);
		}

		public void AddEventHandler(TID eventId, int priority, EventHandler<TEvent> handler)
		{
			eventsMap.Add(eventId, priority, handler);
		}

		public void AddEventHandler(TID eventId, EventHandler<TEvent> handler)
		{
			eventsMap.Add(eventId, 1, handler);
		}

		public EventResponse Dispatch(TID eventId, TEvent @event)
		{
			foreach (var @delegate in eventsMap.GetValueAsOrderedList(eventId))
			{
				if (@delegate.Execute(@event) == EventResponse.Block)
					return EventResponse.Block;
			}

			return EventResponse.PassThrough;
		}

		public IEnumerable<TID> IDs
		{
			get
			{
				return this.eventsMap.Keys;
			}
		}

		public bool Contains(TID eventId)
		{
			return this.eventsMap.ContainsKey(eventId);
		}
	}
}