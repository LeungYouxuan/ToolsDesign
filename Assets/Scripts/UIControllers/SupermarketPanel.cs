using System;
using System.Collections.Generic;
using Models;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using Queries;
using SOScripts;
using Test;
using TypeEvents;

namespace UIControllers
{
	public class SupermarketPanelData : UIPanelData
	{
	}
	public partial class SupermarketPanel : UIPanel,IController
	{
		[SerializeField] private GameItemUIController productPrefab;
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as SupermarketPanelData ?? new SupermarketPanelData();
			this.RegisterEvent<ShowSupermarketMessageBoxEvent>(e =>
			{
				MessageBox.gameObject.SetActive(true);
				var box = MessageBox.GetComponent<SupermarketMessageBoxController>();
				box.GetComponent<SupermarketMessageBoxController>().priceText.text
					= e.price.ToString();
				box.itemPos=e.pos;
			}).UnRegisterWhenGameObjectDestroyed(gameObject);
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
		}
		
		protected override void OnShow()
		{

		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
		}

		private void ShowMarket()
		{
			var market = this.SendQuery(new GetSupermarketQuery());
			if (market == null)
			{
				Debug.LogError("获取超市类失败！");
				return;
			}
			List<GameItem>result=market.GetAllGameItemWithPosFromInventory(out var posList);
			List<InventoryGridInfo> gridInfoList = market.GetInventoryGridInfoList();
			for (int i = 0; i < result.Count; i++)
			{
				var pos = posList[i];
				var product = Instantiate(productPrefab, ProductGrid.GetChild(pos));
				if (gridInfoList[i].GetType() == typeof(BigRunFaSupermarket.BigRunFaSupermarketGridInfo))
				{
					var gridInfo= (BigRunFaSupermarket.BigRunFaSupermarketGridInfo)gridInfoList[i];
					product.SetProperty(result[i].sprite,gridInfo.price,gridInfo.count);
				}
			}
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				ShowMarket();
			}
		}

		public IArchitecture GetArchitecture()
		{
			return GameArchitecture.Interface;
		}
	}
}
