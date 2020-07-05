using BubbleDistortionPhysics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class DynamicResolution : MonoBehaviour
{
    public Text screenText;

    FrameTiming[] frameTimings = new FrameTiming[3];

    public float maxResolutionWidthScale = 1.0f;
    public float maxResolutionHeightScale = 1.0f;
    public float minResolutionWidthScale = 0.5f;
    public float minResolutionHeightScale = 0.5f;
    public float scaleWidthIncrement = 0.1f;
    public float scaleHeightIncrement = 0.1f;

    float m_widthScale = 1.0f;
    float m_heightScale = 1.0f;

    // Variables for dynamic resolution algorithm that persist across frames
    uint m_frameCount = 0;

    const uint kNumFrameTimings = 2;

    float m_gpuFrameTime;
    float m_cpuFrameTime;

    // Use this for initialization
    void Start()
    {
        int rezWidth = (int)Mathf.Ceil(ScalableBufferManager.widthScaleFactor * Screen.currentResolution.width);
        int rezHeight = (int)Mathf.Ceil(ScalableBufferManager.heightScaleFactor * Screen.currentResolution.height);        
    }

    //Doesnt actually do anything!
    // Update is called once per frame
    //void Update()
    //{
    //    float oldWidthScale = m_widthScale;
    //    float oldHeightScale = m_heightScale;

    //    bool blnIncrease = false;
    //    bool blnDecrease = false;

    //    //PlayerController.Instance.RightController?.inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out blnIncrease);
    //    //PlayerController.Instance.RightController?.inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out blnDecrease);

    //    // One finger lowers the resolution
    //    if (blnIncrease)
    //    {
    //        //XRSettings.renderScale = 1.5f;
    //        //XRSettings.eyeTextureResolutionScale = 1.5f;
    //        m_heightScale = Mathf.Max(minResolutionHeightScale, m_heightScale - scaleHeightIncrement);
    //        m_widthScale = Mathf.Max(minResolutionWidthScale, m_widthScale - scaleWidthIncrement);
    //    }

    //    // Two fingers raises the resolution
    //    if (blnDecrease)
    //    {
    //        //XRSettings.renderScale = 0.5f;
    //        //XRSettings.eyeTextureResolutionScale = 0.5f;
    //        m_heightScale = Mathf.Min(maxResolutionHeightScale, m_heightScale + scaleHeightIncrement);
    //        m_widthScale = Mathf.Min(maxResolutionWidthScale, m_widthScale + scaleWidthIncrement);
    //    }

    //    if (m_widthScale != oldWidthScale || m_heightScale != oldHeightScale)
    //    {
    //        ScalableBufferManager.ResizeBuffers(m_widthScale, m_heightScale);
    //        int rezWidth2 = (int)Mathf.Ceil(ScalableBufferManager.widthScaleFactor * Screen.currentResolution.width);
    //        int rezHeight2 = (int)Mathf.Ceil(ScalableBufferManager.heightScaleFactor * Screen.currentResolution.height);
    //        OutputLogManager.OutputText(string.Format("Scale: {0:F3}x{1:F3} Resolution: {2}x{3} ScaleFactor: {4:F3}x{5:F3} GPU: {6:F3} CPU: {7:F3}",
    //        m_widthScale,
    //        m_heightScale,
    //        rezWidth2,
    //        rezHeight2,
    //        ScalableBufferManager.widthScaleFactor,
    //        ScalableBufferManager.heightScaleFactor,
    //        m_gpuFrameTime,
    //        m_cpuFrameTime));            
    //    }
    //    DetermineResolution();
    //    int rezWidth = (int)Mathf.Ceil(ScalableBufferManager.widthScaleFactor * Screen.currentResolution.width);
    //    int rezHeight = (int)Mathf.Ceil(ScalableBufferManager.heightScaleFactor * Screen.currentResolution.height);        
    //}

    private float _fpsUpdate;
    private float _lastFps;
    private float _targetFps = 90f;
    private DateTime _lastChange;
    private double _timeToIncrease = 200;
    private int _currentFrame = 0;
    private int _frameWindow = 30;

    private void Update()
    {
        _currentFrame += 1;

        _lastFps += 1.0f / Time.deltaTime;        

        if (_currentFrame == _frameWindow)
        {
            _lastFps = _lastFps / _frameWindow;
            if (_lastFps > 0 && PlayerController.Instance.LightsCount >= 6f && _lastFps < _targetFps * 0.99f && (DateTime.Now - _lastChange).TotalMilliseconds > 200)
            {
                _timeToIncrease += 200;
                _lastChange = DateTime.Now;
                PlayerController.Instance.LightsCount -= 1;
                OutputLogManager.UpdateLogPerformance("GPU " + _lastFps.ToString() + " Lights count " + PlayerController.Instance.LightsCount);
            }
            else if (_lastFps >= _targetFps && PlayerController.Instance.LightsCount < 20f && (DateTime.Now - _lastChange).TotalMilliseconds > _timeToIncrease)
            {
                _lastChange = DateTime.Now;
                PlayerController.Instance.LightsCount += 1;
                OutputLogManager.UpdateLogPerformance("GPU " + _lastFps.ToString() + " Lights count " + PlayerController.Instance.LightsCount);
            }
            _lastFps = 0;
            _currentFrame = 0;
        }        
    }

    // Estimate the next frame time and update the resolution scale if necessary.
    //private void DetermineResolution()
    //{
    //    ++m_frameCount;
    //    if (m_frameCount <= kNumFrameTimings)
    //    {
    //        return;
    //    }
    //    FrameTimingManager.CaptureFrameTimings();
    //    FrameTimingManager.GetLatestTimings(kNumFrameTimings, frameTimings);
    //    if (frameTimings.Length < kNumFrameTimings)
    //    {
    //        Debug.LogFormat("Skipping frame {0}, didn't get enough frame timings.",
    //            m_frameCount);

    //        return;
    //    }

    //    m_gpuFrameTime = (double)frameTimings[0].gpuFrameTime;
    //    m_cpuFrameTime = (double)frameTimings[0].cpuFrameTime;
    //}
}