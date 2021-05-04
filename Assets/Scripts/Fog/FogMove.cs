using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogMove : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform start;
    public Transform end;
    public float speed;
    void Start()
    {
        
    }
    void Show()
    {
        gameObject.SetActive(true);
    }
    void Hide()
    {
        gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (start.position != end.position)
        {
            transform.position = Vector3.MoveTowards(start.position, end.position, speed * Time.deltaTime);
        }
        else
            Hide();
    }
}
