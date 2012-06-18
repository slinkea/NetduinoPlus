using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using SecretLabs.NETMF.Hardware;
using System.Threading;

namespace Netduino.Home.Controller
{
    class WindowShade : IDisposable
    {
        #region Private Variables

        public enum ShadeCommand
        {
            Open,
            Close,
            Stop
        };

        public enum ShadeState
        {
            Stopped,
            Opened,
            Closed,
            Opening,
            Closing,
            Unknown
        };

        // Output Pins
        private OutputPort _open = new OutputPort(Pins.GPIO_PIN_D3, false);
        private OutputPort _close = new OutputPort(Pins.GPIO_PIN_D4, false);

        // Input Pins
        private InterruptPort _limitUp = new InterruptPort(Pins.GPIO_PIN_D7, true, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeHigh);
        private InterruptPort _limitDown = new InterruptPort(Pins.GPIO_PIN_D6, true, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeHigh);

        // Private Variables
        private ShadeState _state = ShadeState.Unknown;
        private int _key = 0;
        private int _position = 0; //top = 0%, bottom = 100%

        private const int TOTAL_MILLISECONDS_TO_MOVING_UP = 33000;
        private const int TOTAL_MILLISECONDS_TO_MOVING_DOWN = 51000;

        private Timer _timer = null;

        #endregion


        #region Constructors

        public WindowShade()
        {
            _limitUp.OnInterrupt += new NativeEventHandler(OnShadeReachLimitUp);
            _limitDown.OnInterrupt += new NativeEventHandler(OnShadeReachLimitDown);
        }

        #endregion


        #region Public Properties

        public ShadeState State
        {
            get
            {
                return _state;
            }
            set 
            {
                _state = value;

                SendState();
            }
        }

        public int Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }

        #endregion


        #region Implement Interface

        public void Dispose()
        {
            Stop();
        }

        #endregion


        #region Public Methods

        public bool Execute(ShadeCommand command)
        {
            bool valid = IsValidCommand(command);

            if (valid)
            {
                if (command == ShadeCommand.Stop)
                {
                    Stop();

                    this.State = ShadeState.Stopped;
                }
                else if (command == ShadeCommand.Open)
                {
                    Open();

                    this.State = ShadeState.Opening;

                    StartTimer(TOTAL_MILLISECONDS_TO_MOVING_UP);
                }
                else if (command == ShadeCommand.Close)
                {
                    Close();

                    this.State = ShadeState.Closing;

                    StartTimer(TOTAL_MILLISECONDS_TO_MOVING_DOWN);
                }
            }

            return valid;
        }

        #endregion


        #region Private Methods

        private void Stop()
        {
            if (this.State == ShadeState.Opening)
            {
                _open.Write(false);
            }
            else if (this.State == ShadeState.Closing)
            {
                _close.Write(false);
            }
        }

        private void Open()
        {
            _open.Write(true);
        }

        private void Close()
        {
            _close.Write(true);
        }

        private bool IsValidCommand(ShadeCommand command)
        {
            return ((this.State == ShadeState.Unknown) ||
                    ((this.State == ShadeState.Opened) && command == ShadeCommand.Close) || 
                    ((this.State == ShadeState.Closed) && command == ShadeCommand.Open) ||
                    ((this.State == ShadeState.Stopped) && command != ShadeCommand.Stop)) ||
                    ((this.State == ShadeState.Opening || this.State == ShadeState.Closing) && (command == ShadeCommand.Stop));
        }

        private void StartTimer(int runInMilliseconds)
        {
            _timer = new Timer(new TimerCallback(PercentTrig), "Some data", 0, runInMilliseconds);
        }

        private void PercentTrig(object data)
        {
            _position++;

            if (_position == 100)
            {
                _timer.Dispose();
            }
        }

        private void StopTimeOut()
        {
            int elapsedMilliseconds = -1;
            int remainingMilliseconds = -1;

            if (Time.GetInfo(_key, elapsedMilliseconds, remainingMilliseconds))
            {
                //if (elapsedMilliseconds < 0 || elapsedMilliseconds > )
                //{

                //}
                _position = elapsedMilliseconds / (elapsedMilliseconds + remainingMilliseconds);
            }

            Time.Remove(_key);
        }

        private int GetRemainingTimeOut()
        {
            int timeOut = 0;

            if (this.State == ShadeState.Opening)
            {
                timeOut = TOTAL_MILLISECONDS_TO_MOVING_UP * (_position / 100);
            }
            else if (this.State == ShadeState.Closing)
            {
                timeOut = TOTAL_MILLISECONDS_TO_MOVING_DOWN - (TOTAL_MILLISECONDS_TO_MOVING_DOWN * (_position / 100));
            }

            return timeOut;
        }

        private void KillTimeOut()
        {
            Time.Remove(_key);
        }

        private void OnMovingShadeTimeOut()
        {
            Stop();

            KillTimeOut();

            if (this.State == ShadeState.Opening)
            {
                this.State = ShadeState.Opened;
            }
            else if (this.State == ShadeState.Closing)
            {
                this.State = ShadeState.Closed;
            }
        }

        private void OnShadeReachLimitUp(uint port, uint data, DateTime time)
        {
            if (this.State == ShadeState.Opening)
            {
                Stop();

                this.State = ShadeState.Opened;
            }
        }

        private void OnShadeReachLimitDown(uint port, uint data, DateTime time)
        {          
            if (this.State == ShadeState.Closing)
            {
                Stop();

                this.State = ShadeState.Closed;
            }
        }

        private void SendState()
        {
            string message = "Window Shade: ";

            switch (_state)
            {
                case ShadeState.Stopped:
                    message += "Stopped";
                    break;

                case ShadeState.Opened:
                    message += "Opened";
                    break;

                case ShadeState.Closed:
                    message += "Closed";
                    break;

                case ShadeState.Opening:
                    message += "Opening";
                    break;

                case ShadeState.Closing:
                    message += "Closing";
                    break;

                default:
                    message = "Unknown state";
                    break;
            }

            EthernetCommunication.SendMessage(message);
        }

        #endregion
    }
}
