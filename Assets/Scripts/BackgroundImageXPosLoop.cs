using System;
using UnityEngine;
using UnityEngine.UI;
using Game.Stats;

namespace Game.Background 
{
    public class BackgroundImageXPosLoop : MonoBehaviour 
    {
        [SerializeField] private BackgroundElements[] backgroundElements;
        private bool isScrolling;

        private void Start() 
        {
            PlayerStats.onGameOver += ScrollSwitch;
        }

        private void Update() 
        {
            if (isScrolling) 
            {
                foreach (var element in backgroundElements) 
                {
                    element.backgroundImage.uvRect = new Rect(element.backgroundImage.uvRect.position + new Vector2(element.xPosBckgImg, 0) * Time.deltaTime, element.backgroundImage.uvRect.size);
                }
            }
        }

        private void ScrollSwitch(bool isGameOver) 
        {
            isScrolling = !isGameOver;
        }

        [Serializable]
        public class BackgroundElements 
        {
            public RawImage backgroundImage;
            public float xPosBckgImg;
        }
    }
}