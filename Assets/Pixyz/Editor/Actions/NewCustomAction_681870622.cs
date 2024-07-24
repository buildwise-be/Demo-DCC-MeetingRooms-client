using System.Collections.Generic;
using UnityEngine;
using UnityEditor.PixyzPlugin4Unity.UI;
using UnityEngine.PixyzPlugin4Unity.Extensions;
using System.Linq;

public class NewCustomAction_681870622 : ActionOut<IList<GameObject>> {

    public override int id { get { return 681870622;} }
    public override string menuPathRuleEngine { get { return "Buildwise/Get Active Objects"; } }
    public override string menuPathToolbox { get { return "Buildwise/Get Active Objects";} }
    public override string tooltip { get { return "Get Active Objects"; } }

    public override IList<GameObject> run()
    {
        var output = Object.FindObjectsOfType<Transform>(false).Select(t => t.gameObject).ToArray();
        Debug.Log($"Found {output.Length} active objects");
#if UNITY_EDITOR
        //UnityEditor.Undo.RegisterFullObjectHierarchyUndo(new List<GameObject>(output).GetHighestAncestor(), "RuleEngine Entry Point");
#endif
        Output = output;
        return output;
    }
}