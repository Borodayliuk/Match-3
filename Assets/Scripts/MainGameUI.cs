using UnityEngine;
using TMPro;

namespace Assets.Scripts {
    public class MainGameUI : MonoBehaviour {

        [SerializeField] private TextMeshProUGUI _scoreText;

        public void SetTextScore(string text) {
            _scoreText.text = text;
        }
    }
}