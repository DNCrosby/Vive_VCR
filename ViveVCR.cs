using UnityEngine;
using System.Collections;
//using UnityEngine.Networking;

public class ViveVCR : MonoBehaviour
{
    [SerializeField]
    Animation anim;
    [SerializeField]
    SteamVR_TrackedObject trackedObj;
    SteamVR_Controller.Device device;
    private const int mMessageWidth = 200;
    private const int mMessageHeight = 64;

    private readonly Vector2 mXAxis = new Vector2(1, 0);
    private readonly Vector2 mYAxis = new Vector2(0, 1);
    private bool trackingSwipe = false;
    private bool checkSwipe = false;

    // To recognize as swipe user should at lease swipe for this many pixels
    private const float mMinSwipeDist = 0.2f;

    private Vector2 mStartPosition;
    private Vector2 endPosition;

    private float mSwipeStartTime;
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
        device = SteamVR_Controller.Input((int)trackedObj.index);
    }

    // Update is called once per frame
    void Update()
    {
        if (trackedObj != null && anim != null)
        {
            // Touch down, possible chance for a swipe
            if ((int)trackedObj.index != -1 && device.GetTouchDown(Valve.VR.EVRButtonId.k_EButton_Axis0))
            {
                trackingSwipe = true;
                // Record start time and position
                mStartPosition = new Vector2(device.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0).x,
                    device.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0).y);
                mSwipeStartTime = Time.time;
            }
            // Touch up , possible chance for a swipe
            else if (device.GetTouchUp(Valve.VR.EVRButtonId.k_EButton_Axis0))
            {
                trackingSwipe = false;
                trackingSwipe = true;
                checkSwipe = true;
                Debug.Log("Tracking Finished");
            }
            else if (trackingSwipe)
            {
                endPosition = new Vector2(device.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0).x,
                                          device.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0).y);

            }

            if (checkSwipe)
            {
                checkSwipe = false;

                float deltaTime = Time.time - mSwipeStartTime;

                Vector2 swipeVector = endPosition - mStartPosition;

                //float velocity = swipeVector.magnitude / deltaTime;
                //Debug.Log("VELOCITY : " + velocity);
                if (//velocity > mMinVelocity &&
                    swipeVector.magnitude > mMinSwipeDist)
                {
                    // if the swipe has enough velocity and enough distance


                    swipeVector.Normalize();

                    float angleOfSwipe = Vector2.Dot(swipeVector, mXAxis);
                    angleOfSwipe = Mathf.Acos(angleOfSwipe) * Mathf.Rad2Deg;


                    // Todo Need to get name of animation from animation asset.
                    anim.Play("Take 001");
                    anim.Stop();
                    anim["Take 001"].speed = 0;
                    anim["Take 001"].enabled = true;
                    anim["Take 001"].time = (1f / 24f) * angleOfSwipe;

                    Debug.Log("Angle : " + angleOfSwipe);
                }
            }
        }
        else return;
    }
    
}
