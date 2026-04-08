using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

[HideMonoScript]
public class NavGrid : MonoBehaviour
{
    [SerializeField]
    private NeighbourType neighbourType;
    [SerializeField]
    private Tilemap navigationalMap;
    [SerializeField, ReadOnly]
    private GraphAsset graphAsset;
    
    [HideIn(PrefabKind.PrefabAsset)]
    public void BakeGrid() {
        GraphAsset newGraphAsset = ScriptableObject.CreateInstance<GraphAsset>();

        newGraphAsset.GenerateGraph(navigationalMap, neighbourType);
        
        graphAsset = newGraphAsset;
        
        string sceneName = SceneManager.GetActiveScene().name;
        if (!AssetDatabase.IsValidFolder($"Assets/Scenes/{sceneName}")) {
            AssetDatabase.CreateFolder("Assets/Scenes", sceneName);
        }
        AssetDatabase.CreateAsset(
            newGraphAsset, 
            $"Assets/Scenes/{sceneName}/{sceneName}-{nameof(NavGrid)}{GetHashCode()}.asset"
        );
    }

    public void OnDrawGizmos() {
        if (!graphAsset || !graphAsset.initialized) return;
        
        Gizmos.color = Color.cyan;
        
        foreach (GridTileEdge edge in graphAsset.edges) {
            Vector3 start = navigationalMap.layoutGrid.GetCellCenterWorld(edge.Source.coord);
            Vector3 end = navigationalMap.layoutGrid.GetCellCenterWorld(edge.Target.coord);
            Gizmos.DrawLine(start, end);
        }
    }
}
