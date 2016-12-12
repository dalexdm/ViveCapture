using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//Constructs a gameobject representation of a skeleton
public class Actor : MonoBehaviour
{
    public Skeleton skeleton;
    public GameObject[] boneObjects = new GameObject[16];
    public Vector3[] offsets;


    void Start()
    {
        skeleton = new Skeleton();
    }

    private void Update()
    {
        for (int i = 0; i < 16; i++)
        {
        }
    }

    public void pose(Skeleton s)
    {
        for (int i = 0; i < 16; i++)
        {
            boneObjects[i].transform.localRotation = s.rotations[i];
        }
        boneObjects[0].transform.position = skeleton.rootPos;
        this.skeleton = new Skeleton(s);
    }

    

    public int getIndexOf(GameObject o)
    {
        for (int i = 0; i < 16; i++)
        {
            if (boneObjects[i] == o) return i;
        }
        return -1;
    }
}
