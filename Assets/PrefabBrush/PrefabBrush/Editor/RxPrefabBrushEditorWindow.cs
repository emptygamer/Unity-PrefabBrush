using UnityEngine;
using UnityEditor;
using System;

namespace RemptyTool.PrefabBrush{
    public class RxPrefabBrushEditorWindow : EditorWindow{
        private GameObject _target;
        private GameObject _track;
        private float _2d_z;
        private float _3d_distance;
        private Vector3 _rotation;
        private float monitorScale;
        [MenuItem ("RxToolBox/PrefabBrush/PrefabBrush")]
        public static void  ShowWindow () {
            EditorWindow.GetWindow(typeof(RxPrefabBrushEditorWindow), false, "[RxPrefabBrush]");
        }
        private void OnEnable() {
            SceneView.duringSceneGui += this.OnSceneGUI;
            _track = null;
            _target = null;
            monitorScale = 1.25f;
            _2d_z = -10;
            _3d_distance = 10;
        }
        private void OnDisable() {
            SceneView.duringSceneGui -= this.OnSceneGUI;
        }   
        private void _resetBrush(){
            if(_track){
                DestroyImmediate(_track);
                _track = null;
            }
            _target = null;
        }
        public void OnSceneGUI(SceneView sceneView)
        {
            Handles.BeginGUI();
            GUILayout.BeginArea(new Rect(0,0,250,200));

            monitorScale = EditorGUILayout.FloatField("The scale of the monitor", monitorScale);
            _target = (GameObject)EditorGUILayout.ObjectField("PaintPrefab", _target, typeof(GameObject), true);
            
            Camera cam = SceneView.lastActiveSceneView.camera;
            Vector3 mousepos = Event.current.mousePosition;
            Vector3 v;

            if(sceneView.in2DMode){
                _2d_z = EditorGUILayout.FloatField("z of the object", _2d_z);
                mousepos = mousepos*monitorScale;
                mousepos.y = Screen.height - mousepos.y - 36.0f;
                v = cam.ScreenToWorldPoint(mousepos);
                v.z = _2d_z;
            }else{
                _3d_distance = EditorGUILayout.FloatField("distance of the object", _3d_distance);
                // 1.25 is the OS scale
                mousepos = mousepos*monitorScale;
                mousepos.y = Screen.height - mousepos.y - 36.0f;
                v = cam.ScreenPointToRay(mousepos).direction.normalized * _3d_distance;
            }
            try{
                if(_target){
                    if(!_track){
                        _track = Instantiate(_target, new Vector3(mousepos.x, mousepos.y, 0), Quaternion.identity);
                        _track.name = "__track__"+_target.name;
                        _rotation = _track.transform.rotation.eulerAngles;
                    }
                    if(_track.name != "__track__"+_target.name){
                        DestroyImmediate(_track);
                    }
                }
                if(_track){
                    if(sceneView.in2DMode){
                        _track.transform.position = new Vector3(v.x,v.y,v.z);
                    }else{
                        _track.transform.position = cam.transform.position + v;
                    }
                    sceneView.Repaint();
                }
            }catch(Exception e){
                UnityEngine.Debug.LogError(e);
                _resetBrush();
            }
            if(_track){
                _rotation = EditorGUILayout.Vector3Field("Rotation", _rotation);
                _track.transform.rotation = Quaternion.Euler(_rotation.x, _rotation.y, _rotation.z);
                if (Event.current.type == EventType.Ignore && Event.current.button == 0 ) {
                    if(_target){
                        var createdObject = Instantiate(_target, new Vector3(mousepos.x, mousepos.y, 0), Quaternion.identity);
                        createdObject.transform.position = _track.transform.position;
                        createdObject.transform.rotation = _track.transform.rotation;
                    }
                }
            }
            if(GUILayout.Button("Clear",GUILayout.ExpandWidth(false)))
            {
                _resetBrush();
            }
            GUILayout.EndArea();
            Handles.EndGUI();
        }

        private void OnGUI() {
            GUILayout.BeginVertical();
            GUILayout.Box("Setup the prefab you want to create in the scene page, and click the place you want to create the prefab, will paint the prefab for you.");
            GUILayout.EndVertical();
        }
    }
}
