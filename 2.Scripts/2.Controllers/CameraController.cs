using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float _sensitivity = 0;    //  카메라 회전 민감도
    [SerializeField] float _clampAngle = 0;     // 카메라 상하 회전 제한 각도

    float rotX, rotY;   //  마우스 회전 입력값

    float _cameraMaxDistance = 0f;      //  카메라의 최대 거리
    Vector3 _camDir;    //  카메라루트에서 카메라 방향
    PlayerController _player;

    void Start()
    {
        //  카메라루트와 카메라의 거리
        _cameraMaxDistance = (Camera.main.transform.position - transform.position).magnitude;
        //카메라방향
        _camDir = (Camera.main.transform.position - transform.position).normalized;
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void LateUpdate()
    {
        if (UserInfo._instance._isOpenedUI || _player._isDead) return;

        _sensitivity = DataManager._instance._optionData._sensitivity;

        //  카메라 회전
        rotX += Input.GetAxis("Mouse Y") * _sensitivity;
        rotY += Input.GetAxis("Mouse X") * _sensitivity;
        
        if (rotX != 0 || rotY != 0)
        {
            rotX = Mathf.Clamp(rotX, -_clampAngle, _clampAngle);
            Quaternion rotate = Quaternion.Euler(-rotX, rotY, 0);
            transform.rotation = rotate;

        }

        //  카메라가 케릭터를 따라간다.
        Vector3 camPos = GameObject.FindGameObjectWithTag("CameraRootPos").transform.position;
        transform.position = camPos;

        //  레이 방향
        Vector3 rayDirection = transform.forward * -_cameraMaxDistance;
        RaycastHit hit;
        //  카메라루트에서 카메라 방향으로 Ray를 쏜다.
        if (Physics.Raycast(transform.position, rayDirection, out hit, _cameraMaxDistance))
        {
            if (!hit.collider.gameObject.CompareTag("Player"))
            {
                Camera.main.transform.position = hit.point;
            }
        }
        else
        {
            //  카메라를 카메라 최대 거리로 옮긴다.
            Camera.main.transform.localPosition = Vector3.zero;
            Camera.main.transform.Translate(_camDir * _cameraMaxDistance);
        }
    }

    public IEnumerator GameOverCameraMove()
    {
        IngameManager._instance._isCameraMoving = true;
        Vector3 targetPos = transform.position;
        targetPos.y += 10;
        Vector3 dir = _player.transform.position - transform.position;
        yield return new WaitForSeconds(2.0f);
        while (true)
        {
            yield return null;
            //  카메라 이동
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetPos, Time.deltaTime);

            //  카메라 시점 케릭터 고정
            Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime);


            //  이동끝나면 isCameraMoving false
            if ((Camera.main.transform.position - targetPos).magnitude < 0.1f)
            {
                IngameManager._instance._isCameraMoving = false;
                break;
            }
        }
    }
}
