namespace Assets.Scripts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;

    /// <summary>
    /// Represents a Level
    /// </summary>
    public class Level
    {
        /// <summary>
        /// All Sections that are part of the level.
        /// </summary>
        public List<Section> Sections;

        /// <summary>
        /// Difficulty (0-3 or x?) and Level-Number (1-x)
        /// </summary>
        private int _difficulty, _levelNo;

        /// <summary>
        /// Initializes a new instance of the Level class
        /// </summary>
        /// <param name="difficulty">the difficulty setting</param>
        /// <param name="levelNo">number of the level</param>
        public Level(int difficulty, int levelNo)
        {
            Sections = new List<Section>();
            this._difficulty = difficulty;
            this._levelNo = levelNo;
        }

        /// <summary>
        /// Adds a Section to the level.
        /// </summary>
        /// <param name="section">section that is supossed to be added to the level</param>
        public void AddSection(Section section)
        {
            this.Sections.Add(section);
        }
    }
}