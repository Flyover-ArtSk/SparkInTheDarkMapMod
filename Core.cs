using MelonLoader;
using UnityEngine;
using UnityEngine.InputSystem;

[assembly: MelonInfo(typeof(NyahahahahaMap.Core), "Spark in the Dark Map", "1.0.0", "Nyahahahaha", null)]
[assembly: MelonGame("Stellar Fish", "Spark in the Dark")]

namespace NyahahahahaMap
{
    public class Core : MelonMod
    {
        internal static MelonLogger.Instance Logger;

        public override void OnInitializeMelon()
        {
            Logger = base.LoggerInstance;
        }
    }
}