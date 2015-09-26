using UnityEngine;
using SurvivalOfTheAlturist.Creatures;
using SurvivalOfTheAlturist.Environment;
using System.Collections.Generic;

namespace SurvivalOfTheAlturist {

    public class MainController : MonoBehaviour, IController, IReport {

#region Class fields

        private const float MinTimeScale = 0.01f;
        private const float MaxTimeScale = 20f;

        private float prePauseTimeScale;
        private int simulationsRan = 0;
        private int generatorsForSimulationBatchesCount = 0;

#endregion

#region Serialized fields

        [SerializeField]
        private Bounds worldBounds;

        [Header("Controllers")]

        [SerializeField]
        private CreatureController creatureController = null;
        [SerializeField]
        private GroupController groupController = null;
        [SerializeField]
        private EnvironmentController environmentController = null;

        [Header("Properties")]

        [SerializeField]
        private bool runInBackground = false;
        [SerializeField]
        private SimulationReport.SaveType reportSaveType = SimulationReport.SaveType.All;
        [SerializeField]
        private int randomSeedOverride = 0;
        [SerializeField]
        private int simulationMaxTime = 600;
        [SerializeField]
        private int numOfSimulationsToRun = 1;
        [SerializeField]
        private List<EnergyGenerator> generatorsForSimulationBatches = null;

#endregion

#region Properties

        public Bounds WorldBounds { get { return worldBounds; } }

        public CreatureController CreatureController { get { return creatureController; } }

        public EnvironmentController EnvironmentController { get { return environmentController; } }

        public GroupController GroupController { get { return groupController; } }

        public int RandomSeedOverride { get { return randomSeedOverride; } }

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

        public float SimulationsProgress {
            get {
                if (SimulationReport.IsSimulationRunning) {
//                    Debug.LogFormat("simulationsRan = {0}, generatorsForSimulationBatches.Count = {1} generatorsForSimulationBatchesCount = {2}, max = {3}",
//                        simulationsRan, generatorsForSimulationBatches.Count, generatorsForSimulationBatchesCount, numOfSimulationsToRun * generatorsForSimulationBatchesCount);
                    return ((float)simulationsRan + numOfSimulationsToRun * (generatorsForSimulationBatchesCount - generatorsForSimulationBatches.Count - 1)) / (numOfSimulationsToRun * generatorsForSimulationBatchesCount);
                }

                return -1;
            }
        }

#endregion

#region Unity override

        // Use this for initialization
        private void Awake() {
            Init();
            Reset();
        }

        private void FixedUpdate() {
            if (SimulationReport.IsSimulationRunning && SimulationReport.GetCurrentSimulation().CurrentTime > simulationMaxTime) {
                StopSimulation();
            }
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
            Debug.LogWarningFormat("All creatures died");
            StopSimulation();
        }

#endregion

#region IController implementation

        public void Reset() {
            if (randomSeedOverride != 0) {
                UnityEngine.Random.seed = randomSeedOverride;
            }

            if (SimulationReport.IsSimulationRunning) {
                Debug.LogWarning("Simulation timed out");
                StopSimulation();
            }
            SimulationReport.StartSimulationReport(this);

            Debug.Log("Smulation started.");
            Debug.Log("Press 'Space' to pause the current simulation, '+' or '-' to speed up or slow down the simulation.");
            Debug.Log("Press 'S' to see current report, 'O' to save current report, 'R' or 'P' to restart the simulation.");

            environmentController.Reset();
            creatureController.Reset(); // removes all creatures - must be called before "groupController.Reset()"
            groupController.Reset(); // generates all groups and creatures
        }

#endregion

#region IReport implementation

        public string GetReport() {
            return string.Format("Main controller: World bounds: width = {0}, height = {1}", worldBounds.extents.x, worldBounds.extents.y);
        }

#endregion

        public void Init() {
            creatureController.OnAllCreaturesDied = this.OnAllCreaturesDied;
            simulationsRan = 1;
            Application.runInBackground = runInBackground;

            if (generatorsForSimulationBatches != null && generatorsForSimulationBatches.Count > 0) {
                generatorsForSimulationBatchesCount = generatorsForSimulationBatches.Count;
                environmentController.EnergyGenerator = generatorsForSimulationBatches[0];
                generatorsForSimulationBatches.RemoveAt(0);
            }
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
                SimulationReport.SaveToFile(SimulationReport.GetCurrentSimulation(), reportSaveType);
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

        private void StopSimulation() {
            if (SimulationReport.IsSimulationRunning) {
                SimulationReport.SaveToFile(SimulationReport.GetCurrentSimulation(), reportSaveType);
                SimulationReport.StopSimulationReport();
                Debug.Log("Smulation ended. Press 'R' or 'P' to restart the simulation.");
            }

            environmentController.gameObject.SetActive(false);
            creatureController.gameObject.SetActive(false);
            groupController.gameObject.SetActive(false);

            if (numOfSimulationsToRun <= simulationsRan) {
                // if a list of generators is set, use each one once for every simulation batch (as defined by 'numOfSimulationsToRun') run
                if (generatorsForSimulationBatches != null && generatorsForSimulationBatches.Count > 0) {
                    simulationsRan = 1;
                    Reset();
                    environmentController.EnergyGenerator = generatorsForSimulationBatches[0];
                    generatorsForSimulationBatches.RemoveAt(0);
                } else {
                    Debug.Break();
                }
            } else {
                simulationsRan++;
                Reset();
            }
        }
    }

}