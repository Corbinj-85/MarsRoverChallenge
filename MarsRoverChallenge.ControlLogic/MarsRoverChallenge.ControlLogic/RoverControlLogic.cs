using MarsRoverChallenge.ControlLogic.DataModels;
using MarsRoverChallenge.ControlLogic.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MarsRoverChallenge.ControlLogic
{
    public class RoverControlLogic
    {
        private readonly Coordinates _minimumPosition = new Coordinates { XPosition = 0, YPosition = 0 };
        private readonly Coordinates _maximumPosition = new Coordinates { XPosition = 4, YPosition = 4 };
        private int _scuffs = 0;

        private readonly ILogger _logger;

        public RoverControlLogic(ILogger logger)
        {
            _logger = logger;
        }

        public (RoverPosition finalPosition, int scuffs) CalculateRoverPosition(RoverPosition roverPosition, IList<MovementInstruction> instructions)
        {
            // Check inputs and throw exceptions for invalid values.
            ValidateRequest(roverPosition, instructions);

            var instructionNumber = 1;

            foreach (var instruction in instructions)
            {
                switch (instruction)
                {
                    case MovementInstruction.F:
                        // Update coordinates following forward movement.
                        roverPosition.Coordinates = HandleRoverMovement(roverPosition);
                        break;
                    case MovementInstruction.L:
                        // Update face position following left turn.
                        roverPosition.FacePosition = HandleRoverLeftTurn(roverPosition.FacePosition);
                        break;
                    case MovementInstruction.R:
                        // Update face position following right turn.
                        roverPosition.FacePosition = HandleRoverRightTurn(roverPosition.FacePosition);
                        break;
                }

                // Log the current position of the rover following latest instruction.
                _logger.LogInformation($"Rover location after instruction number {instructionNumber}: Coordinates - ({roverPosition.Coordinates.XPosition},{roverPosition.Coordinates.YPosition}); Direction - {roverPosition.FacePosition}; Scuffs - {_scuffs}");
                instructionNumber++;
            }

            // Return final position and number of scuffs following completion of all instructions.
            return (roverPosition, _scuffs);
        }

        private Coordinates HandleRoverMovement(RoverPosition originalPosition)
        {
            var recalculatedCoordinates = new Coordinates
            {
                XPosition = originalPosition.Coordinates.XPosition,
                YPosition = originalPosition.Coordinates.YPosition
            };
            
            // Adjust coordinates according to the current face position.
            switch (originalPosition.FacePosition)
            {
                case FacePosition.N:
                    recalculatedCoordinates.YPosition++;
                    break;
                case FacePosition.E:
                    recalculatedCoordinates.XPosition++;
                    break;
                case FacePosition.S:
                    recalculatedCoordinates.YPosition--;
                    break;
                case FacePosition.W:
                    recalculatedCoordinates.XPosition--;
                    break;
            }

            // Return recalculated position if valid.
            if (CheckRoverPosition(recalculatedCoordinates))
                return recalculatedCoordinates;

            // If recalculated position invalid, add scuff and return the original
            // coordinates, as movement not possible.
            _scuffs++;
            return originalPosition.Coordinates;
        }

        private FacePosition HandleRoverLeftTurn(FacePosition facePosition)
        {
            // Adjusted face position can be calculated by subtracting 1 from the current
            // enum value, except when facing North (as needs to update from 0 to 3).
            if (facePosition == FacePosition.N)
            {
                facePosition = FacePosition.W;
            }
            else
            {
                facePosition--;
            }

            return facePosition;
        }

        private FacePosition HandleRoverRightTurn(FacePosition facePosition)
        {
            // Adjusted face position can be calculated by adding 1 to the current
            // enum value, except when facing West (as needs to update from 3 to 0).
            if (facePosition == FacePosition.W)
            {
                facePosition = FacePosition.N;
            }
            else
            {
                facePosition++;
            }

            return facePosition;
        }

        private bool CheckRoverPosition(Coordinates requestedCoordinates)
        {
            // Check if position is within the min. / max. range.
            if (requestedCoordinates.XPosition < _minimumPosition.XPosition ||
                requestedCoordinates.XPosition > _maximumPosition.XPosition ||
                requestedCoordinates.YPosition < _minimumPosition.YPosition ||
                requestedCoordinates.YPosition > _maximumPosition.YPosition)

                return false;

            return true;
        }

        private void ValidateRequest(RoverPosition startingPosition, IList<MovementInstruction> instructions)
        {
            // Throw exception if starting position is outside of the min. / max. range.
            if (!CheckRoverPosition(startingPosition.Coordinates))
                throw new ArgumentException($"Rover starting coordinates of ({startingPosition.Coordinates.XPosition},{startingPosition.Coordinates.YPosition}) are not valid.");

            // Throw exception if starting face position is an invalid value.
            if (!Enum.IsDefined(typeof(FacePosition), startingPosition.FacePosition))
                throw new ArgumentException($"Rover starting face position of {startingPosition.FacePosition} is not valid.");

            // Throw exception if any of the instructions are invalid.
            if (instructions.Any(x => !Enum.IsDefined(typeof(MovementInstruction), x)))
                throw new ArgumentException($"Rover instructions of '{string.Join(", ",instructions.Select(x => x.ToString()))}' are not valid.");
        }

    }
}
