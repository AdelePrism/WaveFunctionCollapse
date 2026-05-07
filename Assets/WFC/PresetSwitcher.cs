using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PresetSwitcher : MonoBehaviour
{
    [Header("Presets")]
    [Range(1, 2)] public int preset = 1;
    [SerializeField] GameObject preset1;
    [SerializeField] GameObject preset2;

    [Header ("Shader")]
    [SerializeField] bool shaderEnabled = true;
    [SerializeField] ScriptableRendererData shaderFeature;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        shaderFeature.rendererFeatures[1].SetActive(shaderEnabled);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && preset != 1) {
            preset = 1;
            preset2.SetActive(false);
            preset1.SetActive(true);
        } else if (Input.GetKeyDown(KeyCode.Alpha2) && preset != 2) {
            preset = 2;
            preset1.SetActive(false);
            preset2.SetActive(true);
        } else if (Input.GetKeyDown(KeyCode.F)) {
            shaderEnabled = !shaderEnabled;
            shaderFeature.rendererFeatures[1].SetActive(shaderEnabled);
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }


    private void OnValidate() {
        if (preset == 1) {
            preset2.SetActive(false);
            preset1.SetActive(true);
        } else if (preset == 2) {
            preset1.SetActive(false);
            preset2.SetActive(true);
        }
        shaderFeature.rendererFeatures[1].SetActive(shaderEnabled);
    }
}
