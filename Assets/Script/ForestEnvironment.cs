using UnityEngine;

/// <summary>Adds a small, non-colliding forest set around the mini-game arena.</summary>
public sealed class ForestEnvironment : MonoBehaviour
{
    private static readonly Vector2[] TreeLocations =
    {
        new Vector2(-8.7f, -7.2f), new Vector2(-4.9f, -8.8f), new Vector2(0.2f, -8.7f),
        new Vector2(5.2f, -8.5f), new Vector2(8.5f, -6.1f), new Vector2(8.7f, -1.8f),
        new Vector2(8.6f, 3.2f), new Vector2(6.2f, 8.2f), new Vector2(1.6f, 8.6f),
        new Vector2(-3.2f, 8.4f), new Vector2(-7.7f, 7.2f), new Vector2(-8.7f, 2.0f)
    };

    private void Start()
    {
        if (GameObject.Find("Forest Dressing") != null)
            return;

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
            shader = Shader.Find("Standard");
        if (shader == null)
            return;

        Transform forest = new GameObject("Forest Dressing").transform;
        Material bark = MakeMaterial(shader, new Color(0.25f, 0.13f, 0.07f));
        Material pine = MakeMaterial(shader, new Color(0.08f, 0.24f, 0.12f));
        Material pineLight = MakeMaterial(shader, new Color(0.15f, 0.34f, 0.16f));
        Material fern = MakeMaterial(shader, new Color(0.28f, 0.43f, 0.17f));
        Material stone = MakeMaterial(shader, new Color(0.31f, 0.34f, 0.29f));

        for (int i = 0; i < TreeLocations.Length; i++)
        {
            float scale = 0.78f + (i % 4) * 0.11f;
            CreateTree(forest, TreeLocations[i], scale, bark, i % 2 == 0 ? pine : pineLight);
        }

        // Ferns and boulders fill the edge of the clearing while leaving the play area open.
        for (int i = 0; i < 20; i++)
        {
            float angle = i * 2.39996f;
            float radius = 7.2f + (i % 4) * 0.52f;
            Vector2 position = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            CreateFern(forest, position, fern, 0.55f + (i % 3) * 0.12f);

            if (i % 2 == 0)
                CreateRock(forest, position * 1.07f, stone, 0.55f + (i % 3) * 0.12f);
        }
    }

    private static Material MakeMaterial(Shader shader, Color color)
    {
        Material material = new Material(shader)
        {
            name = "Forest " + ColorUtility.ToHtmlStringRGB(color),
            color = color
        };

        if (material.HasProperty("_BaseColor"))
            material.SetColor("_BaseColor", color);
        if (material.HasProperty("_Smoothness"))
            material.SetFloat("_Smoothness", 0.18f);
        return material;
    }

    private static void CreateTree(Transform parent, Vector2 position, float scale, Material bark, Material leaves)
    {
        CreatePrimitive(PrimitiveType.Cylinder, "Pine Trunk", parent,
            new Vector3(position.x, 0.88f * scale, position.y),
            new Vector3(0.2f * scale, 0.88f * scale, 0.2f * scale), bark);

        CreatePrimitive(PrimitiveType.Sphere, "Lower Pine Boughs", parent,
            new Vector3(position.x, 2.05f * scale, position.y),
            new Vector3(1.48f * scale, 1.06f * scale, 1.48f * scale), leaves);
        CreatePrimitive(PrimitiveType.Sphere, "Middle Pine Boughs", parent,
            new Vector3(position.x, 2.78f * scale, position.y),
            new Vector3(1.2f * scale, 1.04f * scale, 1.2f * scale), leaves);
        CreatePrimitive(PrimitiveType.Sphere, "Pine Crown", parent,
            new Vector3(position.x, 3.48f * scale, position.y),
            new Vector3(0.82f * scale, 1.0f * scale, 0.82f * scale), leaves);
    }

    private static void CreateFern(Transform parent, Vector2 position, Material leaves, float scale)
    {
        for (int i = 0; i < 4; i++)
        {
            float angle = i * 90f;
            Quaternion rotation = Quaternion.Euler(0f, angle, -28f);
            GameObject frond = CreatePrimitive(PrimitiveType.Capsule, "Fern Frond", parent,
                new Vector3(position.x, 0.34f * scale, position.y),
                new Vector3(0.17f * scale, 0.55f * scale, 0.17f * scale), leaves);
            frond.transform.rotation = rotation;
        }
    }

    private static void CreateRock(Transform parent, Vector2 position, Material material, float scale)
    {
        GameObject rock = CreatePrimitive(PrimitiveType.Sphere, "Mossy Boulder", parent,
            new Vector3(position.x, 0.18f * scale, position.y),
            new Vector3(0.95f * scale, 0.48f * scale, 0.78f * scale), material);
        rock.transform.rotation = Quaternion.Euler(0f, (position.x + position.y) * 11f, 8f);
    }

    private static GameObject CreatePrimitive(PrimitiveType type, string objectName, Transform parent,
        Vector3 position, Vector3 scale, Material material)
    {
        GameObject item = GameObject.CreatePrimitive(type);
        item.name = objectName;
        item.transform.SetParent(parent, false);
        item.transform.localPosition = position;
        item.transform.localScale = scale;

        Collider collider = item.GetComponent<Collider>();
        if (collider != null)
            collider.enabled = false;

        Renderer renderer = item.GetComponent<Renderer>();
        if (renderer != null)
            renderer.sharedMaterial = material;
        return item;
    }
}
