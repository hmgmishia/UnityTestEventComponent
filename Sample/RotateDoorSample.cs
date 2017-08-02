using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventUtility;

public class RotateDoorSample : SwitchEvent
{
    /// <summary>
    /// 閉じるまでの時間
    /// </summary>
    [SerializeField]
    protected float runTime;
    /// <summary>
    /// 開いているかどうか
    /// </summary>
    private bool isOpen;

    /// <summary>
    /// 現在の遷移時間
    /// </summary>
    protected float currentRuntime = 1;
    private Quaternion rotate = Quaternion.Euler(0, -90, 0);
    private Quaternion init = Quaternion.Euler(0, 0, 0);

    protected override void Oninit()
    {
    }

    protected override void OnPause()
    {
    }

    protected override void OnStart()
    {
        currentRuntime = 1 - currentRuntime;
        isOpen = !isOpen;
    }

    protected override void OnStop()
    {
    }

    protected override void OnUpdate()
    {
        if (isOpen)
        {
            transform.parent.localRotation = Quaternion.Slerp(init, rotate, currentRuntime);
        }
        else
        {
            transform.parent.localRotation = Quaternion.Slerp(rotate, init, currentRuntime);
        }
        if (currentRuntime > 1.0f)
        {
            currentRuntime = 1;
            isEnd.SetValueAndForceNotify(true);
            return;
        }
        currentRuntime += Time.deltaTime / runTime;
    }
}
