//using UnityEditor;
//using UnityEngine;

//public class MapSpawner : MonoBehaviour
//{
//    private static readonly int ColorProperty = Shader.PropertyToID("_BaseColor");
//    private MaterialPropertyBlock _propBlock;

//    public Transform CreateMapHolder(string name, Transform parent)
//    {
//        Transform oldHolder = parent.Find(name);
//        if (oldHolder)
//        {
//#if UNITY_EDITOR
//            Undo.DestroyObjectImmediate(oldHolder.gameObject);
//#else
//            Destroy(oldHolder.gameObject);
//#endif
//        }

//        GameObject gameObject = new GameObject(name);

//#if UNITY_EDITOR
//        Undo.RegisterCreatedObjectUndo(gameObject, "Generate Map");
//#endif

//        gameObject.transform.parent = parent;
//        return gameObject.transform;
//    }


   

//    private void ApplyColor(Renderer renderer, Color color)
//    {
//        if (_propBlock == null)
//            _propBlock = new MaterialPropertyBlock();

//        renderer.GetPropertyBlock(_propBlock);

//        _propBlock.SetColor(ColorProperty, color);

//        renderer.SetPropertyBlock(_propBlock);
//    }

//    private struct VisualModifiers
//    {
//        public Color? color;
//        public float heightScale;
//    }
//}

