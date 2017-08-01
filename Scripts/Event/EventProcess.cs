using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace EventUtility
{
    /// <summary>
    /// 指定したイベントをまとめて実行するコンポーネント
    /// </summary>
    [DisallowMultipleComponent]
    public class EventProcess : MonoBehaviour
    {
        [SerializeField]
        public List<EventComponent> eventList;

        /// <summary>
        /// イベントの一時停止
        /// </summary>
        public void Pause()
        {
            foreach(var x in eventList)
            {
                x.Pause();
            }
        }

        /// <summary>
        /// イベントの実行
        /// </summary>
        public void Run()
        {
            foreach (var x in eventList)
            {
                x.Run();
            }
        }

        /// <summary>
        /// イベントの完全停止
        /// </summary>
        public void Stop()
        {
            foreach (var x in eventList)
            {
                x.Stop();
            }
        }
    }
}
