using Interfaces.Services;
using UnityEngine;

public class App : MonoBehaviour
{
    [SerializeField] private AppContext _appContext;

    private IDataStoreService _dataStoreService;

    private void Awake()
    {
        _appContext.Construct();
        _dataStoreService = _appContext.Resolve<IDataStoreService>();
    }

    private void Start()
    {
        Application.targetFrameRate = 300;
    }

    private void OnEnable()
    {
        _dataStoreService.Enable();
    }

    private void OnDisable()
    {
        _dataStoreService.Disable();
    }
}