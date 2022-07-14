using UnityEngine;

public class HelloFromSourceGenerator : MonoBehaviour
{
    static string GetStringFromSourceGenerator()
    {
        return ExampleSourceGenerated.ExampleSourceGenerated.GetTestText();
    }
    
    private void Start()
    {
        print(GetStringFromSourceGenerator());
    }
}
