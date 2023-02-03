using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class slide : MonoBehaviour
{
	[SerializeField] public Slider _slider;
	[SerializeField] public TMPro.TextMeshProUGUI _slideText;
    // Start is called before the first frame update
    void Start()
	{
		_slider.onValueChanged.AddListener((value) => {
			_slideText.text = value.ToString("0");
		});
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
