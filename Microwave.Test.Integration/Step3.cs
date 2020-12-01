using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NSubstitute.Core.Arguments;
using NUnit.Framework;

namespace Microwave.Test.Integration
{
    class Step3
    {
        private Light uut;
        private IOutput output;
        private Display display;
        private UserInterface userInterface;
        private ICookController _ckctrl;
        private IButton _startCancelButton;
        private IButton _powerButton;
        private IButton _timeButton;
        private IDoor _door;

        [SetUp]
        public void Setup()
        {
            _door = Substitute.For<IDoor>();
            _startCancelButton = Substitute.For<IButton>();
            _powerButton = Substitute.For<IButton>();
            _timeButton = Substitute.For<IButton>();
            _ckctrl = Substitute.For<ICookController>();
            output = Substitute.For<IOutput>(); // used for testing output strings
            display = new Display(output);
            uut = new Light(output);
            userInterface = new UserInterface(_powerButton,_timeButton,_startCancelButton,_door,display, uut,_ckctrl);
            
        }

        [Test]
        public void door_gets_opened_light_turns_on_state_ready()
        {
            _door.Opened += Raise.EventWith(this, EventArgs.Empty); // closed per default so needs to be opened
            output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains("on")));

        }

        [Test]
        public void door_gets_closed_light_turns_off_state_ready()
        {
            _door.Opened += Raise.EventWith(this, EventArgs.Empty);// cant close something that is already closed
            _door.Closed += Raise.EventWith(this, EventArgs.Empty);
            output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains("off")));

        }

    }
}
