﻿using System;
//using System.Collections.Concurrent;
using System.Collections.Generic;
//using System.Diagnostics;

using System.Text;

namespace FHNetSocket
{
	public class RegistrationManager : IDisposable
	{
		private  Dictionary<int, Action<Object>> callBackRegistry;
        private Dictionary<string, Action<M_Message>> eventNameRegistry;

		public RegistrationManager()
		{
			this.callBackRegistry = new  Dictionary<int, Action<Object>>();
            this.eventNameRegistry = new Dictionary<string, Action<M_Message>>();
		}

        public void AddCallBack(M_Message message)
		{
			M_EventMessage eventMessage = message as M_EventMessage;
			if (eventMessage != null)
				this.callBackRegistry.Add(eventMessage.AckId.Value, eventMessage.Callback);
		}
		public void AddCallBack(int ackId, Action<Object>  callback)
		{
			this.callBackRegistry.Add(ackId, callback);
		}
		
		public void InvokeCallBack(int? ackId, string value)
		{
			Action<Object>  target = null;
			if (ackId.HasValue)
			{
				if (this.callBackRegistry.TryGetValue(ackId.Value, out target)) // use TryRemove - callbacks are one-shot event registrations
				{
					//target.BeginInvoke(target.EndInvoke, value);
					target.BeginInvoke(value, target.EndInvoke, null);
					//this.callBackRegistry.Remove(ackId.Value);
				}
			}
		}
		public void InvokeCallBack(int? ackId, JsonEncodedEventMessage value)
		{
			Action<Object>  target = null;
			if (ackId.HasValue)
			{
				if (this.callBackRegistry.TryGetValue(ackId.Value, out target))
				{
					target.Invoke(value);
					//this.callBackRegistry.Remove(ackId.Value);
					//target.BeginInvoke(target.EndInvoke, value);
				}
			}
		}

        public void AddOnEvent(string eventName, Action<M_Message> callback)
		{
			this.eventNameRegistry.Add(eventName, callback );
		}
        public void AddOnEvent(string eventName, string endPoint, Action<M_Message> callback)
		{
			this.eventNameRegistry.Add(string.Format("{0}::{1}",eventName, endPoint), callback);
		}
		/// <summary>
		/// If eventName is found, Executes Action delegate<typeparamref name="T"/> asynchronously
		/// </summary>
		/// <param name="eventName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
        public bool InvokeOnEvent(M_Message value)
		{
			bool foundEvent = false;
			try
			{
                Action<M_Message> target;
				
				string eventName = value.Event;
				if (!string.IsNullOrEmpty(value.Endpoint))
					eventName = string.Format("{0}::{1}", value.Event, value.Endpoint);
				//UnityEngine.Debug.LogError("eventName:" + eventName);
				if (this.eventNameRegistry.TryGetValue(eventName, out target)) // use TryGet - do not destroy event name registration
				{
					foundEvent = true;
					target.Invoke(value);
					//this.eventNameRegistry.Remove(eventName);
					//target.BeginInvoke(value, target.EndInvoke, null);
					//Debug.LogError(string.Format("webSocket_{0}: {1}", value.Event, value.MessageText));
				}
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogError("Exception on InvokeOnEvent: " + ex.Message);
			}
			return foundEvent;
		}

		// Dispose() calls Dispose(true)
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		// The bulk of the clean-up code is implemented in Dispose(bool)
		protected virtual void Dispose(bool disposing)
		{
			this.callBackRegistry.Clear();
			this.eventNameRegistry.Clear();
		}
}
}
