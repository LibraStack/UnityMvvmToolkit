using UnityEngine;

public class App : MonoBehaviour
{
    [SerializeField] private AppContext _appContext;

    private void Awake()
    {
        _appContext.Construct();
    }

    private void Start()
    {
        Application.targetFrameRate = 300;
    }
}