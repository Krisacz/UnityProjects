using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Controllers;
using UnityEngine;

public class SequencerTest : MonoBehaviour
{
	void Start ()
	{
	    var go = this.gameObject;

        //Basic Time Test
        //SequenceController.Instance.AddSingleTimeLink(4f, () => Log.Info("yay!"));

        //More advanced time test
        //SequenceController.Instance.AddChain(
        //    SequenceController.Instance.AddTimeLink(5f, 
        //    () => { Log.Info("1"); go.transform.position = new Vector3(3f, 3f, 0f); }),
        //    SequenceController.Instance.AddTimeLink(4f,
        //    () => { Log.Info("2"); go.transform.position = new Vector3(-3f, -3f, 0f); }),
        //    SequenceController.Instance.AddTimeLink(3f,
        //    () => { Log.Info("3"); go.transform.position = new Vector3(-3f, 3f, 0f); }),
        //    SequenceController.Instance.AddTimeLink(2f,
        //    () =>{ Log.Info("4"); go.transform.position = new Vector3(3f, -3f, 0f); }));

        //Tween chain
        //SequenceController.Instance.AddTweenChain(
        //    () => { Log.Info("All tweens are completed!"); },
        //    new MoveToTweenLink(go, new Vector3(3f, 3f, 0f), 3f, iTween.EaseType.easeInOutBack),
        //    new MoveToTweenLink(go, new Vector3(-3f, -3f, 0f), 3f, iTween.EaseType.easeInOutBounce),
        //    new MoveToTweenLink(go, new Vector3(-3f, 3f, 0f), 3f, iTween.EaseType.easeInOutCirc),
        //    new MoveToTweenLink(go, new Vector3(3f, -3f, 0f), 3f, iTween.EaseType.easeInOutElastic),
        //    new MoveToTweenLink(go, new Vector3(3f, 3f, 0f), 3f, iTween.EaseType.easeInOutExpo),
        //    new MoveToTweenLink(go, new Vector3(-3f, -3f, 0f), 3f, iTween.EaseType.easeInOutSine)
        //    );
    }
}
