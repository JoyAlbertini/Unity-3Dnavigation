using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Octree.GameUtils;

namespace Octree.UI
{
    public class OptionUI : MonoBehaviour
    {
        public static OptionUI Instance { get; private set; }

        private Resolution startResolution;
        private void Awake()
        {
            Instance = this;
        }

        private TMP_Dropdown resolutions;
        private Slider sensibility;
        private Toggle fullscreen; 
        private Button apply;
        private Button exit;

        private enum res { r_current, r_1920_1080, r_1366_768, r_2560_1440, r_3840_2160 }
        private res resToSet;
        private bool full; 

        private void Start()
        {
            startResolution = Screen.currentResolution;
          
            resolutions = transform.GetChild(0).GetComponent<TMP_Dropdown>();
            resolutions.options[0].text = "" + startResolution.width + "x" + startResolution.height;
            fullscreen = transform.GetChild(1).GetComponent<Toggle>();
            apply = transform.GetChild(2).GetComponent<Button>();
            sensibility = transform.GetChild(3).GetComponent<Slider>();
            exit = transform.GetChild(4).GetComponent<Button>();

            resolutions.onValueChanged.AddListener(delegate {
                OnResolutionCheange(resolutions); 
            });

            fullscreen.onValueChanged.AddListener(delegate
            {
                SetFullScreen(fullscreen);
            });
            sensibility.onValueChanged.AddListener(delegate
            {
                OnSliderCheanged(sensibility);
            });

            GlobalGameParemters.MouseSensibility = 100f;
            sensibility.value = 100f;

            apply.onClick.AddListener(() => SetResolution());
            exit.onClick.AddListener(() => CloseApplication());
            SetActive(false);
        }

        private void SetFullScreen(Toggle toggle)
        {
            full = toggle.isOn; 
        }

        private void SetResolution()
        {
            switch(resToSet)
            {
                case res.r_current:
                    Screen.SetResolution(startResolution.width, startResolution.height, full);
                    break;
                case res.r_1366_768:
                    Screen.SetResolution(1366, 768, full);
                    break;
                case res.r_1920_1080:
                    Screen.SetResolution(1920, 1080, full);
                    break;
                case res.r_2560_1440:
                    Screen.SetResolution(2560, 1440, full);
                    break;
                case res.r_3840_2160:
                    Screen.SetResolution(3840, 2160, full);
                    break;
            }

        }

        private void OnResolutionCheange(TMP_Dropdown drop)
        {
            resToSet = (res) drop.value; 
        }
        private void OnSliderCheanged(Slider slide)
        {
            GlobalGameParemters.MouseSensibility = slide.value; 
        }

        private void CloseApplication()
        {
            Application.Quit();
        }
        public void SetActive(bool val)
        {
            transform.gameObject.SetActive(val);
        }
    }
}