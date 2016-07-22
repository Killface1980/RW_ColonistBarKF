using System;
using System.Reflection;
using CommunityCoreLibrary;
using UnityEngine;
using Verse;
using Object = UnityEngine.Object;

namespace RW_ColonistBarKF
{
    public class ModInitializer : ITab
    {
        protected GameObject modInitializerControllerObject;

        public ModInitializer()
        {
            modInitializerControllerObject = new GameObject("RW_ColonistBarKF");
            modInitializerControllerObject.AddComponent<ModInitializerBehaviour>();
            Object.DontDestroyOnLoad(modInitializerControllerObject);
            Log.Message("RW_ColonistBarKF Initialized");

        }

        protected override void FillTab() { }
    }

    class ModInitializerBehaviour : MonoBehaviour
    {
        public void FixedUpdate()
        {
        }

        public void Start()
        {
            MethodInfo method = typeof(RimWorld.ColonistBar).GetMethod("ColonistBarOnGUI", BindingFlags.Instance | BindingFlags.Public);
            MethodInfo method2 = typeof(ColonistBar_KF).GetMethod("ColonistBarOnGUI", BindingFlags.Instance | BindingFlags.Public);

            try
            {
                Detours.TryDetourFromTo(method, method2);

            }
            catch (Exception)
            {
                Log.Error("Could not detour RW_ColonistBarKF");
                throw;
            }
            Settings.firstload = true;
        }


    }
}
