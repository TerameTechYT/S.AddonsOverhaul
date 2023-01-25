using Stationeers.Addons;
using Stationeers.Addons.API;
using UnityEngine;

namespace ActualPressureRegulators.Scripts
{
    public class ActualPressureRegulators : IPlugin
    {
        public void OnLoad()
        {
            Debug.Log("APR: Actual Pressure Regulators Loaded!!");
        }

        public void OnUnload()
        {
            Debug.Log("APR: Actual Pressure Regulators Unoaded!!");
        }
    }
}
