using UnityEngine;
using UnityEngine.UI;

public class Slide : MonoBehaviour {
	Image filler;
	public Slider slider;

	// Use this for initialization
	void Start () {
		filler = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
		filler.fillAmount = slider.value;
	}
}
