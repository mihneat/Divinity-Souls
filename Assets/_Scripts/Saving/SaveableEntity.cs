using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using System;

namespace RPG.Saving
{
    [ExecuteAlways]
    public class SaveableEntity : MonoBehaviour
    {
        [SerializeField] string uniqueIdentifier = "";

        static Dictionary<string, SaveableEntity> globalLookup = new Dictionary<string, SaveableEntity>();

        public string GetUniqueIdentifier()
        {
            // uniqueIdentifier = System.Guid.NewGuid().ToString();
            return uniqueIdentifier;
        }

        public object CaptureState()
        {
            Dictionary<string, object> state = new Dictionary<string, object>();

            foreach (ISaveable saveable in GetComponents<ISaveable>()) {
                string typeString = saveable.GetType().ToString();
                state[typeString] = saveable.CaptureState();
            }

            return state;
        }

        public void RestoreState(object stateObj)
        {
            Dictionary<string, object> state = (Dictionary<string, object>)stateObj;
            
            foreach (ISaveable saveable in GetComponents<ISaveable>()) {
                string typeString = saveable.GetType().ToString();
                if (!state.ContainsKey(typeString))
                    continue;

                saveable.RestoreState(state[typeString]);
            }
        }

#if UNITY_EDITOR

        private void Update()
        {
            if (Application.IsPlaying(gameObject))
                return;

            if (string.IsNullOrEmpty(gameObject.scene.path))
                return;

            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty property = serializedObject.FindProperty("uniqueIdentifier");

            if (string.IsNullOrEmpty(property.stringValue) || !IsUnique(property.stringValue)) {
                property.stringValue = System.Guid.NewGuid().ToString();
                serializedObject.ApplyModifiedProperties();
            }

            globalLookup[property.stringValue] = this;
        }

        private bool IsUnique(string candidate)
        {
            if (!globalLookup.ContainsKey(candidate))
                return true;

            if (globalLookup[candidate] == this)
                return true;

            if (globalLookup[candidate] == null) {
                globalLookup.Remove(candidate);
                return true;
            }

            if (globalLookup[candidate].GetUniqueIdentifier() != candidate) {
                globalLookup.Remove(candidate);
                return true;
            }

            return false;
        }

#endif

    }
}
