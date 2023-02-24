using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _conveyor.Mobile3DFPS.Customization
{
	public class Cosmetic : MonoBehaviour
	{
		[Range(30, 50)]public int Cost;
		
		private CharacterCustomizer custom;
		public bool hasBrought { get; set; }
		
		public void Add()
		{
			if(custom == null)
				custom = FindObjectOfType<CharacterCustomizer>();
				
			custom.enabledCosmetics.Add(this);
		}
		
		public void Remove()
		{
			if(custom == null)
				return;
				
			if(custom.enabledCosmetics.Contains(this))
				custom.enabledCosmetics.Remove(this);
		}
	}
}









