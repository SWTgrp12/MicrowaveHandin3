using System;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;

using NUnit.Framework;
using NSubstitute;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class IT_Step2
    {
        // Modules needed
        public ICookController _uut;
        public IOutput _output;
        public IDisplay _display;
        public IPowerTube _powerTube;


        public IUserInterface _userInterface;
        public ITimer _timer;
        

        [SetUp]
        public void setup()
        {
            _timer = Substitute.For<ITimer>();
            _userInterface = Substitute.For<IUserInterface>();
            _output = Substitute.For<IOutput>();

            _display = new Display(_output);
            _powerTube = new PowerTube(_output);
            _uut = new CookController(_timer, _display,_powerTube,_userInterface);
        }

        [TestCase(12, 12)]
        public void StartCooking_PowerTubeOn(int power, int timer)
        {
            _uut.StartCooking(power, timer);
            _timer.Received().Start(timer);
            _output.Received().OutputLine($"PowerTube works with {power}");
        }

        [TestCase(12, 12)]
        public void Stop_PowertubeOff(int power, int timer)
        {
            _uut.StartCooking(power, timer);
            _output.Received().OutputLine($"PowerTube works with {power}");
            _uut.Stop();
            _timer.Received().Stop();
            _output.Received().OutputLine($"PowerTube turned off");
        }

        [TestCase(12,12)]
        [TestCase(12, 122)]
        public void StartCooking_TimerOnTickUpdatesDisplay(int power, int timer)
        {
            _uut.StartCooking(power, timer);
            _timer.Received().Start(timer);
            int timeleft = timer - 1;
            _timer.TimeRemaining.Returns(timeleft);
            _timer.TimerTick += Raise.Event();
            int min = timeleft / 60;
            int sec = timeleft % 60;
            _output.Received().OutputLine($"Display shows: {min:D2}:{sec:D2}");
        }

        [TestCase(12, 12)]
        public void StartCooking_TimerExpired(int power, int timer)
        {
            _uut.StartCooking(power, timer);
            _timer.Received().Start(timer);
            int timeleft = timer - 1;
            _timer.Expired += Raise.Event();
            _output.Received().OutputLine($"PowerTube turned off");
            _userInterface.Received().CookingIsDone();

        }
    }
}
