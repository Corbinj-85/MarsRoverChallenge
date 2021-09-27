using Xunit;
using FluentAssertions;
using MarsRoverChallenge.ControlLogic.DataModels;
using MarsRoverChallenge.ControlLogic.Enums;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Logging;
using Moq;

namespace MarsRoverChallenge.ControlLogic.Tests
{
    public class RoverControlLogicTests
    {
        private readonly Mock<ILogger> _mockLogger = new Mock<ILogger>();

        [Theory]
        [MemberData(nameof(RoverTestData))]
        // Test that Rover movements are handled correctly for given scenarios.
        public void Given_AStartingPosition_AndMovementInstructions_WhenCalculatingRoverPosition_ItShouldReturnExpectedFinalPositionAndNumberOfScuffs(
            RoverPosition startingPosition, List<MovementInstruction> instructions, RoverPosition expectedFinalPosition, int expectedScuffs)
        {
            // Arrange
            var controlLogic = new RoverControlLogic(_mockLogger.Object);

            // Act
            var (actualFinalPosition, actualScuffs) = controlLogic.CalculateRoverPosition(startingPosition, instructions);

            // Assert
            expectedFinalPosition.Should().BeEquivalentTo(actualFinalPosition);
            expectedScuffs.Should().Be(actualScuffs);
        }

        
        [Fact]
        // Test exception thrown when using invalid starting coordinates.
        public void Given_AnInvalidStartingCoordinate_WhenCalculatingRoverPosition_ExceptionIsThrown()
        {       
            // Arrange
            var startingPosition = new RoverPosition
            {
                Coordinates = new Coordinates
                {
                    XPosition = 0,
                    YPosition = 5
                },
                FacePosition = FacePosition.N
            };

            var instructions = new List<MovementInstruction>();

            var controlLogic = new RoverControlLogic(_mockLogger.Object);

            // Act
            Action calculateRoverPosition = () =>
            {
                controlLogic.CalculateRoverPosition(startingPosition, instructions);
            };

            // Assert
            calculateRoverPosition
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("Rover starting coordinates of (0,5) are not valid.");
        }

        [Fact]
        // Test exception thrown when using invalid starting face position.
        public void Given_AnInvalidStartingFacePosition_WhenCalculatingRoverPosition_ExceptionIsThrown()
        {
            // Arrange
            var startingPosition = new RoverPosition
            {
                Coordinates = new Coordinates
                {
                    XPosition = 0,
                    YPosition = 2
                },
                FacePosition = (FacePosition)4
            };

            var instructions = new List<MovementInstruction>();

            var controlLogic = new RoverControlLogic(_mockLogger.Object);

            // Act
            Action calculateRoverPosition = () =>
            {
                controlLogic.CalculateRoverPosition(startingPosition, instructions);
            };

            // Assert
            calculateRoverPosition
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("Rover starting face position of 4 is not valid.");
        }

        [Fact]
        // Test exception thrown when using an invalid movement instruction.
        public void Given_AnInvalidInstruction_WhenCalculatingRoverPosition_ExceptionIsThrown()
        {
            // Arrange
            var startingPosition = new RoverPosition
            {
                Coordinates = new Coordinates
                {
                    XPosition = 0,
                    YPosition = 2
                },
                FacePosition = FacePosition.N
            };

            var instructions = new List<MovementInstruction>()
            {
                MovementInstruction.F,
                MovementInstruction.L,
                MovementInstruction.R,
                (MovementInstruction)5,
                MovementInstruction.F
            };

            var controlLogic = new RoverControlLogic(_mockLogger.Object);

            // Act
            Action calculateRoverPosition = () =>
            {
                controlLogic.CalculateRoverPosition(startingPosition, instructions);
            };

            // Assert
            calculateRoverPosition
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("Rover instructions of 'F, L, R, 5, F' are not valid.");
        }

