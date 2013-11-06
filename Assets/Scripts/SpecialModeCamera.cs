namespace Assets.Scripts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Assets.Scripts;
    using UnityEngine;

    /// <summary>
    /// The Main Controll Class for the special mode
    /// </summary>
    public class SpecialModeCamera : MonoBehaviour
    {
        /// <summary>
        /// The NetworkController to synchronize the game over network
        /// </summary>
        private NetworkController _net;

        /// <summary>
        /// Gets or sets he distance of the Camera to the Scene
        /// </summary>
        public float CameraDistance { get; set; }

        /// <summary>
        /// sets up the Camera, start up the ProjectilePool, the GameManager, the Checkpoint the Bases and spawns the Heroes
        /// </summary>
        public void Awake()
        {
            CameraDistance = 20;

            ProjectilePool.PoolSize = 100;
            ProjectilePool.GeneratePool();
            GameManager manager = GameManager.GetInstance();
            GameObject g;
            int i = 1;
            while ((g = GameObject.Find("Checkpoint" + i)) != null)
            {
                Checkpoint c = g.GetComponent<Checkpoint>();
                manager.Checkpoints.Add(c);
                c.Init();
                i++;
            }

            Mobs.Base b = GameObject.Find("Base1").GetComponent<Mobs.Base>();
            b.DisplayName = "Base Blue";
            manager.GetTeam(1).AddMember(b);

            b.Team.AddMember(b.transform.FindChild("EliteMob1").GetComponent<Character>());
            b.Team.AddMember(b.transform.FindChild("EliteMob2").GetComponent<Character>());

            b = GameObject.Find("Base2").GetComponent<Mobs.Base>();
            b.DisplayName = "Base Red";
            manager.GetTeam(2).AddMember(b);

            b.Team.AddMember(b.transform.FindChild("EliteMob1").GetComponent<Character>());
            b.Team.AddMember(b.transform.FindChild("EliteMob2").GetComponent<Character>());

            _net = GameManager.GetInstance().NetworkController;
            if (_net.IsServer || _net.IsClient)
            {
                NetworkHeroSpawning();
            }
            else
            {
                SpawnHeroes();
            }
        }

        /// <summary>
        /// Updates the CameraPostion and end the game when ESC is pressed
        /// </summary>
        public void Update()
        {
            Movement(SpecialModeCameraController.CameraPosition());
            if (ControlKeysManager.GetKeyDown(KeyCode.Escape))
            {
                if (_net.IsClient)
                {
                    if (GameObject.Find("HeroClient") != null)
                    { // if someone klicked escape before his hero got initialised
                        _net.SendEscapeDisconnect(GameObject.Find("HeroClient").GetComponent<Hero>().Team.TeamNo);
                    }
                }
                else if (_net.IsServer)
                {
                    if (GameObject.Find("HeroServer") != null)
                    { // if someone klicked escape before his hero got initialised
                        _net.SendEscapeDisconnect(GameObject.Find("HeroServer").GetComponent<Hero>().Team.TeamNo);
                    }
                }

                Application.LoadLevel("MainMenu");
            }
        }

        /// <summary>
        /// Adds an hero to the Map
        /// </summary>
        /// <remarks>[RPC] Visible via network.</remarks>
        /// <param name="oldName">the old name of the hero</param>
        /// <param name="name">the name that the heros is going to have afterwards</param>
        /// <param name="teamNo">the Team the hero is going to be in</param>
        /// <param name="displayName">the player chosen name which should be shown</param>
        [RPC]
        public void AddHero(string oldName, string name, int teamNo, string displayName)
        {
            Hero hero = GameObject.Find(oldName).GetComponent<Hero>();
            hero.DisplayName = displayName;
            hero.name = name;
            hero.Type = Character.Types.HUMAN;
            GameManager.GetInstance().GetTeam(teamNo).AddMember(hero);
        }

        /// <summary>
        /// Moves the Camera in relation to Players
        /// </summary>
        /// <param name="position">Object camera moves towards</param>
        public void Movement(Vector3 position)
        {
            double angle = transform.rotation.eulerAngles.x / 180 * Math.PI;
            float y = (float)Math.Sin(angle) * CameraDistance;
            float z = (float)Math.Cos(angle) * CameraDistance;
            transform.position = new Vector3(position.x, position.y + y, position.z - z);
        }

        /// <summary>
        /// spawn all heroes
        /// </summary>
        private void SpawnHeroes()
        {
            int playerCounter = 1;
            List<Team> teamList = GameManager.GetInstance().GetAllTeams();
            for (int i = 0; i < teamList.Count; i++)
            {
                List<Hero> heroList = teamList[i].GetHeroTeamMembers();

                for (int j = 0; j < heroList.Count; j++)
                {
                    Hero h = heroList[j];
                    h = ((GameObject)Instantiate(h.gameObject)).GetComponent<Hero>();
                    h.PlayerNo = playerCounter;
                    playerCounter++;
                    h.Type = heroList[j].Type;
                    h.DisplayName = heroList[j].DisplayName;
                    h.name = "Hero" + h.PlayerNo;
                    teamList[i].RemoveMember(heroList[j]);
                    teamList[i].AddMember(h);
                    h.SpecialRespawn();
                }
            }

            for (int k = 0; k < GameManager.GetInstance().HeroCount; k++)
            {
                Debug.Log("[CharacterSelection.cs] Add AI hero");
                Hero choosenCharacter = ((GameObject)Instantiate(Datasheet.Heroes()[0])).GetComponent<Hero>();
                choosenCharacter.name = "Hero" + playerCounter + "KI";
                playerCounter++;
                choosenCharacter.Type = Hero.Types.AI;

                Debug.Log("Spawn AI: " + choosenCharacter.name);
                choosenCharacter.SpecialRespawn();

                if (GameManager.GetInstance().TeamCount == 1)
                {
                    teamList[0].AddMember(choosenCharacter);
                }
            }
        }

        /// <summary>
        /// Instantiates Heroes over Network
        /// </summary>
        private void NetworkHeroSpawning()
        {
            // Get hero components
            int teamNo = _net.TeamNumber;
            Team myTeam = GameManager.GetInstance().GetTeam(teamNo);
            UnityEngine.Object prefab = Resources.Load("Heroes/" + _net.Character);

            // initialise
            Hero hero = ((GameObject)Network.Instantiate(prefab, Vector3.zero, Quaternion.identity, 0)).GetComponent<Hero>();
            
            // set/prepare defaults
            string oldName = hero.name;
            hero.PlayerNo = 1;
            hero.Type = Character.Types.HUMAN;
            string objectName;
            if (_net.IsServer)
            {
                objectName = "HeroServer";
            }
            else
            {
                objectName = "HeroClient" + _net.ClientNumber;
            }

            string displayName = (!_net.Username.Equals(string.Empty)) ? _net.Username : hero.DisplayName;
            hero.DisplayName = displayName;
            hero.name = objectName;

            // set team
            myTeam.AddMember(hero);
            hero.Team = myTeam;

            // synchronize
            networkView.RPC("AddHero", RPCMode.Others, oldName, hero.name, teamNo, displayName);
            hero.SpecialRespawn();
        }
    }
}