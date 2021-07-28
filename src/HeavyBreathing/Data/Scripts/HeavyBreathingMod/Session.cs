using Sandbox.Definitions;
using System;
using System.Collections.Generic;
using System.Reflection;
using VRage.Game;
using VRage.Game.Components;

namespace EvilElectricCorpMod
{
    class DefinitionModifier
    {
        private delegate void Callable();

        private List<Callable> _undoCommands = new List<Callable>();

        public void Set<T, U>(T definition, Func<T, U> getter, Action<T, U> setter, U value)
        {
            U originalValue = getter(definition);
            setter(definition, value);
            _undoCommands.Add(() => setter(definition, originalValue));
        }

        public void UnsetAll()
        {
            foreach (var command in _undoCommands)
            {
                command();
            }
        }
    }

    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
    public class Session : MySessionComponentBase
    {
        private static readonly float Multiplier = 16;
        private static readonly float StorageCapacity = 60 * Multiplier;
        private static readonly float Consumption = 0.063f;
        private static readonly float OxygenBottleCapacity = 100 * Multiplier;

        private DefinitionModifier _modifier;

        public override void LoadData()
        {
            _modifier = new DefinitionModifier();

            MyDefinitionManager definitions = MyDefinitionManager.Static;
            foreach (var definition in definitions.GetDefinitionsOfType<MyCharacterDefinition>())
            {
                _modifier.Set(definition, d => d.OxygenConsumption, (d, v) => d.OxygenConsumption = v, Consumption);
                _modifier.Set(definition, d => d.OxygenConsumptionMultiplier, (d, v) => d.OxygenConsumptionMultiplier = v, Multiplier);

                foreach (var storage in definition.SuitResourceStorage)
                {
                    if (storage.Id.SubtypeId == "Oxygen")
                    {
                        _modifier.Set(storage, d => d.MaxCapacity, (d, v) => d.MaxCapacity = v, StorageCapacity);
                    }
                }
            }

            foreach (var definition in definitions.GetDefinitionsOfType<MyOxygenContainerDefinition>())
            {
                if (definition.StoredGasId.SubtypeName == "Oxygen")
                {
                    _modifier.Set(definition, d => d.Capacity, (d, v) => d.Capacity = v, OxygenBottleCapacity);
                }
            }
        }

        protected override void UnloadData()
        {
            _modifier.UnsetAll();
            _modifier = null;
        }
    }
}
