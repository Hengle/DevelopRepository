using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RolePlayingGame
{
    public class GamePlay : MonoBehaviour
    {
        public enum GameState
        {
            PlayerTurn,
            EnemyTurn,
        };

        private void Start()
        {
            StartCoroutine("PlayerTurn");
        }

        // Update is called once per frame
        void Update()
        {
        }

        void ChangeTurn(GameState turn)
        {
            switch (turn)
            {
                case GameState.PlayerTurn:
                    Debug.Log("PlayerTurn");
                    StartCoroutine("PlayerTurn");
                    break;
                case GameState.EnemyTurn:
                    Debug.Log("EnemyTurn");
                    StartCoroutine("EnemyTurn");
                    break;
            }

        }

        IEnumerator PlayerTurn()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            StartCoroutine(player.transform.GetComponent<Player>().MainRoutine());
            yield return new WaitForSeconds(0.5f);
            ChangeTurn(GameState.EnemyTurn);
        }

        IEnumerator EnemyTurn()
        {
            var enemys = GameObject.FindGameObjectsWithTag("Enemy");
            foreach(var enemy in enemys)
            {
                StartCoroutine(enemy.transform.GetComponent<Enemy>().MainRoutine());
            }
            yield return new WaitForSeconds(0.5f);
            ChangeTurn(GameState.PlayerTurn);
        }
    }
}