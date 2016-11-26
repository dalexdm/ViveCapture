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

    public GameObject fing;

    bool isHolding;

	// Use this for initialization
	void Start () {
        ctrl = gameObject.GetComponent<SteamVR_TrackedController>();
        if (ctrl) ctrl.MenuButtonClicked += openUI;
        setFings();
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

            //float theta = Vector3.Angle(ctrl.transform.position - startpos, endpos - startpos);
            //float mag = Vector3.Magnitude(ctrl.transform.position - startpos) * Mathf.Cos(theta * Mathf.Deg2Rad);
            //mag /= Vector3.Magnitude(endpos - startpos);
            //sm.slider.value = Mathf.Max(0, Mathf.Min(mag * sm.slider.maxValue, sm.slider.maxValue));
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
        menu.transform.position = transform.position + transform.forward + Vector3.up;
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
                break;
            case 1:
                sm.recording = false;
                sm.playing = false;
                sm.editing = false;
                sm.leftHolding = null;
                sm.rightHolding = null;
                break;
            case 2:
                sm.playing = false;
                sm.recording = false;
                sm.editing = true;
                break;
            case 3:
                sm.play();
                sm.recording = false;
                sm.editing = false;
                sm.leftHolding = null;
                sm.rightHolding = null;
                break;
            case 4:
                sm.delete();
                sm.leftHolding = null;
                sm.rightHolding = null;
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
            fing.transform.GetChild(0).GetChild(i).gameObject.GetComponent<Image>().color = Color.black;
            fing.transform.GetChild(0).GetChild(i).GetChild(0).gameObject.GetComponent<Image>().color = Color.white;
        }

        if (sm.editing)
        {
            fing.transform.GetChild(0).GetChild(2).gameObject.GetComponent<Image>().color = Color.white;
            fing.transform.GetChild(0).GetChild(2).GetChild(0).gameObject.GetComponent<Image>().color = Color.black;
        } else if (sm.recording)
        {
                fing.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>().color = Color.white;
                fing.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<Image>().color = Color.black;
        } else
        {
            fing.transform.GetChild(0).GetChild(1).gameObject.GetComponent<Image>().color = Color.white;
            fing.transform.GetChild(0).GetChild(1).GetChild(0).gameObject.GetComponent<Image>().color = Color.black;
        }
    }
}