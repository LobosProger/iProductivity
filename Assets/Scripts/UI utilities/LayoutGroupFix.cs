using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class LayoutGroupFix : MonoBehaviour
{
	[SerializeField] private bool _fixLayoutAtStart;
	[Space] 
	[SerializeField] private bool _fixAlsoContentSizeFitter;
	[SerializeField] private ContentSizeFitter _contentSizeFitter;
	
    private LayoutGroup _layoutGroup;

	private void Awake()
	{
		_layoutGroup = GetComponent<LayoutGroup>();

		if (_fixLayoutAtStart)
		{
			FixLayout();
		}
	}

	[Button]
	public void FixLayout()
	{
		if (_fixAlsoContentSizeFitter)
		{
			_ = FixingLayoutAndFixContentSizeFitter();
		}
		else
		{
			_ = FixingLayout();
		}
	}

	private async UniTask FixingLayout()
	{
		await UniTask.WaitForEndOfFrame();
		_layoutGroup.enabled = false;
		await UniTask.WaitForEndOfFrame();
		_layoutGroup.enabled = true;
	}
	
	private async UniTask FixingLayoutAndFixContentSizeFitter()
	{
		// Content size fitter need also fixing, because if just disable layout group and will remain content size fitter
		// with same properties (like vertical or horizontal fit), it will break visual layout (for instance, top panel with
		// crystals, when amount of crystals is changing)
		
		await UniTask.WaitForEndOfFrame();
		
		_contentSizeFitter.enabled = false;
		_layoutGroup.enabled = false;
		
		await UniTask.WaitForEndOfFrame();
		
		_layoutGroup.enabled = true;
		_contentSizeFitter.enabled = true;
	}
}
