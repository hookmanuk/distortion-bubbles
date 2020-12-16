using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace BubbleDistortionPhysics
{
    public class QualitySetting
    {         
        public QualitySetting()
        {                        

        }

        public string Name { get; set; }

        public int DrawDistanceAbove
        {
            get
            {
                switch (DrawDistance)
                {
                    case DrawDistance.Low:
                        return 5;
                    case DrawDistance.Medium:
                        return 10;
                    case DrawDistance.High:
                        return 30;
                    default:
                        return 10;
                }
            }
        }

        public int DrawDistanceBelow
        {
            get
            {
                switch (DrawDistance)
                {
                    case DrawDistance.Low:
                        return 5;
                    case DrawDistance.Medium:
                        return 12;
                    case DrawDistance.High:
                        return 40;
                    default:
                        return 12;
                }
            }
        }

        public int LightsDistanceAbove
        {
            get
            {
                switch (LightsDistance)
                {
                    case LightsDistance.Low:
                        return 6;
                    case LightsDistance.Medium:
                        return 6;
                    case LightsDistance.High:
                        return 8;
                    default:
                        return 8;
                }
            }
        }

        public int LightsDistanceBelow
        {
            get
            {
                switch (LightsDistance)
                {
                    case LightsDistance.Low:
                        return 3;
                    case LightsDistance.Medium:
                        return 3;
                    case LightsDistance.High:
                        return 8;
                    default:
                        return 8;
                }
            }
        }

        public int LightsDistanceHorizontal
        {
            get
            {
                switch (LightsDistance)
                {
                    case LightsDistance.Low:
                        return 8;
                    case LightsDistance.Medium:
                        return 20;
                    case LightsDistance.High:
                        return 50;
                    default:
                        return 15;
                }
            }
        }



        public DrawDistance DrawDistance { get; set; } = DrawDistance.Medium;
        public LightsDistance LightsDistance { get; set; } = LightsDistance.Medium;
        public Effects Effects { get; set; } = Effects.Medium;
    }

    public class QualitySettings
    {
        private static QualitySettings _instance;
        public static QualitySettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new QualitySettings();
                }
                return _instance;
            }
        }
        //public static QualitySettings Instance
        //{
        //    get
        //    {
        //        return new QualitySettings();                
        //    }
        //}

        public QualitySetting QualityLow { get; set; }
        public QualitySetting QualityMedium { get; set; }
        public QualitySetting QualityHigh { get; set; }
        public QualitySettings()
        {
            _instance = this;

            QualityLow = new QualitySetting() { Name = "Low", DrawDistance = DrawDistance.Low, LightsDistance = LightsDistance.Low, Effects = Effects.Low };
            QualityMedium = new QualitySetting() { Name = "Medium", DrawDistance = DrawDistance.Medium, LightsDistance = LightsDistance.Medium, Effects = Effects.Medium };
            QualityHigh = new QualitySetting() { Name = "High", DrawDistance = DrawDistance.High, LightsDistance = LightsDistance.High, Effects = Effects.High };
        }
    }

    public enum DrawDistance
    {
        Low,
        Medium,
        High
    }

    public enum LightsDistance
    {
        Low,
        Medium,
        High
    }

    public enum Effects
    {
        Low,
        Medium,
        High
    }
}
