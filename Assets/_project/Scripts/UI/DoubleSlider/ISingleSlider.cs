using System;
using UnityEngine;
using UnityEngine.Events;

namespace TS.DoubleSlider
{
    public interface ISingleSlider
    {
        GameObject GameObject { get; }
        bool IsEnabled { get; set; }
        float Value { get; set; }
        bool WholeNumbers { get; set; }
        void Setup(float value, float minValue, float maxValue, UnityAction<float> valueChanged);
        void Setup(float value, float minValue, float maxValue, UnityAction<float> valueChanged, DateTime earlisetDate);
    }
}