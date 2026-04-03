using System;
using System.Collections.Generic;
using UnityEngine;

public class SoftSpaceFloatOrbit : MonoBehaviour
{
    public enum OrbitShape
    {
        Circle,
        Sphere
    }

    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Objects")]
    [SerializeField] private List<Transform> orbitObjects = new List<Transform>();
    [SerializeField] private bool autoGetChildren = true;

    [Header("Shape")]
    [SerializeField] private OrbitShape orbitShape = OrbitShape.Sphere;
    [SerializeField] private float radius = 3.5f;
    [SerializeField] private Vector2 randomRadiusOffset = new Vector2(-0.35f, 0.35f);

    [Header("Global Orbit Drift")]
    [Tooltip("Tốc độ cả cụm quay quanh target. Nhỏ để chuyển động nhẹ nhàng.")]
    [SerializeField] private float orbitDriftSpeed = 8f;

    [Header("Floating Motion")]
    [SerializeField] private float floatAmplitude = 0.25f;
    [SerializeField] private float floatFrequency = 0.8f;
    [SerializeField] private float sideAmplitude = 0.18f;
    [SerializeField] private float sideFrequency = 0.6f;

    [Header("Follow Smooth")]
    [Tooltip("Càng nhỏ càng mượt, nhưng hơi trễ hơn.")]
    [SerializeField] private float smoothTime = 0.45f;
    [SerializeField] private float maxMoveSpeed = 5f;

    [Header("Avoid Collision")]
    [SerializeField] private bool avoidOverlap = true;
    [SerializeField] private float personalSpace = 0.8f;
    [SerializeField] private float repulsionStrength = 1.2f;
    [SerializeField] private float repulsionFalloff = 1.25f;

    [Header("Rotation")]
    [SerializeField] private bool faceOutward = false;
    [SerializeField] private bool lookAtTarget = false;
    [SerializeField] private bool selfRotate = true;
    [SerializeField] private Vector2 selfRotateSpeedRange = new Vector2(6f, 18f);

    [Header("Time")]
    [SerializeField] private bool useUnscaledTime = false;

    private readonly List<ItemData> items = new List<ItemData>();

    [Serializable]
    private class ItemData
    {
        public Transform tf;
        public Vector3 baseDir;
        public float radiusOffset;
        public float orbitAngleOffset;
        public float verticalPhase;
        public float sidePhase;
        public float noiseSeed;
        public float selfRotateSpeed;
        public Vector3 velocity;
        public Vector3 currentPos;
    }

    private void Awake()
    {
        CollectObjects();
        BuildData(true);
    }

