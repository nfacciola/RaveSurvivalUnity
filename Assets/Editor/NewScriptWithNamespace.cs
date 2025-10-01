#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class NewScriptWithNamespace : EditorWindow
{
    private enum ScriptKind { MonoBehaviour, ScriptableObject, PlainClass, Interface, StaticClass }

    private string className = "NewClass";
    private string explicitNamespace = "RaveSurvival";
    private bool useAsmdefRootNamespace = true;
    private ScriptKind kind = ScriptKind.MonoBehaviour;

    const string PrefKeyNamespace = "NSWN.DefaultNamespace";
    const string MenuPath = "Assets/Create/Scripting/C# Script (with Rave Survival Namespace)";

    [MenuItem(MenuPath, false, 80)]
    public static void Open() {
        var win = GetWindow<NewScriptWithNamespace>("New Script");
        win.position = new Rect(Screen.width/2f, Screen.height/2f, 420, 220);
        win.explicitNamespace = EditorPrefs.GetString(PrefKeyNamespace, "RaveSurvival");
        win.Focus();
    }

    private void OnGUI()
    {
        GUILayout.Label("Create Script With Namespace", EditorStyles.boldLabel);

        className = EditorGUILayout.TextField("Class Name", className);
        kind = (ScriptKind)EditorGUILayout.EnumPopup("Template", kind);

        EditorGUILayout.Space(5);
        useAsmdefRootNamespace = EditorGUILayout.ToggleLeft("Use asmdef root namespace (if found)", useAsmdefRootNamespace);

        using (new EditorGUI.DisabledScope(useAsmdefRootNamespace))
        {
            explicitNamespace = EditorGUILayout.TextField("Fallback Namespace", explicitNamespace);
        }

        EditorGUILayout.Space(8);
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Create", GUILayout.Height(26))) CreateScript();
            if (GUILayout.Button("Cancel", GUILayout.Height(26))) Close();
        }

        EditorGUILayout.Space(6);
        if (GUILayout.Button("Set as Default Namespace"))
        {
            EditorPrefs.SetString(PrefKeyNamespace, explicitNamespace);
            ShowNotification(new GUIContent($"Saved: {explicitNamespace}"));
        }
    }

    private void CreateScript()
    {
        if (string.IsNullOrWhiteSpace(className) || !IsValidIdentifier(className))
        {
            ShowNotification(new GUIContent("Invalid class name."));
            return;
        }

        // Figure out target folder in Project window
        var targetFolder = GetSelectedFolder();
        if (string.IsNullOrEmpty(targetFolder))
            targetFolder = "Assets";

        string ns = useAsmdefRootNamespace ? GetAsmdefRootNamespace(targetFolder) : null;
        if (string.IsNullOrWhiteSpace(ns)) ns = explicitNamespace;

        var path = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(targetFolder, $"{className}.cs"));
        File.WriteAllText(path, GenerateTemplate(ns, className, kind));
        AssetDatabase.ImportAsset(path);
        var obj = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
        Selection.activeObject = obj;
        EditorGUIUtility.PingObject(obj);
        Close();
    }

    private static string GetSelectedFolder()
    {
        // If multiple selected, use the first folder; if file selected, use its directory
        var guids = Selection.assetGUIDs;
        if (guids != null && guids.Length > 0)
        {
            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            if (Directory.Exists(path)) return path;
            if (File.Exists(path)) return Path.GetDirectoryName(path).Replace("\\", "/");
        }
        return null;
    }

    private static bool IsValidIdentifier(string s)
    {
        if (string.IsNullOrEmpty(s)) return false;
        if (!(char.IsLetter(s[0]) || s[0] == '_')) return false;
        return s.All(c => char.IsLetterOrDigit(c) || c == '_');
    }

    private static string GetAsmdefRootNamespace(string startFolder)
    {
        // Walk up directories looking for an asmdef with a "rootNamespace"
        var dir = new DirectoryInfo(Application.dataPath);
        var projectRoot = dir.Parent?.FullName ?? Application.dataPath;

        var abs = Path.GetFullPath(startFolder.Replace("Assets", Application.dataPath));
        var cur = new DirectoryInfo(abs);
        while (cur != null && cur.FullName.StartsWith(projectRoot))
        {
            var asmdefs = cur.GetFiles("*.asmdef", SearchOption.TopDirectoryOnly);
            foreach (var asm in asmdefs)
            {
                var json = File.ReadAllText(asm.FullName);
                // super-lightweight parse to avoid bringing in JSON libs
                var key = "\"rootNamespace\"";
                var idx = json.IndexOf(key);
                if (idx >= 0)
                {
                    var after = json.Substring(idx + key.Length);
                    var q1 = after.IndexOf('\"');
                    if (q1 >= 0)
                    {
                        var q2 = after.IndexOf('\"', q1 + 1);
                        if (q2 > q1)
                        {
                            var ns = after.Substring(q1 + 1, q2 - q1 - 1);
                            if (!string.IsNullOrWhiteSpace(ns)) return ns.Trim();
                        }
                    }
                }
            }
            cur = cur.Parent;
        }
        return null;
    }

    private static string GenerateTemplate(string ns, string name, ScriptKind kind)
    {
        string body;
        switch (kind)
        {
            case ScriptKind.MonoBehaviour:
                body = 
$@"using UnityEngine;

namespace {ns}
{{
    public class {name} : MonoBehaviour
    {{
        private void Awake() {{ }}
        private void Start() {{ }}
        private void Update() {{ }}
    }}
}}";
                break;

            case ScriptKind.ScriptableObject:
                body =
$@"using UnityEngine;

namespace {ns}
{{
    [CreateAssetMenu(menuName = ""{ns}/{name}"")]
    public class {name} : ScriptableObject
    {{
    }}
}}";
                break;

            case ScriptKind.Interface:
                body =
$@"namespace {ns}
{{
    public interface I{name}
    {{
    }}
}}";
                break;

            case ScriptKind.StaticClass:
                body =
$@"namespace {ns}
{{
    public static class {name}
    {{
    }}
}}";
                break;

            default: // PlainClass
                body =
$@"namespace {ns}
{{
    public class {name}
    {{
    }}
}}";
                break;
        }

        return body.Replace("\t", "    ");
    }
}
#endif
