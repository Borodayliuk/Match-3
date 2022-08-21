using UnityEngine;

namespace Assets.Scripts {
    public class FieldTileSelector : MonoBehaviour {

        [SerializeField] private Grid _grid;
        [SerializeField] private GameObject _tileSelector;

        private Vector2Int _firstSelectedPosition = new Vector2Int(-1, -1);
        private Vector2Int _secondSelectedPosition = new Vector2Int(-1, -1);

        public Vector2Int FirstSelectedPosition {
            set {
                _firstSelectedPosition = value;
                if (_tileSelector != null) {
                    if (IsFirstSelected()) {
                        _tileSelector.transform.position = _grid.GridToWorld(_firstSelectedPosition);
                        _tileSelector.SetActive(true);
                    }
                }
            }
            get { return _firstSelectedPosition; }
        }

        public Vector2Int SecondSelectedPosition {
            set { _secondSelectedPosition = value; }
            get { return _secondSelectedPosition; }
        }

        public bool IsFirstSelected() {
            return _firstSelectedPosition != new Vector2Int(-1, -1);
        }

        public bool IsSecondSelected() {
            return _secondSelectedPosition != new Vector2Int(-1, -1);
        }

        public void FirstUnselect() {
            _firstSelectedPosition = new Vector2Int(-1, -1);
            _tileSelector.SetActive(false);
        }

        public void SecondUnselect() {
            _secondSelectedPosition = new Vector2Int(-1, -1);
        }
    }
}