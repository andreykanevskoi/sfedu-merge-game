using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Field tile", menuName = "Custom tiles/Create field tile")]
public class FieldTile : Tile {
    public bool destructible;
    public AudioClip digAudioClip;
}