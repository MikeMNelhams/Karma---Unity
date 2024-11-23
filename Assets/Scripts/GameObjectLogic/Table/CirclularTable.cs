using System;
using UnityEngine;

namespace PlayTable
{
    public class CirclularTable : MonoBehaviour
    {
        [SerializeField] Vector3 _centre;
        [SerializeField] float _radius;
        [SerializeField] float _tabletopHeight;

        public Vector3 Centre { get => _centre; }

        public Vector3[] PlayerPositions(int numberOfPlayers, float distanceFromTable = 0.5f)
        {
            Vector3 forward = new (_radius + distanceFromTable, 0, 0);

            Vector3[] positions = new Vector3[numberOfPlayers];

            float stepSize = -360.0f / numberOfPlayers;

            for (int i = 0; i < numberOfPlayers; i++)
            {
                positions[i] = _centre + Quaternion.AngleAxis(stepSize * (i - 1), Vector3.up) * forward;
                positions[i].y = 0; 
            }
            return  positions;
        }

        public Vector3[] PlayerKarmaPositions(int numberOfPlayers, float ratioAlongRadius = 0.75f)
        {
            if (ratioAlongRadius < 0 || ratioAlongRadius > 1) 
            { 
                throw new NotSupportedException("Ratios outside of the table are not supported!"); 
            }

            Vector3 forward = new(ratioAlongRadius * _radius, 0, 0);

            Vector3[] positions = new Vector3[numberOfPlayers];

            float stepSize = -360.0f / numberOfPlayers;

            for (int i = 0; i < numberOfPlayers; i++)
            {
                positions[i] = _centre + Quaternion.AngleAxis(stepSize * (i - 1), Vector3.up) * forward;
                positions[i].y = _tabletopHeight;
            }

            return positions;
        }

        public Quaternion[] PlayerKarmaRotations(int numberOfPlayers, Vector3[] positions)
        {
            Quaternion[] rotations = new Quaternion[numberOfPlayers];

            float stepSize = -360.0f / numberOfPlayers;

            for (int i = 0; i < numberOfPlayers; i++)
            {
                Vector3 lookDirection = _centre - positions[i];
                lookDirection.y = 0;

                rotations[i] = Quaternion.LookRotation(lookDirection);
            }

            return rotations;
        }
    }
}