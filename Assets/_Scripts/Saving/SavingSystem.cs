using System;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Saving
{
    public class SavingSystem : MonoBehaviour
    {
        const string lastSceneDictEntry = "lastSceneBuildIndex";

        const string savingExtension = ".sav";
        // const string backupExtension = ".bck";

        public void Save(string saveFile)
        {
            Dictionary<string, object> state = LoadFile(saveFile);
            CaptureState(state);
            SaveFile(saveFile, state);
        }

        public void Load(string saveFile)
        {
            RestoreState(LoadFile(saveFile));
        }

        public IEnumerator LoadLastScene(string saveFile)
        {
            Dictionary<string, object> state = LoadFile(saveFile);

            if (state.ContainsKey(lastSceneDictEntry)) {
                int sceneBuildIndex = (int)state[lastSceneDictEntry];
                // if (sceneBuildIndex != SceneManager.GetActiveScene().buildIndex) {
                    yield return SceneManager.LoadSceneAsync(sceneBuildIndex);
                // }
            } else {
                yield return SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            }

            RestoreState(state);
        }

        public void Delete(string saveFile)
        {
            DeleteFile(saveFile);
        }

        private void SaveFile(string saveFile, object state)
        {
            string path = GetPathFromSaveFile(saveFile);

            using (FileStream stream = File.Open(path, FileMode.Create)) {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, state);
            }
        }

        private Dictionary<string, object> LoadFile(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);

            if (!File.Exists(path))
                return new Dictionary<string, object>();

            using (FileStream stream = File.Open(path, FileMode.Open)) {
                BinaryFormatter formatter = new BinaryFormatter();
                return (Dictionary<string, object>)formatter.Deserialize(stream);
            }
        }

        public void DeleteFile(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);

            if (File.Exists(path))
                File.Delete(path);
        }

        private void CaptureState(Dictionary<string, object> state)
        {
            foreach(SaveableEntity saveableEntity in FindObjectsOfType<SaveableEntity>()) {
                string uuid = saveableEntity.GetUniqueIdentifier();
                state[uuid] = saveableEntity.CaptureState();
            }

            state[lastSceneDictEntry] = SceneManager.GetActiveScene().buildIndex;
        }

        private void RestoreState(Dictionary<string, object> state)
        {
            foreach (SaveableEntity saveableEntity in FindObjectsOfType<SaveableEntity>()) {
                string uuid = saveableEntity.GetUniqueIdentifier();
                if (!state.ContainsKey(uuid))
                    continue;

                saveableEntity.RestoreState(state[uuid]);
            }
        }

        private string GetPathFromSaveFile(string saveFile)
        {
            return Path.Combine(Application.persistentDataPath, saveFile + savingExtension);
        }

        #region Old

        private byte[] SerializeVector(Vector3 vector)
        {
            byte[] vectorBytes = new byte[3 * sizeof(float)];
            BitConverter.GetBytes(vector.x).CopyTo(vectorBytes, 0);
            BitConverter.GetBytes(vector.y).CopyTo(vectorBytes, 1 * sizeof(float));
            BitConverter.GetBytes(vector.z).CopyTo(vectorBytes, 2 * sizeof(float));

            return vectorBytes;
        }

        private Vector3 DeserializeVector(byte[] buffer)
        {
            Vector3 result = new Vector3();
            result.x = BitConverter.ToSingle(buffer, 0);
            result.y = BitConverter.ToSingle(buffer, 1 * sizeof(float));
            result.z = BitConverter.ToSingle(buffer, 2 * sizeof(float));

            return result;
        }

        private static void WriteHolaMundoToFile(FileStream stream)
        {

            // byte[] holaMundoArray = { 0xc2, 0xa1, 0x48, 0x6f, 0x6c, 0x61, 0x20, 0x4d, 0x75, 0x6e, 0x64, 0x6f, 0x21 };
            byte[] holaMundoArray = Encoding.UTF8.GetBytes("¡Hola Mundo!");

            stream.Write(holaMundoArray, 0, holaMundoArray.Length);

            //stream.WriteByte(0xc2); stream.WriteByte(0xa1); // ! (inverted)
            //stream.WriteByte(0x48); // H
            //stream.WriteByte(0x6f); // o
            //stream.WriteByte(0x6c); // l
            //stream.WriteByte(0x61); // a
            //stream.WriteByte(0x20); //  
            //stream.WriteByte(0x4d); // M
            //stream.WriteByte(0x75); // u
            //stream.WriteByte(0x6e); // n
            //stream.WriteByte(0x64); // d
            //stream.WriteByte(0x6f); // o
            //stream.WriteByte(0x21); // !
        }

        #endregion
    }
}
