using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace EventUtility
{
    /// <summary>
    /// イベントを実行するためのベースコンポーネント
    /// </summary>
    public abstract class EventComponent : MonoBehaviour
    {
        /// <summary>
        /// 終了判定
        /// </summary>
        protected ReactiveProperty<bool> isEnd = new ReactiveProperty<bool>();
        public IReadOnlyReactiveProperty<bool> IsEnd { get { return isEnd; } }

        /// <summary>
        /// ポーズ判定
        /// </summary>
        protected ReactiveProperty<bool> isPause = new ReactiveProperty<bool>();
        public IReadOnlyReactiveProperty<bool> IsPause { get { return isPause; } }

        /// <summary>
        /// 実行
        /// </summary>
        public abstract void Run();
        /// <summary>
        /// 処理を停止
        /// </summary>
        public abstract void Stop();
        /// <summary>
        /// 処理を一時停止
        /// </summary>
        public abstract void Pause();
    }

}
