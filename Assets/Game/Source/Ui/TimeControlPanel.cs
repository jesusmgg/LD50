using System.Collections.Generic;
using Game.Ingame;
using Game.Ingame.Simulator;
using Game.Ingame.Tank;
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
        [SerializeField] Animator _timeControlPanelAnimator;
        [SerializeField] Transform _arrow;

        [SerializeField] List<float> _arrowRotations;

        [SerializeField] TextMeshProUGUI _currentProgressText;
        [SerializeField] TextMeshProUGUI _maxProgressText;
        [SerializeField] Slider _progressSlider;

        bool _isPaused;

        TankController _playerTankController;
        TankPlayerInput _tankPlayerInput;
        Simulator _simulator;
        LevelScript _levelScript;
        
        static readonly int IsVisible = Animator.StringToHash("IsVisible");

        [Inject]
        public void Construct(Simulator simulator, TankPlayerInput tankPlayerInput, LevelScript levelScript)
        {
            _simulator = simulator;
            _tankPlayerInput = tankPlayerInput;
            _levelScript = levelScript;
        }
        
        void OnEnable()
        {
            _progressSlider.onValueChanged.AddListener(SetCurrentSimulationTick);
            _levelScript.OnWin.AddListener(OnLevelWin);
        }

        void OnDisable()
        {
            _progressSlider.onValueChanged.RemoveListener(SetCurrentSimulationTick);
            _levelScript.OnWin.RemoveListener(OnLevelWin);
        }

        void Start()
        {
            _playerTankController = _tankPlayerInput.TankController;
            
            _isPaused = false;
            _currentProgressText.text = "0";
            _maxProgressText.text = "0";

            UpdateUi();
        }

        void Update()
        {
            if (!_playerTankController.IsAlive())
            {
                SetSimulationSpeed(0f);
            }
            
            _progressSlider.interactable = _isPaused;

            _progressSlider.value = (float)_simulator.SimulationTick / _simulator.MaxSimulationTick;

            _currentProgressText.text = _simulator.SimulationTick.ToString();
            _maxProgressText.text = _simulator.MaxSimulationTick.ToString();

            float targetRotation = _arrowRotations[0];
            if (!_pauseButton.interactable)
            {
                targetRotation = _arrowRotations[0];
            }
            else if (!_playButton1.interactable)
            {
                targetRotation = _arrowRotations[1];
            }
            else if (!_playButton2.interactable)
            {
                targetRotation = _arrowRotations[2];
            }
            else if (!_playButton3.interactable)
            {
                targetRotation = _arrowRotations[3];
            }

            _arrow.rotation = Quaternion.Lerp(_arrow.rotation, Quaternion.Euler(0, 0, targetRotation),
                Time.deltaTime * 10);
        }

        public void SetSimulationSpeed(float speed)
        {
            SetSimulationSpeed(speed, false);
        }
        
        public void SetSimulationSpeed(float speed, bool ignoreWin)
        {
            if (ignoreWin || !_levelScript.IsWon)
            {
                _simulator.SimulationSpeed = speed;
                UpdateUi();
            }
        }

        void UpdateUi()
        {
            var speed = _simulator.SimulationSpeed;

            _pauseButton.interactable = !(speed < 0.01f);
            _playButton1.interactable = !(speed is >= 0.01f and < 1f);
            _playButton2.interactable = !(speed is >= 1f and < 2f);
            _playButton3.interactable = !(speed >= 2f);

            _isPaused = speed < 0.01f;
            
            _timeControlPanelAnimator.SetBool(IsVisible, _isPaused && !_levelScript.IsWon);
        }

        void SetCurrentSimulationTick(float progress)
        {
            var tickInt = (int)(progress * _simulator.MaxSimulationTick);
            SetCurrentSimulationTick(tickInt);
        }

        public void SetCurrentSimulationTick(int tick)
        {
            if (!_levelScript.IsWon)
            {
                tick = Mathf.Clamp(tick, 0, _simulator.MaxSimulationTick);
                _simulator.SetCurrentSimulationTick(tick);
            }
        }

        void OnLevelWin()
        {
            SetSimulationSpeed(0f, true);
        }
    }
}