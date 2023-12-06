using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tongue : MonoBehaviour
{
    [Header("General Refernces:")]
    public GameObject head;
    public LineRenderer m_lineRenderer;

    [Header("General Settings:")]
    [SerializeField] private int percision = 40;
    [Range(0, 20)] [SerializeField] private float straightenLineSpeed = 5;

    [Header("Tongue Animation Settings:")]
    public AnimationCurve tongueAnimationCurve;
    [Range(0.01f, 4)] [SerializeField] private float StartWaveSize = 2;
    float waveSize = 0;

    [Header("Tongue Progression:")]
    public AnimationCurve tongueProgressionCurve;
    [SerializeField] [Range(1, 50)] private float tongueProgressionSpeed = 1;

    float moveTime = 0;

    [HideInInspector] public bool isGrappling = true;

    bool strightLine = true;

    private void OnEnable()
    {
        moveTime = 0;
        m_lineRenderer.positionCount = percision;
        waveSize = StartWaveSize;
        strightLine = false;

        LinePointsToFirePoint();

        m_lineRenderer.enabled = true;
    }

    private void OnDisable()
    {
        m_lineRenderer.enabled = false;
        isGrappling = false;
    }

    private void LinePointsToFirePoint()
    {
        for (int i = 0; i < percision; i++)
        {
            m_lineRenderer.SetPosition(i, head.firePoint.position);
        }
    }

    private void Update()
    {
        moveTime += Time.deltaTime;
        Drawtongue();
    }

    void Drawtongue()
    {
        if (!strightLine)
        {
            if (m_lineRenderer.GetPosition(percision - 1).x == head.grapplePoint.x)
            {
                strightLine = true;
            }
            else
            {
                DrawtongueWaves();
            }
        }
        else
        {
            if (!isGrappling)
            {
                head.Grapple();
                isGrappling = true;
            }
            if (waveSize > 0)
            {
                waveSize -= Time.deltaTime * straightenLineSpeed;
                DrawtongueWaves();
            }
            else
            {
                waveSize = 0;

                if (m_lineRenderer.positionCount != 2) { m_lineRenderer.positionCount = 2; }

                DrawtongueNoWaves();
            }
        }
    }

    void DrawtongueWaves()
    {
        for (int i = 0; i < percision; i++)
        {
            float delta = (float)i / ((float)percision - 1f);
            Vector2 offset = Vector2.Perpendicular(head.grappleDistanceVector).normalized * tongueAnimationCurve.Evaluate(delta) * waveSize;
            Vector2 targetPosition = Vector2.Lerp(head.firePoint.position, head.grapplePoint, delta) + offset;
            Vector2 currentPosition = Vector2.Lerp(head.firePoint.position, targetPosition, tongueProgressionCurve.Evaluate(moveTime) * tongueProgressionSpeed);

            m_lineRenderer.SetPosition(i, currentPosition);
        }
    }

    void DrawtongueNoWaves()
    {
        m_lineRenderer.SetPosition(0, head.firePoint.position);
        m_lineRenderer.SetPosition(1, head.grapplePoint);
    }
}
