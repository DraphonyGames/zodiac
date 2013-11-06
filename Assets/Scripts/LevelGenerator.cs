namespace Assets.Scripts
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Class that can generate levels
    /// </summary>
    public sealed class LevelGenerator
    {
        private const int MercenaryCount = 2;

        /// <summary>
        /// Contains Prefabs of the different Sections
        /// </summary>
        private readonly List<GameObject> _sectionPrefabs;

        #region "Methods"

        #region "public"

        /// <summary>
        /// Initializes a new instance of the <see cref="LevelGenerator"/> class.
        /// </summary>
        public LevelGenerator()
        {
            _sectionPrefabs = new List<GameObject>();
            foreach (UnityEngine.Object obj in Resources.LoadAll("Levels"))
            {
                _sectionPrefabs.Add((GameObject)obj);
            }
        }

        /// <summary>
        /// Generates a new Level with the current LevelNo in GameManager.
        /// The Level can be builded by the LevelBuilder.
        /// </summary>
        /// <param name="difficulty">the level difficulty</param>
        /// <returns>The generated level</returns>
        public Level GenerateNextLevel(int difficulty)
        {
            int levelNo = GameManager.GetInstance().LevelNo;
            GameManager.GetInstance().LevelNo++;
            levelNo++;

            /*
            // Try to obtain the jungle sections.
            List<GameObject> _sectionArray = GetJungleSections();
            if(_sectionArray.Count == 0)
            {   // No jungle sections present -- simply fetch some random sections
                // (two for now).
                _sectionArray = GetRandomSections(2);
            }
            */
            List<GameObject> sectionArray = GetFixedSections(3);

            Level newLevel = new Level(difficulty, levelNo);
            int sectionCount = sectionArray.Count;
            for (int i = 0; i < sectionCount; i++)
            {
                //// int rand = UnityEngine.Random.Range(0, _sectionPrefabs.Count - 1);
                Section newSection = new Section(sectionArray[i]);
                newSection.SectionNumber = i + 1;
                int x = ((levelNo - 1) * sectionCount) + i + 1;

                List<GameObject> mobs = Datasheet.Mobs();

                int monsterCount = (int)(((3 * x) + Math.Sin(1.5 * x) + 7) * Math.Sqrt(GameManager.GetInstance().GetAllTeams()[0].GetHeroTeamMembers().Count));
                for (int m = 0; m < monsterCount; m++)
                {
                    //// TODO: produce inbalance, so that easy mobs are more often, then hard mobs
                    int random = UnityEngine.Random.Range(0, mobs.Count);
                    newSection.AddMob(mobs[random].GetComponent<Mob>());
                }

                List<GameObject> mercenaries = Datasheet.Mercenaries();

                for (int m = 0; m < MercenaryCount; m++)
                {
                    // TODO: produce inbalance, so that easy mercs are more often, then hard mercs
                    int random = UnityEngine.Random.Range(0, mercenaries.Count);
                    newSection.AddMercenary(mercenaries[random].GetComponent<Mercenary>());
                }

                newLevel.AddSection(newSection);
            }

            return newLevel;
        }

        #endregion

        #region "private"

        /// <summary>
        /// Returns the playmode jungle sections in a given order.
        /// </summary>
        /// <returns>Jungle sections</returns>
        private List<GameObject> GetJungleSections()
        {
            List<GameObject> jungleArr = new List<GameObject>();
            for (int i = 0; i < _sectionPrefabs.Count; i++)
            {
                if (_sectionPrefabs[i].name.Contains("JungleSection" + (i + 1)))
                {
                    jungleArr.Add(_sectionPrefabs[i]);
                }
            }

            return jungleArr;
        }

        /// <summary>
        /// Function returns a fixed Array of a number of SectionPrefabs
        /// </summary>
        /// <param name="sections">count of sections</param>
        /// <returns>The fixed Array of Sections</returns>
        private List<GameObject> GetFixedSections(int sections)
        {
            /*
            List<GameObject> _sectionArray = new List<GameObject>(sections);
            GameObject[] _shuffledArray = null;

            int sectionCount = _sectionPrefabs.Count;
            Debug.Log("[LevelGenerator.cs] " + sectionCount);

            int currentSection = 0;
            int shuffePosition = 0;
            while (currentSection < sections)
            {
                if (shuffePosition == 0)
                {
                    _shuffledArray = _getShuffledSections();
                }
                _sectionArray.Add(_shuffledArray[shuffePosition]);

                shuffePosition = ++shuffePosition % sectionCount;
                currentSection++;
                return_sectionArray
            }
            */
            return _sectionPrefabs;
        }

        /// <summary>
        /// Returns a shuffled Array of all SectionPrefabs
        /// </summary>
        /// <returns>The shuffled Array</returns>
        private GameObject[] _getShuffledSections()
        {
            GameObject[] sectionArray = _sectionPrefabs.ToArray();
            int n = sectionArray.Length;
            while (n > 1)
            {
                n--;
                int k = UnityEngine.Random.Range(0, sectionArray.Length);
                GameObject value = sectionArray[k];
                sectionArray[k] = sectionArray[n];
                sectionArray[n] = value;
            }

            return sectionArray;
        }

        #endregion

        #endregion
    }
}