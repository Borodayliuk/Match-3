using UnityEngine;

namespace Assets.Scripts {
    public class TileVisual : MonoBehaviour {

        [SerializeField] private Tile _tile;
        private SpriteRenderer _spriteRenderer;

        private void Awake() {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _tile.OnTypeChanged.AddListener(UpdateSprite);
        }
        private void UpdateSprite(Tile tile) {
            _spriteRenderer.sprite = tile.Type.sprite;
        }
    }
}