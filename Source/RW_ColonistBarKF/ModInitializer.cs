using System;
using System.Reflection;
using CommunityCoreLibrary;
using RimWorld;
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
            Settings.Firstload = true;

            MethodInfo method = typeof(ColonistBar).GetMethod("ColonistBarOnGUI", BindingFlags.Instance | BindingFlags.Public);
            MethodInfo method2 = typeof(ColonistBar_KF).GetMethod("ColonistBarOnGUI", BindingFlags.Instance | BindingFlags.Public);

            MethodInfo method3 = typeof(ColonistBar).GetMethod("ColonistsInScreenRect", BindingFlags.Instance | BindingFlags.Public);
            MethodInfo method4 = typeof(ColonistBar_KF).GetMethod("ColonistsInScreenRect", BindingFlags.Instance | BindingFlags.Public);

            MethodInfo method5 = typeof(ColonistBar).GetMethod("ColonistAt", BindingFlags.Instance | BindingFlags.Public);
            MethodInfo method6 = typeof(ColonistBar_KF).GetMethod("ColonistAt", BindingFlags.Instance | BindingFlags.Public);

            try
            {
                Detours.TryDetourFromTo(method, method2);
                Detours.TryDetourFromTo(method3, method4);
                Detours.TryDetourFromTo(method5, method6);
            }
            catch (Exception)
            {
                Log.Error("Could not detour RW_ColonistBarKF");
                throw;
            }
        }


    }
}
