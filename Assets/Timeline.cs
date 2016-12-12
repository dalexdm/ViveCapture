using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Timeline : MonoBehaviour
{

    // Static singleton property
    public static Timeline Instance { get; private set; }

    void Awake()
    {
        // Save a reference to the AudioHandler component as our singleton instance
        Instance = this;
    }

    //Members
    public List<Skeleton> keyframes = new List<Skeleton>();
    public List<Skeleton> controlPoints = new List<Skeleton>();
    public float[] boneLengths = new float[16];

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < 16; i++)
        {
            boneLengths[i] = 0;
        }

        addKeyframe(new Skeleton(), 0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void addKeyframe(Skeleton newKeyframe, float key)
    {

        Skeleton newSkeleton = new Skeleton(newKeyframe);
        newSkeleton.key = key;

        //check if a keyframe exists at this key
        Skeleton keyToReplace = null;
        foreach (Skeleton s in keyframes)
        {
            if (Mathf.Abs(s.key - newSkeleton.key) < 0.001)
            {
                keyToReplace = s;
            }
        }

        //if so, replace it
        if (keyToReplace != null) keyframes.Remove(keyToReplace);
        keyframes.Add(newSkeleton);

        // sort keyframes and reconstruct control points for each quat
        keyframes.Sort();
        computeControlPoints();
    }

    void computeControlPoints()
    {
        controlPoints.Clear();
        int numKeys = keyframes.Count;
        if (numKeys <= 1) return;

        Skeleton b0, b1, b2, b3;
        Skeleton q0p, q0a, q1p, q1a;

        for (int segment = 0; segment < numKeys - 1; segment++)
        {
            //rootpos control points
            Vector3 v0, v1, v2, v3;
            v0 = keyframes[segment].rootPos;
            v3 = keyframes[segment + 1].rootPos;

            Vector3 slope, slope2;
            if (segment == 0) slope = v3 - v0;
            else slope = (v3 - keyframes[segment - 1].rootPos) / 2;
            if (segment == keyframes.Count - 2) slope2 = v3 - v0;
            else slope2 = (keyframes[segment + 2].rootPos - v0) / 2;

            v1 = v0 + slope / 3;
            v2 = v3 - slope2 / 3;


            //Quaternion control points
            b0 = keyframes[segment];
            b3 = keyframes[segment + 1];

            //clamp end
            if (segment == numKeys - 2) q0p = sDouble(b3, b3);
            else q0p = sDouble(keyframes[segment + 2], b3);
            q0a = sBisect(b0, q0p);
            //clamp beginning
            if (segment == 0) q1p = sDouble(b0, b0);
            else q1p = sDouble(keyframes[segment - 1], b0);
            q1a = sBisect(q1p, b3);
            //calculate control points
            b1 = slerp(b0, q1a, 1.0f / 3.0f);
            b2 = slerp(b3, q0a, 1.0f / 3.0f);

            b0.rootPos = v0;
            b1.rootPos = v1;
            b2.rootPos = v2;
            b3.rootPos = v3;

            controlPoints.Add(b0);
            controlPoints.Add(b1);
            controlPoints.Add(b2);
            controlPoints.Add(b3);
        }
    }

    Skeleton sDouble(Skeleton a, Skeleton b)
    {
        Skeleton output = new Skeleton();
        for (int i = 0; i < 16; i++)
        {
            output.rotations[i] = b.rotations[i] * Quaternion.Inverse(a.rotations[i]) * b.rotations[i];
        }
        return output;
    }

    Skeleton sBisect(Skeleton a, Skeleton b)
    {
        Skeleton output = new Skeleton();
        for (int i = 0; i < 16; i++)
        {
            Quaternion aq = a.rotations[i];
            Quaternion bq = b.rotations[i];
            Vector4 temp = new Vector4(aq[0] + bq[0], aq[1] + bq[1], aq[2] + bq[2], aq[3] + bq[3]);
            temp.Normalize();

            output.rotations[i] = new Quaternion(temp[0], temp[1], temp[2], temp[3]);
        }
        return output;
    }

    Skeleton slerp(Skeleton a, Skeleton b, float u)
    {
        Skeleton output = new Skeleton();
        for (int i = 0; i < 16; i++)
        {
            output.rotations[i] = Quaternion.Slerp(a.rotations[i], b.rotations[i], u);
        }
        return output;
    }

public void delete(int key)
{
    if (keyframes.Count < 2) return;
    Skeleton keyToReplace = null;
    foreach (Skeleton s in keyframes)
    {
        if (Mathf.Abs(s.key - key) < 0.001)
        {
            keyToReplace = s;
        }
    }

    if (keyToReplace != null) keyframes.Remove(keyToReplace);
}

public Skeleton getFramePose(float key)
{
    if (keyframes.Count > 1)
    {
        int segment = getSegment(key);
        float u = (key - keyframes[segment].key) / (keyframes[segment + 1].key - keyframes[segment].key);
        u = Mathf.Max(0, Mathf.Min(1, u));
        return getSlerped(segment, u);
    }
    else if (keyframes.Count == 1)
    {
        return new Skeleton(keyframes[0]);
    }
    else
    {
        Debug.Log("No frames to use");
        return null;
    }
}

public bool containsKey(float key)
{
    foreach (Skeleton s in keyframes)
    {
        if (s.key == key)
            return true;
    }
    return false;
}

int getSegment(float key)
{
    if (key <= keyframes[0].key)
        return 0;
    if (key > keyframes[keyframes.Count - 1].key)
        return keyframes.Count - 2;

    for (int i = 0; i < keyframes.Count; i++)
    {
        if (key > keyframes[i].key && key <= keyframes[i + 1].key)
            return i;
    }

    return 666;
}

Skeleton lerp(int seg, float u)
{
    Skeleton output = new Skeleton(keyframes[seg]);
    for (int i = 0; i < 16; i++)
    {
        if (i == 12)
            output.rootPos = Vector3.Lerp(keyframes[seg].rootPos, keyframes[seg + 1].rootPos, u);
        Quaternion v1 = keyframes[seg].rotations[i];
        Quaternion v2 = keyframes[seg + 1].rotations[i];
        output.rotations[i] = Quaternion.Slerp(v1, v2, u);
    }
    return output;
}

    Skeleton getSlerped (int seg, float u)
    {
        Skeleton b0, b1, b2, b3;
        b0 = controlPoints[seg * 4];
        b1 = controlPoints[seg * 4 + 1];
        b2 = controlPoints[seg * 4 + 2];
        b3 = controlPoints[seg * 4 + 3];

        Skeleton b01, b11, b21, b02, b12, b03;
        Vector3 v0, v1, v2, v3;
        // TODO: Return the result of Scubic based on the cubic quaternion curve control points b0, b1, b2 and b3

        b01 = slerp(b0, b1, u);
        b11 = slerp(b1, b2, u);
        b21 = slerp(b2, b3, u);
        b02 = slerp(b01, b11, u);
        b12 = slerp(b11, b21, u);
        b03 = slerp(b02, b12, u);

        v0 = b0.rootPos;
        v1 = b1.rootPos;
        v2 = b2.rootPos;
        v3 = b3.rootPos;

        b03.rootPos = v0 * (1 - u) * (1 - u) * (1 - u) +
            3 * v1 * (1 - u) * (1 - u) * u +
            3 * v2 * (1 - u) * u * u +
            v3 * u * u * u;

        return b03;
    }
}
