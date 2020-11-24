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

namespace Microwave.Test.Unit
{
    class Step3
    {
        private Light uut;
        private Output output;
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
            output = new Output();
            display = new Display(output);
            uut = new Light(output);
            userInterface = new UserInterface(_powerButton,_timeButton,_startCancelButton,_door,display, uut,_ckctrl);
            
        }

        [Test]
        public void door_close_light_off()
        {
            bool notified = false; 
            _door.Opened += (sender, args) => notified= true;
            output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains("on")));


        }
    }
}
