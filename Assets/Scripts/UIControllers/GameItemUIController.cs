using System.Collections;
using System.Collections.Generic;
using Interface;
using QFramework;
using Queries;
using Test;
using TMPro;
using TypeEvents;
using UnityEngine;
using UnityEngine.UI;

namespace UIControllers
{
    public class GameItemUIController : MonoBehaviour,ICanSetProperty,ICanSendEvent,IController
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text priceText;
        [SerializeField] private TMP_Text countText;
        [SerializeField] private Button pressBt;
        public void SetProperty(Sprite sp,int price,int count)
        {
            icon.sprite = sp;
            priceText.text = price.ToString();
            pressBt.onClick.AddListener(() =>
            {
                var parent = transform.parent;
                var pos = parent.GetSiblingIndex();
                var market = this.SendQuery(new GetSupermarketQuery());
                market.FindGameItemByPos(pos, out var gameItem, out var gridInfo);
                var newGridInfo = (BigRunFaSupermarket.BigRunFaSupermarketGridInfo)gridInfo;
                this.SendEvent(new ShowSupermarketMessageBoxEvent(){price = newGridInfo.price,pos=parent.GetSiblingIndex()});
            });
            //countText.text = count.ToString();
        }

        public IArchitecture GetArchitecture()
        {
            return GameArchitecture.Interface;
        }
    }
}