    private void OnEnable()
    {
        if (items.Count == 0)
        {
            CollectObjects();
            BuildData(true);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        radius = Mathf.Max(0.1f, radius);
        floatAmplitude = Mathf.Max(0f, floatAmplitude);
        sideAmplitude = Mathf.Max(0f, sideAmplitude);
        floatFrequency = Mathf.Max(0f, floatFrequency);
        sideFrequency = Mathf.Max(0f, sideFrequency);
        smoothTime = Mathf.Max(0.01f, smoothTime);
        maxMoveSpeed = Mathf.Max(0.01f, maxMoveSpeed);
        personalSpace = Mathf.Max(0.01f, personalSpace);
        repulsionStrength = Mathf.Max(0f, repulsionStrength);
        repulsionFalloff = Mathf.Max(0.01f, repulsionFalloff);
    }
#endif

    private void LateUpdate()
    {
        if (target == null || items.Count == 0) return;

        float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        float time = useUnscaledTime ? Time.unscaledTime : Time.time;

        Vector3 center = target.position;
        float globalAngle = orbitDriftSpeed * time;

        Vector3[] desiredPositions = new Vector3[items.Count];

        for (int i = 0; i < items.Count; i++)
        {
            ItemData item = items[i];
            if (item.tf == null) continue;

            Vector3 dir = RotateBaseDirection(item.baseDir, globalAngle + item.orbitAngleOffset);

            float r = radius + item.radiusOffset;
            Vector3 anchor = center + dir * r;

            Vector3 upOffset = Vector3.up * (Mathf.Sin(time * floatFrequency + item.verticalPhase) * floatAmplitude);

            Vector3 tangent = Vector3.Cross(Vector3.up, dir).normalized;
            if (tangent.sqrMagnitude < 0.0001f)
                tangent = Vector3.right;

            Vector3 sideOffset = tangent * (Mathf.Sin(time * sideFrequency + item.sidePhase) * sideAmplitude);

            Vector3 depthOffset = dir * (Mathf.Cos(time * (sideFrequency * 0.75f) + item.noiseSeed) * sideAmplitude * 0.35f);

            desiredPositions[i] = anchor + upOffset + sideOffset + depthOffset;
        }

        if (avoidOverlap)
        {
            ApplyRepulsion(desiredPositions, dt);
        }

        for (int i = 0; i < items.Count; i++)
        {
            ItemData item = items[i];
            if (item.tf == null) continue;

            item.currentPos = Vector3.SmoothDamp(
                item.tf.position,
                desiredPositions[i],
                ref item.velocity,
                smoothTime,
                maxMoveSpeed,
                dt
            );

            item.tf.position = item.currentPos;

            Vector3 fromCenter = (item.tf.position - center).normalized;

            if (lookAtTarget)
            {
                Vector3 lookDir = center - item.tf.position;
                if (lookDir.sqrMagnitude > 0.0001f)
                    item.tf.rotation = Quaternion.Slerp(
                        item.tf.rotation,
                        Quaternion.LookRotation(lookDir, Vector3.up),
                        dt * 4f
                    );
            }
            else if (faceOutward)
            {
                if (fromCenter.sqrMagnitude > 0.0001f)
                    item.tf.rotation = Quaternion.Slerp(
                        item.tf.rotation,
                        Quaternion.LookRotation(fromCenter, Vector3.up),
                        dt * 4f
                    );
            }

            if (selfRotate)
            {
                item.tf.Rotate(Vector3.up * item.selfRotateSpeed * dt, Space.Self);
            }
        }
    }

    private Vector3 RotateBaseDirection(Vector3 baseDir, float angleDeg)
    {
        Quaternion rot = Quaternion.AngleAxis(angleDeg, Vector3.up);
        return (rot * baseDir).normalized;
    }

    private void ApplyRepulsion(Vector3[] positions, float dt)
    {
        for (int i = 0; i < positions.Length; i++)
        {
            Vector3 push = Vector3.zero;

            for (int j = 0; j < positions.Length; j++)
            {
                if (i == j) continue;

                Vector3 delta = positions[i] - positions[j];
                float dist = delta.magnitude;

                if (dist <= 0.0001f)
                {
                    delta = UnityEngine.Random.onUnitSphere * 0.01f;
                    dist = delta.magnitude;
                }

                if (dist < personalSpace)
                {
                    float t = 1f - (dist / personalSpace);
                    float force = Mathf.Pow(t, repulsionFalloff) * repulsionStrength;
                    push += delta.normalized * force;
                }
            }

            positions[i] += push * dt;
        }
    }

    [ContextMenu("Collect Children")]
    public void CollectObjects()
    {
        if (!autoGetChildren) return;

        orbitObjects.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child != null)
                orbitObjects.Add(child);
        }
    }

    [ContextMenu("Rebuild Orbit Data")]
    public void RebuildOrbitData()
    {
        BuildData(false);
    }

    private void BuildData(bool snapToStart)
    {
        items.Clear();

        if (orbitObjects == null || orbitObjects.Count == 0) return;

        int count = orbitObjects.Count;

        for (int i = 0; i < count; i++)
        {
            Transform obj = orbitObjects[i];
            if (obj == null) continue;

            ItemData item = new ItemData();
            item.tf = obj;

            float horizontalAngle = (360f / Mathf.Max(1, count)) * i;

            if (orbitShape == OrbitShape.Circle)
            {
                Vector3 dir = Quaternion.Euler(0f, horizontalAngle, 0f) * Vector3.forward;
                item.baseDir = dir.normalized;
            }
            else
            {
                float yaw = horizontalAngle;
                float pitch = Mathf.Lerp(-35f, 35f, (count <= 1) ? 0.5f : i / (float)(count - 1));

                Vector3 dir = Quaternion.Euler(pitch, yaw, 0f) * Vector3.forward;
                item.baseDir = dir.normalized;
            }

            item.radiusOffset = UnityEngine.Random.Range(randomRadiusOffset.x, randomRadiusOffset.y);
            item.orbitAngleOffset = UnityEngine.Random.Range(-8f, 8f);
            item.verticalPhase = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            item.sidePhase = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            item.noiseSeed = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            item.selfRotateSpeed = UnityEngine.Random.Range(selfRotateSpeedRange.x, selfRotateSpeedRange.y);

            Vector3 center = target != null ? target.position : Vector3.zero;
            Vector3 startPos = center + item.baseDir * (radius + item.radiusOffset);
            item.currentPos = startPos;

            if (snapToStart && obj != null)
                obj.position = startPos;

            items.Add(item);
        }
    }

    private void Reset()
    {
        if (target == null && Camera.main != null)
            target = Camera.main.transform;

        CollectObjects();
        BuildData(true);
    }

    private void OnDrawGizmosSelected()
    {
        if (target == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(target.position, radius);
    }
}