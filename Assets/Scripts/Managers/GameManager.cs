using System.Threading.Tasks;
using Commands;
using QFramework;
using SOScripts;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
namespace Managers
{
    public class GameManager:MonoSingleton<GameManager>,IController
    {
        [Header("配置表")] 
        [SerializeField]private GameItemPool gameItemPool;
        private async Task LoadGameItemPoolAsync()
        {
            AsyncOperationHandle<GameItemPool> handle = Addressables.LoadAssetAsync<GameItemPool>("Assets/GameConfig/GameItemPool.asset");
            await handle.Task;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                gameItemPool = handle.Result;
                return;
            }
            Debug.LogError("从AA包中加载GameItemPool失败");
        }
        private async void Awake()
        {
            var arch = this.GetArchitecture();
            GameArchitecture gameArch=arch as GameArchitecture;
            if (gameArch == null)
            {
                Debug.LogError("架构转换失败");
                return;
            }
            //注册Model；System;Utility等
            gameArch.RegisterModelCustom();
            gameArch.RegisterSystemCustom();
            await LoadGameItemPoolAsync();
            //给架构注入数据
            this.SendCommand(new InitInventoryModelCommand(gameItemPool));
            this.SendCommand(new InitInventorySystemCommand());
        }
        private void Start()
        {

        }
        private void Update()
        {

        }
        public IArchitecture GetArchitecture()
        {
            return GameArchitecture.Interface;
        }
    }
}