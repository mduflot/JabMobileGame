﻿namespace Service
{
    public interface IGameService : IService
    {
        public GlobalSettingsGameSO GlobalSettingsSO  {get;}

        public void LoadMainMenuScene();

        public void LoadGameScene(string environmentAddressableName, string enemyAddressableName, bool isDebugFight, bool isTutorialFight);
    }
}