        public static TheoryData<RoverPosition, List<MovementInstruction>, RoverPosition, int> RoverTestData
        {
            get
            {
                var roverTestData = new TheoryData<RoverPosition, List<MovementInstruction>, RoverPosition, int>();

                // Scenario One
                roverTestData.Add(
                    new RoverPosition
                    {
                        Coordinates = new Coordinates
                        {
                            XPosition = 0,
                            YPosition = 2
                        },
                        FacePosition = FacePosition.E
                    },
                    new List<MovementInstruction>()
                    {
                        MovementInstruction.F,
                        MovementInstruction.L,
                        MovementInstruction.F,
                        MovementInstruction.R,
                        MovementInstruction.F,
                        MovementInstruction.F,
                        MovementInstruction.F,
                        MovementInstruction.R,
                        MovementInstruction.F,
                        MovementInstruction.F,
                        MovementInstruction.R,
                        MovementInstruction.R
                    },
                    new RoverPosition
                    {
                        Coordinates = new Coordinates
                        {
                            XPosition = 4,
                            YPosition = 1
                        },
                        FacePosition = FacePosition.N
                    },
                    0);

                // Scenario Two
                roverTestData.Add(
                    new RoverPosition
                    {
                        Coordinates = new Coordinates
                        {
                            XPosition = 4,
                            YPosition = 4
                        },
                        FacePosition = FacePosition.S
                    },
                    new List<MovementInstruction>()
                    {
                        MovementInstruction.L,
                        MovementInstruction.F,
                        MovementInstruction.L,
                        MovementInstruction.L,
                        MovementInstruction.F,
                        MovementInstruction.F,
                        MovementInstruction.L,
                        MovementInstruction.F,
                        MovementInstruction.F,
                        MovementInstruction.F,
                        MovementInstruction.R,
                        MovementInstruction.F,
                        MovementInstruction.F
                    },
                    new RoverPosition
                    {
                        Coordinates = new Coordinates
                        {
                            XPosition = 0,
                            YPosition = 1
                        },
                        FacePosition = FacePosition.W
                    },
                    1);

                // Scenario Three
                roverTestData.Add(
                    new RoverPosition
                    {
                        Coordinates = new Coordinates
                        {
                            XPosition = 2,
                            YPosition = 2
                        },
                        FacePosition = FacePosition.W
                    },
                    new List<MovementInstruction>()
                    {
                        MovementInstruction.F,
                        MovementInstruction.L,
                        MovementInstruction.F,
                        MovementInstruction.L,
                        MovementInstruction.F,
                        MovementInstruction.L,
                        MovementInstruction.F,
                        MovementInstruction.R,
                        MovementInstruction.F,
                        MovementInstruction.R,
                        MovementInstruction.F,
                        MovementInstruction.R,
                        MovementInstruction.F,
                        MovementInstruction.R,
                        MovementInstruction.F
                    },
                    new RoverPosition
                    {
                        Coordinates = new Coordinates
                        {
                            XPosition = 2,
                            YPosition = 2
                        },
                        FacePosition = FacePosition.N
                    },
                    0);

                // Scenario Four
                roverTestData.Add(
                    new RoverPosition
                    {
                        Coordinates = new Coordinates
                        {
                            XPosition = 1,
                            YPosition = 3
                        },
                        FacePosition = FacePosition.N
                    },
                    new List<MovementInstruction>()
                    {
                        MovementInstruction.F,
                        MovementInstruction.F,
                        MovementInstruction.L,
                        MovementInstruction.F,
                        MovementInstruction.F,
                        MovementInstruction.L,
                        MovementInstruction.F,
                        MovementInstruction.F,
                        MovementInstruction.F,
                        MovementInstruction.F,
                        MovementInstruction.F
                    },
                    new RoverPosition
                    {
                        Coordinates = new Coordinates
                        {
                            XPosition = 0,
                            YPosition = 0
                        },
                        FacePosition = FacePosition.S
                    },
                    3);

                return roverTestData;
            }
        }
    }
}
