using UnityEngine;
using UnityEngine.UI;

namespace Music.Assets.Scripts // To change correctly
{
    public class SliderController : MonoBehaviour
    {
        private Slider sliderInstance;

        public void InitSlider()
        {
            sliderInstance.minValue = 0;
            sliderInstance.maxValue = 1;
            sliderInstance.wholeNumbers = false;
            sliderInstance.value = 0.85f;
        }

        public float GetSliderVolume()
        {
            return sliderInstance.value;
        }

        public void SetSliderVolume(float newVolume)
        {
            sliderInstance.value = newVolume;
        }
    }
}