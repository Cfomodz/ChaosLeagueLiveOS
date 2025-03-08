using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldenVisuals : MonoBehaviour
{
    [SerializeField] private LineRenderer _borderPath;

    [SerializeField] private GameObject _borderObj;
    [SerializeField] public GameObject _coverObj;

    [SerializeField] private MeshRenderer _borderMesh;
    [SerializeField] private MeshRenderer _coverMesh;

    [SerializeField] private List<ParticleSystem> _particleSystems;
    [SerializeField] private float _spinSpeed;

    [SerializeField] Material _Golds;
    [SerializeField] Material _Rubys;
    [SerializeField] Material _Curse;
    [SerializeField] Material _Mystery;
    [SerializeField] Material _Nulls;

    private float v = 0f;
    private float h = 0f;

    private bool vUp = true;
    private bool hUp = true;

    float totalLength = 0f;
    float t = 0;

    private void Awake()
    {
        // Calculate the total length of the line
        for (int i = 1; i < _borderPath.positionCount; i++)
        {
            totalLength += Vector3.Distance(_borderPath.GetPosition(i - 1), _borderPath.GetPosition(i));

        }

    }

    private void Update()
    {

        for (int i = 0; i < _particleSystems.Count; i++)
        {
            var ps = _particleSystems[i];
            float percentage = (t + (1 / (float)_particleSystems.Count * i)) % 1;
            Vector3 nextPos = GetPosOnLineByPercentage(percentage);
            Vector3 prevPos = ps.transform.position;
            ps.transform.localPosition = nextPos;
            ps.transform.LookAt(prevPos);
            if (vUp)
                v += 0.001f;
            else
                v -= 0.001f;

            if (hUp)
                h += 0.0005f;
            else
                h -= 0.0005f;

            if (v > 5f)
                vUp = false;
            else if (v < -5f)
                vUp = true;

            if (h > 2f)
                hUp = false;
            else if (h < -3f)
                hUp = true;


            _borderMesh.material.SetTextureOffset("_BaseMap", new Vector2(v, h));
            _coverMesh.material.SetTextureOffset("_BaseMap", new Vector2(v, h));

        }

        if (GameTile.GoldenSpids == 5)
        {
            _borderPath.material = _Nulls;
            GameTile.GoldenSpids = 71717;
        }
        if (GameTile.GoldenSpids == 4)
        {
            _borderPath.material = _Mystery;
            GameTile.GoldenSpids = 71717;
        }
        if (GameTile.GoldenSpids == 3)
        {
            _borderPath.material = _Curse;
            GameTile.GoldenSpids = 71717;
        }
        if (GameTile.GoldenSpids == 2)
        {
            _borderPath.material = _Rubys;
            GameTile.GoldenSpids = 71717;
        }
        if (GameTile.GoldenSpids == 1)
        {
            _borderPath.material = _Golds;
            GameTile.GoldenSpids = 71717;
        }

        t = (t + _spinSpeed) % 1;
    }

    public void UpdateSettings(int mode)
    {
        if (mode == 5)
        {
            _borderMesh.material = _Nulls;
            _borderObj.gameObject.SetActive(true);
        }
        if (mode == 4)
        {
            _borderMesh.material = _Mystery;
            _borderObj.gameObject.SetActive(true);
        }
        if (mode == 3)
        {
            _borderMesh.material = _Curse;
            _borderObj.gameObject.SetActive(true);
        }
        if (mode == 2)
        {
            _borderMesh.material = _Rubys;
            _borderObj.gameObject.SetActive(true);
        }
        if (mode == 1)
        {
            _borderMesh.material = _Golds;
            _borderObj.gameObject.SetActive(true);
        }
        if (mode == 0)
        {
            _borderObj.gameObject.SetActive(false);
        }
    }

    private Vector3 GetPosOnLineByPercentage(float t)
    {
        if (t < 0)
            t = 0;
        else if (t > 1)
            t = 1;        
    
        float targetLength = t * totalLength;
        float currentLength = 0f;

        // Find the segment of the line where the target length lies
        for (int i = 1; i < _borderPath.positionCount; i++)
        {
            Vector3 startPos = _borderPath.GetPosition(i - 1);
            Vector3 endPos = _borderPath.GetPosition(i);
            float segmentLength = Vector3.Distance(startPos, endPos);

            if (currentLength + segmentLength >= targetLength)
            {
                // Interpolate within this segment to find the position
                float remainingLength = targetLength - currentLength;
                float interpolationFactor = remainingLength / segmentLength;
                return Vector3.Lerp(startPos, endPos, interpolationFactor);
            }

            currentLength += segmentLength;
        }

        // If the target length is at the end of the line, return the last point
        return _borderPath.GetPosition(_borderPath.positionCount - 1);
    }

}
