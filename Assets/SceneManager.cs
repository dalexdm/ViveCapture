using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{

    public Actor mainActor;

    //skeleton construction
    Skeleton inProgress;
    int leftIndex, rightIndex, centerIndex;
    public GameObject testPrefab;
    GameObject[] testPoints = new GameObject[16];
    Vector3[] skelePoints = new Vector3[16];

    //Slider Module
    public Slider slider;
    public Text sliderText;
    public GameObject sliderKeyPrefab;
    public float currentKey;
    public float maxKey;
    public int playbackSpeed;
    public bool playing = false;
    public bool isScaling;

    public bool editing = false;
    public GameObject rightHolding = null;
    public GameObject leftHolding = null;

    //Point Status
    public Image statusImage;
    Vector3 lFoot, rFoot;

    //Tracker lines
    public SteamVR_TrackedController leftCtrl, rightCtrl;
    public bool recording;
    public LineRenderer LeftLR, RightLR;
    public Vector3[] LPoints, RPoints;
    public GameObject lpp, rpp;
    public Quaternion ls, rs;
    public Quaternion ljs, rjs;

    static Vector3 offsetV = new Vector3(0, 1, 0);

    public bool inputDisabled = false;
    Color keyColor, noKeyColor;

    // Use this for initialization
    void Start()
    {
        Timeline.Instance.addKeyframe(new Skeleton(), 0);
        maxKey = slider.maxValue;
        LeftLR = GameObject.Find("LineLeft").GetComponent<LineRenderer>();
        RightLR = GameObject.Find("LineRight").GetComponent<LineRenderer>();

        lFoot = mainActor.boneObjects[9].transform.position;
        rFoot = mainActor.boneObjects[15].transform.position;

        for (int i = 0; i < 16; i++)
        {
            testPoints[i] = (GameObject)Instantiate(testPrefab);
        }

        keyColor = new Color32(0xFF, 0x6F, 0x6F, 0xFF);
        noKeyColor = new Color32(0x6F, 0xB1, 0xFF, 0xFF);
        mainActor.skeleton = new Skeleton();
        ChangeFrame();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (rightHolding)
        {
            if (rightHolding == mainActor.gameObject)
            {
                rightHolding.transform.position = rightCtrl.transform.position - (mainActor.transform.rotation * (mainActor.offsets[0] * 0.3f));
                footIK(mainActor.boneObjects[7], mainActor.boneObjects[8], mainActor.boneObjects[9], lFoot);
                footIK(mainActor.boneObjects[13], mainActor.boneObjects[14], mainActor.boneObjects[15], rFoot);
            }
            rightHolding.transform.rotation = ((rightCtrl.transform.rotation * Quaternion.Inverse(rs)) * rjs);
        }
        if (leftHolding)
        {
            if (leftHolding == mainActor.gameObject)
            {
                leftHolding.transform.position = leftCtrl.transform.position - (mainActor.transform.rotation * (mainActor.offsets[0] * 0.3f));
                footIK(mainActor.boneObjects[7], mainActor.boneObjects[8], mainActor.boneObjects[9], lFoot);
                footIK(mainActor.boneObjects[13], mainActor.boneObjects[14], mainActor.boneObjects[15], rFoot);
            }
            leftHolding.transform.rotation = (leftCtrl.transform.rotation * Quaternion.Inverse(ls)) * ljs;
        }

        //ensure the controllers exist and allow editing joints to follow
        if (!leftCtrl || !rightCtrl) acquireControllers();
    }

    public void delete()
    {
        Timeline.Instance.delete((int)currentKey);
    }

    public void ChangeFrame()
    {
        currentKey = slider.value;

        for (int i = 0; i < 16; i++)
        {
            skelePoints[i] = new Vector3(0, 0, 0);
            testPoints[i].SetActive(false);
        }
        centerIndex = 0;
        leftIndex = 0;
        rightIndex = 0;

        //update slider
        sliderText.text = currentKey.ToString();
        float perc = currentKey / slider.maxValue;
        perc = (1 - perc) / 1.1037f + 0.04774f;
        sliderText.transform.parent.localPosition = 800f * new Vector3(-Mathf.Cos(perc * 2 * Mathf.PI), -Mathf.Sin(perc * 2 * Mathf.PI), 0);

        //turn slider indicator red for keyframes, blue otherwise
        if (Timeline.Instance.containsKey(currentKey)) sliderText.transform.parent.gameObject.GetComponent<Image>().color = keyColor;
        else sliderText.transform.parent.gameObject.GetComponent<Image>().color = noKeyColor;

        //move progress markers smoothly
        if (currentKey < 119 && lpp.activeSelf)
        {
                lpp.transform.position = LPoints[(int)currentKey] + offsetV;
                rpp.transform.position = RPoints[(int)currentKey] + offsetV;
        }

        //if we have any skeletons so far, display the main actor in the correct pose
        Skeleton pose = Timeline.Instance.getFramePose(currentKey);
        mainActor.pose(pose);
    }

    public void play()
    {
        slider.value = 0;
        playing = !playing;
        if (playing) StartCoroutine(playRoutine());
    }

    public void scale ()
    {
        
        StartCoroutine(scaling());
    }

    IEnumerator scaling()
    {
        while (true) {
            if (rightCtrl.triggerPressed && leftCtrl.triggerPressed) break;
            if (!isScaling) yield break;
            yield return null;
        }

        float dist = (rightCtrl.transform.position - leftCtrl.transform.position).magnitude;
        float initialScale = mainActor.boneObjects[0].transform.localScale[0];
        float newScale = 0;

        while (rightCtrl.triggerPressed && leftCtrl.triggerPressed)
        {
            newScale = initialScale * ((rightCtrl.transform.position - leftCtrl.transform.position).magnitude / dist);
            mainActor.boneObjects[0].transform.localScale = new Vector3(newScale,newScale,newScale);
            yield return null;
        }

        if (isScaling) StartCoroutine(scaling());
    }

    IEnumerator playRoutine()
    {
        //onion.gameObject.SetActive(false);
        while (playing)
        {
            slider.value += 1;
            if (slider.value >= slider.maxValue) slider.value = 0;
            yield return new WaitForSeconds(1.0f / (float)playbackSpeed);
        }
    }

    IEnumerator recordRoutine()
    {
        Vector3[] lps = new Vector3[(int)slider.maxValue];
        Vector3[] rps = new Vector3[(int)slider.maxValue];

        LeftLR.SetPositions(lps);
        RightLR.SetPositions(rps);
        LPoints = lps;
        RPoints = rps;

        leftCtrl.gameObject.GetComponent<TrailRenderer>().enabled = true;
        rightCtrl.gameObject.GetComponent<TrailRenderer>().enabled = true;

        int i = 0;

        while (i < slider.maxValue)
        {
            lps[i] = leftCtrl.transform.position;
            rps[i] = rightCtrl.transform.position;
            slider.value = i;
            i++;
            yield return new WaitForSeconds(1.0f / (float)playbackSpeed);
        }

        LeftLR.SetPositions(lps);
        RightLR.SetPositions(rps);
        LPoints = lps;
        RPoints = rps;

        leftCtrl.gameObject.GetComponent<TrailRenderer>().enabled = false;
        rightCtrl.gameObject.GetComponent<TrailRenderer>().enabled = false;
        lpp.SetActive(true);
        rpp.SetActive(true);
        recording = false;
        slider.value = 0;
    }

    void acquireControllers()
    {
        if (!leftCtrl)
        {
            GameObject leftObject = GameObject.Find("Controller (left)");
            if (leftObject)
            {
                leftCtrl = leftObject.GetComponent<SteamVR_TrackedController>();
                leftCtrl.TriggerClicked += leftClicked;
                leftCtrl.Gripped += clearFrame;
                leftCtrl.TriggerUnclicked += finishEditLeft;
            }
        }

        if (!rightCtrl)
        {
            GameObject rightObject = GameObject.Find("Controller (right)");
            if (rightObject)
            {
                rightCtrl = rightObject.GetComponent<SteamVR_TrackedController>();
                rightCtrl.TriggerClicked += rightClicked;
                rightCtrl.Gripped += clearFrame;
                rightCtrl.TriggerUnclicked += finishEditRight;
            }
        }
    }

    GameObject getClosestBone (Vector3 pos)
    {
        GameObject closest = null;
        float dist = 1;
        for (int i = 0; i < 16; i++)
        {
            GameObject b = mainActor.boneObjects[i];
            Vector3 center = b.transform.position + (b.transform.rotation * (0.3f * mainActor.offsets[i]));
            if ((center - pos).sqrMagnitude < dist)
            {
                dist = (center - pos).sqrMagnitude;
                closest = b;
            }
        }
        return closest;
    }

    void leftClicked(object sender, ClickedEventArgs e)
    {
        if (inputDisabled) return;
        if (recording) StartCoroutine(recordRoutine());
        else if (editing)
        {
            lFoot = mainActor.boneObjects[9].transform.position;
            rFoot = mainActor.boneObjects[15].transform.position;
            leftHolding = getClosestBone(leftCtrl.transform.position);
            if (leftHolding == null) return;
            ls = leftCtrl.transform.rotation;
            ljs = leftHolding.transform.rotation;
        }
    }

    void rightClicked(object sender, ClickedEventArgs e)
    {
        if (inputDisabled) return;
        if (recording) StartCoroutine(recordRoutine());
        else if (editing)
        {
            lFoot = mainActor.boneObjects[9].transform.position;
            rFoot = mainActor.boneObjects[15].transform.position;
            rightHolding = getClosestBone(rightCtrl.transform.position);
            if (rightHolding == null) return;
            rs = rightCtrl.transform.rotation;
            rjs = rightHolding.transform.rotation;
        }
    }

    void finishEditLeft(object sender, ClickedEventArgs e)
    {
        if (leftHolding != null)
        {
            mainActor.skeleton.rotations[mainActor.getIndexOf(leftHolding)] = leftHolding.transform.localRotation;
            mainActor.skeleton.rootPos = mainActor.boneObjects[0].transform.position;
            Timeline.Instance.addKeyframe(mainActor.skeleton, currentKey);
        }
        leftHolding = null;
    }

    void finishEditRight(object sender, ClickedEventArgs e)
    {
        if (rightHolding != null)
        {
            mainActor.skeleton.rotations[mainActor.getIndexOf(rightHolding)] = rightHolding.transform.localRotation;
            mainActor.skeleton.rootPos = mainActor.boneObjects[0].transform.position;
            Timeline.Instance.addKeyframe(mainActor.skeleton, currentKey);
        }
        rightHolding = null;
    }

    void clearFrame(object sender, ClickedEventArgs e)
    {
        ChangeFrame();
    }

    public void startRecord ()
    {
        recording = true;
    }

    void footIK(GameObject hip, GameObject knee, GameObject ankle, Vector3 target)
    {
        float l1 = knee.transform.localPosition.magnitude;
        float l2 = ankle.transform.localPosition.magnitude;

        // compute vector between target and base joint
        Vector3 base2tar = target - hip.transform.position;
        if (base2tar.magnitude > l1 + l2)
            base2tar *= (l1 + l2 + 0.000001f) / base2tar.magnitude;

        float d = base2tar.magnitude;
        // Compute desired angle for middle joint 
        float phi = Mathf.Acos((l1 * l1 + l2 * l2 - d * d) / (2 * l1 * l2));
        float theta2 = phi - Mathf.PI;
        float theta1 = Mathf.Asin((l2 * Mathf.Sin(phi)) / d);

        // given desired angle and midJointAxis, compute new local middle joint rotation matrix and update joint transform
        Quaternion rotMid;
        Quaternion rotBase;

        rotMid = Quaternion.AngleAxis(theta2 * Mathf.Rad2Deg, hip.transform.right);
        rotBase = Quaternion.AngleAxis(theta1 * Mathf.Rad2Deg, hip.transform.right);

        knee.transform.localRotation = rotMid;
        hip.transform.localRotation = rotBase;

        // compute vector between target and base joint
        Vector3 base2end = ankle.transform.position - hip.transform.position;

        // Compute base joint rotation axis (in global coords) and desired angle
        float thetaLast = Vector3.Angle(base2end, base2tar);
        Vector3 baseAxis = hip.transform.rotation * Vector3.Cross(base2end,base2tar).normalized;

        // transform base joint rotation axis to local coordinates
        Quaternion rotBase2;
        rotBase2 = Quaternion.AngleAxis(thetaLast, baseAxis);
        // 9. given desired angle and local rotation axis, compute new local rotation matrix and update base joint transform
        hip.transform.localRotation = hip.transform.localRotation * rotBase2;

        //mainActor.skeleton.rotations[mainActor.getIndexOf(hip)] = hip.transform.localRotation;
        //mainActor.skeleton.rotations[mainActor.getIndexOf(knee)] = knee.transform.localRotation;
    }

}
