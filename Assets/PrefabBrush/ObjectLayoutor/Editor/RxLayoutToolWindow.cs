using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace RemptyTool.PrefabBrush{
    public class RxLayoutToolWindow : EditorWindow{
        public BoxCollider _layoutBox;
        public BoxBoundsHandle bbHandle = new BoxBoundsHandle();

        public Vector3 _pos = Vector3.zero;
        public float _w;
        public float _h;
        public float _d;
        public float _r;
        public float _c;
        public float _p;
        public GameObject _spawnPrefab;
        public bool _autoSpawn;
        
        [MenuItem ("RxToolBox/PrefabBrush/LayoutTool")]
        public static void ShowWindow () {
            EditorWindow.GetWindow(typeof(RxLayoutToolWindow), false, "[RxLayoutTool]");
        }
        
        private void OnEnable() {
            SceneView.duringSceneGui += this.OnSceneGUI;
            _w = 50;
            _h = 40;
            _d = 30;
            _r = 5;
            _c = 4;
            _p = 3;
        }
        private void OnDisable() {
            SceneView.duringSceneGui -= this.OnSceneGUI;
        }

        private void OnGUI() {
            GUILayout.Box("- Layout Area -");
            _w = EditorGUILayout.FloatField("width",_w);
            _h = EditorGUILayout.FloatField("height",_h);
            _d = EditorGUILayout.FloatField("depth",_d);
            GUILayout.Box("- Layout Setting -");
            _c = EditorGUILayout.FloatField("column",_c);
            _r = EditorGUILayout.FloatField("row",_r);
            _p = EditorGUILayout.FloatField("page",_p);
            GUILayout.Space(10);
            _autoSpawn = EditorGUILayout.Toggle("Auto Spawn",_autoSpawn);
            _spawnPrefab = EditorGUILayout.ObjectField(_spawnPrefab, typeof(GameObject), true) as GameObject;

            if(GUILayout.Button("Auto Layout"))
            {
                foreach(GameObject obj in Selection.objects){
                    obj.SetActive(false);
                }
                var start_x = _pos.x - _w/2;
                var end_x = _pos.x + _w/2;
                var start_y = _pos.y + _h/2;
                var end_y = _pos.y - _h/2;
                var start_z = _pos.z + _d/2;
                var end_z = _pos.z - _d/2;

                int count = 0;
                for (int k = 0; k < _p; k++){
                    float _z_percent = (k+1)/(_p+1);
                    var result_z = Mathf.Lerp(start_z, end_z, _z_percent);

                    for (int j = 0; j < _r; j++){
                        float _y_percent = (j+1)/(_r+1);
                        var result_y = Mathf.Lerp(start_y, end_y, _y_percent);

                        for (int i = 0; i < _c; i++){
                            float _x_percent = (i+1)/(_c+1);
                            var result_x = Mathf.Lerp(start_x, end_x, _x_percent);
                            if (_autoSpawn){
                                if (_spawnPrefab != null){
                                    var _obj = Instantiate(_spawnPrefab, new Vector3(result_x,result_y, result_z), Quaternion.identity);
                                    _obj.SetActive(true);
                                }else{
                                    Debug.LogError("SpawnPrefab not set.");
                                    return;
                                }
                            }else{
                                if (count < Selection.objects.Length){
                                    var _obj  = Selection.objects[count] as GameObject;
                                    _obj.SetActive(true);
                                    _obj.transform.position = new Vector3(result_x,result_y, result_z);
                                    count++;
                                }else{
                                    Debug.LogError("Selection no enough items.");
                                    return;
                                }
                            }
                            
                        }
                    }
                }
            }
            string requiredCount = "Invalid";
            try{
                requiredCount = (_c * _r * _p).ToString();
            }catch{

            }
            GUILayout.Label($"Selection Count : {Selection.objects.Length} / {requiredCount}");
        }
        private void OnSceneGUI(SceneView sceneView) {
            Handles.color = new Color(1,0,0,1);
            bbHandle.center = _pos;
            bbHandle.size = new Vector3(_w,_h,_d);
            EditorGUI.BeginChangeCheck();
            bbHandle.DrawHandle();
            Vector3 _new_pos = Handles.PositionHandle(_pos,Quaternion.identity);
            if(EditorGUI.EndChangeCheck()){
                _pos = _new_pos;
                _w = bbHandle.size.x;
                _h = bbHandle.size.y;
                _d = bbHandle.size.z;
            }
        }
    }

}
