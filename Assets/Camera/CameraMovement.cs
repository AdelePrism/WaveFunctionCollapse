using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.U2D;

public class CameraMovement : MonoBehaviour
{
    //PixelFeature pixelShader;
    //PixelPerfectCamera pixelPerfectCamera;
    //[SerializeField] private UniversalRendererData pixelShader;
    //PixelFeature pixelFeature;
    // Start is called before the first frame update
    [SerializeField] Material pixelShader;
    Transform cameraTransform;
    float pixelHeight;
    Vector2 blockSize;

    void Start()
    {
        cameraTransform = GameObject.Find("Moving Camera").transform;
        //Debug.Log(pixelShader.GetPropertyNames(MaterialPropertyType.Float)[1]);
        pixelHeight = pixelShader.GetFloat(Shader.PropertyToID("_PixelHeight"));
        blockSize = new Vector2(1 / Mathf.CeilToInt(Camera.main.aspect * pixelHeight), 1 / pixelHeight);
        //pixelShader = 
        //pixelFeature = pixelShader.rendererFeatures.FirstOrDefault(feature => feature is PixelFeature) as PixelFeature;
        //pixelPerfectCamera = GetComponent<PixelPerfectCamera>();

    }

    //private void LateUpdate() {
    //    //int pixelHeight = pixelFeature.settings.screenHeight;
    //    int screenHeight = GetComponent<Camera>().pixelHeight;
    //    if (screenHeight > 0 ) {
    //        //pixelPerfectCamera.assetsPPU = screenHeight / pixelHeight;
    //    }
    //}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W)) {
            transform.position += (cameraTransform.forward - new Vector3(0, cameraTransform.forward.y, 0)).normalized * 0.03f;
            //transform.position += Vector3.forward * blockSize.y * 3;
        } else if (Input.GetKey(KeyCode.S)) {
            transform.position -= (cameraTransform.forward - new Vector3(0, cameraTransform.forward.y, 0)).normalized * 0.03f;
            //transform.position -= Vector3.forward * blockSize.y * 3;
        } 
        if (Input.GetKey(KeyCode.D)) {
            transform.position += cameraTransform.right.normalized * 0.03f;
        } else if (Input.GetKey(KeyCode.A)) {
            transform.position -= cameraTransform.right.normalized * 0.03f;
        } 
    }
}
