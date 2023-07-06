namespace States
{
    public enum GameState { Main, Paused, LocalSolo, LocalVersus, OnlinePlay };
    public enum PlayerState { Spectate, Direction, Power, Watch, Result,
#if UNITY_EDITOR
        FreeRoam
#endif
    }
    public enum MenuState { Main, Pause, Settings, Controls, LevelSelect, PowerBar, Continue, Game };
    public enum Settings { Quality, Sensitivity };
    public enum Quality { Min, Med, Max };
}
