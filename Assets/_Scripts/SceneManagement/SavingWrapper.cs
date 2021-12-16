using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        const string defaultSaveFile = "Save01";

        [SerializeField] float fadeInTime = 0.75f;

        private SavingSystem savingSystem;

        private void Awake()
        {
            savingSystem = GetComponent<SavingSystem>();
            StartCoroutine(LoadLastScene());
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F5)) {
                Save();
            }

            if (Input.GetKeyDown(KeyCode.F9)) {
                StartCoroutine(LoadLastScene());
            }

            if (Input.GetKeyDown(KeyCode.F12)) {
                Delete();
            }
        }

        public void Save()
        {
            Debug.Log("Saving current progress...");
            savingSystem.Save(defaultSaveFile);
        }

        public void Load()
        {
            savingSystem.Load(defaultSaveFile);
        }

        public IEnumerator LoadLastScene()
        {
            // Fade out immediately
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();

            yield return savingSystem.LoadLastScene(defaultSaveFile);

            // Fade in
            yield return fader.FadeIn(fadeInTime);
        }

        private void Delete()
        {
            Debug.Log("Deleting save file...");
            savingSystem.Delete(defaultSaveFile);
        }
    }
}
