using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlaceableFactory : ScriptableObject {
    [SerializeField] private Placeable[] prefabs;

    private Dictionary<int, Placeable> _prefabsDictionary;

    public void Init() {
        _prefabsDictionary = new Dictionary<int, Placeable>();

        Debug.Log("Factory Init");

        foreach (var prefab in prefabs) {
            if (!_prefabsDictionary.ContainsKey(prefab.prefabId)) {
                _prefabsDictionary.Add(prefab.prefabId, prefab);
                Debug.Log(prefab.prefabId);
            }
            else {
                Debug.LogError("Duplicated prefab id");
            }
        }

        // prefabs = null;
    }

    public Placeable Create(int prefabId) {
        return Instantiate(GetPrefab(prefabId));
    }

    public Placeable GetPrefab(int prefabId) {
        if (_prefabsDictionary.ContainsKey(prefabId)) {
            Placeable prefab = _prefabsDictionary[prefabId];
            return prefab;
        }

        Debug.LogError("No such prefab id");
        return null;
    }
}
