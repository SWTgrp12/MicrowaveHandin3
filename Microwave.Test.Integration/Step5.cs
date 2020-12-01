using System;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;

using NUnit.Framework;
using NSubstitute;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class IT_Step5
    {
        // Modules needed
        public CookController _uut;
        public IOutput _output;
        public IDisplay _display;
        public IPowerTube _powerTube;
        public ILight _light;
        public IUserInterface _userInterface;

        // Substitute
        public ITimer _timer;
        public IButton _powerButton;
        public IButton _timeButton;
        public IButton _startCancelButton;
        public IDoor _door;


        [SetUp]
        public void setup()
        {
            // Substitutes
            _timer = Substitute.For<ITimer>();
            _output = Substitute.For<IOutput>();
            _powerButton = Substitute.For<IButton>();
            _timeButton = Substitute.For<IButton>();
            _startCancelButton = Substitute.For<IButton>();
            _door = Substitute.For<IDoor>();

            _light = new Light(_output);
            _display = new Display(_output);
            _powerTube = new PowerTube(_output);
            _uut = new CookController(_timer, _display, _powerTube);
            _userInterface = new UserInterface(
                _powerButton, _timeButton, _startCancelButton,
                _door,
                _display,
                _light,
                _uut);
            _uut.UI = _userInterface;
        }

        [TestCase(1, 1)]
        public void StartCooking_PowerTubeOn(int powerPressed, int timerPressed)
        {
            int power = 50;
            _powerButton.Pressed += Raise.Event();
            for (int i = 0; i < powerPressed; i++)
            {
                _powerButton.Pressed += Raise.Event();
                power += 50;
            }

            int time = 60;
            _timeButton.Pressed += Raise.Event();
            for (int i = 0; i < timerPressed; i++)
            {
                _timeButton.Pressed += Raise.Event();
                time += 60;
            }

            _startCancelButton.Pressed += Raise.Event();
            _output.Received().OutputLine($"Light is turned on");
            _timer.ReceivedWithAnyArgs().Start(time);
            _output.Received().OutputLine($"PowerTube works with {power}");
        }

        [TestCase(12, 12)]
        [TestCase(0, 0)]
        [TestCase(4, 8)]
        public void StartCooking_StartCancelButton_(int powerPressed, int timerPressed)
        {
            int power = 50;
            _powerButton.Pressed += Raise.Event();
            for (int i = 0; i < powerPressed; i++)
            {
                _powerButton.Pressed += Raise.Event();
                power += 50;
            }

            int time = 60;
            _timeButton.Pressed += Raise.Event();
            for (int i = 0; i < timerPressed; i++)
            {
                _timeButton.Pressed += Raise.Event();
                time += 60;
            }

            _startCancelButton.Pressed += Raise.Event();
            _output.Received().OutputLine($"Light is turned on");
            _timer.ReceivedWithAnyArgs().Start(time);
            _output.Received().OutputLine($"PowerTube works with {power}");
        }

        [Test]
        public void StopCooking_OnTimerExpired()
        {
            // Start Cooking
            _startCancelButton.Pressed += Raise.Event();
            _output.Received().OutputLine($"Light is turned on");
            _timer.ReceivedWithAnyArgs().Start(60);
            _output.Received().OutputLine($"PowerTube works with {50}");

            _timer.Expired += Raise.Event();
            _output.Received().OutputLine($"PowerTube turned off");
            _output.Received().OutputLine($"Display cleared");
            _output.Received().OutputLine($"Light is turned off");
        }

        [Test]
        public void StopCooking_OnDoorOpened()
        {
            // Start Cooking
            _startCancelButton.Pressed += Raise.Event();
            _output.Received().OutputLine($"Light is turned on");
            _timer.ReceivedWithAnyArgs().Start(60);
            _output.Received().OutputLine($"PowerTube works with {50}");

            // Door Opens, stop cooking.
            _door.Opened += Raise.Event();
            _timer.Received().Stop();
            _output.Received().OutputLine($"PowerTube turned off");
            _output.Received().OutputLine($"Display cleared");
        }

        [TestCase(12, 12)]
        public void StopCooking_StartCancelButton(int powerPressed, int timerPressed)
        {
            int power = 50;
            for (int i = 0; i < powerPressed; i++)
            {
                _powerButton.Pressed += Raise.Event();
                power += 50;
            }

            int time = 60;
            for (int i = 0; i < timerPressed; i++)
            {
                _timeButton.Pressed += Raise.Event();
                time += 60;
            }
            // Start Cooking
            _startCancelButton.Pressed += Raise.Event();
            _output.Received().OutputLine($"Light is turned on");
            _timer.ReceivedWithAnyArgs().Start(time);
            _output.Received().OutputLine($"PowerTube works with {power}");

            // Cooking is now On. Stop with StartButton
            _startCancelButton.Pressed += Raise.Event();
            _timer.Received().Stop();
            _output.Received().OutputLine($"PowerTube turned off");
            _output.Received().OutputLine($"Display cleared");
            _output.Received().OutputLine($"Light is turned off");
        }
    }
}
