using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1Phase2Behavior : MonoBehaviour
{
    public GameObject arcSegment;

    [System.Serializable]
    public struct RingData
    {
        public float radius;
        public float thickness;
        public float angleSize;
        public int numSegments;
        public int resolutionPerSegment; 
    }

    private enum BehaviorState { Arena}
    private BehaviorState _state = BehaviorState.Arena;

    [Header("Outer Ring")]
    public Transform outerRing;
    public RingData outerRingData;
    public float outerRingArenaSpinSpeed = 2f;

    [Header("Inner Ring")]
    public Transform innerRing;
    public RingData innerRingData;
    public float innerRingArenaSpinSpeed = -2f;


    // Start is called before the first frame update
    void Start()
    {
        // Outer ring
        SpawnRing(outerRing, outerRingData);

        // Inner ring
        SpawnRing(innerRing, innerRingData);
    }

    // Update is called once per frame
    void Update()
    {
        Behavior();
        Transition();
        
    }

    private void Behavior()
    {
        switch (_state)
        {
            default:
            case BehaviorState.Arena:
                SpinRing(outerRing, outerRingArenaSpinSpeed);
                SpinRing(innerRing, innerRingArenaSpinSpeed);
                break;

        }
    }
    private void Transition()
    {
        switch (_state)
        {
            default:
            case BehaviorState.Arena:
                break;
        }
    }

    private void SpinRing(Transform ring, float speed)
    {
        ring.rotation *= Quaternion.Euler(0f, 0f, speed);
    }

    private void SpawnRing(Transform parent, RingData ringData)
    {
        float segmentAngle = ringData.angleSize / ringData.numSegments;
        float currAngle = 0f;
        for (int i = 0; i < ringData.numSegments; i++)
        {
            GameObject arc = Instantiate(arcSegment, parent);
            CircleSegment segment = arc.GetComponent<CircleSegment>();
            segment.segments = ringData.resolutionPerSegment;
            segment.startAngle = currAngle;
            segment.endAngle = currAngle + segmentAngle;
            segment.innerRadius = ringData.radius - ringData.thickness * 0.5f;
            segment.outerRadius = ringData.radius + ringData.thickness * 0.5f;

            currAngle += segmentAngle;
        }
    }

    private void OnDrawGizmos()
    {
        // Ring
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, outerRingData.radius + 0.5f * outerRingData.thickness);
        Gizmos.DrawWireSphere(transform.position, outerRingData.radius - 0.5f * outerRingData.thickness);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, innerRingData.radius + 0.5f * innerRingData.thickness);
        Gizmos.DrawWireSphere(transform.position, innerRingData.radius - 0.5f * innerRingData.thickness);

    }
}
