using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace FateGames
{
    public class UILoseScreen : MonoBehaviour
    {
        [SerializeField] private Text levelText = null;

        private void Awake()
        {
            levelText.text = "LEVEL " + PlayerProgression.CurrentLevel.ToString();
        }

        // Called by ContinueButton onClick
        public void Continue()
        {
            SceneManager.LoadCurrentLevel();
        }
    }
}