﻿using UnityEngine;
using System.Collections;
using SurvivalOfTheAlturist.Creatures;
using SurvivalOfTheAlturist.Environment;
using AssemblyCSharp;

namespace SurvivalOfTheAlturist {

    public class MainController : MonoBehaviour {

#region Class fields

        private const float MinTimeScale = 0.01f;
        private const float MaxTimeScale = 20f;

        private float prePauseTimeScale;

#endregion

#region Serialized fields

        [SerializeField]
        private Bounds worldBounds;

        [Header("Controllers")]

        [SerializeField]
        private CreatureController creatureController = null;
        [SerializeField]
        private EnvironmentController environmentController = null;

#endregion

#region Properties

        public Bounds WorldBounds { get { return worldBounds; } }

        public float TimeScale {
            get {
                return Time.timeScale;
            }
            set {
                Time.timeScale = value;
                Debug.LogFormat("TimeScale = {0}", value);
            }
        }

        public bool TimePause {
            get { return TimeScale == 0; }
            set {
                if (value) {
                    prePauseTimeScale = TimeScale;
                    TimeScale = 0;
                } else {
                    TimeScale = prePauseTimeScale;
                }
            }
        }

#endregion

#region Unity override

        // Use this for initialization
        private void Awake() {
            Init();
            Reset();
        }
    
        // Update is called once per frame
        private void Update() {
            UpdateInput();
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.gray;
            Gizmos.DrawWireCube(worldBounds.center, worldBounds.extents * 2);
        }

#endregion

#region Delegates

        private void OnAllCreaturesDied() {
            Simulation simulation = SimulationReport.StopSimulationReport();
            Debug.LogWarningFormat("All creatures died - simulation report: \n{0}", simulation.GetReport());
            Debug.Log("Smulation ended. Press 'R' or 'P' to restart the simulation.");
            SimulationReport.SaveToFile(simulation);
        }

#endregion

        public void Init() {
            creatureController.OnAllCreaturesDied = this.OnAllCreaturesDied;
        }

        public void Reset() {
            if (SimulationReport.IsSimulationRunning) {
                SimulationReport.PrintCurrentReport();
                SimulationReport.StopSimulationReport();
            }
            SimulationReport.StartSimulationReport();
            
            environmentController.Reset();
            creatureController.Reset();
        }

        public Vector3 GetRandomWorldPosition() {
            float xMin = worldBounds.min.x;
            float xMax = worldBounds.max.x;
            float yMin = worldBounds.min.y;
            float yMax = worldBounds.max.y;

            Vector3 position = new Vector3();
            position.x = Random.Range(xMin, xMax);
            position.y = Random.Range(yMin, yMax);

            return position;
        }

        public void RemoveEnvironmentObject(EnvironmentObject envObj) {
            environmentController.RemoveEnvironmentObject(envObj);
        }

        private void UpdateInput() {
            if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.P)) {
                Reset();
            } else if (Input.GetKeyDown(KeyCode.S)) {
                SimulationReport.PrintCurrentReport();
            } else if (Input.GetKeyDown(KeyCode.O)) {
                SimulationReport.SaveToFile(SimulationReport.GetCurrentSimulation());
            } else if (Input.GetKeyDown(KeyCode.Space)) {
                TimePause = !TimePause;
            } else if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus)) {
                TimeSlowDown();
            } else if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus)) {
                TimeSpeedUp();
            }
        }

        public void TimeSlowDown() {
            if (TimePause) {
                return;
            }

            if (TimeScale < MinTimeScale) {
                TimePause = true;
                return;
            }

            TimeScale *= 0.5f;
        }

        public void TimeSpeedUp() {
            if (TimePause) {
                TimeScale = MinTimeScale;
//                return;
            }

            TimeScale *= 1.5f;
            
            if (TimeScale > MaxTimeScale) {
                TimeScale = MaxTimeScale;
            }
        }

    }

}