using UnityEngine;
using SurvivalOfTheAlturist.Creatures;
using SurvivalOfTheAlturist.Environment;

namespace SurvivalOfTheAlturist {

    public class MainController : MonoBehaviour, IController {

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
        private GroupController groupController = null;
        [SerializeField]
        private EnvironmentController environmentController = null;

        [Header("Properties")]

        [SerializeField]
        private int randomSeedOverride = 0;
        [SerializeField]
        private int simulationMaxTime = 600;

#endregion

#region Properties

        public Bounds WorldBounds { get { return worldBounds; } }

        public CreatureController CreatureController { get { return creatureController; } }

        public EnvironmentController EnvironmentController { get { return environmentController; } }

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

        public int RandomSeedOverride { get { return randomSeedOverride; } }

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
            SimulationReport.StartSimulationReport();

            environmentController.Reset();
            creatureController.Reset(); // removes all creatures - must be called before "groupController.Reset()"
            groupController.Reset(); // generates all groups and creatures
        }

#endregion

        public void Init() {
            creatureController.OnAllCreaturesDied = this.OnAllCreaturesDied;
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

        private void StopSimulation() {
            if (SimulationReport.IsSimulationRunning) {
                SimulationReport.SaveToFile(SimulationReport.StopSimulationReport());
                Debug.Log("Smulation ended. Press 'R' or 'P' to restart the simulation.");
            }

            environmentController.gameObject.SetActive(false);
            creatureController.gameObject.SetActive(false);
            groupController.gameObject.SetActive(false);
        }
    }

}