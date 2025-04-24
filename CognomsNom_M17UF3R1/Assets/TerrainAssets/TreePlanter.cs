using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TreePlanterUsingTerrainTexture : MonoBehaviour
{
    public Terrain terrain;              // Referencia al terreno
    public GameObject[] treePrefabs;     // Lista de prefabs de árboles
    public int selectedTextureIndex;     // Índice de la capa de textura del terreno a usar como máscara
    public int numberOfTrees = 100;      // Número de árboles a instanciar
    public float threshold = 0.1f;       // Umbral para considerar la presencia de la textura

    // Método para plantar árboles, ejecutable desde el editor
    [ContextMenu("Plant Trees Using Texture Mask")]
    public void PlantTreesUsingTextureMask()
    {
        // Validar que los campos necesarios estén asignados
        if (terrain == null || treePrefabs == null || treePrefabs.Length == 0)
        {
            Debug.LogError("Terrain o treePrefabs no están asignados.");
            return;
        }

        TerrainData terrainData = terrain.terrainData;
        float[,,] alphamaps = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);

        // Crear un objeto padre para organizar los árboles
        GameObject treesParent = new GameObject("Arboles");

        int treesPlanted = 0;
        while (treesPlanted < numberOfTrees)
        {
            // Generar una posición aleatoria dentro de los límites del terreno
            float x = Random.Range(0, terrainData.size.x);
            float z = Random.Range(0, terrainData.size.z);
            Vector3 position = new Vector3(x, 0, z);

            // Obtener la altura del terreno en esa posición
            position.y = terrainData.GetHeight((int)(x / terrainData.size.x * terrainData.heightmapResolution),
                                              (int)(z / terrainData.size.z * terrainData.heightmapResolution));

            // Calcular las coordenadas normalizadas para los alphamaps
            int alphamapX = (int)((x / terrainData.size.x) * terrainData.alphamapWidth);
            int alphamapZ = (int)((z / terrainData.size.z) * terrainData.alphamapHeight);

            // Asegurarse de que las coordenadas están dentro de los límites
            alphamapX = Mathf.Clamp(alphamapX, 0, terrainData.alphamapWidth - 1);
            alphamapZ = Mathf.Clamp(alphamapZ, 0, terrainData.alphamapHeight - 1);

            // Obtener el valor de la capa de textura en esa posición
            float maskValue = alphamaps[alphamapZ, alphamapX, selectedTextureIndex];

            // Si el valor de la máscara supera el umbral, plantar un árbol
            if (maskValue > threshold)
            {
                // Elegir un prefab aleatorio
                GameObject treePrefab = treePrefabs[Random.Range(0, treePrefabs.Length)];

                // Instanciar el árbol con rotación aleatoria
                Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                GameObject tree = Instantiate(treePrefab, position + terrain.transform.position, rotation, treesParent.transform);

                // Aplicar escala aleatoria para variedad
                float scale = Random.Range(0.8f, 1.2f);
                tree.transform.localScale = new Vector3(scale, scale, scale);

                treesPlanted++;
            }
        }

#if UNITY_EDITOR
        // Marcar la escena como modificada (solo en el editor)
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
#endif
    }
}