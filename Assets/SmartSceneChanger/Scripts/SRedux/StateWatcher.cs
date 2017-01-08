using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace SSC
{

    /// <summary>
    /// StateWatcher class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StateWatcher<T> where T : new()
    {

        /// <summary>
        /// Current state
        /// </summary>
        T m_state = new T();

        /// <summary>
        /// Action list
        /// </summary>
        protected List<Action<T>> m_actionList = new List<Action<T>>();

        /// <summary>
        /// Add Action
        /// </summary>
        /// <param name="action">add action</param>
        public void addAction(Action<T> action)
        {
            this.m_actionList.Add(action);
        }

        /// <summary>
        /// Remove Action
        /// </summary>
        /// <param name="action">remove action</param>
        public void removeAction(Action<T> action)
        {
            this.m_actionList.Remove(action);
        }

        /// <summary>
        /// Get current state
        /// </summary>
        /// <returns>return current state</returns>
        public T state()
        {
            return this.m_state;
        }

        /// <summary>
        /// Send state to each action
        /// </summary>
        public void sendState()
        {
            foreach (Action<T> action in this.m_actionList)
            {
                action(this.m_state);
            }
        }

    }

}
