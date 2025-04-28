using Zenject;

public class GameContext : MonoInstaller
{
    public override void InstallBindings()
    {
        PhotonInstaller.Install(Container);
        GameMenuInstaller.Install(Container);
        GameInstaller.Install(Container);
    }
}

