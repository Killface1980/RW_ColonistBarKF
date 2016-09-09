
using UnityEngine;
using Verse;

namespace ColonistBarKF       // Replace with yours.
{       // This code is mostly borrowed from Pawn State Icons mod by Dan Sadler, which has open source and no license I could find, so...
    [StaticConstructorOnStartup]
    public class MapComponentInjectorBehavior : MonoBehaviour
    {
        public static readonly string mapComponentName = "ColonistBarKF.MapComponent_FollowMe";       // Ditto.
        private static readonly MapComponent_FollowMe mapComponent = new MapComponent_FollowMe();       // Ditto.
        public static readonly string mapComponentName2 = "ColonistBarKF.MapComponent_ZoomToMouse";       // Ditto.
        private static readonly MapComponent_ZoomToMouse mapComponent2 = new MapComponent_ZoomToMouse();       // Ditto.
        private GameObject _psiObject;

        #region No editing required
        protected bool reinjectNeeded = false;
        protected float reinjectTime = 0;

        public void OnLevelWasLoaded(int level)
        {
            reinjectNeeded = true;
            if (level >= 0)
            {
                reinjectTime = 1;
            }
            else
            {
                reinjectTime = 0;
            }
        }

        public void FixedUpdate()
        {
            if (Current.ProgramState != ProgramState.MapPlaying)
            {
                return;
            }

            if (reinjectNeeded)
            {
                Log.Message("ColonistBarKFInjector - needs reinject");
                reinjectTime -= Time.fixedDeltaTime;
                if (reinjectTime <= 0)
                {
                    reinjectNeeded = false;
                    reinjectTime = 0;
                    if (Find.Map != null && Find.Map.components != null)
                    {
                        if (Find.Map.components.FindAll(x => x.GetType().ToString() == mapComponentName).Count != 0)
                        {
                            Log.Message("MapComponentInjector: map already has " + mapComponentName + ".");
                            //Destroy(gameObject);
                        }
                        else
                        {
                            Log.Message("MapComponentInjector: adding " + mapComponentName + "...");
                            Find.Map.components.Add(mapComponent);
                            Log.Message("MapComponentInjector: success!");
                            //Destroy(gameObject);
                        }
                        if (Find.Map.components.FindAll(x => x.GetType().ToString() == mapComponentName2).Count != 0)
                        {
                            Log.Message("MapComponentInjector: map already has " + mapComponentName2 + ".");
                            //Destroy(gameObject);
                        }
                        else
                        {
                            Log.Message("MapComponentInjector: adding " + mapComponentName2 + "...");
                            Find.Map.components.Add(mapComponent2);
                            Log.Message("MapComponentInjector: success!");
                            //Destroy(gameObject);
                        }
                    }
                    _psiObject = GameObject.Find("PSIMain") ?? new GameObject("PSIMain");
                    _psiObject.AddComponent<PSI.PSI>();
                    Log.Message("PSI Injected!!");
                }
            }
        }

        public void Start()
        {
            OnLevelWasLoaded(-1);
        }
    }

    class MapComponentInjector : ITab
    {
        protected GameObject initializer;

        public MapComponentInjector()
        {
            Log.Message("MapComponentInjector: initializing for " + MapComponentInjectorBehavior.mapComponentName);
            Log.Message("MapComponentInjector: initializing for " + MapComponentInjectorBehavior.mapComponentName2);
            initializer = new GameObject("CBKFMapComponentInjector");
            initializer.AddComponent<MapComponentInjectorBehavior>();
            Object.DontDestroyOnLoad((Object)initializer);
        }

        protected override void FillTab()
        {

        }
    }
}
#endregion