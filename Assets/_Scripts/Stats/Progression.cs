using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
	[CreateAssetMenu(fileName = "Progression", menuName = "RPG/Stats/New Progression", order = 1)]
	public class Progression : ScriptableObject
	{
		[SerializeField] ProgressionCharacterClass[] characterClasses = null;

		Dictionary<CharacterClass, ProgressionCharacterClass> lookupTable = null;

		public float GetStat(Stat stat, CharacterClass characterClass, int level)
		{
			// Build the Dictionary
			BuildLookup();

			// If the class doesn't exist, return 0
			if (!lookupTable.ContainsKey(characterClass))
				return 0.0f;

			// Otherwise, call the GetStat method in the found class
			return lookupTable[characterClass].GetStat(stat, level);
		}

		public int GetLevels(Stat stat, CharacterClass characterClass)
		{
			// Build the Dictionary
			BuildLookup();

			// If the class doesn't exist, return 0
			if (!lookupTable.ContainsKey(characterClass))
				return 0;

			// Otherwise, call the GetLevel method in the found class
			return lookupTable[characterClass].GetLevels(stat);
		}

		private void BuildLookup()
		{
			if (lookupTable != null)
				return;

			// Create the Dictionary
			lookupTable = new Dictionary<CharacterClass, ProgressionCharacterClass>();

			// For each element in the array, add the class to the dictionary
			// if it has not already been added
			foreach (ProgressionCharacterClass progressionClass in characterClasses) {
				CharacterClass currentClass = progressionClass.GetCharacterClass;
				if (!lookupTable.ContainsKey(currentClass)) {
					lookupTable.Add(currentClass, progressionClass);
				}
			}
		}
	}

	[System.Serializable]
	class ProgressionCharacterClass
	{
		public CharacterClass GetCharacterClass {
			get {
				return characterClass;
			}
		}

		public ProgressionStat[] GetStats {
			get {
				return stats;
			}
		}

		[SerializeField] CharacterClass characterClass;
		[SerializeField] ProgressionStat[] stats = null;

		Dictionary<Stat, ProgressionStat> lookupTable = null;

		public float GetStat(Stat stat, int level)
		{
			// Build the Dictionary
			BuildLookup();

			// If the stat doesn't exist, return 0
			if (!lookupTable.ContainsKey(stat))
				return 0.0f;

			// Otherwise, call the GetStatByLevel method in the found class
			return lookupTable[stat].GetStatByLevel(level);
		}

		public int GetLevels(Stat stat)
		{
			// Build the Dictionary
			BuildLookup();

			// If the stat doesn't exist, return 0
			if (!lookupTable.ContainsKey(stat))
				return 0;

			// Otherwise, return the length of the levels vector
			return lookupTable[stat].GetLevels.Length;
		}

		private void BuildLookup()
		{
			if (lookupTable != null)
				return;

			// Create the Dictionary
			lookupTable = new Dictionary<Stat, ProgressionStat>();

			// For each element in the array, add the class to the dictionary
			// if it has not already been added
			foreach (ProgressionStat progressionStat in stats) {
				Stat currentStat = progressionStat.GetStat;
				if (!lookupTable.ContainsKey(currentStat)) {
					lookupTable.Add(currentStat, progressionStat);
				}
			}
		}
	}

	[System.Serializable]
	class ProgressionStat
	{
		public Stat GetStat {
			get {
				return stat;
			}
		}

		public float[] GetLevels {
			get {
				return levels;
			}
		}

		[SerializeField] Stat stat;
		[SerializeField] float[] levels;

		public float GetStatByLevel(int level)
		{
			level = Mathf.Clamp(level, 1, levels.Length);
			return levels[level - 1];
		}
	}
}
