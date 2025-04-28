using Zenject;

public class GameMenuInstaller : Installer<GameMenuInstaller>
{
    private const string GameViewPath = "GameMenuView";
    public override void InstallBindings()
    {
        Container.Bind<GameMenuView>()
            .FromComponentInNewPrefabResource(GameViewPath)
            .UnderTransformGroup("Canvas")
            .AsSingle()
            .NonLazy();
        
        Container.BindInterfacesTo<GameMenuPresenter>().AsSingle().NonLazy();
    }
}