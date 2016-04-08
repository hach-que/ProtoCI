using Ninject.Modules;
using ProtoCI.Core.Protocols;

namespace ProtoCI
{
    public class ProtoCIModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IProtocol>().To<SSHProtocol>().InSingletonScope().Named("SSH");
            Bind<IProtocol>().To<WinRMProtocol>().InSingletonScope().Named("WinRM");
        }
    }
}