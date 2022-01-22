using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TTW.Persistent;
using System;
using UnityEngine.SceneManagement;

namespace TTW.Combat
{
    public class GameOverListener : MonoBehaviour
    {
        [SerializeField] private CombatUnloader bench;
        [SerializeField] private GameObject gameOverText;
        ActorEntity[] completeParty;
        AnimationController animationController;
        public static GameOverListener singleton;
        bool gameOver = false;

        private void Awake()
        {
            animationController = AnimationController.singleton;
            singleton = this;
        }

        private void Update()
        {
            //if (!gameOver) return;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        private void GameOver()
        {
            print("GAME OVER!");
            gameOverText.SetActive(true);
            animationController.StartAnimationFreeze();

            gameOver = true;
        }

        public void GameOverCheck()
        {
            completeParty = FindObjectsOfType<ActorEntity>();
            bool gameOverTest = true;

            if (!bench.CheckBenchVacant())
            {
                print("bench still occupied");
                gameOverTest = false;
            }

            foreach (ActorEntity a in completeParty)
            {
                if (a.GetComponent<StatsHandler>().Alive)
                {
                    print("actor: " + a.name + " is still alive");
                    gameOverTest = false;
                }
                    
            }

            if (gameOverTest)
            {
                GameOver();
            }
        }
    }
}