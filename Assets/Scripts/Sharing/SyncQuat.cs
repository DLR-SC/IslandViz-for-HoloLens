﻿using HoloIslandVis.Core;
using HoloToolkit.Sharing.SyncModel;
using System.Collections.Generic;
using UnityEngine;

namespace HoloIslandVis.Sharing
{
    public class SyncQuat : SyncObject
    {
        [SyncData] private SyncFloat x = null;
        [SyncData] private SyncFloat y = null;
        [SyncData] private SyncFloat z = null;
        [SyncData] private SyncFloat w = null;

        private Dictionary<long, SyncPrimitive> _primitiveMap;

#if UNITY_EDITOR
        public override object RawValue
        {
            get { return Value; }
        }
#endif

        public Quaternion Value
        {
            get { return new Quaternion(x.Value, y.Value, z.Value, w.Value); }
            set
            {
                x.Value = value.x;
                y.Value = value.y;
                z.Value = value.z;
                w.Value = value.w;
            }
        }

        public SyncQuat(string field) : base(field)
        {
            _primitiveMap = new Dictionary<long, SyncPrimitive>();
        }

        protected override void OnFloatElementChanged(long elementID, float newValue)
        {
            if (GameObject.Find("AppConfig").GetComponent<AppConfig>().IsServerInstance)
            {
                base.OnFloatElementChanged(elementID, newValue);
            }
            else
            {
                if (_primitiveMap.ContainsKey(elementID))
                {
                    SyncPrimitive primitive = _primitiveMap[elementID];
                    primitive.UpdateFromRemote(newValue);
                    NotifyPrimitiveChanged(primitive);
                }
                else
                {
                    Debug.LogWarningFormat("Unknown primitive, discarding update.  Value: {1}, Id: {2}", elementID, newValue);
                }
            }
        }

        public void AddRemotePrimitive(long guid, SyncPrimitive syncPrimitve)
        {
            _primitiveMap.Add(guid, syncPrimitve);
        }
    }
}
