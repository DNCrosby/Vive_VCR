// Written By : Derek Crosby
// Copyright 09/2016
// Last Updated : 9/6/2016

// Vive VCR Controller, to add functionality for scrubbing through animation for animation reviews in VR
// Uage: Drop onto Vive Controller, and drag in an animation object. If Neccessary change the animation name that needs to be played back.
// Currently only works with one animation.
// TODO: Add Animation List to be able to play all animations at once, or sequence through them.

using UnityEngine;
using System.Collections;

public class ViveVCR : MonoBehaviour
{
    [SerializeField]
    Animation anim;
    [SerializeField]
    SteamVR_TrackedObject trackedObj;
    SteamVR_Controller.Device device;

    //VTR Parameters 
    public int maxPlaybackSpeed = 3;
    public int frameAdvancePerTurn = 16;

    private bool updatePosition = false;

    // Jog Shuttle Play Controlls
    private enum JogShuttle { jog, shuttle, play };
    private JogShuttle jogShuttle;
    private float old_controlRotation = 0;
    private float controlRotation;
    private float playbackPosition;
    private Vector2 mStartPosition;
    private Vector2 mEndPosition;

    void Awake()
    {
        if (trackedObj == null)
        {
            trackedObj = GetComponent<SteamVR_TrackedObject>();
        }

        if (anim == null)
        {
            anim = GameObject.FindObjectOfType<Animation>();
        }

    }


    // Use this for initialization
    void Start()
    {
        controlRotation = 0;
        device = SteamVR_Controller.Input((int)trackedObj.index);
        jogShuttle = JogShuttle.play;
        PlayAnimationFromPosition(0f, 0f);
    }


    // Utility for getting angle from pad.
    public static float GetAngleDegree(Vector2 origin, Vector2 target)
    {
        var n = 270 - (Mathf.Atan2(origin.y - target.y, origin.x - target.x)) * 180 / Mathf.PI;
        return n % 360;
    }

    // Update is called once per frame
    void Update()
    {
        if (trackedObj != null && anim != null)
        {

            // Check / Change If Jog Or Shuttle Mode With Touch Button
            if (device.GetPressUp(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad))
            {
                if (jogShuttle == JogShuttle.jog)
                    jogShuttle = JogShuttle.shuttle;
                else if (jogShuttle == JogShuttle.shuttle)
                    jogShuttle = JogShuttle.play;
                else if (jogShuttle == JogShuttle.play)
                    jogShuttle = JogShuttle.jog;

                Debug.Log("Mode is : " + jogShuttle.ToString());
            }


            // Touch down, possible chance for a beginning position
            if ((int)trackedObj.index != -1 && device.GetTouchDown(Valve.VR.EVRButtonId.k_EButton_Axis0))
            {
                updatePosition = true;
                // Record start position
                mStartPosition = new Vector2(device.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0).x,
                    device.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0).y);

            }
            // Touch up , possible chance for a new position
            else if (device.GetTouchUp(Valve.VR.EVRButtonId.k_EButton_Axis0))
            {
                updatePosition = false;
                updatePosition = true;
                //Debug.Log("Tracking Finished");
            }

            else if (updatePosition)
            {
                mEndPosition = new Vector2(device.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0).x,
                                          device.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0).y);

                //if (mEndPosition != new Vector2(0f, 0f))
                controlRotation = GetAngleDegree(Vector2.zero, mEndPosition);

                float delta = 0;
                switch (jogShuttle)
                {
                    case JogShuttle.jog:
                        //ChangeAnimationPosition(playbackPosition + ((controlRotation / 360) * frameAdvancePerTurn));                        
                        //Debug.Log("Changed Position to : " + (playbackPosition + (controlRotation * 360 / frameAdvancePerTurn)).ToString());
                        
                        if (controlRotation != 0)
                        {
                            Debug.Log("Control Rotation is : " + old_controlRotation.ToString());
                            delta = ((controlRotation - old_controlRotation) % 360) * ((frameAdvancePerTurn / 360)*30);

                        }
                        else
                        {
                            delta = 0;
                        }

                        ChangeAnimationPosition(delta + playbackPosition);
                        old_controlRotation = controlRotation;

                        Debug.Log(delta.ToString());
                        break;
                    case JogShuttle.shuttle:
                        if (controlRotation > 240f && controlRotation < 360f)
                        {
                            delta = Mathf.DeltaAngle(0f, controlRotation);
                        }
                        if (controlRotation > 0f && controlRotation < 90f)
                        {
                            delta = Mathf.DeltaAngle(0f, controlRotation);
                        }
                        ChangeAnimationPlaybackSpeed((delta / 90) * maxPlaybackSpeed);
                        Debug.Log("Changed Playback Speed to : " + ((delta / 90) * maxPlaybackSpeed).ToString());
                        break;
                    case JogShuttle.play:
                        if (mEndPosition.x != 0f)
                        {
                            Debug.Log("PlaybackPosition entered : " + playbackPosition.ToString());
                            PlayAnimationFromPosition(playbackPosition, mEndPosition.x);
                            Debug.Log("Position.x : " + mEndPosition.x.ToString());
                        }
                        break;
                    default:
                        Debug.LogError("Could not find " + jogShuttle + " in options for JogShuttleSwitch ");
                        break;
                }
                //updatePosition = false;
            }
            playbackPosition = anim["Take 001"].time;
        }
        else return;


    }


    // Modules to Change Time based on controller input.
    void ChangeAnimationPlaybackSpeed(float speed)
    {
        // Todo Need to get name of animation from animation asset.
        if (!anim.isPlaying)
        {
            anim.Play("Take 001");
        }
        anim["Take 001"].speed = speed;
        anim["Take 001"].enabled = true;

    }


    void ChangeAnimationPosition(float time)
    {
        anim["Take 001"].enabled = true;
        // Todo Need to get name of animation from animation asset.
        if (!anim.isPlaying)
        {
            anim.Play("Take 001");
        }
        anim["Take 001"].speed = 0;
        anim["Take 001"].time = time;
    }

    void PlayAnimationFromPosition(float time, float direction)
    {
        // Todo Need to get name of animation from animation asset.

        //anim["Take 001"].time = time;
        anim["Take 001"].enabled = true;
        if (direction > .5f)
        {
            anim.Play("Take 001");
            anim["Take 001"].speed = 1;
        }
        if (direction < -.5)
        {
            anim.Play("Take 001");
            anim["Take 001"].speed = -1;
        }
        if (direction < .5 && direction > -.5)
        {
            if (anim["Take 001"].speed == 1)
            {
                anim["Take 001"].speed = 0;
            }
            else
            {
                anim["Take 001"].speed = 1;
            }
        }

    }
}
