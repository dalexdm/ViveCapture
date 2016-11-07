using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{

    public Actor mainActor;
    public Actor onionActor;

    //skeleton construction
    public Skeleton inProgress;
    public int leftIndex, rightIndex, centerIndex;

    //Slider Module
    public Slider slider;
    public Text sliderText;
    public GameObject sliderKeyPrefab;
    public float currentKey;
    public float maxKey;
    public InputField input;
    public int playbackSpeed;
    public bool playing;

    //Point Status
    public Image statusImage;

    //Tracker lines
    public SteamVR_TrackedController leftCtrl, rightCtrl;
    public bool recording;
    public LineRenderer LeftLR, RightLR;
    public Vector3[] LPoints, RPoints;
    public GameObject lpp, rpp;

    static Vector3 offsetV = new Vector3(0, 1, 0);

    // Use this for initialization
    void Start()
    {
        ChangeFrame();
        maxKey = slider.maxValue;
        LeftLR = GameObject.Find("LineLeft").GetComponent<LineRenderer>();

        RightLR = GameObject.Find("LineRight").GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

        if (!leftCtrl || !rightCtrl) acquireControllers();

        //accept key input to create a skeleton on the currently selected frame
        //Get left controller
        if (Input.GetMouseButtonUp(0) && Input.GetKey(KeyCode.LeftShift))
        {
            Plane plane = new Plane(new Vector3(0, 0, 1), new Vector3());
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float distance;
            Vector3 hitpoint;
            if (plane.Raycast(ray, out distance))
            {
                hitpoint = ray.GetPoint(distance);
                if (leftIndex < 6)
                    addBone(hitpoint, 1);
            }
        }

        //Get right controller
        if (Input.GetMouseButtonUp(1) && Input.GetKey(KeyCode.LeftShift))
        {
            Plane plane = new Plane(new Vector3(0, 0, 1), new Vector3());
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float distance;
            Vector3 hitpoint;
            if (plane.Raycast(ray, out distance))
            {
                hitpoint = ray.GetPoint(distance);
                if (rightIndex < 6)
                    addBone(hitpoint, 2);
            }
        }

        //Get center button tapped on either controller
        if (Input.GetMouseButtonUp(2) && Input.GetKey(KeyCode.LeftShift))
        {
            Plane plane = new Plane(new Vector3(0, 0, 1), new Vector3());
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float distance;
            Vector3 hitpoint;
            if (plane.Raycast(ray, out distance))
            {
                hitpoint = ray.GetPoint(distance);
                if (centerIndex < 3)
                    addBone(hitpoint, 0);
            }
        }
    }

    public void ChangeFrame()
    {

        int previous = (int)currentKey;

        currentKey = slider.value;
        sliderText.text = currentKey.ToString();

        if (Timeline.Instance.containsKey(currentKey))
        {
            sliderText.transform.parent.gameObject.GetComponent<Image>().color = Color.red;
            sliderText.color = Color.white;
        }
        else
        {
            sliderText.transform.parent.gameObject.GetComponent<Image>().color = Color.white;
            sliderText.color = Color.black;
        }

        //move progress markers
        if (currentKey < 120)
        {
            if (!playing && currentKey > previous) StartCoroutine(moveThings(previous));
            else
            {
                lpp.transform.position = LPoints[(int)currentKey] + offsetV;
                rpp.transform.position = RPoints[(int)currentKey] + offsetV;
            }
        }

        inProgress = null;
        leftIndex = 0;
        rightIndex = 0;
        centerIndex = 0;
        statusImage.gameObject.SetActive(false);

        //if we have any skeletons so far, display the main actor in the correct pose
        Skeleton touse = Timeline.Instance.getFramePose(currentKey);
        if (touse == null)
        {
            mainActor.gameObject.SetActive(false);
            onionActor.gameObject.SetActive(false);
            return;
        }
        touse.update();
        mainActor.pose(touse);
        onionActor.pose(Timeline.Instance.getFramePose(Mathf.Max(0, currentKey - 5)));

    }

    public void addBone(Vector3 pos, int clr)
    {

        mainActor.gameObject.SetActive(true);
        onionActor.gameObject.SetActive(true);

        //one first input of the frame, we should begin constructing a new skeleton
        if (inProgress == null)
        {
            inProgress = new Skeleton(currentKey);
            statusImage.gameObject.SetActive(true);
            for (int i = 0; i < 15; i++)
            {
                statusImage.transform.GetChild(i).gameObject.GetComponent<Image>().color = Color.red;
            } //construct the necessary components for the actor business
        }

        switch (clr)
        {
            case 0:
                inProgress.bones[centerIndex + 12] = new Bone(pos, centerIndex + 12);
                statusImage.transform.GetChild(centerIndex + 12).gameObject.GetComponent<Image>().color = Color.blue;
                inProgress.addJointCenter(centerIndex);
                centerIndex++;
                break;
            case 1:
                inProgress.bones[leftIndex] = new Bone(pos, leftIndex);
                statusImage.transform.GetChild(leftIndex).gameObject.GetComponent<Image>().color = Color.blue;
                inProgress.addJointLeft(leftIndex);
                leftIndex++;
                break;
            case 2:
                inProgress.bones[rightIndex + 6] = new Bone(pos, rightIndex + 6);
                statusImage.transform.GetChild(rightIndex + 6).gameObject.GetComponent<Image>().color = Color.blue;
                inProgress.addJointRight(rightIndex);
                rightIndex++;
                break;
        }


        //subsequent input requires us to rebuild actor
        mainActor.pose(inProgress);

        //once we have 15 skeleton.bones we can package the actor and send it to the timeline
        for (int i = 0; i < 15; i++)
        {
            if (inProgress.bones[i] == null)
                return;
        }

        inProgress.bones[15] = new Bone(Camera.main.transform.position, 15);
        inProgress.bones[12].addChild(inProgress.bones[15]);

        Timeline.Instance.addKeyframe(inProgress);
        GameObject newKeyIcon = (GameObject)Instantiate(sliderKeyPrefab, sliderText.transform.position, new Quaternion());
        newKeyIcon.transform.SetParent(slider.transform.parent);
        newKeyIcon.transform.localScale = new Vector3(1, 1, 1);
        newKeyIcon.transform.position -= new Vector3(0, 10, 0);
        slider.value += 10;

    }

    public void play()
    {
        slider.value = 0;
        playing = !playing;
        if (playing) StartCoroutine(playRoutine());
    }

    IEnumerator playRoutine()
    {
        onionActor.gameObject.SetActive(false);
        while (playing)
        {
            slider.value += 1;
            if (slider.value > Timeline.Instance.keyframes[Timeline.Instance.keyframes.Count - 1].key + 10) slider.value = 0;
            yield return new WaitForSeconds(1.0f / (float)playbackSpeed);
        }
        onionActor.gameObject.SetActive(true);
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
        recording = false;
        slider.value = 0;
    }

    IEnumerator moveThings(int old)
    {
        float t = 0;
        float interpolant = 0;
        int offset = 0;

        while (old + t < currentKey)
        {
            offset = (int)Mathf.Floor(t);
            interpolant = t - offset;
            lpp.transform.position = Vector3.Lerp(LPoints[old + offset], LPoints[old + offset + 1], interpolant) + offsetV;
            rpp.transform.position = Vector3.Lerp(RPoints[old + offset], RPoints[old + offset + 1], interpolant) + offsetV;
            t += 0.15f;
            yield return null;
        }
    }

    public void inputReceived()
    {
        this.playbackSpeed = int.Parse(input.text);
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
                leftCtrl.PadClicked += lCenterClicked;
                leftCtrl.Gripped += clearFrame;
            }
        }

        if (!rightCtrl)
        {
            GameObject rightObject = GameObject.Find("Controller (right)");
            if (rightObject)
            {
                rightCtrl = rightObject.GetComponent<SteamVR_TrackedController>();
                rightCtrl.TriggerClicked += rightClicked;
                rightCtrl.PadClicked += rCenterClicked;
                rightCtrl.Gripped += clearFrame;
            }
        }
    }

    void leftClicked(object sender, ClickedEventArgs e)
    {
        if (recording) StartCoroutine(recordRoutine());
        else if (leftIndex < 6) addBone(leftCtrl.transform.position, 1);
    }

    void rightClicked(object sender, ClickedEventArgs e)
    {
        if (recording) StartCoroutine(recordRoutine());
        else if (rightIndex < 6) addBone(rightCtrl.transform.position, 2);
    }

    void lCenterClicked(object sender, ClickedEventArgs e)
    {
        if (centerIndex < 3) addBone(leftCtrl.transform.position, 0);
    }

    void rCenterClicked(object sender, ClickedEventArgs e)
    {
        if (centerIndex < 3) addBone(rightCtrl.transform.position, 0);
    }

    void clearFrame(object sender, ClickedEventArgs e)
    {
        ChangeFrame();
    }

    public void startRecord ()
    {
        recording = true;
    }

}
