using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSwitchTrigger : MonoBehaviour
{
	[Header("场景切换设置")]
	[Tooltip("要切换到的场景名称")]
	public string targetSceneName;
	[SerializeField] private KeyCode key = KeyCode.A;

	[Header("触发设置")]
	[SerializeField] private bool ignoreSelf = true;
	[SerializeField] private Collider2D triggerCollider;

	void Awake()
	{
		if (triggerCollider == null)
		{
			triggerCollider = gameObject.GetComponent<Collider2D>();
		}
	}

	private void OnTriggerStay2D(Collider2D other)
	{
		if (ignoreSelf && other.transform == transform)
		{
			return;
		}

		if (other.CompareTag("Player") && Input.GetKey(key) && GameDataManager.Instance.IsSceneUnlocked(2))
		{
			SwitchScene();
		}
	}

	private void SwitchScene()
	{
		if (string.IsNullOrEmpty(targetSceneName))
		{
			Debug.LogError("目标场景名称未设置！", this);
			return;
		}
		Loader.SetTargetScene(targetSceneName);
	}
}
