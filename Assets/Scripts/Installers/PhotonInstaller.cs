using Zenject;

public class PhotonInstaller : Installer<PhotonInstaller>
{
    public override void InstallBindings()
    {
        Container
            .BindInterfacesAndSelfTo<PhotonService>()
            .FromNewComponentOnNewGameObject()
            .AsSingle()                                    
            .NonLazy();  
        
        Container.Bind<IRoomService>()
            .To<RoomService>()
            .AsSingle();
    }
}