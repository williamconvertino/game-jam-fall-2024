using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1Behavior : MonoBehaviour
{
    [Header("Defensive Ring")]
    public Transform ringParent;
    public GameObject arcSegment;
    [Range(0, 360)] public float angleSize = 300f;
    [Range(1, 20)]  public int numSegments = 5;
    [Range(1, 20)]  public int resolutionPerSegment = 5;
    public float radius = 1f;
    public float thickness = 0.5f;
    public float spinSpeed = 2f;
    public float idleSpinSpeed = 1f;

    [Header("Head Laser")]
    public Transform player;
    public Transform headParent;
    public Transform head;
    public LineRenderer laser;
    public float detectionRadius = 10f;
    public float headSpeed = 1f;
    public float headChargingSpeed = 0.5f;
    public float laserDamage = 0f;
    public float laserChargeUpTime = 1f;
    public float laserUntilLockonTime = 0.5f; // Less than laserChargeUpTime
    public float laserBlastTime = 0.2f; // time the laser is firing
    public float laserCooldown = 5f;

    private LineRenderer _laserTrackingBeam;
    private float _timeSinceLaser = 0f;
    private float _chargeTime = 0f;
    private Vector3 _fireDir;

    private enum BehaviorState { Idle, Tracking, Charging, Firing, Stunned, Death };
    private BehaviorState _state = BehaviorState.Idle;



    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.Instance != null)
        {
            player = GameManager.Instance.playerShip.transform;
        }
        SpawnDefensiveRing();
        _laserTrackingBeam = head.gameObject.GetComponent<LineRenderer>();
        _laserTrackingBeam.positionCount = 2;
        _laserTrackingBeam.useWorldSpace = true;
        _laserTrackingBeam.enabled = false;

        laser.positionCount = 2;
        laser.useWorldSpace = true;
        laser.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        Behavior();
        Transition();
        //Debug.Log(_state);
    }

    private void Behavior()
    {
        switch (_state)
        {
            default:
            case BehaviorState.Idle:
                SpinRing(idleSpinSpeed);
                break;
            case BehaviorState.Tracking:
                SpinRing(spinSpeed);
                HeadTrackPlayer(headSpeed);
                _timeSinceLaser += Time.deltaTime;
                break;
            case BehaviorState.Charging:
                SpinRing(idleSpinSpeed);
                if (_chargeTime < laserUntilLockonTime)
                {
                    HeadTrackPlayer(headChargingSpeed);
                    TrackLaser();
                }
                _chargeTime += Time.deltaTime;
                break;
            case BehaviorState.Firing:
                _timeSinceLaser += Time.deltaTime;
                break;
            case BehaviorState.Stunned:
                break;
        }
    }
    private void Transition()
    {
        switch (_state)
        {
            default:
            case BehaviorState.Idle:
                if (IdleToTracking()) { _state = BehaviorState.Tracking; _timeSinceLaser = 0f; }
                break;
            case BehaviorState.Tracking:
                if (TrackingToIdle()) _state = BehaviorState.Idle;
                if (TrackingToCharging()) { _state = BehaviorState.Charging; _chargeTime = 0f; _laserTrackingBeam.enabled = true; }
                break;
            case BehaviorState.Charging:
                if (ChargingToFiring()) { _state = BehaviorState.Firing; _timeSinceLaser = 0f; FireLaser(); }
                break;
            case BehaviorState.Firing:
                if(FiringToTracking()) { _state = BehaviorState.Tracking; _timeSinceLaser = 0f; laser.enabled = false; }
                break;
            case BehaviorState.Stunned:
                break;
        }
    }
    #region Transition Conditions
    private bool IdleToTracking() { return ((player.position - transform.position).sqrMagnitude < detectionRadius * detectionRadius); }
    private bool TrackingToIdle() { return !IdleToTracking(); }
    private bool TrackingToCharging() { return _timeSinceLaser > laserCooldown; }
    private bool ChargingToFiring() { return _chargeTime > laserChargeUpTime; }
    private bool FiringToTracking() { return _timeSinceLaser > laserBlastTime; }
    #endregion

    
    private void SpawnDefensiveRing()
    {
        float segmentAngle = angleSize / numSegments;
        float currAngle = 0f;
        for (int i = 0; i < numSegments; i++)
        {
            GameObject arc = Instantiate(arcSegment, ringParent);
            CircleSegment segment = arc.GetComponent<CircleSegment>();
            segment.segments = resolutionPerSegment;
            segment.startAngle = currAngle;
            segment.endAngle = currAngle + segmentAngle;
            segment.innerRadius = radius - thickness * 0.5f;
            segment.outerRadius = radius + thickness * 0.5f;

            currAngle += segmentAngle;
        }
    }

    #region Behavior
    private void SpinRing(float speed)
    {
        ringParent.rotation *= Quaternion.Euler(0f, 0f, speed);
    }

    private void HeadTrackPlayer(float speed)
    {
        // Get player angle from center
        float angleBetween = Vector2.SignedAngle(head.position - transform.position, player.position - transform.position);

        headParent.rotation *= Quaternion.Euler(0f, 0f, Mathf.Sign(angleBetween) * Mathf.Min(Mathf.Abs(angleBetween), headSpeed));
    }

    private void TrackLaser()
    {
        _laserTrackingBeam.SetPosition(0, head.position);
        // Track laser to player
        _fireDir = (player.position - head.position).normalized;

        _laserTrackingBeam.SetPosition(1, head.position + _fireDir * 100f);
    }

    private void FireLaser()
    {
        laser.SetPosition(0, head.position);
        laser.SetPosition(1, head.position + _fireDir * 100f);
        laser.enabled = true;
        _laserTrackingBeam.enabled = false;
    }

    #endregion

    private void OnDrawGizmos()
    {
        // Ring
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius + 0.5f * thickness);
        Gizmos.DrawWireSphere(transform.position, radius - 0.5f * thickness);

        // Detection radius
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
