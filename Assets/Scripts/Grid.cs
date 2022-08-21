using System;
using UnityEngine;

namespace Assets.Scripts {
    public class Grid : MonoBehaviour {

        public event Action<GridObjectArgs> OnGridObjectRemoved;

        [SerializeField] private Vector2Int _size;
        [SerializeField] private float _cellSize;

        private GridObject[,] _gridObjects;
        private Vector3 _offset;
        private int _queueHeight;

        private Func<GridObject, Vector2Int, object> _syncAction;

        public int QueueHeight {
            get { return _queueHeight; }
        }

        private void Awake() {
            _gridObjects = new GridObject[_size.x, _size.y * 2];
            _queueHeight = _size.y * 2;
            _offset = new Vector3(_size.x * _cellSize / 2 - _cellSize / 2, _size.y * _cellSize / 2 - _cellSize / 2);
        }

        public Vector2Int WorldToGrid(Vector3 position) {
            Vector3 pos = position + _offset;
            return new Vector2Int(Mathf.RoundToInt(pos.x / _cellSize), Mathf.RoundToInt(pos.y / _cellSize));
        }

        public Vector3 GridToWorld(Vector2Int position, float zPosition = 0) {
            return new Vector3(position.x * _cellSize, position.y * _cellSize, zPosition) - _offset;
        }

        public void Set(GridObject gridObject, Vector2Int position, bool useDefaultSync = false) {
            _gridObjects[position.x, position.y] = gridObject;
            if (gridObject != null) {
                if (useDefaultSync) {
                    gridObject.transform.position = GridToWorld(position);
                } else {
                    _syncAction?.Invoke(gridObject, position);
                }
            }
        }

        public void Set<T>(GridObject gridObject, Vector2Int position, out T syncResult, bool useDefaultSync = false) {
            T result = default;
            _gridObjects[position.x, position.y] = gridObject;
            if (gridObject != null) {
                if (useDefaultSync) {
                    gridObject.transform.position = GridToWorld(position);
                } else {
                    result = (T)(_syncAction?.Invoke(gridObject, position));
                }
            }
            syncResult = result;
        }

        public void Remove(Vector2Int position) {
            var gridObject = Get(position);
            _gridObjects[position.x, position.y] = null;
            OnGridObjectRemoved?.Invoke(new GridObjectArgs() {
                gridObject = gridObject,
                gridPosition = position
            });
        }

        public bool Has(Vector2Int position) {
            return _gridObjects[position.x, position.y] != null;
        }

        public GridObject Get(Vector2Int position) {
            if (position.x >= 0 && position.x < _size.x && position.y >= 0 && position.y < _queueHeight) {
                return _gridObjects[position.x, position.y];
            } else {
                return null;
            }
        }

        public Vector2Int GetGridSize() {
            return _size;
        }

        public int GetLastObjectFromQueue(int xPosition) {
            int topY = 0;
            for (int y = 0; y < _queueHeight; y++) {
                if (Has(new Vector2Int(xPosition, y))) {
                    topY = y;
                }
            }
            return topY;
        }

        public bool HasPosition(Vector2Int position) {
            return position.x >= 0 && position.x < _size.x && position.y >= 0 && position.y < _size.x;
        }

        public void AddToQueue(int xPosition, GridObject gridObject) {
            var result = true;

            for (int y = _size.y; y < _queueHeight; y++) {
                if (!Has(new Vector2Int(xPosition, y))) {
                    Set(gridObject, new Vector2Int(xPosition, y), out result, true);
                    break;
                }
            }
        }

        public void SetSyncAction(Func<GridObject, Vector2Int, object> syncAction) => _syncAction = syncAction;

        public Vector2Int GetMouseGridPosition(Camera camera) {
            Vector2Int result = Vector2Int.zero;

            result = WorldToGrid(camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1)));

            return result;
        }

        public struct GridObjectArgs {
            public GridObject gridObject;
            public Vector2Int gridPosition;
        }
    }
}