using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    [Range(0, 1)]
    public float pMovementSmooth = 0.1f;
    [Range(0, 1)]
    public float pZoomSmooth = 0.1f;
    [Range(0, 100)]
    public float pMovementSpeed = 10;
    [Range(0, 100)]
    public float pZoomSpeed = 40;
    [Range(0, 20)]
    public float pRotateSpeed = 5;
    public int[] pZoomValues = { 30, 25, 20, 15, 10, 5, 2, 1 };
    public bool pBlocked;

    private static Transform mTarget;

    private int mZoomState;
    private Vector3 mTargetPos;
    private Camera mCam;
    private bool mIsLoading;

    private void Start()
    {
        mTargetPos = GridManager.pInstance.GetCenter().transform.position;
        //transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, -60, transform.localEulerAngles.z);
        mCam = Camera.main;
        mZoomState = 3;
    }

    private void Update()
    {
        if (mTarget != null)
        {
            mTargetPos = mTarget.position;
        }

        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

#if !UNITY_EDITOR
            if (Input.mousePosition.x > Screen.width - 20)
            {
                moveX = 1;
            }
            if (Input.mousePosition.x < 20)
            {
                moveX = -1;
            }
            if (Input.mousePosition.y > Screen.height - 20)
            {
                moveY = 1;
            }
            if (Input.mousePosition.y < 20)
            {
                moveY = -1;
            }
#endif
        float zoomFactor = ((float)pZoomValues[mZoomState] / pZoomValues[0]);

        if (moveX != 0 || moveY != 0)
        {
            mTarget = null;

            mTargetPos += transform.right * pMovementSpeed * 10 * zoomFactor * moveX * Time.deltaTime;
            mTargetPos += (transform.forward * pMovementSpeed * 10 * zoomFactor * moveY * Time.deltaTime) +
                        (transform.up * pMovementSpeed * 10 * zoomFactor * moveY * Time.deltaTime);
        }

        //Moves the Camera
        transform.position = Vector3.Slerp(transform.position, new Vector3(mTargetPos.x, transform.position.y, mTargetPos.z), pMovementSmooth);

        //Camera Zoom
        Vector3 pos = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, pZoomValues.Last(), 200), transform.position.z);
        pos.y = Mathf.Lerp(pos.y, pZoomValues[mZoomState], pZoomSmooth);
        transform.position = pos;

        //Camera Rotation on Zooming
        transform.localEulerAngles = new Vector3(Mathf.Clamp(16 + pos.y * 4, 40, 60), transform.localEulerAngles.y, transform.localEulerAngles.z);

        //Change Distance between Camera and Pivot to smaller rotation radius while zoomig in
        
        mCam.transform.localPosition =
            Vector3.Lerp(mCam.transform.localPosition, new Vector3(0, 0, -pZoomValues[mZoomState] * 4), pZoomSmooth);

        if (Input.GetKey(KeyCode.E))
        {
            transform.eulerAngles -= new Vector3(0, pRotateSpeed * Time.deltaTime * 20, 0);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            transform.eulerAngles += new Vector3(0, pRotateSpeed * Time.deltaTime * 20, 0);
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (mZoomState < pZoomValues.Length - 1)
            {
                mZoomState++;
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (mZoomState > 0)
            {
                mZoomState--;
            }
        }
    }

    public static void SetTarget(Transform target)
    {
        mTarget = target;
    }

    public static void ResetTarget()
    {
        mTarget = null;
    }
}
