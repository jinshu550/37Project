using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuButtonBinder : MonoBehaviour
{
    [Header("面板设置")]
    [SerializeField] private Button closeBtn;      // 面板内关闭按钮

    void Awake()
    {
        //绑定关闭按钮事件
        if (closeBtn != null)
            closeBtn.onClick.AddListener(CloseTeamPanel);
    }


    private void CloseTeamPanel()
    {
        if (gameObject != null && gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }

}
