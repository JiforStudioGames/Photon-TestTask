using Zenject;

public class MainMenuContext : MonoInstaller
{
    public override void InstallBindings()
    {
        PhotonInstaller.Install(Container);
        MainMenuInstaller.Install(Container);
    }
}
