using System.Collections.Generic;
using UnityEngine.UI;

public class ToggleGroupExtended : ToggleGroup
{
	public List<Toggle> GetAllToggles() => m_Toggles;
}
