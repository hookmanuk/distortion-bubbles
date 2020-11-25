using BubbleDistortionPhysics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;
using UnityEngine.VR;
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
    //void Start()
    //{
    //    int rezWidth = (int)Mathf.Ceil(ScalableBufferManager.widthScaleFactor * Screen.currentResolution.width);
    //    int rezHeight = (int)Mathf.Ceil(ScalableBufferManager.heightScaleFactor * Screen.currentResolution.height);
    //}

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


    //MJH This is my code for only lighting a certain distance around the player
    private float _fpsUpdate;
    public static float LastFPS;
    private float _targetFps = 90f;
    private DateTime _lastChange;
    private double _timeToIncrease = 200;
    private int _currentFrame = 0;
    private int _frameWindow = 1;
    public HDRenderPipelineAsset PipelineSettings;
    public static float ResScale = 1f;
    public static float ReportedFPS;
    private float _lastReportedScale;
    private bool _reportedScale1;
    public static bool DynamicResolutionEnabled = true;

    private void Start()
    {
        DynamicResolutionHandler.SetDynamicResScaler(SetDynamicResolutionScale, DynamicResScalePolicyType.ReturnsMinMaxLerpFactor);
    }

    private float SetDynamicResolutionScale()
    {
        if (DynamicResolutionEnabled)
        {
            if (ResScale < 1)
            {
                if (ResScale != _lastReportedScale)
                {
                    //Debug.Log(_reportedFps.ToString() + "fps, scale now " + _resScale.ToString());
                    _lastReportedScale = ResScale;
                }
                _reportedScale1 = false;
                return ResScale;
            }
            else
            {
                if (!_reportedScale1)
                {
                    //Debug.Log(_reportedFps.ToString() + "fps, scale now 1");
                    _reportedScale1 = true;
                }
                return 1;
            }
        }
        else
        {
            return 1;
        }
    }

    private void Update()
    {
        if (DynamicResolutionEnabled)
        {
            _currentFrame += 1;

            var frameFPS = 1.0f / Time.deltaTime;
            LastFPS += frameFPS;

            //run increase loop every framewindow
            if (_currentFrame == _frameWindow)
            {
                LastFPS = (float)Math.Round(LastFPS / (float)_frameWindow, 1);

                if (ResScale < 1 && LastFPS >= _targetFps * 0.99f)// && (DateTime.Now - _lastChange).TotalMilliseconds > _timeToIncrease)
                {
                    ReportedFPS = LastFPS;
                    _lastChange = DateTime.Now;
                    //PlayerController.Instance.LightsDistance += 1;
                    if (ResScale < 1)
                    {
                        ResScale = (float)Math.Round(ResScale + 0.01f, 2);
                    }

                    //OutputLogManager.UpdateLogPerformance("GPU " + _lastFps.ToString() + " Lights distance " + PlayerController.Instance.LightsDistance);
                }
                LastFPS = 0;
                _currentFrame = 0;
            }

            //run decrease loop every frame

            if (ResScale > 0 && frameFPS > 0 && frameFPS < _targetFps * 0.90f)// && (DateTime.Now - _lastChange).TotalMilliseconds > 200)
            {
                ReportedFPS = (float)Math.Round(frameFPS, 2);
                _timeToIncrease += 200;
                _lastChange = DateTime.Now;
                ResScale = (float)Math.Round(ResScale - 0.1f, 2);
                //PlayerController.Instance.LightsDistance -= 1;
                //OutputLogManager.UpdateLogPerformance("GPU " + _lastFps.ToString() + " Lights distance " + PlayerController.Instance.LightsDistance);
                //Debug.Log(_lastFps.ToString() + "fps, scale now " + _resScale.ToString());
            }
        }
        else
        {
            ReportedFPS = (float)Math.Round(1.0f / Time.deltaTime, 2);
        }
    }
}