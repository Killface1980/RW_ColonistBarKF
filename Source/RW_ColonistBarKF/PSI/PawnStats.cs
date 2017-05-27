using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace ColonistBarKF.PSI
{
    public class PawnStats
    {
        public int IconCount;

        public float TotalEfficiency = 1f;

        public float TooCold = -1f;

        public float TooHot = -1f;

        public float BleedRate = -1f;

        public Vector3 TargetPos = Vector3.zero;

        public float DiseaseDisappearance = 1f;

        public float ApparelHealth = 1f;

        //    public float Drunkness = 0f;

        public int BedStatus = -1;

        public bool IsSick = false;

        public int CabinFeverMoodLevel = 0;

        public int PainMoodLevel = 0;

        public float ToxicBuildUp = 0;

        public MentalStateDef MentalSanity = null;

        public float HealthDisease = 1f;

        public bool HasLifeThreateningDisease = false;

        public List<Thought> Thoughts;
        public double LastStatUpdate = 0;
        public MentalBreaker Mb;
        public Need_Mood Mood;
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
        public bool isMasochist=false;
        public float withDrawalPercent = 0f;
        public float pawnHealth =1f;
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
    }
}
