using Sandbox.Definitions;
using VRage.Game.Components;

namespace EvilElectricCorpMod
{
    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
    public class Session : MySessionComponentBase
    {
        private static readonly float Multiplier = 12;
        private static readonly float StorageCapacity = 60 * Multiplier;
        private static readonly float Consumption = 0.063f;

        public override void BeforeStart()
        {
            MyDefinitionManager definitions = MyDefinitionManager.Static;
            foreach (var definition in definitions.GetDefinitionsOfType<MyCharacterDefinition>())
            {
                definition.OxygenConsumption = Consumption;
                definition.OxygenConsumptionMultiplier = Multiplier;

                foreach (var storage in definition.SuitResourceStorage)
                {
                    if (storage.Id.SubtypeId == "Oxygen")
                    {
                        storage.MaxCapacity = StorageCapacity;
                    }
                }
            }
        }
    }
}
