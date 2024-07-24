using System.Collections.Generic;
using UnityEngine;
using UnityEditor.PixyzPlugin4Unity.UI;

public class NewCustomAction_29583945 : ActionInOut<IList<GameObject>, IList<GameObject>> {
    [UserParameter]
    public string name1 = "Text";
    [UserParameter]
    public string name2 = "Text";
    [UserParameter]
    public string name3 = "Text";
    [UserParameter]
    public string name4 = "Text";
    [UserParameter]
    public string name5 = "Text";

    [HelperMethod]
    public void resetParameters() {
        name1 = name2 = name3 = name4 = name5 = "Text";
        return;
    }

    public override int id { get { return 29583945;} }
    public override string menuPathRuleEngine { get { return "Buildwise/Contain Multiple Names";} }
    public override string menuPathToolbox { get { return "Buildwise/Contain Multiple Names"; } }
    public override string tooltip { get { return "Contain Multiple Names"; } }

    public override IList<GameObject> run(IList<GameObject> input) {
        IList<GameObject> output = new List<GameObject>();
        /// Your code here
        foreach (var item in input)
        {
            if (item.name == name1 || item.name == name2 || item.name == name3 || item.name == name4 || item.name == name5)
            {
                output.Add(item);
                Debug.Log(item.name + "added");
            }
        }
        Debug.Log("output count: " + output.Count);
        return output;
    }
}