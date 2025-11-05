using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElvesUndeadDialogueEnd : MonoBehaviour
{
    
    [SerializeField] private NPCDialogueSystem npcDialogueSystem;
    public DialogueLine[] repeatDialogue; // 重复对话内容（首次对话后触发）

    [Header("选项对话内容")]
    [SerializeField] private string optionsDialogue1Txt; // 选项内容
    [SerializeField] private DialogueLine[] optionsDialogue1; // 选项对话内容
    [SerializeField] private string optionsDialogue2Txt; // 选项内容
    [SerializeField] private DialogueLine[] optionsDialogue2; // 选项对话内容

    void Update()
    {
        
        if (npcDialogueSystem.isDialogueActive == true)
            //如果有选项 根据不同的选项播放不同的对话
            OptionsDialogue();
    }
    private void ElvenUndeadFirstMeeting()
    {
        if (!GameDataManager.Instance.IsElvenUndeadFirstMeeting())
        {
            npcDialogueSystem = GetComponent<NPCDialogueSystem>();
            npcDialogueSystem.repeatDialogue = repeatDialogue;
        }
    }

    private void OptionsDialogue()
    {
        for (int i = 0; i < npcDialogueSystem.repeatDialogue.Length; i++)
        {
            if (npcDialogueSystem.repeatDialogue[i].options != null && npcDialogueSystem.repeatDialogue[i].options.Count > 0)
            {
                for (int j = 0; j < npcDialogueSystem.repeatDialogue[i].options.Count; j++)
                {
                    if (npcDialogueSystem.repeatDialogue[i].options[j].optionText == optionsDialogue1Txt)
                    {
                        npcDialogueSystem.repeatDialogue[i].options[j].nextDialogue = optionsDialogue1;
                    }
                    else if (npcDialogueSystem.repeatDialogue[i].options[j].optionText == optionsDialogue2Txt)
                    {
                        npcDialogueSystem.repeatDialogue[i].options[j].nextDialogue = optionsDialogue2;
                    }
                }
            }
        }

    }
}
