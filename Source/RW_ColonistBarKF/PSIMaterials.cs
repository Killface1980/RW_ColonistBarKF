namespace ColonistBarKF
{
    using System;
    using System.Linq;

    using UnityEngine;

    using Verse;

    public enum Icon
    {
        None,
        Idle,
        Aggressive,
        Leave,
        Dazed,
        Panic,
        Bloodloss,
        Health,
        Toxicity,
        Unarmed,
        Pacific,
        Pyromaniac,
        Drunk,
        Sad,
        Hungry,
        Tired,
        TooCold,
        TooHot,
        Pain,
        ApparelHealth,
        Effectiveness,
        Naked,
        NightOwl,
        Greedy,
        Jealous,
        Prosthophile,
        Prosthophobe,
        CabinFever,
        MedicalAttention,
        LeftUnburied,
        Bedroom,
        Draft,
        Target,
        TargetHair,
        TargetSkin,
        Length
    }

    public class Materials
    {


        private readonly Material[] _data = new Material[40];
        private readonly string _matLibName;

        public Materials(string matLib = "default")
        {
            this._matLibName = matLib;
        }

        public Material this[Icon icon] => this._data[(int)icon];

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
            foreach (Icon icons in Enum.GetValues(typeof(Icon)).Cast<Icon>())
            {
                switch (icons)
                {
                    case Icon.None:
                    case Icon.Length:
                        continue;
                    default:
                        string path = this._matLibName + "/" + Enum.GetName(typeof(Icon), icons);
                        this._data[(int)icons] = this.LoadIconMat(path, smooth);
                        continue;
                }
            }
        }
    }


}