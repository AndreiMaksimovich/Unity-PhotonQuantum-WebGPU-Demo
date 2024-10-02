using System;
using UnityEngine;

namespace Amax.QuantumDemo
{
    public class AnimatedRobotKyleGenerator : GameSingleton<AnimatedRobotKyleGenerator>
    {
        [SerializeField] private int initialRobotRowCount = 1;
        [SerializeField] private Vector3 startPosition = Vector3.zero;
        [SerializeField] private int robotsPerRow = 10;
        [SerializeField] private float distanceBetweenRobots = 2f;
        [SerializeField] private float distanceBetweenRows = 2f;
        [SerializeField] private GameObject robotPrefab;
        [SerializeField] private Transform robotRoot;
        
        public int RobotCount {get; private set;}
        public int RobotRowCount { get; private set; }

        private void Start()
        {
            GenerateRobots(initialRobotRowCount);
        }

        public void UpdateRobotCount(int rowCount)
        {
            GenerateRobots(Mathf.Clamp(rowCount, 0, 100));
        }

        public void GenerateRobots(int rows)
        {
            RobotRowCount = rows;
            RobotCount = rows * robotsPerRow;
            Clear();

            for (var row = 0; row < rows; row++)
            {
                var zPosition = row * distanceBetweenRows;
                for (var column = 0; column < robotsPerRow; column++)
                {
                    var xPosition = (-robotsPerRow/2f + 0.5f  + column) * distanceBetweenRobots;
                    var robot = Instantiate(robotPrefab, robotRoot);
                    robot.transform.position = new Vector3(xPosition, 0, zPosition);
                }
            }
        }

        private void Clear()
        {
            foreach (Transform transform in robotRoot)
            {
                Destroy(transform.gameObject);
            }
        }
    }
}