using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class hasPart : MonoBehaviour
{
    [Header("Button")]
    [SerializeField] public GameObject button;
    // Start is called before the first frame update

    [Header("banner")]
    [SerializeField] public GameObject Banner;

    [Header("Fan Not Overlapped")]
    [SerializeField] public GameObject fan;

     [Header("Fan Overlapped")]
    [SerializeField] public GameObject fan1;

    private void Update() {
        hasChild();
    }

    private void hasChild() {

        if(transform.childCount > 0) {
            button.SetActive(true);
            Banner.SetActive(true);
            fan.SetActive(false);
            fan1.SetActive(true);
        } else {
            //button.SetActive(false);
            //Banner.SetActive(true);
        }
    }

}
