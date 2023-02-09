using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float _sensitivity = 0;    //  ī�޶� ȸ�� �ΰ���
    [SerializeField] float _clampAngle = 0;     // ī�޶� ���� ȸ�� ���� ����

    float rotX, rotY;   //  ���콺 ȸ�� �Է°�

    float _cameraMaxDistance = 0f;      //  ī�޶��� �ִ� �Ÿ�
    Vector3 _camDir;    //  ī�޶��Ʈ���� ī�޶� ����
    PlayerController _player;

    void Start()
    {
        //  ī�޶��Ʈ�� ī�޶��� �Ÿ�
        _cameraMaxDistance = (Camera.main.transform.position - transform.position).magnitude;
        //ī�޶����
        _camDir = (Camera.main.transform.position - transform.position).normalized;
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void LateUpdate()
    {
        if (UserInfo._instance._isOpenedUI || _player._isDead) return;

        _sensitivity = DataManager._instance._optionData._sensitivity;

        //  ī�޶� ȸ��
        rotX += Input.GetAxis("Mouse Y") * _sensitivity;
        rotY += Input.GetAxis("Mouse X") * _sensitivity;
        
        if (rotX != 0 || rotY != 0)
        {
            rotX = Mathf.Clamp(rotX, -_clampAngle, _clampAngle);
            Quaternion rotate = Quaternion.Euler(-rotX, rotY, 0);
            transform.rotation = rotate;

        }

        //  ī�޶� �ɸ��͸� ���󰣴�.
        Vector3 camPos = GameObject.FindGameObjectWithTag("CameraRootPos").transform.position;
        transform.position = camPos;

        //  ���� ����
        Vector3 rayDirection = transform.forward * -_cameraMaxDistance;
        RaycastHit hit;
        //  ī�޶��Ʈ���� ī�޶� �������� Ray�� ���.
        if (Physics.Raycast(transform.position, rayDirection, out hit, _cameraMaxDistance))
        {
            if (!hit.collider.gameObject.CompareTag("Player"))
            {
                Camera.main.transform.position = hit.point;
            }
        }
        else
        {
            //  ī�޶� ī�޶� �ִ� �Ÿ��� �ű��.
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
            //  ī�޶� �̵�
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetPos, Time.deltaTime);

            //  ī�޶� ���� �ɸ��� ����
            Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime);


            //  �̵������� isCameraMoving false
            if ((Camera.main.transform.position - targetPos).magnitude < 0.1f)
            {
                IngameManager._instance._isCameraMoving = false;
                break;
            }
        }
    }
}
