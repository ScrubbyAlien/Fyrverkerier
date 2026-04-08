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
    public void BakeGraph() {
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
}
