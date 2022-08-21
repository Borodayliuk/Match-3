using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts {
    public class MainGame : MonoBehaviour {

        [SerializeField] private Grid _grid;
        [SerializeField] private GridObject _gridObjectPrefab;
        [SerializeField] private FieldTileSelector _fieldTileSelector;
        [SerializeField] private FieldLogic _fieldLogic;
        [SerializeField] private List<Tile.TileType> _tileTypes;
        [SerializeField] private MainGameUI _mainGameUI;

        private Vector2Int _gridSize;
        private bool _isSwapping = false;
        private int _score = 0;

        private void Awake() {
            _grid.SetSyncAction((obj, pos) => {
                var tween = obj.transform
                    .DOMove(_grid.GridToWorld(pos), 0.5f)
                    .SetEase(Ease.OutBounce);
                return tween;
            });
            _fieldLogic.OnQueueTileAdded.AddListener(OnQueueTileAdded);
            _fieldLogic.OnSwapped.AddListener(OnSwapped);
            _fieldLogic.OnTileRemoved.AddListener(OnTileRemoved);
            _mainGameUI.SetTextScore(_score.ToString());
        }

        private void Start() {
            FillingGrid();
        }

        private void Update() {
            if (Input.GetMouseButtonUp(0) && _fieldLogic.IsRemoved) {
                if (_grid.HasPosition(_grid.GetMouseGridPosition(Camera.main))) {
                    if (_fieldTileSelector.IsFirstSelected() == false) {
                        _fieldTileSelector.FirstSelectedPosition = _grid.GetMouseGridPosition(Camera.main);
                    } else if (_fieldTileSelector.FirstSelectedPosition != _grid.GetMouseGridPosition(Camera.main) && _fieldTileSelector.IsSecondSelected() == false && _isSwapping == false) {
                        _fieldTileSelector.SecondSelectedPosition = _grid.GetMouseGridPosition(Camera.main);

                        _fieldLogic.Swap(_fieldTileSelector.FirstSelectedPosition, _fieldTileSelector.SecondSelectedPosition);

                        _fieldTileSelector.FirstUnselect();
                        _fieldTileSelector.SecondUnselect();
                    }
                }
            }
        }

        private void FillingGrid() {
            _gridSize = _grid.GetGridSize();
            for (int x = 0; x < _gridSize.x; x++) {
                for (int y = 0; y < _gridSize.y; y++) {
                    GridObject gridObject = Instantiate(_gridObjectPrefab.gameObject).GetComponent<GridObject>();
                    Vector2Int gridPosition = new Vector2Int(x, y);

                    _grid.Set(gridObject, gridPosition);
                    if (gridObject.TryGetComponent(out Tile tile)) {
                        tile.Type = RandomType(gridPosition);
                    }
                }
            }
        }

        private Tile.TileType RandomType(Vector2Int position) {
            List<Tile.TileType> tileTypes = new List<Tile.TileType>(_tileTypes);
            GridObject left1 = _grid.Get(new Vector2Int(position.x - 1, position.y));
            GridObject left2 = _grid.Get(new Vector2Int(position.x - 2, position.y));
            GridObject down1 = _grid.Get(new Vector2Int(position.x, position.y - 1));
            GridObject down2 = _grid.Get(new Vector2Int(position.x, position.y - 2));

            if (left2 != null && left1.GetComponent<Tile>().Type.typeName == left2.GetComponent<Tile>().Type.typeName) {
                tileTypes.Remove(left1.GetComponent<Tile>().Type);
            }
            if (down2 != null && down1.GetComponent<Tile>().Type.typeName == down2.GetComponent<Tile>().Type.typeName) {
                tileTypes.Remove(down1.GetComponent<Tile>().Type);
            }

            return tileTypes[Random.Range(0, tileTypes.Count)];
        }

        private void OnQueueTileAdded(Tile tile) {
            tile.Type = _tileTypes[Random.Range(0, _tileTypes.Count)];
            _score += 1 * 100;
            _mainGameUI.SetTextScore(_score.ToString());
        }

        private void OnSwapped(Tile firstTile, Tile secondTile, object firstSyncResult, object secondSyncResult) {
            Tween firstTween = (Tween) firstSyncResult;
            Tween secondTween = (Tween) secondSyncResult;

            _isSwapping = true;

            var matching = _fieldLogic.GetMatchedPositions().Count > 0;
            firstTween.onComplete = () => {
                _isSwapping = false;
                secondTile.transform.DOKill(true);
                firstTile.transform.DOKill(true);
                if (matching) {
                    _fieldLogic.DoMatching(firstTile, secondTile);
                } else {
                    _fieldLogic.SetTile(firstTile, _grid.WorldToGrid(secondTile.transform.position));
                    _fieldLogic.SetTile(secondTile, _grid.WorldToGrid(firstTile.transform.position));
                }
            };
        }

        private void OnTileRemoved(Tile tile, bool destroy) {
            tile.transform.DOKill(true);
        }
    }
}