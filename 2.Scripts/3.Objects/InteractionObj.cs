using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineUtility;

public class InteractionObj : MonoBehaviour
{
    [SerializeField] eUIType _myUIType;
    [SerializeField] string _interactionMessage;
    PrisonUIWindow _prisonUI;

    bool _isTrigger = false;
    
    private void Update()
    {
        if (!_isTrigger) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            _prisonUI.SetActiveUI(_myUIType,true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            if (_prisonUI == null)
                _prisonUI = GameObject.FindGameObjectWithTag("PrisonUIWindow").GetComponent<PrisonUIWindow>();

            //  상호작용 메뉴 이름 정하기
            _prisonUI.SetInteractionMessage(_interactionMessage);
            _prisonUI.SetActiveUI(eUIType.Interaction,true);
            _isTrigger = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            _prisonUI.SetActiveUI(eUIType.Interaction, false);
            _isTrigger = false;
        }
    }
}
