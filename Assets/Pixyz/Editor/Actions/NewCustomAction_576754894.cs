using System.Collections.Generic;
using UnityEngine;
using UnityEditor.PixyzPlugin4Unity.UI;

public class NewCustomAction_576754894 : ActionInOut<IList<GameObject>, IList<GameObject>> {

    [UserParameter]
    public Material material = null;

    [UserParameter]
    public string childName = "Text";

    [HelperMethod]
    public void resetParameters() {
        material = null;
        childName = "Text";
    }

    public override int id { get { return 576754894;} }
    public override string menuPathRuleEngine { get { return "Buildwise/Set Material to Children"; } }
    public override string menuPathToolbox { get { return "Buildwise/Set Material to Children";} }
    public override string tooltip { get { return "Set Material to Children"; } }

    public override IList<GameObject> run(IList<GameObject> input) {
        /// Your code here
        foreach (GameObject go in input)
        {
            foreach (Transform child in go.transform)
            {
                if (child.name == childName)
                {
                    Renderer renderer = child.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material = material;
                    }
                }
            }
        }
        Output = input;
        return input;
    }
}