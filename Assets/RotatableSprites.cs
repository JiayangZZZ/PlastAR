﻿using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class RotatableSprites : MonoBehaviour {

    public GameObject _ControlSlider;                           // rotation slider
    
    private GameObject _CastModels;                              // container that stores models of the casts
    

    public GameObject _HotSpotSprites;

    public Camera _cam;                                         // fake camera that handles hotspot placement projection on 2D canvas from 3D space

    public Text _HotSpotDescription;                            // Text field that displays the description of the hotspot

    public GameObject _HotSpotsPanel;                           // Panel that contains hotspot description

    public int _frameAmount;                                   // amount of rotation frames

    public Hotspot2D[] _hotspotsInfo;

    public Transform[] _HotSpotsSpritesTransform;              // Transform of the list of HotSpotsSprites that are displayed on UI

    private int _heightOffset;                            // height offset due to the placement of the cast

    public Sprite[][] BG;                                         // container that stores frames of rotation of the current cast model

    private int _currentHotspot = -1;

    public GameObject _currentCastModel;

    public int _currentFrame = 0;


    public int _startFrame = 0;

    public float minSwipeDistY;
 
    public float minSwipeDistX;

    public bool _wrapOver;
         
    private Vector2 startPos;

    private Touch oldTouch1;
    private Touch oldTouch2;

    public GameObject _onboarding;

    private bool _isOnboardingFading = false;

    public Sprite _visitedSprite;

    public GameObject _UIController;

    public bool _isInHotspot = false;

    // Use this for initialization
    void Awake()
    {
        _cam = GameObject.FindGameObjectWithTag("HotSpotCamera").GetComponent<Camera>();
        // _HotSpotSprites = transform.parent.FindChild("HotSpotSprites").gameObject;
    }

    // enable cast model and hotspots


    void Start() {
        // _HotSpotSprites = transform.parent.FindChild("HotSpotSprites").gameObject;
        // _UIController = GameObject.FindGameObjectWithTag("UIController");
    }
    void Update() {
        if (Input.touchCount <= 0)
        {
            return;
        }

        if (1 == Input.touchCount)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 deltaPos = touch.deltaPosition;
            int frameChange =  (_currentFrame + (int) (-deltaPos.x / 2)  + _frameAmount ) % _frameAmount;
            SetFrame(frameChange);
            if (_onboarding.activeSelf && !_isOnboardingFading) {
                StartCoroutine(DisableOnboarding());
            }            
            //transform.Rotate(Vector3.right * deltaPos.y, Space.World);
        }

        

        

    }

    public void FadeOut(GameObject target, float time) {
        iTween.ValueTo(gameObject, iTween.Hash("from", 1.0f, "to", 0.0f, "time", time, "easetype", "linear","onupdate", (Action<object>) (newAlpha => {
			Image[] images = target.GetComponentsInChildren<Image>();
         	foreach (Image mObj in images) {
					mObj.color = new Color(
					mObj.color.r, mObj.color.g, 
					mObj.color.b, (float) newAlpha);
			}
		})));
    }

    IEnumerator DisableOnboarding() {
        _isOnboardingFading = true;
        FadeOut(_onboarding, 0.5f);
        yield return new WaitForSeconds(0.5f);
        _onboarding.SetActive(false);
    }

    public void InitializeContent(bool isRemote) {
        string objectName;
        if (isRemote) {
            objectName = "CastModels(Clone)";
        } else {
            objectName = "CastModels";
        }
        _CastModels = GameObject.Find(objectName);
        
        // _CastInfos = _CastModels.GetComponentsInChildren<CastInformation>();
        SetFrame(_startFrame);
        _HotSpotSprites.SetActive(false);
        ToggleHotSpotInfo(0);
        _ControlSlider.GetComponent<Slider>().maxValue = _frameAmount - 1;
        _ControlSlider.GetComponent<Slider>().value = _startFrame;
        Image defaultSprite = GetComponentsInChildren<Image>()[0];
        Color color = defaultSprite.color;
        color.a = 1;
        defaultSprite.color = color;
    }


    // frame control when rotating the model
	public void SetFrame(int frame) {
        _currentFrame = frame;
		if (frame < _frameAmount && frame >= 0) {
            for (int i = 0; i < _hotspotsInfo.Length; i++) {
                transform.GetChild(i).gameObject.GetComponent<Image>().sprite = BG[i][frame];
            }
            RotateModel(frame);
            UpdateHotSpot();
		} else {
            Debug.Log("request frame exceeds total frame count");
        }
		
	}

    public void SetFrame() {
        int frame = _frameAmount - (int) (_ControlSlider.GetComponent<Slider>().value);
        _currentFrame = frame;
		if (frame < _frameAmount && frame >= 0) {
            for (int i = 0; i < _hotspotsInfo.Length; i++) {
                transform.GetChild(i).gameObject.GetComponent<Image>().sprite = BG[i][frame];
            }
            RotateModel(frame);
            UpdateHotSpot();
		} else {
            Debug.Log("request frame exceeds total frame count");
        }
		
	}

    

    // update hotspot inforamtion when a new cast is enabled
    void UpdateHotSpot() {
        for (int i=1; i < _hotspotsInfo.Length; i++) {
            Vector3 screenPos = _cam.WorldToScreenPoint(_hotspotsInfo[i].gameObject.transform.position);
            screenPos.y = (screenPos.y) / 2 * 1.05f - 250 - 70;
            screenPos.x = (screenPos.x) / 2 * 1.05f - 400;
            _HotSpotsSpritesTransform[i].GetComponent<RectTransform>().anchoredPosition = screenPos;
            // enable the hotspot of it is visiable
            _HotSpotsSpritesTransform[i].gameObject.SetActive(_hotspotsInfo[i].CheckVisibility(_cam));
        }
       
    }

    // rotate model
    void RotateModel(int factor) {
        float rotation = factor * 20f;
        _currentCastModel.transform.rotation = Quaternion.Euler(0, rotation, 0);
    }

    

    // toggle display of the hotspot information, update inforamtion
    public void ToggleHotSpotInfo(int hotspotID){
        Debug.Log("toggled");
        if (!_isOnboardingFading && _onboarding.activeSelf && hotspotID > 0) {
            StartCoroutine(DisableOnboarding());
        }
        
        if (hotspotID < _hotspotsInfo.Length && hotspotID >= 0) {
            

            if (hotspotID == 0) {

                if (_HotSpotSprites.activeSelf) {
                    // Debug.Log(_HotSpotSprites.activeSelf);
                    
                    _UIController.GetComponent<MenuClick>().Exit();
                    
                }

                _HotSpotSprites.SetActive(true);

                // _HotSpotsPanel.SetActive(false);
                _HotSpotDescription.text = "Tap on the hotspots to learn about this cast.";

                Image[] fragmentSprites = transform.GetComponentsInChildren<Image>();

                foreach (Transform hotspot in _HotSpotsSpritesTransform) {
                    try {
                        HotspotShine shine = hotspot.GetComponentInChildren<HotspotShine>();
                        shine.Shine();
                        // hotspot.gameObject.SetActive(true);
                        // shine.gameObject.transform.parent.localScale = new Vector3(0.416f, 0.416f, 0.416f);
                    } catch {
                        
                    }
                }

                foreach (Image sprites in fragmentSprites) {
                    float targetAlpha = 0f;
                    if (sprites.transform.GetSiblingIndex() == 0) {
                        targetAlpha = 1f;
                        
                    }
                    Color color = sprites.color;
                    color.a = targetAlpha;
                    sprites.color = color;
                }
                
            } else {
                // update text description

                _HotSpotSprites.SetActive(false);
                
                _HotSpotDescription.text = _hotspotsInfo[hotspotID]._description;
                
                // highlight fragments

                Image[] fragmentSprites = transform.GetComponentsInChildren<Image>();
                foreach (Image sprites in fragmentSprites) {
                    float targetAlpha = 0f;
                    if (sprites.transform.GetSiblingIndex() == hotspotID) {
                        targetAlpha = 0.6f;
                    }
                    if (sprites.transform.GetSiblingIndex() == 0) {
                        targetAlpha = 1f;
                    }
                    Color color = sprites.color;
                    color.a = targetAlpha;
                    sprites.color = color;
                }

                // make changes on hotspot 

                foreach (Transform hotspot in _HotSpotsSpritesTransform) {
                    try {
                        HotspotShine shine = hotspot.GetComponentInChildren<HotspotShine>();
                        shine.StopShine();
                        // hotspot.gameObject.SetActive(false);
                    } catch(Exception e) {
                        Debug.Log("Hotspot stopshine error: " + e.Message);
                    }
                }

                HotspotShine visitedHotspotShine = _HotSpotsSpritesTransform[hotspotID].GetComponentInChildren<HotspotShine>();

                visitedHotspotShine._isVisited = true;
                visitedHotspotShine.transform.parent.GetChild(1).GetComponent<Image>().sprite = _visitedSprite;

                
                
            }

            _currentHotspot = hotspotID;
           
        } else {
            Debug.Log("hotspotID invalid");
        }
    }

}
