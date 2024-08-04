using System;
using System.Collections;
using System.Collections.Generic;
using Interface;
using QFramework;
using Queries;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIControllers
{
    public class SupermarketMessageBoxController : MonoBehaviour,ICanSetProperty,IController
    {
        public TMP_Text priceText;
        [SerializeField] private Button buyBt;
        [SerializeField] private Button cancelBt;
        [SerializeField] private Button addCountBt;
        [SerializeField] private Button subtractCountBt;
        [SerializeField] private TMP_Text buyCountText;
        [SerializeField] private BindableProperty<int> buyCount=new BindableProperty<int>();
        public int itemPos;
        private void Awake()
        {
            buyCount.Register(value =>
            {
                var market = this.SendQuery(new GetSupermarketQuery());
                if (value < 0)
                {
                    value = 0;
                }
                if (value >= market.GetGridSize())
                {
                    value = market.GetGridSize();
                }
                buyCountText.text = value.ToString();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            cancelBt.onClick.AddListener(() =>
            {
                gameObject.SetActive(false);
            });
            buyBt.onClick.AddListener(() =>
            {
                var market = this.SendQuery(new GetSupermarketQuery());
                if(market==null)return;
                var result=market.GetInventoryGridInfoList();
                for (int i = 0; i < buyCount.Value; i++)
                {
                    market.SellProductById(result[itemPos].id);
                }
                gameObject.SetActive(false);
            });
            addCountBt.onClick.AddListener(() =>
            {
                buyCount.Value += 1;
            });
            subtractCountBt.onClick.AddListener(() =>
            {
                buyCount.Value -= 1;
            });
        }

        private void OnDisable()
        {
            buyCount.Value = 0;
            buyCountText.text = "0";
        }

        public IArchitecture GetArchitecture()
        {
            return GameArchitecture.Interface;
        }
    }
}
