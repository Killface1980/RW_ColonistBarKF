namespace ColonistBarKF.PSI
{
    using System;
    using System.Linq;

    using UnityEngine;

    using Verse;

    public enum Icons
    {
        None,
        Target,
        TargetHair,
        TargetSkin,
        Aggressive,
        ApparelHealth,
        Bloodloss,
        TooCold,
        Dazed,
        DeadColonist,
        Draft,
        Drunk,
        Greedy,
        Health,
        TooHot,
        Hungry,
        Idle,
        Effectiveness,
        Jealous,
        Leave,
        Naked,
        NightOwl,
        Pacific,
        Pain,
        Prosthophile,
        Prosthophobe,
        Pyromaniac,
        CabinFever,
        Sad,
        Tired,
        Unarmed,
        Panic,
        MedicalAttention,
        LeftUnburied,
        Bedroom,
        Toxicity,
        Marriage,
        Length
    }

    public class Materials
    {
        public static Material skinMat;
        public static Material hairMat;
        public static Material targetMat;

        private readonly Material[] _data = new Material[40];
        private readonly string _matLibName;

        public Materials(string matLib = "default")
        {
            this._matLibName = matLib;
        }

        public Material this[Icons icon] => this._data[(int)icon];

        private Material LoadIconMat(string path, bool smooth = false)
        {
            Texture2D tex = ContentFinder<Texture2D>.Get("UI/Overlays/PawnStateIcons/" + path, false);

            Material material;
            if (tex == null)
            {
                material = null;
            }
            else
            {
                if (smooth)
                {
                    tex.filterMode = FilterMode.Trilinear;
                    tex.mipMapBias = -0.5f;
                    tex.anisoLevel = 9;
                    tex.wrapMode = TextureWrapMode.Repeat;

                    // tex.Apply();
                    // tex.Compress(true);
                }
                else
                {
                    tex.filterMode = FilterMode.Point;
                    tex.wrapMode = TextureWrapMode.Repeat;

                    // tex.Apply();
                    // tex.Compress(true);
                }

                material = MaterialPool.MatFrom(new MaterialRequest(tex, ShaderDatabase.MetaOverlay));
            }

            return material;
        }

        public void ReloadTextures(bool smooth = false)
        {
            foreach (Icons icons in Enum.GetValues(typeof(Icons)).Cast<Icons>())
            {
                switch (icons)
                {
                    case Icons.None:
                    case Icons.Length:
                        continue;
                    default:
                        string path = this._matLibName + "/" + Enum.GetName(typeof(Icons), icons);
                        this._data[(int)icons] = this.LoadIconMat(path, smooth);
                        continue;
                }
            }
        }
    }


}