using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace UIControllers
{
	// Generate Id:4a1bf95d-bcfd-4597-99f7-e8a4bd1dd2a6
	public partial class SupermarketPanel
	{
		public const string Name = "SupermarketPanel";
		
		[SerializeField]
		public UnityEngine.RectTransform ProductGrid;
		[SerializeField]
		public UnityEngine.UI.Image MessageBox;
		
		private SupermarketPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			ProductGrid = null;
			MessageBox = null;
			
			mData = null;
		}
		
		public SupermarketPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		SupermarketPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new SupermarketPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
