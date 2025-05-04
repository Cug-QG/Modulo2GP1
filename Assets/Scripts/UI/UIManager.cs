using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    public static UIManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else { instance = this; }
    }

    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject gameOverMenu;
    [SerializeField] TextMeshProUGUI waveCounter;
    [SerializeField] TextMeshProUGUI pointsCounter;
    [SerializeField] TextMeshProUGUI afterDeathPoints;

    public void TogglePauseMenu(bool isActive) => pauseMenu.SetActive(isActive);
    public void ToggleGameOverMenu(bool isActive) => gameOverMenu.SetActive(isActive);
    public void SetWaveCounter(float value) { waveCounter.text = "Wave: " + value; }
    public void SetPointsCounter(float value) { pointsCounter.text = "" + value; }
    public void SetADPointsCounter(float value) { afterDeathPoints.text = "Points: " + value; }

    private bool placing = false;
    private GameObject objectToDraw;
    private Camera mainCamera;
    Vector3 rayHitPosition = Vector3.zero;
    Vector3 containerPosition = Vector3.zero;
    bool clicking = false;
    bool spawning = false;
    Ray ray;
    RaycastHit hit;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public LayerMask layerMask;

    bool selected = false;
    Placeable selectedTurret;

    private void Update()
    {
        if (!GameManager.Instance.GetGameStatus()) { return; }

        ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (!placing) 
        { 
            if (Input.GetMouseButtonDown(0)) 
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~layerMask, QueryTriggerInteraction.Ignore))
                {
                    if (hit.collider.CompareTag("Turret"))
                    {
                        if (!selected || selectedTurret == null || selectedTurret != hit.transform.GetComponent<Placeable>()) 
                        { 
                            if (selectedTurret != null && selectedTurret.gameObject != null) { selectedTurret.HideRange(); }
                            selectedTurret = hit.transform.GetComponent<Placeable>();
                            selectedTurret.ShowRange(); 
                            selected = true;
                            return; }
                        if (!GetSlot(hit.collider.gameObject).IsAbove(hit.collider.gameObject)) { return; }
                        GetPrefab(hit.collider.gameObject);
                        GetSlot(hit.collider.gameObject).DestroyTurret(hit.collider.gameObject);
                        StartPlacing(prefab);
                    }
                    else 
                    {
                        if (selectedTurret != null && selectedTurret.gameObject != null) { selectedTurret.HideRange(); }
                        selected = false; }
                }
            }
            return; 
        }

        if (Input.GetMouseButtonUp(0))
        {
            placing = false;
            clicking = false;
            spawning = false;
            

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~0, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.CompareTag("TurretSlot"))
                {
                    hit.transform.GetComponent<TurretSlot>().PlaceTurret(objectToDraw);
                }
            }
        }
        if (Input.GetMouseButton(0))
        {

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~0, QueryTriggerInteraction.Ignore))
            {
                
                rayHitPosition = hit.point;
                if (hit.collider.CompareTag("TurretSlot"))
                {
                    if (!hit.transform.GetComponent<TurretSlot>().Spawnable(objectToDraw)) { return; }
                    spawning = true;
                    containerPosition = hit.transform.GetComponent<TurretSlot>().GetSpawnPoint().position;
                }
                else { spawning = false; }
            }
        }
    }
    GameObject prefab;
    private void GetPrefab(GameObject turret)
    {
        if (turret.GetComponent<Turret>() != null)
        {
            prefab = turret.GetComponent<Turret>()?.GetOriginalPrefab();
        }
        if (turret.GetComponent<SupportTurret>() != null)
        {
            prefab = turret.GetComponent<SupportTurret>()?.GetOriginalPrefab();
        }
    }

    private TurretSlot GetSlot(GameObject turret)
    {
        TurretSlot output = null;
        if (turret.GetComponent<Turret>() != null)
        {
            output = turret.GetComponent<Turret>()?.GetSlot();
        }
        if (turret.GetComponent<SupportTurret>() != null)
        {
            output = turret.GetComponent<SupportTurret>()?.GetSlot();
        }
        return output;
    }

    public void StartPlacing(GameObject obj) 
    { 
        clicking = true;
        placing = true;
        objectToDraw = obj;
    }

    private void DrawMesh(Material material, Vector3 meshPosition, Mesh mesh, Vector3 scale)
    {
        material.SetPass(0);
        Matrix4x4 matrix = Matrix4x4.TRS(meshPosition, Quaternion.identity, scale);
        Graphics.DrawMeshNow(mesh, matrix);
    }

    void OnRenderObject()
    {
        if (clicking)
        {
            DrawMultipleMeshes(objectToDraw, rayHitPosition, false);
        }

        if (spawning)
        {
            DrawMultipleMeshes(objectToDraw, containerPosition, true);
        }
    }

    void DrawMultipleMeshes(GameObject obj, Vector3 canvaPosition, bool transparent)
    {
        MeshFilter[] filters = obj.GetComponentsInChildren<MeshFilter>();
        foreach (MeshFilter filter in filters)
        {
            Material mat = new(filter.GetComponent<MeshRenderer>().sharedMaterial);

            if (transparent) { mat = SetMaterialTransparent(mat); }

            DrawMesh(mat, canvaPosition + filter.transform.position, filter.sharedMesh, filter.transform.localScale);
        }
    }

    private Material SetMaterialTransparent(Material mat)
    {
        mat.SetFloat("_Surface", 1); // 0 = Opaque, 1 = Transparent
        mat.SetFloat("_Blend", 0); // Alpha Blending
        mat.SetFloat("_AlphaClip", 0); // Disabilita il Cutout
        mat.SetFloat("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetFloat("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetFloat("_ZWrite", 0); // Disabilita la scrittura nello ZBuffer per trasparenza
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

        // Abilita la parola chiave per trasparenza
        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");

        // Modifica il colore con trasparenza
        if (mat.HasProperty("_BaseColor"))
        {
            Color color = mat.GetColor("_BaseColor");
            color.a = 0.8f; // 30% trasparente
            mat.SetColor("_BaseColor", color);
        }
        return mat;
    }
}