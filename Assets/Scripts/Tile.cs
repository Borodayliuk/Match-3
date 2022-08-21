using System;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts {
    public class Tile : MonoBehaviour {

        [HideInInspector] public readonly UnityEvent<Tile> OnTypeChanged = new UnityEvent<Tile>();

        private TileType _tileType;

        public TileType Type {
            set {
                _tileType = value;
                OnTypeChanged.Invoke(this);
            }
            get => _tileType;
        }

        [Serializable]
        public struct TileType {
            public string typeName;
            public Sprite sprite;
        }
    }
}