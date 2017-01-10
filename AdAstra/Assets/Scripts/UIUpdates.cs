using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIUpdates : MonoBehaviour
{
    public void StatTextValueUpdate(GameObject textGameObject)
    {
        var slider = this.GetComponent<Slider>();
        var value = (int)slider.value;
        var text = textGameObject.GetComponent<Text>();
        text.text = string.Format("{0}%", value);
    }
}
