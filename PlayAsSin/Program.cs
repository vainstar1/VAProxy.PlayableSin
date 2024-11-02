using BepInEx;
using UnityEngine;
using UnityEngine.SceneManagement;
using TrailsFX;

[BepInPlugin("SinModelSwap", "Swaps Sen's model with Sin's.", "1.0.0")]
public class SinModelSwap : BaseUnityPlugin
{
    private GameObject senHead;
    private GameObject sinHead;
    private GameObject senHumanbot;
    private GameObject sinHumanbot;
    private GameObject senUpperCape;
    private GameObject senLowerCape;
    private GameObject senNeck;
    private GameObject senCorrupt;
    private GameObject justSen;

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "BirdCage")
        {
            Debug.Log("DaDemo scene loaded, starting model swap process.");

            try
            {
                FindGameObjects();

                if (senHumanbot != null && sinHumanbot != null && senUpperCape != null && senLowerCape != null && senNeck != null && senCorrupt != null)
                {
                    PerformModelSwap();
                    // Increase player scale, remove this and justSen if you want small Sen
                    // justSen.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
                }
                else
                {
                    Debug.LogError("One or more GameObjects not found!");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error during model swap process: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }

    void FindGameObjects()
    {
        senHumanbot = GameObject.Find("S-105.1/Humanbot_A_Geom");
        sinHumanbot = GameObject.Find("World/Areas/ERI Grave/SkyBase/bossFight/S-104/Humanbot_A_Geom");
        senUpperCape = GameObject.Find("S-105.1/ROOT/Hips/Spine/Spine1/UpperCape");
        senLowerCape = GameObject.Find("S-105.1/ROOT/Hips/Spine/Spine1/UpperCape/LowerCape");
        senNeck = GameObject.Find("S-105.1/ROOT/Hips/Spine/Spine1/Neck");
        senCorrupt = GameObject.Find("S-105.1/ROOT/Hips/Spine/Spine1/Corrupt");
        justSen = GameObject.Find("S-105.1");
    }

    void PerformModelSwap()
    {
        ReplaceMaterials(senHumanbot, sinHumanbot);
        ReplaceSharedMesh(senHumanbot, sinHumanbot);
        DisableSenCorrupt();
        DisableUpperCape();
        DisableLowerCape();
        CopyTopsMaterial();
        CopyAndPlaceTops();
        UpdateTrailsFXColor();
        InstantiateSinCorruptHead();

        Debug.Log("Model swap process completed successfully.");
    }

    void InstantiateSinCorruptHead()
    {
        GameObject sinCorruptHead = GameObject.Find("World/Areas/ERI Grave/SkyBase/bossFight/S-104/ROOT/Hips/Spine/Spine1/Neck/Head/Pivot/Corrupt");
        if (sinCorruptHead != null)
        {
            GameObject newCorruptHead = Instantiate(sinCorruptHead);
            newCorruptHead.name = "Corrupt(Clone)";

            Transform senSpine1 = GameObject.Find("S-105.1/ROOT/Hips/Spine/Spine1").transform;
            if (senSpine1 != null)
            {
                newCorruptHead.transform.SetParent(senSpine1, false);

                // Adjust position, rotation, and scale
                newCorruptHead.transform.localPosition = new Vector3(0f, 0.4067f, -0.0272f);
                newCorruptHead.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                newCorruptHead.transform.localScale = new Vector3(0.3153f, 0.3153f, 0.3153f);

                Debug.Log("Sin's Corrupt head instantiated and positioned under Sen's Spine1.");
            }
            else
            {
                Debug.LogError("Sen's Spine1 not found!");
            }
        }
        else
        {
            Debug.LogError("Sin's Corrupt head object not found in scene!");
        }
    }

    void ReplaceMaterials(GameObject senObj, GameObject sinObj)
    {
        try
        {
            SkinnedMeshRenderer senRenderer = senObj.GetComponent<SkinnedMeshRenderer>();
            SkinnedMeshRenderer sinRenderer = sinObj.GetComponent<SkinnedMeshRenderer>();

            if (senRenderer != null && sinRenderer != null)
            {
                Material sinMaterial = sinRenderer.material;
                senRenderer.material = Instantiate(sinMaterial);

                Debug.Log("Sen's material replaced with Sin's material.");
            }
            else
            {
                Debug.LogError("SkinnedMeshRenderer component not found in Sen or Sin objects!");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error replacing materials: {ex.Message}\n{ex.StackTrace}");
        }
    }

    void ReplaceSharedMesh(GameObject senObj, GameObject sinObj)
    {
        try
        {
            SkinnedMeshRenderer senRenderer = senObj.GetComponent<SkinnedMeshRenderer>();
            SkinnedMeshRenderer sinRenderer = sinObj.GetComponent<SkinnedMeshRenderer>();

            if (senRenderer != null && sinRenderer != null)
            {
                Mesh sinMesh = sinRenderer.sharedMesh;
                senRenderer.sharedMesh = sinMesh;

                Debug.Log("Sen's sharedMesh replaced with Sin's sharedMesh.");
            }
            else
            {
                Debug.LogError("SkinnedMeshRenderer component not found in Sen or Sin objects!");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error replacing shared meshes: {ex.Message}\n{ex.StackTrace}");
        }
    }

    void CopyTopsMaterial()
    {
        try
        {
            GameObject sinTops = GameObject.Find("World/Areas/ERI Grave/SkyBase/bossFight/S-104/ROOT/Hips/Spine/Spine1/Tops_14");

            if (sinTops != null)
            {
                SkinnedMeshRenderer sinTopsRenderer = sinTops.GetComponent<SkinnedMeshRenderer>();
                if (sinTopsRenderer != null)
                {
                    Material sinTopsMaterial = sinTopsRenderer.material;

                    UpdateCapeMaterial(senUpperCape, sinTopsMaterial);
                    UpdateCapeMaterial(senLowerCape, sinTopsMaterial);

                    Debug.Log("Replaced materials of UpperCape and LowerCape with Tops_14 material.");
                }
                else
                {
                    Debug.LogError("SkinnedMeshRenderer not found on Tops_14!");
                }
            }
            else
            {
                Debug.LogError("Tops_14 not found in Sin's directory!");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error replacing Tops_14 material: {ex.Message}\n{ex.StackTrace}");
        }
    }

    void UpdateCapeMaterial(GameObject capeObject, Material newMaterial)
    {
        try
        {
            if (capeObject != null)
            {
                MeshRenderer capeRenderer = capeObject.GetComponent<MeshRenderer>();
                if (capeRenderer != null)
                {
                    capeRenderer.material = Instantiate(newMaterial);
                    Debug.Log($"{capeObject.name} material replaced with Tops_14 material.");
                }
                else
                {
                    Debug.LogError($"MeshRenderer not found on {capeObject.name}!");
                }
            }
            else
            {
                Debug.LogError("Cape object is null!");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error updating cape material: {ex.Message}\n{ex.StackTrace}");
        }
    }

    void CopyAndPlaceTops()
    {
        try
        {
            GameObject sinTops = GameObject.Find("World/Areas/ERI Grave/SkyBase/bossFight/S-104/ROOT/Hips/Spine/Spine1/Tops_14");

            if (sinTops != null)
            {
                GameObject newTops = Instantiate(sinTops);
                newTops.name = "Tops_14(Clone)";

                Transform senSpine1 = GameObject.Find("S-105.1/ROOT/Hips/Spine/Spine1").transform;
                if (senSpine1 != null)
                {
                    newTops.transform.SetParent(senSpine1, false);

                    newTops.transform.localPosition = sinTops.transform.localPosition;
                    newTops.transform.localRotation = sinTops.transform.localRotation;

                    Debug.Log("Copied Tops_14 and placed it under Sen's Spine1.");
                }
                else
                {
                    Debug.LogError("Sen's Spine1 not found!");
                }
            }
            else
            {
                Debug.LogError("Tops_14 not found in Sin's directory!");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error copying and placing Tops_14: {ex.Message}\n{ex.StackTrace}");
        }
    }

    void DisableSenCorrupt()
    {
        try
        {
            if (senCorrupt != null)
            {
                Renderer corruptRenderer = senCorrupt.GetComponent<Renderer>();
                if (corruptRenderer != null)
                {
                    corruptRenderer.enabled = false;
                    Debug.Log("Hid Sen's Corrupt Renderer.");
                }
                else
                {
                    Debug.LogError("Renderer component not found on Sen's Corrupt object!");
                }
            }
            else
            {
                Debug.LogError("Corrupt object not found in Sen's directory!");
            }

            Transform corruptTransform = senCorrupt.transform;
            Transform eyeActiveTransform = corruptTransform.Find("EyeActive");
            if (eyeActiveTransform != null)
            {
                eyeActiveTransform.gameObject.SetActive(false);
                Debug.Log("Disabled EyeActive GameObject.");
            }
            else
            {
                Debug.LogError("EyeActive object not found under Sen's Corrupt object!");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error disabling Sen's Corrupt: {ex.Message}\n{ex.StackTrace}");
        }
    }

    void DisableUpperCape()
    {
        try
        {
            if (senUpperCape != null)
            {
                Renderer upperCapeRenderer = senUpperCape.GetComponent<Renderer>();
                if (upperCapeRenderer != null)
                {
                    upperCapeRenderer.enabled = false;
                    Debug.Log("Hid Sen's UpperCape Renderer.");
                }
                else
                {
                    Debug.LogError("Renderer component not found on Sen's UpperCape object!");
                }
            }
            else
            {
                Debug.LogError("UpperCape object not found in Sen's directory!");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error disabling Sen's UpperCape: {ex.Message}\n{ex.StackTrace}");
        }
    }

    void DisableLowerCape()
    {
        try
        {
            if (senLowerCape != null)
            {
                Renderer lowerCapeRenderer = senLowerCape.GetComponent<Renderer>();
                if (lowerCapeRenderer != null)
                {
                    lowerCapeRenderer.enabled = false;
                    Debug.Log("Hid Sen's LowerCape Renderer.");
                }
                else
                {
                    Debug.LogError("Renderer component not found on Sen's LowerCape object!");
                }
            }
            else
            {
                Debug.LogError("LowerCape object not found in Sen's directory!");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error disabling Sen's LowerCape: {ex.Message}\n{ex.StackTrace}");
        }
    }

    void UpdateTrailsFXColor()
    {
        try
        {
            var trailComponent = senHumanbot.GetComponent<TrailEffect>();
            if (trailComponent != null)
            {
                trailComponent.enabled = true; 
                trailComponent.active = true; 
                trailComponent.color = new Color(0f, 0f, 0f, 1f); 
                Debug.Log("Updated TrailsEffect: enabled, _active set to true, and color set to (0, 0, 0, 1).");
            }
            else
            {
                Debug.LogError("TrailEffect component not found on Humanbot_A_Geom!");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error updating TrailsEffect: {ex.Message}\n{ex.StackTrace}");
        }
    }
}
