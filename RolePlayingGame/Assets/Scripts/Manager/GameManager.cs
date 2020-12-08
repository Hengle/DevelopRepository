using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RolePlayingGame
{
    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        [SerializeField] GamePlay gamePlay;

        private void Awake()
        {
            gamePlay.enabled = true;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
