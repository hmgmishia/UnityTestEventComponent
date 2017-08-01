using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using EventUtility;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EventPlugin
{
    /// <summary>
    /// イベント実行スクリプトのシーンビュー編集拡張機能クラス
    /// </summary>
    [CustomEditor(typeof(EventProcess))]
    public class EventProcessEditor : Editor
    {
        /// <summary>
        /// 実行する入力イベント
        /// </summary>
        private enum EditorEventType
        {
            //イベントなし
            None,
            //別オブジェクト選択
            AnotherSelect,
            //選択スタート
            DragStart,
            //選択終了
            DragEnd,
            //項目表示
            CheckStart,
            //項目閉じる
            CheckClear,
        }
        /// <summary>
        /// 編集対象のイベントスクリプト
        /// </summary>
        private EventProcess targetScript;
        /// <summary>
        /// 起動イベント項目編集対象
        /// </summary>
        private GameObject checkingObject;
        /// <summary>
        /// 起動イベント項目編集対象が持っているイベント
        /// </summary>
        private List<EventComponent> checkingResult = new List<EventComponent>();
        /// <summary>
        /// 拡張機能対象が持つイベントをUndo用に管理するリスト
        /// </summary>
        private List<EventComponent> newEventList = new List<EventComponent>();
        /// <summary>
        /// ドラッグされているか
        /// </summary>
        private bool isDrag;
        /// <summary>
        /// 説明のメニューを出すかどうか
        /// </summary>
        private static bool isFoldingText;
        /// <summary>
        /// スクロールしたポジション
        /// </summary>
        private Vector2 scrollPosition;
        /// <summary>
        /// 現在の入力イベント
        /// </summary>
        private EditorEventType currentEvent;
        /// <summary>
        /// イベントリストのプロパティ
        /// </summary>
        private SerializedProperty eventListProperty;

        private void OnEnable()
        {
            targetScript = target as EventProcess;
            if (targetScript.eventList == null)
            {
                targetScript.eventList = new List<EventComponent>();
            }
            currentEvent = EditorEventType.None;
            //有効化された時のイベント保持をする
            newEventList = new List<EventComponent>(targetScript.eventList);
            eventListProperty = serializedObject.FindProperty("eventList");
        }
        /// <summary>
        /// イベントの一斉追加
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="addChildComponent"></param>
        private void AddEventComponent(GameObject obj, bool addChildComponent)
        {
            EventComponent[] list;
            if (addChildComponent)
            {
                list = obj.transform.GetComponentsInChildren<EventComponent>(true);
            }
            else list = obj.GetComponents<EventComponent>();
            foreach (var x in list)
            {
                if (x == null)
                {
                    continue;
                }
                if (newEventList.Contains(x))
                {
                    continue;
                }
                eventListProperty.InsertArrayElementAtIndex(eventListProperty.arraySize);
                eventListProperty.GetArrayElementAtIndex(eventListProperty.arraySize - 1).objectReferenceValue = x;
            }
        }

        /// <summary>
        /// ドラッグアンドドロップ処理
        /// </summary>
        /// <param name="type"></param>
        private void DragSelect(EditorEventType type)
        {
            if (isDrag)
            {
                Vector3 mousePos = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
                Handles.color = Color.yellow;
                //Handles.ConeHandleCap(EditorGUIUtility.GetControlID(FocusType.Passive), mousePos, Quaternion.LookRotation(mousePos - targetScript.transform.position), 0.5f, EventType.Repaint);
                Handles.DrawLine(targetScript.transform.position, mousePos);
                SceneView.RepaintAll();
            }
            Ray ray;
            RaycastHit hit;
            switch (type)
            {
                case EditorEventType.DragStart:
                    ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    RaycastHit[] hits = Physics.SphereCastAll(ray, 0.2f);
                    hits = hits.Where(x => x.collider.gameObject == targetScript.gameObject).ToArray();
                    if (hits.Length == 0)
                    {
                        break;
                    }
                    isDrag = true;
                    break;
                 case EditorEventType.DragEnd:
                    if (!isDrag)
                    {
                        break;
                    }
                    ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    if (!Physics.Raycast(ray, out hit))
                    {
                        isDrag = false;
                        break;
                    }
                    isDrag = false;
                    AddEventComponent(hit.collider.gameObject, true);
                    break;
            }
        }

        /// <summary>
        /// イベントタイプ取得
        /// </summary>
        /// <returns></returns>
        private EditorEventType GetCurrentEventType()
        {
            Event current = Event.current;
            EventType type = current.type;
            switch (type)
            {
                case EventType.MouseDown:
                    if (current.button == 1) return EditorEventType.CheckStart;
                    else if (current.button == 0) return EditorEventType.AnotherSelect;
                    break;
                case EventType.MouseDrag:
                    if (current.button == 0) return EditorEventType.DragStart;
                    break;
                case EventType.MouseUp:
                    if (checkingObject != null) break;
                    return EditorEventType.DragEnd;
                case EventType.KeyDown:
                    if (current.alt) return EditorEventType.CheckClear;
                    break;
            }
            return EditorEventType.None;
        }

        /// <summary>
        /// 既に選択されているオブジェクトの矢印描画
        /// </summary>
        private void AlReadySelectEventDraw()
        {
            if (newEventList == null)
            {
                return;
            }
            bool isalreadyDrawSelf = false;
            foreach (var x in newEventList)
            {
                if (x == null)
                {
                    newEventList.Remove(x);
                    continue;
                }
                if (x.gameObject == targetScript.gameObject)
                {
                    if (isalreadyDrawSelf) continue;
                    Handles.BeginGUI();
                    EditorGUI.TextArea(new Rect(HandleUtility.WorldToGUIPoint(targetScript.transform.position), new Vector2(40, 20)), "Self");
                    Handles.EndGUI();
                    isalreadyDrawSelf = true;
                    continue;
                }
                //固定の色分け
                if (x is OnceEvent) Handles.color = Color.red;
                else if (x is SwitchEvent) Handles.color = Color.yellow;
                else if (x is TriggerEvent) Handles.color = Color.blue;
                else Handles.color = Color.green;

                Handles.ConeHandleCap(EditorGUIUtility.GetControlID(FocusType.Passive), x.transform.position, Quaternion.LookRotation(x.transform.position - targetScript.transform.position), 0.5f, EventType.Repaint);
                Handles.DrawLine(targetScript.transform.position, x.transform.position);
                //Handles.SelectionFrame(GUIUtility.GetControlID(FocusType.Passive), x.transform.position, Quaternion.identity, 1);
            }
        }

        /// <summary>
        /// イベントの一覧を選択する処理
        /// </summary>
        /// <param name="type"></param>
        private void EventSelectChecker(EditorEventType type)
        {
            Ray ray;
            RaycastHit hit;
            //選択したオブジェクトのコンポーネントを取ってくる
            switch (type)
            {
                case EditorEventType.DragStart:
                case EditorEventType.CheckClear:
                    checkingObject = null;
                    break;
                case EditorEventType.AnotherSelect:
                    //もし同じコンポーネントがついているならばそちらに対象を移す
                    ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    if (!Physics.SphereCast(ray, 0.2f, out hit))
                    {
                        checkingObject = null;
                        break;
                    }
                    if (hit.collider.GetComponent<EventProcess>() != null)
                    {
                        checkingObject = null;
                        Selection.activeGameObject = hit.collider.gameObject;
                        return;
                    }
                    break;
                case EditorEventType.CheckStart:
                    ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    if (!Physics.SphereCast(ray, 0.2f, out hit))
                    {
                        checkingObject = null;
                        break;
                    }
                    if (hit.collider.gameObject == checkingObject)
                    {
                        checkingObject = null;
                        break;
                    }
                    checkingResult.Clear();
                    hit.collider.gameObject.GetComponents(checkingResult);
                    hit.collider.gameObject.GetComponentsInParent(true, checkingResult);
                    hit.collider.gameObject.GetComponentsInChildren(true, checkingResult);
                    if (checkingResult.Count == 0)
                    {
                        checkingObject = null;
                        break;
                    }
                    checkingObject = hit.collider.gameObject;
                    break;
            }
            //対象がいなければ選択処理を無視
            if (checkingObject == null) return;
            //他のオブジェクトにあるイベントを選択するレイアウトを表示
            GUILayout.BeginArea(new Rect(HandleUtility.WorldToGUIPoint(checkingObject.transform.position), new Vector2(200, 100)));
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUI.skin.box);
            for (int i = 0;i < checkingResult.Count;++i)
            {
                bool check;
                bool iscontains = newEventList.Contains(checkingResult[i]);
                
                check = iscontains;
                iscontains = GUILayout.Toggle(iscontains, checkingResult[i].name + " " + checkingResult[i].GetType(), GUI.skin.button);
                if (iscontains == check)
                {
                    continue;
                }
                if (iscontains)
                {
                    newEventList.Add(checkingResult[i]);
                }
                else
                {
                    newEventList.Remove(checkingResult[i]);
                }

            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        void OnSceneGUI()
        {
            serializedObject.Update();
            //左上レイアウトを先に設定
            Handles.BeginGUI();
            bool button = false;
            button = GUILayout.Button("ツール解除", GUILayout.Width(100));
            if (button)
            {
                Tools.current = Tool.None;
            }
            isFoldingText = GUILayout.Toggle(isFoldingText, "▼", GUI.skin.button, GUILayout.Width(20));
            if (isFoldingText)
            {
                GUILayout.TextArea("オブジェクト右クリック->イベント選択", GUI.skin.box);
                GUILayout.TextArea("alt->イベント選択解除", GUI.skin.box);
                GUILayout.TextArea("ドラッグ->ドロップ先のイベント一斉選択", GUI.skin.box);
            }
            Handles.EndGUI();

            EditorGUI.BeginChangeCheck();
            //範囲選択無視
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            DragSelect(currentEvent);
            AlReadySelectEventDraw();
            EventSelectChecker(currentEvent);
            currentEvent = GetCurrentEventType();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetScript, "EventProcess Changed");
                targetScript.eventList = newEventList;
            }
            //プロパティが変更されたときを考えnewEventListも新しく対象からリストを取得
            serializedObject.ApplyModifiedProperties();
            newEventList = new List<EventComponent>(targetScript.eventList);
            //他のオブジェクトをクリックしても無視するように変更
            if (Selection.activeGameObject != targetScript.gameObject)
            {
                if (Selection.activeGameObject.GetComponent<EventProcess>() == null)
                {
                    Selection.activeGameObject = targetScript.gameObject;
                }
            }
        }
    }
}