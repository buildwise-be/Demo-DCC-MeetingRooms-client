using System.Collections.Generic;
using UnityEngine;
using UnityEditor.PixyzPlugin4Unity.UI;

public class NewCustomAction_975614871 : ActionInOut<IList<GameObject>, IList<GameObject>> {


    [UserParameter]
    public bool activeOnOff = false;

    [HelperMethod]
    public void resetParameters() {
        activeOnOff = false;
        return;
    }

    public override int id { get { return 975614871;} }
    public override string menuPathRuleEngine { get { return "Buildwise/Set ActiveOnOff"; } }
    public override string menuPathToolbox { get { return "Buildwise/Set ActiveOnOff"; } }
    public override string tooltip { get { return "Set ActiveOnOff";} }

    public override IList<GameObject> run(IList<GameObject> input) {
        /// Your code here
        if (input == null)
        {
            Debug.Log("input is null");
            return input;
        }
        foreach (GameObject go in input)
        {
            Debug.Log(go.name + " set to " + activeOnOff);
            go.SetActive(activeOnOff);
        }
        Output = input;
        return input;
    }
}