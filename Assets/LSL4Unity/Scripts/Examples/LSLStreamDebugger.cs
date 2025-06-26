using UnityEngine;
using LSL;

namespace Assets.LSL4Unity.Scripts.AbstractInlets
{

    public class LSLStreamDebugger : MonoBehaviour
    {
        private int streamsCount = 0;
        
        public int StreamsCount { get { return streamsCount; } }

        public void Awake()
        {
            var results = LSL.liblsl.resolve_streams(1.0);
            Debug.Log($"Streams found: {results.Length}");
            streamsCount = results.Length;
            foreach (var info in results)
            {
                Debug.Log($"Stream Name: {info.name()}, Type: {info.type()}, Source ID: {info.source_id()}");
            }
        }
    }
}
