namespace Assets.Scripts.AIHelper
{
    using System.Collections;
    using Pathfinding;
    using UnityEngine;

    /// <summary>
    /// The graph builder scans the given area and generates a grid graph for the ai
    /// </summary>
    public sealed class GraphBuilder : MonoBehaviour
    {
        /// <summary>
        /// Stores the last instanciated GraphBuilder for later updates (section change etc.)
        /// </summary>
        private static GraphBuilder _instance;

        /// <summary>
        /// currentliy unused, repeat mapscanning if running towards a waypoint
        /// </summary>
        private float _mapScanRate = 5f;

        /// <summary>
        /// storage temp the last scan time
        /// </summary>
        private float _lastMapScanning = -9999;

        /// <summary>
        /// currently waiting for map scan, so don't start a new scan
        /// </summary>
        private bool _isRepeatingMapScanning = false;

        /// <summary>
        /// the astar component short cut
        /// </summary>
        private AstarPath _path;

        /// <summary>
        /// the current grid graph for ai
        /// </summary>
        private GridGraph _gridGraph;

        /// <summary>
        /// indicates if graph builder is already trying to rescan
        /// </summary>
        private bool _alreadyTrying;

        /// <summary>
        /// static getter for istance
        /// </summary>
        /// <returns>the graph builder instance</returns>
        public static GraphBuilder GetInstance()
        {
            return _instance;
        }

        /// <summary>
        /// called at the first frame
        /// </summary>
        public void Start()
        {
            _path = GetComponent<AstarPath>();
            _path.logPathResults = PathLog.None;

            _gridGraph = (GridGraph)_path.graphs[0];
            _instance = this;
        }

        /// <summary>
        /// scan map for target if current target is a waypoint
        /// </summary>
        public void TryMapScanning()
        {
            if (GameManager.GetInstance().CurrentSection == null)
            {
                return;
            }

            if (_gridGraph == null)
            {
                StartCoroutine(TryRescan());
            }
            else
            {
                Debug.Log("[GraphBuilder.cs] Rescan graph");

                Section section = GameManager.GetInstance().CurrentSection;

                _gridGraph.nodeSize = 0.25f;
                _gridGraph.center = section.transform.position - (Vector3.up * 15);
                _gridGraph.unclampedSize = new Vector2(section.TerrainSize.x * 1.8f, section.TerrainSize.z);

                _path.Scan();
            }
        }

        /// <summary>
        /// rescane courutine
        /// </summary>
        /// <returns>waiting time for coroutine</returns>
        public IEnumerator TryRescan()
        {
            if (!_alreadyTrying)
            {
                _alreadyTrying = true;
                yield return new WaitForSeconds(0.3f);
                _alreadyTrying = false;

                TryMapScanning();
            }
        }
    }
}
