# Unity_TestEventComponent
拡張機能と３種イベントを扱うUnityコンポーネント
UnityEventなどとは違って３種の決まった動作をするイベントを扱い、制御を少し持っておくものとなります。

このコンポーネントを扱うにはUniRxの導入が必要となります。
対応しているUnityバージョンは5.6.0f3となっています。

# イベント起動
このイベントはEventProcesで一斉起動、ポーズ、停止が行えます。
登録するにはEventComponentを継承したSwitch、Trigger、OnceEventを使う必要があります。

# 拡張機能
拡張機能はEventProcessが持っており、EventComponentに対して登録、解除が行えます。

# イベントの基本
単体でも操作を行えます。

Runメソッドで実行
Pauseメソッドで一時停止
Stopメソッドで停止

Pause後にRunメソッドを呼ぶことで復帰することができます。

他には３種のイベントは状態変数を持っています。

共通変数
isEnd trueでイベント終了
isPause trueで一時停止

上記の変数を変更することで状態を変えることができます。

固有状態変数

SwitchEvent

isOn trueでOnの状態であることを示す。（変化は起動した瞬間）

Switch Trigger 

isSkipRestart trueであれば実行中に再度Runが呼ばれた時初期化を無視する

#使用ライブラリ
UniRx
https://github.com/neuecc/UniRx
Copyright (c) 2014 Yoshifumi Kawai


