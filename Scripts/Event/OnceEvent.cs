using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;

namespace EventUtility
{
    /// <summary>
    /// 一度のみ実行するイベント
    /// </summary>
    public abstract class OnceEvent : EventComponent
    {
        /// <summary>
        /// 既に起動されているかどうか
        /// </summary>
        private bool alreadyRun = false;

        /// <summary>
        /// コンポーネント生成時の処理
        /// アップデートと終了時の処理を決定する
        /// </summary>
        private void Awake()
        {   
            Oninit();
            this.UpdateAsObservable()
                .Where(x => !isPause.Value && !isEnd.Value)
                .Subscribe(_ => OnUpdate());
            this.IsEnd
                .Subscribe(x =>
                {
                    OnStop();
                });
        }

        /// <summary>
        /// イベントの実行
        /// </summary>
        public override void Run()
        {
            if (isEnd.Value)
            {
                return;
            }
            if (isPause.Value)
            {
                isPause.SetValueAndForceNotify(false);
                return;
            }
            if (alreadyRun)
            {
                return;
            }
            OnStart();
            isPause.SetValueAndForceNotify(false);
            alreadyRun = true;
        }

        /// <summary>
        /// イベントの完全停止
        /// </summary>
        public override void Stop()
        {
            isEnd.SetValueAndForceNotify(true);
        }


        /// <summary>
        /// イベントの一時停止
        /// </summary>
        public override void Pause()
        {
            OnPause();
            isPause.SetValueAndForceNotify(true);
        }

        /// <summary>
        /// 初期化時の処理
        /// </summary>
        protected abstract void Oninit();
        /// <summary>
        /// 更新時の処理
        /// </summary>
        protected abstract void OnUpdate();
        /// <summary>
        /// 停止した瞬間の処理
        /// </summary>
        protected abstract void OnStop();
        /// <summary>
        /// ポーズした瞬間の処理
        /// </summary>
        protected abstract void OnPause();
        /// <summary>
        /// イベント実行の瞬間の処理
        /// </summary>
        protected abstract void OnStart();
    }
}