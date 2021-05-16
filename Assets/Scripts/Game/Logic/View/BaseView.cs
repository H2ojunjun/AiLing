using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseView : MonoBehaviour
{
    protected const string DEATH_LISTENER_PATH = "listeners/deathListener";
    protected const string REGENERATION_LISTENER_PATH = "listeners/regenerationListener";

    [HideInInspector]
    public List<GameObject> unartPara;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
