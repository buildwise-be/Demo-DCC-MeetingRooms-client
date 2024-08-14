#region Includes
using System;
using System.Globalization;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
#endregion

namespace TS.DoubleSlider
{
    [RequireComponent(typeof(Slider))]
    public class DateSingleSlider : MonoBehaviour, ISingleSlider
    {
        #region Variables

        [Header("References")]
        [SerializeField] private Label _label;

        private Slider _slider;
        private DateTime _minDate;

        [HideInInspector]

        public bool IsEnabled
        {
            get { return _slider.interactable; }
            set { _slider.interactable = value; }

        }
        public float Value
        {
            get { return _slider.value; }
            set
            {
                _slider.value = value;
                _slider.onValueChanged.Invoke(_slider.value);

                UpdateLabel();
            }
        }
        public bool WholeNumbers
        {
            get { return _slider.wholeNumbers; }
            set { _slider.wholeNumbers = value; }
        }
        public GameObject GameObject 
        {
            get 
            {
                if (_slider == null)
                {
                    _slider = GetComponent<Slider>();
                }
                return _slider.gameObject; 
            }
        }

        #endregion

        private void Awake()
        {
            if (!TryGetComponent<Slider>(out _slider))
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.LogError("Missing Slider Component");
#endif
            }
        }

        public void Setup(float value, float minValue, float maxValue, UnityAction<float> valueChanged)
        {
            _slider.minValue = minValue;
            _slider.maxValue = maxValue;

            _slider.value = value;
            _slider.onValueChanged.AddListener(Slider_OnValueChanged);
            _slider.onValueChanged.AddListener(valueChanged);

            UpdateLabel();
        }

        public void Setup(float value, float minValue, float maxValue, UnityAction<float> valueChanged, DateTime minDate)
        {
            _slider.minValue = minValue;
            _slider.maxValue = maxValue;

            _slider.value = value;
            _slider.onValueChanged.AddListener(Slider_OnValueChanged);
            _slider.onValueChanged.AddListener(valueChanged);

            _minDate = minDate;

            UpdateLabel();
        }

        private void Slider_OnValueChanged(float arg0)
        {
            UpdateLabel();
        }

        protected virtual void UpdateLabel()
        {
            if (_label == null) { return; }
            DateTime date = _minDate.AddDays(Value);
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            _label.Text = date.ToShortDateString();
        }
    }
}