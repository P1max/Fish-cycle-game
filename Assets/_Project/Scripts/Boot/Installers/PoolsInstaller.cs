using Spawners;
using Zenject;

namespace Installers
{
    public class PoolsInstaller : Installer<PoolsInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<FishPool>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<CoinsPool>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<FoodPool>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<EffectsPool>().FromComponentInHierarchy().AsSingle();
        }
    }
}