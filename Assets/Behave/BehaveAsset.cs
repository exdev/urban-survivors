using UnityEngine;
using BehaveLibrary;



[System.Serializable]
public class BehaveAsset : ScriptableObject, IBehaveAsset
{
	[HideInInspector]
	public byte[] data;
	
	
	public byte[] Data
	{
		get
		{
			return data;
		}
		set
		{
			data = value;
		}
	}
}