using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.YieldInstructions
{

    //new WaitWhile();
    //new WaitUntil()
    // WaitForEndOfFrame;
    // WaitForFixedUpdate;
    // WaitForSeconds;
    // WaitForSecondsRealtime;

    public class YI_WaitForGameResume : CustomYieldInstruction
    {
        protected IEnumerator iNestedEnumerator= null;

        public YI_WaitForGameResume()
        {

        }

        public YI_WaitForGameResume(IEnumerator nested_enumerator)
        {
            iNestedEnumerator = nested_enumerator;
        }

        public override bool keepWaiting 
        {
            get
            {
                return Configuration.IsPaused || ((iNestedEnumerator != null) && iNestedEnumerator.MoveNext());
            }
        }
    }

    public class YI_WaitForSecondsOrGameResume: YI_WaitForGameResume
    {
        public YI_WaitForSecondsOrGameResume()
        {

        }

        public YI_WaitForSecondsOrGameResume(float seconds)
        {
            iNestedEnumerator = new YI_WaitForSeconds(seconds);
        }
    }

    public class YI_WaitForSecondsRealtimeAndGameResume : YI_WaitForGameResume
    {
        public YI_WaitForSecondsRealtimeAndGameResume()
        {

        }

        public YI_WaitForSecondsRealtimeAndGameResume(float seconds)
        {
            iNestedEnumerator = new YI_WaitForSecondsRealtime(seconds);
        }
    }

    public class YI_WaitForSeconds: CustomYieldInstruction
    {
        protected float iWaitTime = 0f;
        protected float iPivotTime = 0f;
        protected IEnumerator iNestedEnumerator = null;

        public YI_WaitForSeconds()
        {

        }

        public YI_WaitForSeconds(float seconds)
        {
            iWaitTime = seconds;
            iPivotTime = Time.time;
        }

        public YI_WaitForSeconds(float seconds, IEnumerator nested_enumerator)
        {
            iWaitTime = seconds;
            iPivotTime = Time.time;
            iNestedEnumerator = nested_enumerator;
        }

        public override bool keepWaiting
        {
            get
            {
                return (Time.time - iPivotTime < iWaitTime) || ((iNestedEnumerator != null) && iNestedEnumerator.MoveNext());
            }
        }
    }

    public class YI_WaitForSecondsRealtime : CustomYieldInstruction
    {
        protected float iWaitTime = 0f;
        protected float iPivotTime = 0f;
        protected IEnumerator iNestedEnumerator = null;

        public YI_WaitForSecondsRealtime()
        {

        }

        public YI_WaitForSecondsRealtime(float seconds)
        {
            iWaitTime = seconds;
            iPivotTime = Time.time;
        }

        public YI_WaitForSecondsRealtime(float seconds, IEnumerator nested_enumerator)
        {
            iWaitTime = seconds;
            iPivotTime = Time.realtimeSinceStartup;
            iNestedEnumerator = nested_enumerator;
        }

        public override bool keepWaiting
        {
            get
            {
                return (Time.realtimeSinceStartup - iPivotTime < iWaitTime) || ((iNestedEnumerator != null) && iNestedEnumerator.MoveNext());
            }
        }
    }

    public class YI_WaitOr: CustomYieldInstruction
    {
        protected IEnumerator iLeftEnumerator = null;
        protected IEnumerator iRightEnumerator = null;

        public YI_WaitOr()
        {

        }

        public YI_WaitOr(IEnumerator left_instruction, IEnumerator right_instruction)
        {
            iLeftEnumerator = left_instruction;
            iRightEnumerator = right_instruction;
        }

        public override bool keepWaiting
        {
            get
            {
                return ((iLeftEnumerator != null) && iLeftEnumerator.MoveNext()) || ((iRightEnumerator != null) && iRightEnumerator.MoveNext());
            }
        }
    }

    public class YI_WaitAnd : CustomYieldInstruction
    {
        protected IEnumerator iLeftEnumerator = null;
        protected IEnumerator iRightEnumerator = null;

        public YI_WaitAnd()
        {

        }

        public YI_WaitAnd(IEnumerator left_instruction, IEnumerator right_instruction)
        {
            iLeftEnumerator = left_instruction;
            iRightEnumerator = right_instruction;
        }

        public override bool keepWaiting
        {
            get
            {
                return ((iLeftEnumerator != null) && iLeftEnumerator.MoveNext()) && ((iRightEnumerator != null) && iRightEnumerator.MoveNext());
            }
        }
    }

    public class YI_WaitNot: CustomYieldInstruction
    {
        protected IEnumerator iInstruction = null;

        public YI_WaitNot()
        {

        }

        public YI_WaitNot(IEnumerator instruction)
        {
            iInstruction = instruction;
        }

        public override bool keepWaiting
        {
            get
            {
                return ((iInstruction != null) && !iInstruction.MoveNext());
            }
        }
    }

}
