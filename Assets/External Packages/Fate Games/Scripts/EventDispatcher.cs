using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FateGames
{
    public class EventDispatcher : MonoBehaviour
    {
        private int count = 0;
        private Dictionary<int, Action> actions = new Dictionary<int, Action>();

        public int Bind(Action action)
        {
            int id = count++;
            actions.Add(id, action);
            return id;
        }

        public void Call()
        {
            foreach (Action action in actions.Values)
                action();
        }

        public void Remove(int id)
        {
            if (actions.ContainsKey(id)) actions.Remove(id);
        }

        public void Clear()
        {
            actions.Clear();
        }
    }
}

