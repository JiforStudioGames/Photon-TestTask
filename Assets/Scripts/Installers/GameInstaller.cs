using Zenject;

public class GameInstaller : Installer<GameInstaller>
{
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<PlayerService>().AsSingle().NonLazy();
        Container.BindInterfacesTo<PlayerPresenter>().AsSingle().NonLazy();
        Container.BindInterfacesTo<MedkitPresenter>().AsSingle().NonLazy();
    }
}