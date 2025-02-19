﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hotspot2D : MonoBehaviour {

	public string _name;
	
	[TextArea(3,10)]
	public string _description;

	public Sprite[] _sprites;

	public bool CheckVisibility(Camera cam) {
		RaycastHit hit;
		float sizeFactor = 0f;
		/* 
		Vector3[] edgePoints = new Vector3[] {
			new Vector3(transform.position.x + sizeFactor, transform.position.y + sizeFactor, transform.position.z),
			// new Vector3(transform.position.x + sizeFactor, transform.position.y - sizeFactor, transform.position.z),
			// new Vector3(transform.position.x - sizeFactor, transform.position.y - sizeFactor, transform.position.z),
			// new Vector3(transform.position.x - sizeFactor, transform.position.y + sizeFactor, transform.position.z),
		};

		// Debug.Log(direction);
		foreach (Vector3 e in edgePoints) {
			if(Physics.Raycast(e, cam.transform.position - e, out hit)) {
				Debug.DrawRay(e, cam.transform.position - e, Color.red, 10f);
				Debug.Log((hit.collider.tag));
				if (hit.collider.tag == "HotSpotCamera") {
					return true;
				} else {
					return false;
				};
			} else {
				return false;
			}
		}

		return true;

		*/

		Vector3 e = new Vector3(transform.position.x, transform.position.y, transform.position.z);

		if(Physics.Raycast(e, cam.transform.position - e, out hit)) {
			// Debug.DrawRay(e, cam.transform.position - e, Color.red, 10f);
			// Debug.Log((hit.collider.tag));
			if (hit.collider.tag == "HotSpotCamera") {
				return true;
			} else {
				return false;
			};
		} else {
			return false;
		}
		
	}
}
