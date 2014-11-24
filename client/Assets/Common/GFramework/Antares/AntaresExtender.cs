//Created by Andrey PAramonov aka Neodrop
//mail:neodrop@unity3d.ru
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Antares
{
    public static class Console
    {
        private const int HistoryCapacity = 128;
        public class DebugObject
        {
            public DateTime lastTime;

            public Color color = Color.white;

            public List<DebugObject> history = new List<DebugObject>();
            public Vector2 scroll;

            public object Value
            {
                get { return _value; }
                set
                {
                    lastTime = DateTime.Now; _value = value;
                    history.Insert(0, new DebugObject(value.ToString()));
                    if (history.Count > HistoryCapacity) history.RemoveAt(HistoryCapacity);
                }
            }
            private object _value;
            public bool open = false;

            public DebugObject(object value, Color color)
            {
                lastTime = DateTime.Now;
                history.Add(new DebugObject(value.ToString()));
                _value = value;
                this.color = color;
            }

            public DebugObject(object value)
            {
                lastTime = DateTime.Now;
                _value = value;
            }

            public Dictionary<string, DebugObject> GetMessageHolder()
            {
                return Value as Dictionary<string, DebugObject>;
            }

            public void RemoveEntry(string entryName)
            {
                Dictionary<string, DebugObject> messages = _value as Dictionary<string, DebugObject>;
                if (messages == null || !messages.ContainsKey(entryName)) return;
                messages.Remove(entryName);
                _value = messages;
            }

            public GameObject owner;
            public bool noMonobeh = false;
        }

        private const uint Capacity = 128;
        public static Dictionary<object, DebugObject> entries = new Dictionary<object, DebugObject>();
		private static int uniqueID = 0;

        public static void Log(object sender, string description, object value, Color color)
        {
            if (value == null) return;
            DebugObject obj;
            if (entries.TryGetValue(sender, out obj))
            {
				if (string.IsNullOrEmpty(description))
					description = string.Format("#{0}", uniqueID++);

                Dictionary<string, DebugObject> member = obj.GetMessageHolder();
                switch (member.ContainsKey(description))
                {
                    case true:
                        member[description].Value = value;
                        break;
                    case false:
                        member.Add(description, new DebugObject(value, color));
                        break;
                }
                obj.color = color;
                obj.lastTime = DateTime.Now;
                return;
            }
            Dictionary<string, DebugObject> d = new Dictionary<string, DebugObject>();
            d.Add(description, new DebugObject(value, color));
            entries.Add(sender, new DebugObject(d, color));
            MonoBehaviour mb = sender as MonoBehaviour;
            if (mb == null)
            {
                switch (sender.GetType().ToString())
                {
                    case "UnityEngine.GameObject":
                        entries[sender].owner = (GameObject)sender;
                        return;
                }
                entries[sender].noMonobeh = true;
                return;
            }
            entries[sender].owner = mb.gameObject;
            if (entries.Count > Capacity)
            {
                object[] keys = new object[entries.Count];
                entries.Keys.CopyTo(keys, 0);
                entries.Remove(keys[keys.Length - 1]);
            }
        }

        public static void Log(object sender, string description, object value)
        {
            Log(sender, description, value, Color.white);
        }

        public static void ClearAll()
        {
            entries.Clear();
        }
    }

    public static class AntaresExtender
    {
        #region =========================== TRANSFORM ==============================
        /// <summary>
        /// AntaresExtender. Set transform.position.x
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="x"></param>
        public static void X(this Transform transform, float x)
        {
            if (transform == null) return;
            Vector3 pos = transform.position;
            pos.x = x;
            transform.position = pos;
        }

        /// <summary>
        /// AntaresExtender. Set transform.position.y
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="y"></param>
        public static void Y(this Transform transform, float y)
        {
            if (transform == null) return;
            Vector3 pos = transform.position;
            pos.y = y;
            transform.position = pos;
        }

        /// <summary>
        /// AntaresExtender. Set transform.position.z
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="z"></param>
        public static void Z(this Transform transform, float z)
        {
            if (transform == null) return;
            Vector3 pos = transform.position;
            pos.z = z;
            transform.position = pos;
        }
        #endregion

        #region ========================== CAMERA ==================================
        /// <summary>
        /// AntaresExtender. Add layer to camera.layerMask
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="layer"></param>
        public static void LayerAdd(this Camera camera, int layer)
        {
            LayerMask currentMask = camera.cullingMask;
            if (currentMask == (currentMask | (1 >> layer)))
                currentMask += (int)(Mathf.Pow(2, layer));
            camera.cullingMask = currentMask;
        }

        /// <summary>
        /// AntaresExtender. Remove layer from camera.layerMask
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="layer"></param>
        public static void LayerRemove(this Camera camera, int layer)
        {
            LayerMask currentMask = camera.cullingMask;
            if (currentMask == (currentMask | (1 << layer)))
                currentMask -= (int)(Mathf.Pow(2, layer));
            camera.cullingMask = currentMask;
        }
        #endregion

        #region ========================== LIGHT ==================================
        /// <summary>
        /// AntaresExtender. Add layer to light.layerMask
        /// </summary>
        /// <param name="light"></param>
        /// <param name="layer"></param>
        public static void LayerAdd(this Light light, int layer)
        {
            LayerMask currentMask = light.cullingMask;
            if (currentMask == (currentMask | (1 >> layer)))
                currentMask += (int)(Mathf.Pow(2, layer));
            light.cullingMask = currentMask;
        }

        /// <summary>
        /// AntaresExtender. Remove layer from light.layerMask
        /// </summary>
        /// <param name="light"></param>
        /// <param name="layer"></param>
        public static void LayerRemove(this Light light, int layer)
        {
            LayerMask currentMask = light.cullingMask;
            if (currentMask == (currentMask | (1 << layer)))
                currentMask -= (int)(Mathf.Pow(2, layer));
            light.cullingMask = currentMask;
        }
        #endregion

        #region ========================== COLOR ==========================================
        /// <summary>
        /// AntaresExtender.Return new color from source color with specified A
        /// </summary>
        /// <param name="color"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public static Color A(this Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }

        /// <summary>
        /// AntaresExtender.Return new color from source color with specified R
        /// </summary>
        /// <param name="color"></param>
        /// <param name="red"></param>
        /// <returns></returns>
        public static Color R(this Color color, float red)
        {
            return new Color(red, color.g, color.b, color.a);
        }

        /// <summary>
        /// AntaresExtender.Return new color from source color with specified G
        /// </summary>
        /// <param name="color"></param>
        /// <param name="green"></param>
        /// <returns></returns>
        public static Color G(this Color color, float green)
        {
            return new Color(color.r, green, color.b, color.a);
        }

        /// <summary>
        /// AntaresExtender.Return new color from source color with specified B
        /// </summary>
        /// <param name="color"></param>
        /// <param name="blue"></param>
        /// <returns></returns>
        public static Color B(this Color color, float blue)
        {
            return new Color(color.r, color.g, blue, color.a);
        }
        #endregion

       
    }
}