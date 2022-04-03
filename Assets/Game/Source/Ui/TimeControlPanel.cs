using Game.Ingame.Simulator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Ui
{
    public class TimeControlPanel : MonoBehaviour
    {
        [SerializeField] Button _pauseButton;
        [SerializeField] Button _playButton1;
        [SerializeField] Button _playButton2;
        [SerializeField] Button _playButton3;

        [SerializeField] TextMeshProUGUI _currentProgressText;
        [SerializeField] TextMeshProUGUI _maxProgressText;
        [SerializeField] Slider _progressSlider;

        [Header("Colors")]
        [SerializeField] Color _activeButtonColor = Color.red;
        [SerializeField] Color _normalButtonColor = Color.white;

        bool _isPaused; 
        
        Simulator _simulator;
        
        [Inject]
        public void Construct(Simulator simulator)
        {
            _simulator = simulator;
        }

        void OnEnable()
        {
            _progressSlider.onValueChanged.AddListener(SetCurrentSimulationTick);
        }
        
        void OnDisable()
        {
            _progressSlider.onValueChanged.RemoveListener(SetCurrentSimulationTick);
        }

        void Start()
        {
            _isPaused = false;
            _currentProgressText.text = "0";
            _maxProgressText.text = "0";
            
            UpdateUi();
        }

        void Update()
        {
            _progressSlider.interactable = _isPaused;

            _progressSlider.value = (float)_simulator.SimulationTick / _simulator.MaxSimulationTick;

            _currentProgressText.text = _simulator.SimulationTick.ToString();
            _maxProgressText.text = _simulator.MaxSimulationTick.ToString();
        }
        
        public void SetSimulationSpeed(float speed)
        {
            _simulator.SimulationSpeed = speed;
            UpdateUi();
        }

        void UpdateUi()
        {
            var speed = _simulator.SimulationSpeed;

            _pauseButton.image.color = speed < 0.01f ? _activeButtonColor : _normalButtonColor;
            _playButton1.image.color = speed is >= 0.01f and < 1f ? _activeButtonColor : _normalButtonColor;
            _playButton2.image.color = speed is >= 1f and < 2f ? _activeButtonColor : _normalButtonColor;
            _playButton3.image.color = speed >= 2f ? _activeButtonColor : _normalButtonColor;
            
            _isPaused = speed < 0.01f;
        }
        
        void SetCurrentSimulationTick(float progress)
        {
            var tickInt = (int) (progress * _simulator.MaxSimulationTick);
            SetCurrentSimulationTick(tickInt);
        }
        
        public void SetCurrentSimulationTick(int tick)
        {
            tick = Mathf.Clamp(tick, 0, _simulator.MaxSimulationTick);
            _simulator.SetCurrentSimulationTick(tick);
        }
    }
}