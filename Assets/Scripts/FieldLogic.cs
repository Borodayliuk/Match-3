using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts {
    public class FieldLogic : MonoBehaviour {

        [HideInInspector] public UnityEvent<Tile, Tile, object, object> OnSwapped = new UnityEvent<Tile, Tile, object, object>();
        [HideInInspector] public UnityEvent<Tile> OnQueueTileAdded = new UnityEvent<Tile>();
        [HideInInspector] public UnityEvent<Tile, bool> OnTileRemoved = new UnityEvent<Tile, bool>();

        [SerializeField] private Grid _grid;

        private bool _isRemoved;
        private Vector2Int _gridSize;

        public bool IsRemoved {
            get { return _isRemoved; }
        }

        private void Awake() {
            _gridSize = _grid.GetGridSize();
            _isRemoved = true;
        }

        public void SetTile(Tile tile, Vector2Int position, bool useDefaultSync = false) {
            _grid.Set(tile.GetComponent<GridObject>(), position, useDefaultSync);
        }

        public void SetTile<T>(Tile tile, Vector2Int position, out T syncResult, bool useDefaultSync = false) {
            _grid.Set(tile.GetComponent<GridObject>(), position, out syncResult, useDefaultSync);
        }

        public void RemoveTile(Vector2Int position, bool destroy = false) {
            Tile tile = _grid.Get(position).GetComponent<Tile>();
            if (destroy) {
                Destroy(tile.gameObject);
            }
            _grid.Remove(position);

            OnTileRemoved.Invoke(tile, destroy);
        }

        public void Swap(Vector2Int firstPosition, Vector2Int secondPosition) {
            if (Vector2Int.Distance(firstPosition, secondPosition) <= 1) {
                object firstSyncResult;
                object secondSyncResult;

                var firstTile = _grid.Get(firstPosition).GetComponent<Tile>();
                var secondTile = _grid.Get(secondPosition).GetComponent<Tile>();
                SetTile(firstTile, secondPosition, out firstSyncResult);
                SetTile(secondTile, firstPosition, out secondSyncResult);

                OnSwapped.Invoke(firstTile, secondTile, firstSyncResult, secondSyncResult);
            }
        }

        public bool DoMatching(Tile firstTile, Tile secondTile) {
            var matchedPositions = GetMatchedPositions();
            if (matchedPositions.Count > 0) {
                StartCoroutine(TilesRemoving(matchedPositions));
                return true;
            } else {
                return false;
            }
        }

        public HashSet<Vector2Int> GetMatchedPositions() {
            HashSet<Vector2Int> matchedPositions = new HashSet<Vector2Int>();
            for (int x = 0; x < _gridSize.x; x++) {
                for (int y = 0; y < _gridSize.y; y++) {
                    Vector2Int position = new Vector2Int(x, y);
                    GridObject gridObject = _grid.Get(position);
                    GridObject left1 = _grid.Get(new Vector2Int(position.x - 1, position.y));
                    GridObject left2 = _grid.Get(new Vector2Int(position.x - 2, position.y));
                    GridObject down1 = _grid.Get(new Vector2Int(position.x, position.y - 1));
                    GridObject down2 = _grid.Get(new Vector2Int(position.x, position.y - 2));
                    if (gridObject != null) {
                        if (left2 != null && left1 != null
                                    && left1.GetComponent<Tile>().Type.typeName == left2.GetComponent<Tile>().Type.typeName
                                    && left2.GetComponent<Tile>().Type.typeName == gridObject.GetComponent<Tile>().Type.typeName) {
                            matchedPositions.Add(position);
                            matchedPositions.Add(position - new Vector2Int(1, 0));
                            matchedPositions.Add(position - new Vector2Int(2, 0));
                        }
                        if (down2 != null && down1 != null
                                    && down1.GetComponent<Tile>().Type.typeName == down2.GetComponent<Tile>().Type.typeName
                                    && down2.GetComponent<Tile>().Type.typeName == gridObject.GetComponent<Tile>().Type.typeName) {
                            matchedPositions.Add(position);
                            matchedPositions.Add(position - new Vector2Int(0, 1));
                            matchedPositions.Add(position - new Vector2Int(0, 2));
                        }
                    }
                }
            }
            return matchedPositions;
        }

        private IEnumerator TilesRemoving(HashSet<Vector2Int> positions) {
            _isRemoved = false;
            foreach (Vector2Int position in positions) {
                Tile tile = _grid.Get(position).GetComponent<Tile>();
                if (tile != null) {
                    RemoveTile(position);
                    _grid.AddToQueue(position.x, tile.GetComponent<GridObject>());
                    OnQueueTileAdded.Invoke(tile);
                }
            }
            FieldTilesFalling();
            yield return new WaitForSeconds(0.5f);
            var matchedPositions = GetMatchedPositions();
            if (matchedPositions.Count > 0) {
                StartCoroutine(TilesRemoving(matchedPositions));
            }
            _isRemoved = true;
        }

        public bool ColumnTilesFalling(int x) {
            //get first empty cells
            Vector2Int emptyCellPosition = new Vector2Int(-1, -1);
            int emptyCellsCount = 0;
            for (int y = 0; y < _gridSize.y; y++) {
                if (_grid.Has(new Vector2Int(x, y)) && emptyCellPosition != new Vector2Int(-1, -1)) {
                    break;
                }
                if (_grid.Has(new Vector2Int(x, y)) == false) {
                    if (emptyCellPosition == new Vector2Int(-1, -1)) {
                        emptyCellPosition = new Vector2Int(x, y);
                    }
                    emptyCellsCount += 1;
                }
            }
            //falling
            for (int y = 0; y < _grid.QueueHeight; y++) {
                var cellPosition = emptyCellPosition + new Vector2Int(0, y);
                var tileGridObject = _grid.Get(cellPosition);
                if (tileGridObject != null) {
                    RemoveTile(cellPosition);
                    SetTile(tileGridObject.GetComponent<Tile>(), cellPosition - new Vector2Int(0, emptyCellsCount));
                }
            }
            return emptyCellsCount > 0;
        }

        public void FieldTilesFalling() {
            for (int x = 0; x < _grid.GetGridSize().x; x++) {
                var hasEmptyCells = ColumnTilesFalling(x);
                while (hasEmptyCells) {
                    x = 0;
                    hasEmptyCells = ColumnTilesFalling(x);
                }
            }
        }
    }
}