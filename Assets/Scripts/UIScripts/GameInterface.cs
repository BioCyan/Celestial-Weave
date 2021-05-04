using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* Attaches to GamePanel */
public class GameInterface : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Image leftPortalImage;
    [SerializeField] private Image rightPortalImage;

    private void Start()
    {
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }
    void Update()
    {
        UpdateReticle();
        
    }

    private void UpdateReticle()
    {
        Color emptyColor = new Color(0.8f, 0.8f, 0.8f, 0.5f),
            leftColor = new Color(1f, 0.5f, 0f, 0.5f),
            rightColor = new Color(0f, 0.5f, 1f, 0.5f);
        leftPortalImage.color = playerCamera.GetComponent<PortalCamera>().leftPortal == null ? emptyColor : leftColor;
        rightPortalImage.color = playerCamera.GetComponent<PortalCamera>().rightPortal == null ? emptyColor : rightColor;
    }
}
