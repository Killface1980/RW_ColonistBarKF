namespace ColonistBarKF
{
    using System.Collections.Generic;

    using RimWorld;

    using UnityEngine;

    using Verse;

    public class PawnStats : IExposable
    {

        public Pawn pawn;

        public float TotalEfficiency = 1f;

        public float TooCold = -1f;

        public float TooHot = -1f;

        public float BleedRate = -1f;

        public Vector3 TargetPos = Vector3.zero;

        public float DiseaseDisappearance = 1f;

        public float ApparelHealth = 1f;

        // public float Drunkness = 0f;
        public int BedStatus = -1;

        public bool IsSick = false;

        public int CabinFeverMoodLevel = 0;

        public int PainMoodLevel = 0;

        public float ToxicBuildUpVisible = 0;

        public MentalStateDef MentalSanity = null;

        public float HealthDisease = 1f;

        public bool HasLifeThreateningDisease = false;

        public float NextStatUpdate = 1f;
        public MentalBreaker Mb = null;
        public Need_Mood Mood = null;
        public int drugDesire = 0;
        public bool traitsCheck = false;
        public bool isAddict = false;
        public ThoughtDef painThought;
        public bool withDrawal = false;
        public string drugUserLabel = null;
        public string addictionLabel = null;
        public int wantsToHump = -1;
        public int feelsNaked = -1;
        public int prostho = 0;
        public int prosthoWant = 0;
        public int nightOwlUnhappy = -1;
        public bool greedy = false;
        public bool greedyThought = false;
        public bool jealous = false;
        public bool jealousThought = false;
        public int unburied = -1;
        public bool isPacifist = false;
        public bool isPyromaniac = false;
        public bool isMasochist = false;
        public float withDrawalPercent = 0f;
        public float pawnHealth = 1f;
        public float severity;
        public float immunity;
        public string humpTip;
        public string sickTip;
        public string painTip;
        public string cabinFeverTip;
        public string nakedTip;
        public string nightOwlTip;
        public bool isNightOwl = false;
        public string unburiedTip;

        public string BedStatusTip;
        public Color BGColor = Color.gray;

        public int thisColCount;

        public string toxicTip;

        public string healthTip;

        public bool ShouldBeTendedNowUrgent;

        public bool ShouldBeTendedNow;

        public bool ShouldHaveSurgeryDoneNow;

        public int LastStatUpdate;

        public void ExposeData()
        {

        }
    }
}
