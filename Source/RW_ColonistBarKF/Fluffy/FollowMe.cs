// Karel Kroeze
// FollowMe.cs
// 2016-12-27

namespace ColonistBarKF
{
    using System;
    using System.Linq;
    using System.Reflection;

    using JetBrains.Annotations;

    using RimWorld;
    using RimWorld.Planet;

    using UnityEngine;

    using Verse;
    using Verse.Sound;

    public class FollowMe : GameComponent
    {

        #region Public Fields

        public static Thing _followedThing;

        public static bool CurrentlyFollowing;

        #endregion Public Fields

        #region Private Fields

        private static readonly FieldInfo _cameraDriverDesiredDollyField =
            typeof(CameraDriver).GetField("desiredDolly", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo _cameraDriverRootPosField =
            typeof(CameraDriver).GetField("rootPos", BindingFlags.Instance | BindingFlags.NonPublic);

        private static bool _cameraHasJumpedAtLeastOnce;

        private static bool _enabled = true;

        [NotNull]
        private readonly KeyBindingDef[] _followBreakingKeyBindingDefs =
            {
                KeyBindingDefOf.MapDollyDown, KeyBindingDefOf.MapDollyUp, KeyBindingDefOf.MapDollyRight,
                KeyBindingDefOf.MapDollyLeft
            };

        [NotNull]
        private readonly KeyBindingDef _followKey = KeyBindingDef.Named("FollowSelected");

        #endregion Private Fields

        #region Public Constructors

        public FollowMe([NotNull] Game game)
        {
            // noop
        }

        public FollowMe()
        {
        }

        #endregion Public Constructors

        #region Public Properties

        [NotNull]
        private static string FollowedLabel
        {
            get
            {
                if (_followedThing == null)
                {
                    return string.Empty;
                }

                Pawn pawn = _followedThing as Pawn;
                if (pawn != null)
                {
                    return pawn.NameStringShort;
                }

                return _followedThing.LabelCap;
            }
        }

        #endregion Public Properties

        #region Private Properties

        private static Vector2 CameraDesiredDolly
        {
            get
            {
                if (_cameraDriverDesiredDollyField == null)
                {
                    throw new NullReferenceException("CameraDriver.desiredDolly field info NULL");
                }

                return (Vector2)_cameraDriverDesiredDollyField.GetValue(Find.CameraDriver);
            }
        }

        private static Vector3 CameraRootPosition
        {
            get
            {
                if (_cameraDriverRootPosField == null)
                {
                    throw new NullReferenceException("CameraDriver.rootPos field info NULL");
                }

                return (Vector3)_cameraDriverRootPosField.GetValue(Find.CameraDriver);
            }
        }

        // ReSharper disable once InconsistentNaming
        private static bool MouseOverUI => Find.WindowStack.GetWindowAt(UI.MousePositionOnUIInverted) != null;

        #endregion Private Properties

        #region Public Methods

        public static void StopFollow([NotNull] string reason)
        {
#if DEBUG
            Log.Message($"FollowMe :: Stopped following {FollowedLabel} :: {reason}");
#endif

            Messages.Message("FollowMe.Cancel".Translate(FollowedLabel), MessageSound.Negative);
            _followedThing = null;
            CurrentlyFollowing = false;
            _cameraHasJumpedAtLeastOnce = false;
        }

        private static void TryJumpSmooth(GlobalTargetInfo target)
        {
            target = CameraJumper.GetAdjustedTarget(target);
            if (!target.IsValid)
            {
                StopFollow("invalid target");
                return;
            }

            // we have to use our own logic for following spawned things, as CameraJumper
            // uses integer positions - which would be jerky.
            if (target.HasThing)
            {
                TryJumpSmoothInternal(target.Thing);
            }
            else
            {
                // However, if we don't have a thing to follow, integer positions will do just fine.
                CameraJumper.TryJump(target);
            }

            _cameraHasJumpedAtLeastOnce = true;
        }

        public static void TryStartFollow([CanBeNull] Thing thing)
        {
            if (!CurrentlyFollowing && thing == null)
            {
                if (Find.Selector.NumSelected > 1)
                {
                    Messages.Message("FollowMe.RejectMultiple".Translate(), MessageSound.RejectInput);
                }
                else if (Find.Selector.NumSelected == 0)
                {
                    Messages.Message("FollowMe.RejectNoSelection".Translate(), MessageSound.RejectInput);
                }
                else
                {
                    Messages.Message("FollowMe.RejectNotAThing".Translate(), MessageSound.RejectInput);
                }
            }
            else if (CurrentlyFollowing && thing == null || thing == _followedThing)
            {
                // cancel current follow (toggle or thing == null)
                StopFollow("toggled");
            }
            else if (thing != null)
            {
                // follow new thing
                StartFollow(thing);
            }
        }

        // public override void GameComponentOnGUI()
        // {
        // if (Current.ProgramState != ProgramState.Playing)
        // return; // gamecomp is already active in the 'setup' stage, but follow me shouldnt be.
        // if (Event.current.type == EventType.mouseUp &&
        // Event.current.button == 1)
        // {
        // // get mouseposition, invert y axis (because UI has origin in top left, Input in bottom left).
        // Vector3 pos = Input.mousePosition;
        // pos.y = Screen.height - pos.y;
        // Thing thing = Find.ColonistBar.ColonistOrCorpseAt(pos);
        // if (thing != null)
        // {
        // // start following
        // TryStartFollow(thing);
        // // use event so it doesn't bubble through
        // Event.current.Use();
        // }
        // }
        // }
        public override void GameComponentUpdate()
        {
            if (!_enabled)
            {
                return;
            }

            try
            {
                if (CurrentlyFollowing)
                {
                    this.CheckKeyScroll();
                    this.CheckScreenEdgeScroll();
                    this.CheckCameraJump();
                    CheckDolly();
                }

                // start/stop following thing on key press
                if (this._followKey.KeyDownEvent)
                {
                    TryStartFollow(Find.Selector.SingleSelectedObject as Thing);
                }

                // move camera
                Follow();
            }
            catch (Exception e)
            {
                // catch exception to avoid error spam
                _enabled = false;
                Log.Error(e.ToString());
            }
        }

        public override void LoadedGame()
        {
            if (CurrentlyFollowing)
            {
                StopFollow("Game loaded");
            }

            base.LoadedGame();
        }

        public override void StartedNewGame()
        {
            if (CurrentlyFollowing)
            {
                StopFollow("New game started");
            }

            base.StartedNewGame();
        }

        #endregion Public Methods

        #region Private Methods

        private static void CheckDolly()
        {
            if (CameraDesiredDolly != Vector2.zero)
            {
                StopFollow("dolly");
            }
        }

        private static void Follow()
        {
            if (!CurrentlyFollowing || _followedThing?.Map == null)
            {
                return;
            }

            TryJumpSmooth(_followedThing);
        }

        private static void StartFollow([NotNull] Thing thing)
        {
            _followedThing = thing;
            CurrentlyFollowing = true;

            Messages.Message("FollowMe.Follow".Translate(FollowedLabel), MessageSound.Benefit);
        }

        private static void TryJumpSmoothInternal([NotNull] Thing thing)
        {
            // copypasta from Verse.CameraJumper.TryJumpInternal( Thing ),
            // but with drawPos instead of PositionHeld.
            if (Current.ProgramState != ProgramState.Playing)
            {
                return;
            }

            Map mapHeld = thing.MapHeld;
            if (mapHeld != null && thing.PositionHeld.IsValid && thing.PositionHeld.InBounds(mapHeld))
            {
                bool flag = CameraJumper.TryHideWorld();
                if (Current.Game.VisibleMap != mapHeld)
                {
                    Current.Game.VisibleMap = mapHeld;
                    if (!flag)
                    {
                        SoundDefOf.MapSelected.PlayOneShotOnCamera();
                    }
                }

                Find.CameraDriver.JumpToVisibleMapLoc(thing.DrawPos); // <---
            }
            else
            {
                StopFollow("invalid thing position");
            }
        }

        private void CheckCameraJump()
        {
            // to avoid cancelling the following immediately after it starts, allow the camera to move to the followed thing once
            // before starting to compare positions
            if (_cameraHasJumpedAtLeastOnce)
            {
                // the actual location of the camera right now
                IntVec3 currentCameraPosition = Find.CameraDriver.MapPosition;

                // the location the camera has been requested to be at
                IntVec3 requestedCameraPosition = CameraRootPosition.ToIntVec3();

                // these normally stay in sync while following is active, since we were the last to request where the camera should go.
                // If they get out of sync, it's because the camera has been asked to jump to somewhere else, and we should stop
                // following our thing.
                if ((currentCameraPosition - requestedCameraPosition).LengthHorizontal > 1)
                {
                    StopFollow("map moved (camera jump)");
                }
            }
        }

        private void CheckKeyScroll()
        {
            if (this._followBreakingKeyBindingDefs.Any(key => key.IsDown))
            {
                StopFollow("moved map (key)");
            }
        }

        private void CheckScreenEdgeScroll()
        {
            if (!Prefs.EdgeScreenScroll || MouseOverUI)
            {
                return;
            }

            Vector3 mousePosition = Input.mousePosition;
            Rect[] screenCorners =
                {
                    new Rect(0f, 0f, 200f, 200f), new Rect(Screen.width - 250, 0f, 255f, 255f),
                    new Rect(0f, Screen.height - 250, 225f, 255f),
                    new Rect(Screen.width - 250, Screen.height - 250, 255f, 255f)
                };
            if (screenCorners.Any(e => e.Contains(mousePosition)))
            {
                return;
            }

            if (mousePosition.x < 20f || mousePosition.x > Screen.width - 20 || mousePosition.y > Screen.height - 20f
                || mousePosition.y < (Screen.fullScreen ? 6f : 20f))
            {
                StopFollow("moved map (dolly)");
            }
        }

        #endregion Private Methods
    }
}