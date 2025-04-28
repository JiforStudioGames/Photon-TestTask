using Zenject;

public class MainMenuInstaller : Installer<MainMenuInstaller>
{
    private const string MainMenuViewPath = "MainMenuView";
    public override void InstallBindings()
    {
        Container.Bind<MainMenuView>()
            .FromComponentInNewPrefabResource(MainMenuViewPath)
            .UnderTransformGroup("Canvas")
            .AsSingle()
            .NonLazy();
        
        Container.BindInterfacesTo<MainMenuPresenter>().AsSingle().NonLazy();
    }
}