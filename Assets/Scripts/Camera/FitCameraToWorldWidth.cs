using System;
using Unity.Cinemachine;
using UnityEngine;

public class FitCameraToWorldWidth : MonoBehaviour
{
    private const float RatioChangeThreshold = 0.01f;

    [SerializeField] private CinemachineCamera cinemachineCam;

    [Header("How many world Unity units fit into the screen width")] [SerializeField]
    private float width = 10f;

    private float _currRatio;

    private void Awake()
    {
        if (cinemachineCam == null)
            cinemachineCam = FindFirstObjectByType<CinemachineCamera>();
    }

    private void Start()
    {
        _currRatio = (float)Screen.width / Screen.height;
        FitToWidth();
    }

    private void Update()
    {
        var newRatio = (float)Screen.width / Screen.height;
        if (Math.Abs(newRatio - _currRatio) > RatioChangeThreshold)
        {
            _currRatio = newRatio;
            FitToWidth();
        }
    }

    private void FitToWidth()
    {
        if (cinemachineCam == null) return;

        var currHeight = cinemachineCam.Lens.OrthographicSize * 2;
        var currWidth = currHeight * _currRatio;
        var ratioChange = width / currWidth;
        cinemachineCam.Lens.OrthographicSize *= ratioChange;
    }
}