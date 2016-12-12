using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ControllerUI : MonoBehaviour {

    SteamVR_TrackedController ctrl;
    public GameObject menu;
    public SceneManager sm;
    public GameObject handle;
    Vector3 endpos;
    Vector3 startpos;
    public ControllerUI otherUI;
    public Transform ring;
    public GameObject fing;
    Color boop;

    bool isHolding;

	// Use this for initialization
	void Start () {
        boop = new Color32(0x3F, 0x3F, 0x3F, 0xFF);
        ctrl = gameObject.GetComponent<SteamVR_TrackedController>();
        if (ctrl) ctrl.MenuButtonClicked += openUI;
        setFings();
        ring = sm.slider.transform.parent.GetChild(1);
    }
	
	// Update is called once per frame
	void Update () {
        if (isHolding)
        {

            handle.transform.position = ctrl.transform.position;
            Vector3 toCtrl = handle.transform.localPosition;
            toCtrl[2] = 0;
            toCtrl.Normalize();

            if (toCtrl[1] < 0.3 && toCtrl[1] > 0 && toCtrl[0] < 0) {
                toCtrl[1] = 0.287f;
                toCtrl[0] = -0.957f;
            } else if (toCtrl[1] > -0.3 && toCtrl[1] < 0 && toCtrl[0] < 0) {
                toCtrl[1] = -0.287f;
                toCtrl[0] = -0.957f;
            }

            //calculate timeline point
            handle.transform.localPosition = toCtrl * 800f;

            float perc;
            if (toCtrl[1] > 0)
            {
                perc = Mathf.Acos(-toCtrl[0]) / 6.2831f;
                perc = (perc - 0.04774f) * 1.1037f;
            }
            else
            {
                perc = Mathf.Acos(-toCtrl[0]) / 6.2831f;
                perc = 1 - (perc - 0.04774f) * 1.1037f;
            }


            sm.slider.value = perc * (sm.slider.maxValue - 1);
        }

        fing.transform.position = this.transform.position;
        fing.transform.rotation = this.transform.rotation;
    }

    void openUI(object sender, ClickedEventArgs e)
    {
        if (menu.activeSelf)
        {
            sm.inputDisabled = false;
            menu.SetActive(false);
            ctrl.TriggerClicked -= grabHold;
            ctrl.TriggerUnclicked -= unHold;
            return;
        }

        sm.inputDisabled = true;
        ctrl.TriggerClicked += grabHold;
        ctrl.TriggerUnclicked += unHold;
        menu.SetActive(true);
        menu.transform.position = transform.position;
        menu.transform.LookAt(Camera.main.transform.position + 2 * (menu.transform.position - Camera.main.transform.position));
        endpos = sm.slider.transform.GetChild(4).position;
        startpos = sm.slider.transform.GetChild(3).position;

        StartCoroutine(detectPress());
    }

    void grabHold (object sender, ClickedEventArgs e)
    {
        if (Vector3.SqrMagnitude(handle.transform.position - transform.position) < 0.25f)
        {
            isHolding = true;
        }
    }

    void unHold(object sender, ClickedEventArgs e)
    {
        isHolding = false;
    }

    IEnumerator detectPress ()
    {

        Quaternion basefing = Quaternion.Euler(menu.transform.localRotation.eulerAngles);
        Quaternion baseRing = Quaternion.Euler(0, 0, 0);
        menu.transform.localRotation = Quaternion.Euler(menu.transform.localRotation.eulerAngles + new Vector3(0,0,180));
        ring.localRotation = Quaternion.Euler(0,0,-180);
        handle.SetActive(false);

        float perc = 0;
        while (true)
        {
            menu.transform.localRotation = Quaternion.Slerp(menu.transform.localRotation, basefing, 0.17f);
            ring.localRotation = Quaternion.Slerp(ring.localRotation, baseRing, 0.17f);
            ring.localPosition = new Vector3(ring.localPosition[0], ring.localPosition[1], (1 - perc) * 50);
            perc += 0.0434f;
            menu.GetComponent<CanvasGroup>().alpha = perc * perc * perc;
            ring.GetComponent<CanvasGroup>().alpha = perc * perc * perc;
            if (Quaternion.Angle(menu.transform.localRotation, basefing) < 3) break;
            yield return null;
        }

        handle.SetActive(true);
        float skale = 0;
        while (skale < 1.5f)
        {
            handle.transform.localScale = new Vector3(skale, skale, skale);
            skale += 0.2f;
            yield return null;
        }

        while (true)
        {

            for (int i = 0; i < 5; i++)
            {
                if (Vector3.SqrMagnitude(menu.transform.GetChild(i).position - transform.position) < 0.15f && !isHolding)
                {
                    action(i);
                    sm.inputDisabled = false;
                    menu.SetActive(false);
                    ctrl.TriggerClicked -= grabHold;
                    ctrl.TriggerUnclicked -= unHold;
                    yield break;
                }
            }
            yield return null;
        }
    }

    void action (int i)
    {
        switch (i)
        {
            case 0:
                sm.playing = false;
                sm.slider.value = 0;
                sm.editing = false;
                sm.leftHolding = null;
                sm.rightHolding = null;
                sm.startRecord();
                sm.isScaling = false;
                break;
            case 1:
                sm.recording = false;
                sm.playing = false;
                sm.editing = false;
                sm.leftHolding = null;
                sm.rightHolding = null;
                sm.isScaling = true;
                sm.scale();
                break;
            case 2:
                sm.playing = false;
                sm.recording = false;
                sm.editing = true;
                sm.isScaling = false;
                break;
            case 3:
                sm.play();
                sm.recording = false;
                sm.editing = false;
                sm.leftHolding = null;
                sm.rightHolding = null;
                sm.isScaling = false;
                break;
            case 4:
                sm.delete();
                sm.leftHolding = null;
                sm.rightHolding = null;
                sm.isScaling = false;
                break;
            default:
                break;
        }

        setFings();
        otherUI.setFings();
    }

    void setFings()
    {
        for (int i = 0; i < 3; i ++)
        {
            fing.transform.GetChild(0).GetChild(i).gameObject.GetComponent<Image>().color = boop;
            fing.transform.GetChild(0).GetChild(i).GetChild(0).gameObject.GetComponent<Image>().color = Color.white;
        }

        if (sm.editing)
        {
            fing.transform.GetChild(0).GetChild(2).gameObject.GetComponent<Image>().color = Color.white;
            fing.transform.GetChild(0).GetChild(2).GetChild(0).gameObject.GetComponent<Image>().color = boop;
        } else if (sm.recording)
        {
                fing.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>().color = Color.white;
                fing.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<Image>().color = boop;
        } else
        {
            fing.transform.GetChild(0).GetChild(1).gameObject.GetComponent<Image>().color = Color.white;
            fing.transform.GetChild(0).GetChild(1).GetChild(0).gameObject.GetComponent<Image>().color = boop;
        }
    }
}