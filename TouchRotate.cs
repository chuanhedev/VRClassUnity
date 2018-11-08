using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchRotate : MonoBehaviour {

	//float x;

	//float y;

	public float dragSpeed = 5.0f;

	//float ySpeed = 5.0f;

	float pinchSpeed;

	float distance = 10f;

	float minimumDistance = 5f;

	float maximumDistance = 100f;

	Touch touch;

	float lastDist = 0f;

	float curDist = 0f;

	public float maximumScale = 3.0f;
	public float minimumScale = 0.5f;

	private float accumulateScale = 1.0f;

	private float accumulateXAngle = 0;
	private float accumulateYAngle = 0;

	private Quaternion originalRotation;
	private Vector3 originalScale;
	private Vector3 originalPosition;

	private bool isDown;
	private Vector3 mouseDownPosition;
	private Vector3 prevMousePosition;
	public Text text;
	public float minMouseDragDis;
	Quaternion touchedRotation;

	void Awake(){
		MainCamera.instant = this.GetComponent<Camera>();
		originalRotation = this.transform.localRotation;
		originalScale = this.transform.localScale;
		originalPosition = this.transform.localPosition;

	}

	void OnEnable(){

		this.transform.localRotation = originalRotation;
		this.transform.localPosition = originalPosition;
		this.transform.localScale = originalScale;
	}

	void Update () 
	{
		// if (!SceneController.instant.enabled) {
		// 	isDown = false;
		// 	return;
		// }
		if (Input.GetMouseButtonDown(0)) {
			//Debug.Log ("down");
			prevMousePosition = mouseDownPosition = Input.mousePosition;
			touchedRotation = transform.localRotation;
			isDown = true;
		}

		if (isDown) {

			mouseDownPosition = Input.mousePosition;
			touchedRotation = transform.localRotation;
			float dis = (Input.mousePosition - prevMousePosition).magnitude;

			if (dis < minMouseDragDis) {

			} else {
				//prevMousePosition = Input.mousePosition;
				float x = (Input.mousePosition.x - prevMousePosition.x) * dragSpeed * Time.deltaTime;
				float y = (Input.mousePosition.y - prevMousePosition.y) * dragSpeed * Time.deltaTime;
				//Debug.Log (x.ToString () + " " + y.ToString ());
				//Debug.Log(touchedRotation.eulerAngles.x - y);
				float targetX =( touchedRotation.eulerAngles.x > 180 ? touchedRotation.eulerAngles.x - 360 : touchedRotation.eulerAngles.x)-y;
				transform.localRotation = Quaternion.Euler ( Mathf.Clamp(targetX, -80f, 80f), touchedRotation.eulerAngles.y + x, 0);
				prevMousePosition =  mouseDownPosition;
			}
			
		}

		if(Input.GetMouseButtonUp(0)){
			isDown = false;
		}

	}
		
}


public class MainCamera{

	public static Camera instant;
}