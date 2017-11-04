namespace ColonistBarKF
{
    using System;
    using System.Linq;

    using JetBrains.Annotations;

    using UnityEngine;

    using Verse;

    public class Materials
    {
        [NotNull]
        private readonly Material[] _data = new Material[40];

        [NotNull]
        private readonly string _matLibName;

        public Materials(string matLib = "default")
        {
            this._matLibName = matLib;
        }

        [CanBeNull]
        public Material this[Icon icon] => this._data[(int)icon];

        public void ReloadTextures(bool smooth = false)
        {
            foreach (Icon icons in Enum.GetValues(typeof(Icon)).Cast<Icon>())
            {
                switch (icons)
                {
                    case Icon.None:
                    case Icon.Length: continue;
                    default:
                        string path = this._matLibName + "/" + Enum.GetName(typeof(Icon), icons);
                        this._data[(int)icons] = this.LoadIconMat(path, smooth);
                        continue;
                }
            }
        }

        [CanBeNull]
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
    }
}