#region Includes
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
#endregion

namespace TS.DoubleSlider
{
    [RequireComponent(typeof(RectTransform))]
    public class DoubleSlider : MonoBehaviour
    {
        #region Variables

        [Header("References")]
        [SerializeField] private RectTransform _fillArea;
        [SerializeField] private ISingleSlider _sliderMin;
        [SerializeField] private ISingleSlider _sliderMax;

        [Header("Configuration")]
        [SerializeField] private bool _setupOnStart;
        [SerializeField] private float _minValue;
        [SerializeField] private float _maxValue;
        [SerializeField] private float _minDistance;
        [SerializeField] private bool _wholeNumbers;
        [SerializeField] private float _initialMinValue;
        [SerializeField] private float _initialMaxValue;

        [Header("Events")]
        public UnityEvent<float, float> OnValueChanged;

        public bool IsEnabled
        {
            get { return _sliderMax.IsEnabled && _sliderMin.IsEnabled; }
            set
            {
                _sliderMin.IsEnabled = value;
                _sliderMax.IsEnabled = value;
            }
        }
        public float MinValue
        {
            get { return _sliderMin.Value; }
        }
        public float MaxValue
        {
            get { return _sliderMax.Value; }
        }
        public bool WholeNumbers
        {
            get { return _wholeNumbers; }
            set
            {
                _wholeNumbers = value;

                _sliderMin.WholeNumbers = _wholeNumbers;
                _sliderMax.WholeNumbers = _wholeNumbers;
            }
        }

        private RectTransform _fillRect;

        #endregion

        private void Awake()
        {
            foreach (ISingleSlider slider in GetComponentsInChildren<ISingleSlider>())
            {
                if (slider.GameObject.name == "SliderMin")
                {
                    _sliderMin = slider;
                }
                else if (slider.GameObject.name == "SliderMax")
                {
                    _sliderMax = slider;
                }
            }
            if (_sliderMin == null || _sliderMax == null)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD

                Debug.LogError("Missing slider min: " + _sliderMin + ", max: " + _sliderMax);
#endif
                return;
            }
            if (_fillArea == null)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD

                Debug.LogError("Missing fill area");
#endif
                return;
            }

            _fillRect = _fillArea.transform.GetChild(0).transform as RectTransform;
        }
        private void Start()
        {
            if (!_setupOnStart) { return; }
            Setup(_minValue, _maxValue, _initialMinValue, _initialMaxValue);
        }

        public void Setup(float minValue, float maxValue, float initialMinValue, float initialMaxValue)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            _initialMinValue = initialMinValue;
            _initialMaxValue = initialMaxValue;

            _sliderMin.Setup(_initialMinValue, minValue, maxValue, MinValueChanged);
            _sliderMax.Setup(_initialMaxValue, minValue, maxValue, MaxValueChanged);

            MinValueChanged(_initialMinValue);
            MaxValueChanged(_initialMaxValue);
        }
        public void Setup(List<MeetingRoomData> meetingRoomDatas)
        {
            Debug.Log("Setting up double slider...");
            _minValue = 0;
            IEnumerable<DateTimeOffset?> startTimes = meetingRoomDatas
            .SelectMany(data => data.StartTimes)
            .Where(startTime => startTime.HasValue);
             DateTimeOffset? latestDate = startTimes.Max();
            DateTimeOffset? earliestDate = startTimes.Min();

            Debug.Log("Earliest date: " + earliestDate);
            Debug.Log("Latest date: " + latestDate);
            _maxValue = (latestDate - earliestDate).Value.Days;
            Debug.Log("delta days: " + _maxValue);
            _initialMinValue = _minValue;
            _initialMaxValue = _maxValue;

            Debug.Log("Min value: " + _minValue + ", Max value: " + _maxValue);

            _sliderMin.Setup(_initialMinValue, _minValue, _maxValue, MinValueChanged, earliestDate.Value.DateTime);
            _sliderMax.Setup(_initialMaxValue, _minValue, _maxValue, MaxValueChanged, earliestDate.Value.DateTime);

            MinValueChanged(_initialMinValue);
            MaxValueChanged(_initialMaxValue);
        }
        private void MinValueChanged(float value)
        {
            float offset = ((MinValue - _minValue) / (_maxValue - _minValue)) * _fillArea.rect.width;

            _fillRect.offsetMin = new Vector2(offset, _fillRect.offsetMin.y);

            if ((MaxValue - value) < _minDistance)
            {
                _sliderMin.Value = MaxValue - _minDistance;
            }

            OnValueChanged.Invoke(MinValue, MaxValue);
            _sliderMin.GameObject.transform.SetAsLastSibling();
        }
        private void MaxValueChanged(float value)
        {
            float offset = (1 - ((MaxValue - _minValue) / (_maxValue - _minValue))) * _fillArea.rect.width;

            _fillRect.offsetMax = new Vector2(-offset, _fillRect.offsetMax.y);

            if ((value - MinValue) < _minDistance)
            {
                _sliderMax.Value = MinValue + _minDistance;
            }

            OnValueChanged.Invoke(MinValue, MaxValue);
            _sliderMax.GameObject.transform.SetAsLastSibling();
        }
    }
}